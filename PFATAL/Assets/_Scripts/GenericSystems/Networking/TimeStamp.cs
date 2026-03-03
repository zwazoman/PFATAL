using System;

public static class TimeStamp
{
    public static float Now => (float)(DateTime.UtcNow.Ticks-DateTime.Today.Ticks) / TimeSpan.TicksPerMillisecond;
}
