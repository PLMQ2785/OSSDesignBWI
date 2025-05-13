using openMediaPlayer.Models;
using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services
{
    public class ProcessRunner : IProcessRunner
    {
        public async Task<ProcessResult> RunProcessAsync(string executablePath, string arguments, string workingDirectory = "")
        {
            var result = new ProcessResult();
            var processStartInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = string.IsNullOrEmpty(workingDirectory) ? AppDomain.CurrentDomain.BaseDirectory : workingDirectory
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                //프로세스 시작
                process.Start();

                //비동기 표준 출력/오류스트림 읽기
                Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                Task<string> errorTask = process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                result.ExitCode = process.ExitCode;
                result.StandardOutput = await outputTask;
                result.StandardError = await errorTask;

                //Fail?
                if (!result.Success)
                {
                    //for debug
                    //Debug.WriteLine($"Error running {executablePath}: {result.StandardError}");
                }
                //Debug.WriteLine($"--- Process End ---"); //debug

                Debug.WriteLine($"Exit Code: {result.ExitCode}");
                if (!string.IsNullOrEmpty(result.StandardOutput))
                {
                    Debug.WriteLine($"Standard Output:\n{result.StandardOutput}");
                }
                // 이 부분을 조건 없이 실행하여 Whisper CLI의 출력을 확인합니다.
                // ExitCode가 0이더라도 경고나 진행 상황 메시지가 StandardError로 나올 수 있습니다.
                if (!string.IsNullOrEmpty(result.StandardError))
                {
                    Debug.WriteLine($"Standard Error (ExitCode {result.ExitCode}):\n{result.StandardError}");
                }
                Debug.WriteLine($"--- Process End ---");
            }

            return result;
        }
    }
}
