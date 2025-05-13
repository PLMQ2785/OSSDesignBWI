using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services.Interfaces
{
    public interface ISubtitleGenerator
    {
        Task<bool> GenerateSubtitlesAsync(string audioFilePath, string outputSrtFilePath, string lang = "", string? modelName = null);
    }
}
