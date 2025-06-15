using openMediaPlayer.Models;
using openMediaPlayer.Services.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace openMediaPlayer.Services
{
    public class SettingsController : ISettingsController
    {
        private readonly string _filePath;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true, // 사람이 읽기 쉽도록 들여쓰기
            PropertyNameCaseInsensitive = true
        };

        public AppSettings CurrentSettings { get; private set; }

        public event EventHandler<AppSettings>? SettingsChanged;

        public SettingsController(string filePath)
        {
            _filePath = filePath;
            CurrentSettings = new AppSettings(); // 기본값으로 초기화
        }

        public async Task LoadSettingsAsync()
        {
            if (!File.Exists(_filePath))
            {
                // 파일이 없으면 기본 설정으로 새로 저장
                await SaveSettingsAsync();
                return;
            }

            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                var loadedSettings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);
                if (loadedSettings != null)
                {
                    CurrentSettings = loadedSettings;
                }
            }
            catch (Exception ex)
            {
                // 오류 발생 시 기본 설정 유지
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
                CurrentSettings = new AppSettings();
            }
            SettingsChanged?.Invoke(this, CurrentSettings);
        }

        public async Task SaveSettingsAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(CurrentSettings, _jsonOptions);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
            SettingsChanged?.Invoke(this, CurrentSettings);
        }
    }
}
