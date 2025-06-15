using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services.Interfaces
{
    public interface ILiveSupportController
    {
        bool IsInitialized { get; }
        Task InitializeAsync();
        Task<string> ProcessUserInputAsync(string userInput);
    }
}
