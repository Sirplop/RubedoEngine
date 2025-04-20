using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Rubedo.Debug;

/// <summary>
/// Wrapper for <see cref="Stopwatch"/> that supplies useful functions for constructing performance timers.
/// </summary>
public class Timer
{
    private Stopwatch _timer;

    private readonly List<(string, double)> info = new List<(string, double)>();

    public Timer()
    {
        _timer = new Stopwatch();
    }

    public void Start()
    {
        if (_timer.IsRunning)
            _timer.Restart();
        else
            _timer.Start();
    }
    public void Step()
    {
        _timer.Stop();
        info.Add(("", _timer.Elapsed.TotalMilliseconds));
        _timer.Restart();
    }
    public void Step(string value)
    {
        _timer.Stop();
        info.Add((value, _timer.Elapsed.TotalMilliseconds));
        _timer.Restart();
    }
    public void Stop()
    {
        _timer.Stop();
        info.Add(("", _timer.Elapsed.TotalMilliseconds));
    }
    public void Stop(string value)
    {
        info.Add((value, _timer.Elapsed.TotalMilliseconds));
        _timer.Stop();
    }

    public void Reset()
    {
        info.Clear();
    }
    public List<string> Get()
    {
        List<string> outList = new List<string>();
        for (int i = 0; i < info.Count; i++)
        {
            outList.Add(info[i].Item1 + info[i].Item2.ToString("0.00"));
        }
        return outList;
    }
    public string GetAsString(string separator)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < info.Count; i++)
        {
            stringBuilder.Append(info[i].Item1 + info[i].Item2.ToString("0.00"));
            if (i != info.Count - 1)
                stringBuilder.Append(separator);
        }
        return stringBuilder.ToString();
    }
}