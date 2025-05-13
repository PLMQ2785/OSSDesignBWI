using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services.Interfaces
{
    public interface IPreferencesController
    {
        string ffmpegPath { get; }
        string whisperPath { get; }
        string whisperModelPath { get; }
        string defaultWhisperModel { get; }
        string whisperExecutablePath { get; }
        bool CheckDependencies(out string errorMSG);
    }
}
