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
    public class SubtitleGenerator : ISubtitleGenerator
    {
        private readonly IPreferencesController _preferencesController;
        private readonly IProcessRunner _processRunner;

        public SubtitleGenerator(IPreferencesController preferencesController, IProcessRunner processRunner)
        {
            _preferencesController = preferencesController;
            _processRunner = processRunner;
        }

        public async Task<bool> GenerateSubtitlesAsync(string audioFilePath, string outputSrtFilePath, string lang = "", string? modelName = null)
        {
            string outputDirectory = Path.GetDirectoryName(outputSrtFilePath)!; //null 가능성 제거 위해서
            string outputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputSrtFilePath);
            Directory.CreateDirectory(outputDirectory);

            string targetModelName = string.IsNullOrEmpty(modelName) ? _preferencesController.defaultWhisperModel : modelName;
            string modelPath = Path.Combine(_preferencesController.whisperModelPath, targetModelName);
            string outputDirectoryForWhisper = Path.Combine(outputDirectory, outputFileNameWithoutExtension); //이름.srt로 저장

            //whisper command-line 구성
            // -m model -f inAudio -l auto -osrt -of outName
            // -m model -f inAudio -l specified_lang -osrt -of outName
            string arguments = $"-m \"{modelPath}\" -f \"{audioFilePath}\" -l {lang} -osrt -of \"{outputDirectoryForWhisper}\"";

            //string arguments = $"-m \"{modelPath}\" -f \"{audioFilePath}\" -l auto -osrt -of \"{Path.Combine(outputDirectory, outputFileNameWithoutExtension)}\"";

            if (string.IsNullOrEmpty(lang))
            {
                arguments = $"-m \"{modelPath}\" -f \"{audioFilePath}\" -l auto -osrt -of \"{outputDirectoryForWhisper}\"";
            }

            ProcessResult result = await _processRunner.RunProcessAsync(_preferencesController.whisperPath, arguments, _preferencesController.whisperExecutablePath); //작업 경로 설정

            string outputSubtitlePath = outputDirectoryForWhisper + ".srt";
            return result.Success && File.Exists(outputSubtitlePath);
        }
    }
}