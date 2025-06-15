using openMediaPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer
{
    public class MediaItem : ViewModelBase
    {
        public string FilePath { get; }

        public string DisplayName { get; }

        public TimeSpan Duration { get; set; }

        public MediaItem(string filePath)
        {
            //경로나 파일 없으면 예외
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                throw new ArgumentException("Invalid file path", nameof(filePath));
            }

            FilePath = filePath;
            DisplayName = Path.GetFileName(filePath);
            Duration = TimeSpan.Zero; // 초기값
        }

        // Equals 및 GetHashCode를 재정의 -> 컬렉션에서 객체를 올바르게 비교/검색할 수 있게...
        public override bool Equals(object? obj)
        {
            return obj is MediaItem item &&
                   FilePath == item.FilePath;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FilePath);
        }

        public override string ToString()
        {
            return DisplayName; // UI 바인딩 시 기본적으로 표시될 텍스트
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetProperty(ref _isPlaying, value); // SetProperty는 ViewModelBase에 있음
        }
    }
}
