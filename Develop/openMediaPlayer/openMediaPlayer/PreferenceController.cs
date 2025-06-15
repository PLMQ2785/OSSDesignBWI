using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services
{

    public class PreferenceController : IPreferencesController
    {
        private readonly string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        //R/O param 정의
        public string ffmpegPath => Path.Combine(baseDir, "ffmpeg", "bin", "ffmpeg.exe");
        public string whisperPath => Path.Combine(baseDir, "whisper", "whisper-cli.exe");
        public string whisperModelPath => Path.Combine(baseDir, "whisper", "models");
        public string defaultWhisperModel => "medium.bin";
        public string whisperExecutablePath => Path.GetDirectoryName(whisperPath) ?? baseDir; //is null? use default

        //llm 모델 경로
        public string llmModelPath => Path.Combine(baseDir, "llm_models", "model.gguf");

        public bool CheckDependencies(out string errorMSG)
        {
            //check ffmpeg, whisper-cli, whisper model, guard part
            errorMSG = string.Empty;
            if (!File.Exists(ffmpegPath))
            {
                errorMSG += "ffmpeg not found!\n";
                return false;
            }
            if (!File.Exists(whisperPath))
            {
                errorMSG += "whisper not found!\n";
                return false;
            }
            string defaultModelFullPath = Path.Combine(whisperModelPath, defaultWhisperModel);
            if (!Directory.Exists(whisperModelPath) || !File.Exists(defaultModelFullPath))
            {
                errorMSG += "whisper model not found!\n";
                return false;
            }

            if (!File.Exists(llmModelPath))
            {
                errorMSG += "LLM model not found!\n";
                return false;
            }
            return true;
        }
    }
}
