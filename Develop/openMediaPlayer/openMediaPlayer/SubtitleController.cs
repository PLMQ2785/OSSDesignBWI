using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services
{
    public class SubtitleController : ISubtitleController
    {
        private readonly IAudioExtractor _audioExtractor;
        private readonly ISubtitleGenerator _subtitleGenerator;
        private readonly IMediaPlayerController _mediaPlayerController;
        private readonly IPreferencesController _preferencesController;

        public event EventHandler? SubtitleGenerationStarted;
        public event EventHandler<(bool success, string messageOrPath)>? SubtitleGenerationCompleted;

        public SubtitleController(IAudioExtractor audioExtractor, ISubtitleGenerator subtitleGenerator, IMediaPlayerController mediaPlayerController, IPreferencesController preferencesController)
        {
            _audioExtractor = audioExtractor;
            _subtitleGenerator = subtitleGenerator;
            _mediaPlayerController = mediaPlayerController;
            _preferencesController = preferencesController;
        }

        public async Task GenerateAndLoadSubtitlesAsync(string mediaPath, string? modelName = null)
        {
            SubtitleGenerationStarted?.Invoke(this, EventArgs.Empty);

            Debug.WriteLine($"Starting subtitle generation for: {mediaPath}"); //debug

            string tempAudioPath = null;
            //임시 srt 파일 생성, 원본미디어+GUID
            string srtBaseName = Path.GetFileNameWithoutExtension(mediaPath) + "_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            string tempSrtPath = Path.GetTempPath();
            string tempSrtFilePathBase = Path.Combine(tempSrtPath, srtBaseName);
            string outputSrtPath = tempSrtFilePathBase + ".srt"; //실제 whisper 생성 경로

            try
            {
                Debug.WriteLine("Checking dependencies...");
                if (!_preferencesController.CheckDependencies(out string dependencyError))
                {
                    Debug.WriteLine($"Dependency Error: {dependencyError}");
                    SubtitleGenerationCompleted?.Invoke(this, (false, $"필수 프로그램을 불러오는데 문제가 발생했습니다.\n{dependencyError}"));
                    return;
                }
                Debug.WriteLine("Dependencies OK.");

                Debug.WriteLine($"Temp audio path: {tempAudioPath}");
                Debug.WriteLine($"Temp SRT base path: {tempSrtFilePathBase}");
                Debug.WriteLine($"Final SRT path: {outputSrtPath}");

                //오디오 추출
                Debug.WriteLine("Extracting audio...");
                tempAudioPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.wav");
                bool isAudioExtracted = await _audioExtractor.ExtractAudioAsync(mediaPath, tempAudioPath);
                if (!isAudioExtracted)
                {
                    Debug.WriteLine("Audio extraction failed.");
                    SubtitleGenerationCompleted?.Invoke(this, (false, "오디오 추출에 실패했습니다."));
                    return;
                }
                Debug.WriteLine($"Audio extracted successfully to: {tempAudioPath}, Exists: {File.Exists(tempAudioPath)}");

                //자막 생성
                Debug.WriteLine("Generating subtitles using Whisper...");
                bool isSubtitleGenerated = await _subtitleGenerator.GenerateSubtitlesAsync(tempAudioPath, outputSrtPath, "", modelName ?? _preferencesController.defaultWhisperModel);
                if (!isSubtitleGenerated)
                {
                    Debug.WriteLine($"Subtitle generation failed. Generated: {isSubtitleGenerated}, SRT Exists: {File.Exists(outputSrtPath)}");
                    SubtitleGenerationCompleted?.Invoke(this, (false, "자막 생성에 실패했습니다."));
                    return;
                }
                Debug.WriteLine($"Subtitles generated successfully: {outputSrtPath}");

                //자막 로드
                Debug.WriteLine("Loading subtitles into player...");
                bool isSubtitleLoaded = _mediaPlayerController.AddSubtitle(outputSrtPath);
                if (!isSubtitleLoaded)
                {
                    Debug.WriteLine("Failed to load subtitles into player.");
                    SubtitleGenerationCompleted?.Invoke(this, (false, "자막을 생성했으나 불러오지 못했습니다."));
                }
                else
                {
                    Debug.WriteLine("Subtitles loaded successfully.");
                    SubtitleGenerationCompleted?.Invoke(this, (true, "자막 생성 완료"));
                }


            }
            catch (Exception ex)
            {
                SubtitleGenerationCompleted?.Invoke(this, (false, $"자막 생성 중 오류가 발생했습니다.\n{ex.Message}"));
            }
            finally
            {
                if (tempAudioPath != null && File.Exists(tempAudioPath))
                {
                    try
                    {
                        File.Delete(tempAudioPath);
                    }
                    catch (IOException ex)
                    {
                        //for debug
                        //System.Diagnostics.Debug.WriteLine($"임시 파일 삭제 오류 {ex.Message}");
                    }
                }
            }
        }
    }
}
