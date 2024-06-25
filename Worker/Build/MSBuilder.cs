using LAB_V2.Jobs;
using System.IO;
using System.Diagnostics;
using LAB_V2.Worker.Other;

namespace LAB_V2.Worker.Build
{
    public class MSBuilder
    {
        public string SolutionFinder(Job job)
        {
            string solution = "";
            var files = Directory.GetFiles(job.SolutionPath);
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                if (fileName.Contains(".sln"))
                    solution = fileName;
            }
            return solution;
        }

        public string[] SplitByNewLine(string text)
        {
            return text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        private string FindMSBuild()
        {
            var processInfo = new ProcessStartInfo("cmd.exe")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };

            var process = new Process { StartInfo = processInfo };

            process.Start();
            process.StandardInput.WriteLine(@"""%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe & exit");
            process.WaitForExit(1000);

            string standardOutput = process.StandardOutput.ReadToEnd();
            string[] lines = SplitByNewLine(standardOutput);
            string pathLine = lines[lines.Length - 2];

            process.Dispose();
            return pathLine;
        }

        public bool MSBuild(Messenger messenger, Job job)
        {
            if (job.DoMSBuild)
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    RedirectStandardInput = true,
                    WorkingDirectory = job.SolutionPath,
                    CreateNoWindow = false,
                    RedirectStandardOutput = false
                };

                messenger.UpdateStatus("Building solution...", 15);

                using (Process process = Process.Start(processStartInfo))
                {
                    if (processStartInfo != null)
                    {
                        using (var sw = process.StandardInput)
                        {
                            if (sw.BaseStream.CanWrite)
                                process.StandardInput.WriteLine("msbuild " + SolutionFinder(job)); //TODO findMSBuild
                        }
                        process.WaitForExitAsync();
                        process.Close();
                    }
                }
            }
            return true;
        }
    }
}