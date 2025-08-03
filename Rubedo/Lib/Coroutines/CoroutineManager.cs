using Rubedo.Lib.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Rubedo.Lib.Coroutines;

/// <summary>
/// Manages coroutines, which can pause execution at yielded points for specific periods of time.
/// </summary>
/// <remarks>If you need asynchronous multithreaded execution where you don't care when it ends, use a Job (NYI)</remarks>
public class CoroutineManager
{
    private class CoroutineInternal : ICoroutine, IPoolable
    {
        public IEnumerator enumerator;

        /// <summary>
        /// Anytime a delay is yielded it is added to the waitTimer which tracks the delays
        /// </summary>
        public float waitTimer;

        public bool isDone;
        public CoroutineInternal waitForCoroutine;
        public bool useUnscaledDeltaTime = false;

        public int version = 0;

        public bool Completed(int startingVersion)
        {
            return version != startingVersion;
        }

        public void Stop()
        {
            if (isDone) 
               return;
            isDone = true;
            version++;
        }

        public ICoroutine UseUnscaledDeltaTime(bool useRawDelta)
        {
            useUnscaledDeltaTime = useRawDelta;
            return this;
        }

        internal void PrepareForReuse()
        {
            isDone = false;
        }

        void IPoolable.Reset()
        {
            isDone = true;
            waitTimer = 0;
            waitForCoroutine = null;
            enumerator = null;
            useUnscaledDeltaTime = false;
        }
    }

    /// <summary>
    /// If a coroutine is created during coroutine updating, then it needs to be added to
    /// the next frame instead of the current to prevent concurrent modification issues.
    /// </summary>
    private bool _updating = false;

    readonly List<CoroutineInternal> _unblockedCoroutines = new List<CoroutineInternal>();
    readonly List<CoroutineInternal> _shouldRunNextFrame = new List<CoroutineInternal>();

    /// <summary>
    /// Stops all active coroutines. Can be called from within a coroutine, but will still execute that coroutine until the next yield.
    /// Some coroutines might not be returned to the pool until the next update cycle.
    /// </summary>
    public void StopAllCoroutines()
    {
        for (var i = 0; i < _unblockedCoroutines.Count; i++)
            _unblockedCoroutines[i].isDone = true;
        for (var i = 0; i < _shouldRunNextFrame.Count; i++)
            _shouldRunNextFrame[i].isDone = true;
    }

    /// <summary>
    /// Immediately clears all coroutines and returns them to the pool for reuse.
    /// Do not to call this during coroutine updates.
    /// </summary>
    public void ClearAllCoroutines()
    {
        if (_updating)
        {
            throw new System.Exception("Cannot call ClearAllCoroutines() while CoroutineManager is updating.");
        }
        else
        {
            for (var i = 0; i < _unblockedCoroutines.Count; i++)
                GlobalPool<CoroutineInternal>.Release(_unblockedCoroutines[i]);
            for (var i = 0; i < _shouldRunNextFrame.Count; i++)
                GlobalPool<CoroutineInternal>.Release(_shouldRunNextFrame[i]);

            _unblockedCoroutines.Clear();
            _shouldRunNextFrame.Clear();
        }
    }

    /// <summary>
    /// Adds the IEnumerator to the CoroutineManager. Coroutines get ticked before Update is called each frame.
    /// </summary>
    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        // find or create a coroutine
        CoroutineInternal coroutine = GlobalPool<CoroutineInternal>.Obtain();
        coroutine.PrepareForReuse();

        // setup the coroutine and add it
        coroutine.enumerator = enumerator;
        var shouldContinueCoroutine = TickCoroutine(coroutine);

        // guard against empty coroutines
        if (!shouldContinueCoroutine)
            return null;

        if (_updating)
            _shouldRunNextFrame.Add(coroutine);
        else
            _unblockedCoroutines.Add(coroutine);

        Coroutine handle = new Coroutine();
        handle.routine = coroutine;
        handle.startingVersion = coroutine.version;

        return handle;
    }

    /// <summary>
    /// Executes a coroutine until it yields. This method will put finished coroutines back in the pool.
    /// </summary>
    /// <returns><c>true</c>, if coroutine was ticked, <c>false</c> otherwise.</returns>
    /// <param name="coroutine">Coroutine.</param>
    private bool TickCoroutine(CoroutineInternal coroutine)
    {
        // This coroutine has finished
        if (!coroutine.enumerator.MoveNext() || coroutine.isDone)
        {
            GlobalPool<CoroutineInternal>.Release(coroutine);
            return false;
        }

        if (coroutine.enumerator.Current == null)
        {
            // yielded null. run again next frame
            return true;
        }

        switch (coroutine.enumerator.Current)
        {
            case WaitForSeconds:
                coroutine.waitTimer = (coroutine.enumerator.Current as WaitForSeconds).waitTime;
                return true;
            case IEnumerator enumerator:
                coroutine.waitForCoroutine = StartCoroutine(enumerator).routine as CoroutineInternal;
                return true;
            case CoroutineInternal:
                coroutine.waitForCoroutine = coroutine.enumerator.Current as CoroutineInternal;
                return true;
            default:
                // This coroutine yielded some value we don't understand. run it next frame.
                return true;
        }
    }

    public void Update()
    {
        _updating = true;
        for (var i = 0; i < _unblockedCoroutines.Count; i++)
        {
            var coroutine = _unblockedCoroutines[i];

            // check for stopped coroutines
            if (coroutine.isDone)
            {
                GlobalPool<CoroutineInternal>.Release(coroutine);
                continue;
            }

            // are we waiting for any other coroutines to finish?
            if (coroutine.waitForCoroutine != null)
            {
                if (coroutine.waitForCoroutine.isDone)
                {
                    coroutine.waitForCoroutine = null;
                }
                else
                {
                    _shouldRunNextFrame.Add(coroutine);
                    continue;
                }
            }

            // deal with timers if we have them
            if (coroutine.waitTimer > 0)
            {
                // still has time left. decrement and run again next frame being sure to decrement with the appropriate deltaTime.
                coroutine.waitTimer -= coroutine.useUnscaledDeltaTime ? (float)Time.RawDeltaTime : Time.DeltaTime;
                _shouldRunNextFrame.Add(coroutine);
                continue;
            }

            if (TickCoroutine(coroutine))
                _shouldRunNextFrame.Add(coroutine);
        }

        _unblockedCoroutines.Clear();
        _unblockedCoroutines.AddRange(_shouldRunNextFrame);
        _shouldRunNextFrame.Clear();

        _updating = false;
    }
}