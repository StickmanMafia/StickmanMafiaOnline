using System;
using System.Diagnostics;

public struct StopwatchInfo
{
    public int SecondsElapsed { get; set; }
    public int ExpectedDigit { get; set; }

    public StopwatchInfo(Stopwatch stopwatch, int maxTime)
    {
        SecondsElapsed = Convert.ToInt32(stopwatch.Elapsed.TotalSeconds);
        ExpectedDigit =  maxTime - SecondsElapsed;
    }
}
