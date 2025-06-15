using openMediaPlayer.Models;
using openMediaPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsController _settingsController;

        // UI 바인딩을 위한 설정 객체
        public AppSettings Settings { get; private set; }

        // 창을 닫기 위한 Action
        public Action? CloseAction { get; set; }

        public RelayCommandController SaveCommand { get; }

        public SettingsViewModel(ISettingsController settingsController)
        {
            _settingsController = settingsController;

            // 현재 설정을 UI에 바인딩 (직접 수정 방식)
            Settings = _settingsController.CurrentSettings;

            SaveCommand = new RelayCommandController(async param => await SaveAndClose());
        }

        #region ComboBox ItemsSources

        // ComboBox에 표시될 모델 목록
        public List<string> AvailableSttModels { get; } = new List<string>
        {
            "tiny.bin",
            "base.bin",
            "small.bin",
            "medium.bin",
            "large-v1.bin",
            "large-v2.bin"
        };

        public List<string> AvailableSttLanguages { get; } = new List<string>
        {
            "ko",
            "en",
            "ja"
        };
        #endregion

        private async Task SaveAndClose()
        {
            await _settingsController.SaveSettingsAsync();
            CloseAction?.Invoke(); // 저장 후 창 닫기
        }
    }
}
