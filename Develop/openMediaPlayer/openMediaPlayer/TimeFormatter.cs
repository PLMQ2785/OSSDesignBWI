using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services
{
    public class TimeFormatter : ITimeFormatter
    {
        public string FormatTime(long timeInMilliseconds)
        {
            if (timeInMilliseconds < 0) { timeInMilliseconds = 0; }

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(timeInMilliseconds);
            if (timeSpan.TotalHours >= 1)
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
            }
            else
            {
                return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            }
        }
    }
}
