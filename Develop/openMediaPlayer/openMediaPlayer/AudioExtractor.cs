using openMediaPlayer.Models;
using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services
{
    public class AudioExtractor : IAudioExtractor
    {
        private readonly IPreferencesController _preferencesController;
        private readonly IProcessRunner _processRunner;

        public AudioExtractor(IPreferencesController preferencesController, IProcessRunner processRunner)
        {
            _preferencesController = preferencesController;
            _processRunner = processRunner;
        }

        public async Task<bool> ExtractAudioAsync(string videoPath, string outAudioPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outAudioPath)!); //null 가능성 제거 위해서

            string arguments = $"-i \"{videoPath}\" -y -vn -acodec pcm_s16le \"{outAudioPath}\"";
            ProcessResult result = await _processRunner.RunProcessAsync(_preferencesController.ffmpegPath, arguments);

            return result.Success && File.Exists(outAudioPath);
        }
    }
}
                                                                                        