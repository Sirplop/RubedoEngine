using System.Collections;

namespace Rubedo.Lib.Coroutines;

/// <summary>
/// A coroutine can pause execution at yielded points for specific periods of time, and is updated after everything else in a frame.
/// </summary>
public class Coroutine
{
    public static Coroutine Start(IEnumerator func) => RubedoEngine.Instance._coroutineManager.StartCoroutine(func);
    public static object WaitForSeconds(float seconds) => Coroutines.WaitForSeconds.waiter.Wait(seconds);
    /// <summary>
    /// Stops all active coroutines. Can be called from within a coroutine, but will still execute that coroutine until the next yield.
    /// Some coroutines might not be returned to the pool until the next update cycle.
    /// </summary>
    public static void StopAllCoroutines() => RubedoEngine.Instance._coroutineManager.StopAllCoroutines();

    /// <summary>
    /// Immediately clears all coroutines and returns them to the pool for reuse.
    /// Do not to call this during coroutine updates.
    /// </summary>
    public static void ClearAllCoroutines() => RubedoEngine.Instance._coroutineManager.ClearAllCoroutines();

    internal Coroutine() { }

    internal int startingVersion;
    internal ICoroutine routine;

    /// <summary>
    /// Returns whether or not this coroutine has concluded execution.
    /// </summary>
    /// <returns></returns>
    public bool Completed()
    {
        return routine.Completed(startingVersion);
    }
}

public interface ICoroutine
{
    /// <summary>
    /// Returns whether or not this coroutine has concluded execution.
    /// </summary>
    bool Completed(int startingVersion);

    /// <summary>
    /// Stops the coroutine.
    /// </summary>
    void Stop();
    /// <summary>
    /// Sets if this coroutine should use either <see cref="Time.DeltaTime"/> or <see cref="Time.RawDeltaTime"/>.
    /// </summary>
    /// <param name="useRawDelta">If it should use <see cref="Time.RawDeltaTime"/></param>
    ICoroutine UseUnscaledDeltaTime(bool useRawDelta);
}

/// <summary>
/// Helper for when a coroutine wants to wait for some amount of time. <see cref="Coroutine.WaitForSeconds(float)"/> returns one of these.
/// </summary>
class WaitForSeconds
{
    internal static WaitForSeconds waiter = new WaitForSeconds();
    internal float waitTime;

    internal WaitForSeconds Wait(float seconds)
    {
        waiter.waitTime = seconds;
        return waiter;
    }
}