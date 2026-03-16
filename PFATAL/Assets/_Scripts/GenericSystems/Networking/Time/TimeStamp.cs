using System;

namespace NetworkTime
{
    public static class TimeStamp
    {
        internal static float Correction = 0;

        const float TicksToSecondRatio = .001f / TimeSpan.TicksPerMillisecond;
        public static float LocalNow => (DateTime.UtcNow.Ticks - DateTime.Today.Ticks) * TicksToSecondRatio;
        public static float Now => LocalNow + Correction;
    }
}