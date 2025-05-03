using System.Collections.Generic;
using System.Text;

namespace Rubedo.EngineDebug;

/// <summary>
/// A slightly-better stopwatch, fit for crude performance metrics.
/// </summary>
public class Timer
{
    private long startTimestamp;
    private long endTimestamp;

    private const double TICKS_PER_MILL = 1d / System.TimeSpan.TicksPerMillisecond;

    private readonly List<(string, double)> info = new List<(string, double)>();

    public Timer() { }

    public void Start()
    {
        info.Clear();
        startTimestamp = System.DateTime.Now.Ticks;
    }
    public void Step()
    {
        endTimestamp = System.DateTime.Now.Ticks;
        double time = GetTime();
        info.Add(("", time));
        startTimestamp = System.DateTime.Now.Ticks;
    }
    public void Step(string value)
    {
        endTimestamp = System.DateTime.Now.Ticks;
        double time = GetTime();
        info.Add((value, time));
        startTimestamp = System.DateTime.Now.Ticks;
    }
    public void Stop()
    {
        endTimestamp = System.DateTime.Now.Ticks;
        double time = GetTime();
        info.Add(("", time));
    }
    public void Stop(string value)
    {
        endTimestamp = System.DateTime.Now.Ticks;
        double time = GetTime();
        info.Add((value, time));
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

    private double GetTime()
    {
        return (endTimestamp - startTimestamp) * TICKS_PER_MILL;
    }
}