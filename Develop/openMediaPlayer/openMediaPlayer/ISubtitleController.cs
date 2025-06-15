using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services.Interfaces
{
    public interface ISubtitleController
    {
        event EventHandler? SubtitleGenerationStarted;
        event EventHandler<(bool success, string messageOrPath)>? SubtitleGenerationCompleted;
        Task GenerateAndLoadSubtitlesAsync(string mediaPath, string? modelName = null, string? lang = null);
    }
}
