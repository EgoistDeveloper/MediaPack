using System;
using System.Collections.Generic;
using System.Text;

namespace MediaPack.Helpers
{
    public static class DateTimeHelpers
    {
        public static string TimeSpanToTimeString(this TimeSpan span)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
        }
    }
}