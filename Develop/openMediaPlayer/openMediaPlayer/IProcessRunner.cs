using openMediaPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services.Interfaces
{
    public interface IProcessRunner
    {
        Task<ProcessResult> RunProcessAsync(string executablePath, string arguments, string workingDirectory = "");
    }
}
