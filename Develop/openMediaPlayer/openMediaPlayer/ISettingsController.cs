using openMediaPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Models
{
    public interface ISettingsController
    {
        AppSettings CurrentSettings { get; }
        event EventHandler<AppSettings>? SettingsChanged;
        Task LoadSettingsAsync();
        Task SaveSettingsAsync();
    }
}
