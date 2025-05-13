using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services.Interfaces
{
    public interface ITimeFormatter
    {
        string FormatTime(long timeInMilliseconds);
    }
}
