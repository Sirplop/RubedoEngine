using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Rubedo.Lib.Threading;

/// <summary>
/// A fixed-bucket thread dispatcher for workloads where each item is expected to be of similar cost.
/// Do not use concurrently with itself.
/// </summary>
public sealed class FixedThreadDispatcher : IDisposable
{
    private readonly int _threadCount;
    private readonly Thread[] _threads;
    private readonly ManualResetEventSlim[] _startSignals;
    private readonly CountdownEvent _completionSignal;
    private readonly int[] _startIndices;
    private readonly int[] _endIndices; // exclusive
    private readonly Exception[] _exceptions;

    private Action<int> _body;
    private volatile bool _shutdown;

    /// <param name="threadCount">
    /// Number of worker threads to pre-allocate. Defaults to Environment.ProcessorCount.
    /// </param>
    public FixedThreadDispatcher(int threadCount = -1)
    {
        _threadCount = threadCount > 0 ? threadCount : Environment.ProcessorCount;

        _threads = new Thread[_threadCount];
        _startSignals = new ManualResetEventSlim[_threadCount];
        _startIndices = new int[_threadCount];
        _endIndices = new int[_threadCount];
        _exceptions = new Exception[_threadCount];
        _completionSignal = new CountdownEvent(_threadCount);

        for (int i = 0; i < _threadCount; i++)
        {
            _startSignals[i] = new ManualResetEventSlim(false);
            int workerIndex = i;

            var thread = new Thread(() => WorkerLoop(workerIndex))
            {
                IsBackground = true,
                Name = $"{nameof(FixedThreadDispatcher)}-Worker-{i}"
            };
            _threads[i] = thread;
            thread.Start();
        }
    }

    /// <summary>
    /// Runs body(i) for every i in [fromInclusive, toExclusive), split statically across
    /// the pre-allocated worker threads. Blocks the calling thread until all workers finish.
    /// </summary>
    public void For(int fromInclusive, int toExclusive, Action<int> body)
    {
        if (body == null) throw new ArgumentNullException(nameof(body));

        int total = toExclusive - fromInclusive;
        if (total <= 0) return;

        _body = body;

        // Work is given to as many threads as there are items, but all threads contribute to the wait.

        int activeThreads = System.Math.Min(_threadCount, total);
        int baseChunk = total / activeThreads;
        int remainder = total % activeThreads; // first `remainder` buckets get one extra item

        int cursor = fromInclusive;
        for (int i = 0; i < _threadCount; i++)
        {
            if (i < activeThreads)
            {
                int chunkSize = baseChunk + (i < remainder ? 1 : 0);
                _startIndices[i] = cursor;
                _endIndices[i] = cursor + chunkSize;
                cursor += chunkSize;
            }
            else
            {
                _startIndices[i] = 0;
                _endIndices[i] = 0; // no-op bucket
            }

            _exceptions[i] = null;
        }

        _completionSignal.Reset(_threadCount);

        for (int i = 0; i < _threadCount; i++)
        {
            _startSignals[i].Set();
        }

        _completionSignal.Wait();

        List<Exception> errors = null;
        for (int i = 0; i < _threadCount; i++)
        {
            if (_exceptions[i] != null)
            {
                errors ??= new List<Exception>();
                errors.Add(_exceptions[i]);
            }
        }
        if (errors != null)
        {
            throw new AggregateException(
                $"{errors.Count} of {_threadCount} worker thread(s) threw an exception.", errors);
        }
    }

    private void WorkerLoop(int workerIndex)
    {
        var signal = _startSignals[workerIndex];

        while (true)
        {
            signal.Wait();
            signal.Reset();

            if (_shutdown) return;

            try
            {
                int start = _startIndices[workerIndex];
                int end = _endIndices[workerIndex];
                var body = _body;

                for (int i = start; i < end; i++)
                {
                    body(i);
                }
            }
            catch (Exception ex)
            {
                _exceptions[workerIndex] = ex;
            }
            finally
            {
                _completionSignal.Signal();
            }
        }
    }

    /// <summary>
    /// Shuts down and joins all worker threads. Safe to call once, at the end of the dispatcher's life.
    /// </summary>
    public void Dispose()
    {
        _shutdown = true;
        foreach (var s in _startSignals) 
            s.Set();
        foreach (var t in _threads) 
            t.Join();
        foreach (var s in _startSignals) 
            s.Dispose();
        _completionSignal.Dispose();
    }
}
