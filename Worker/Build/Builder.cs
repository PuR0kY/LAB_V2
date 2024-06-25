using LAB_V2.Jobs;
using LAB_V2.Worker.Other;
using System.Diagnostics;
using System.Windows;

namespace LAB_V2.Worker.Build
{
    public class Builder
    {
        public static MainWindow mainWindow = new MainWindow();

        public Builder()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static async Task<bool> NPMBuild(string localPath, string? buildCommand, Messenger messenger)
        {
            string output = "";
            string error = "";
            var psiNpmRunDist = new ProcessStartInfo
            {
                FileName = "cmd",
                RedirectStandardInput = true,
                WorkingDirectory = localPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false
            };

            messenger.UpdateStatus("Starting to build selected project...", 10);
            try
            {
                using (var pNpmRunDist = Process.Start(psiNpmRunDist))
                {
                    if (pNpmRunDist is not null)
                    {
                        var outputTask = Task.Run(async () => await pNpmRunDist.StandardOutput.ReadToEndAsync());
                        var errorTask = Task.Run(async () => await pNpmRunDist.StandardError.ReadToEndAsync());
                        using (var sw = pNpmRunDist.StandardInput)
                        {
                            if (sw.BaseStream.CanWrite)
                            {
                                pNpmRunDist.StandardInput.WriteLine("npm install");
                                pNpmRunDist.StandardInput.WriteLine($"npm run 1-build{buildCommand ?? ""}");
                                sw.WriteLine("exit");
                            }
                        }

                        await pNpmRunDist.WaitForExitAsync();
                        output = await outputTask;
                        error = await errorTask;
                        pNpmRunDist.Close();
                    }
                    else Console.WriteLine("Failed to start the npm process.");
                }

                if (output.ToLower().Contains("failed") || output.ToLower().Contains("error"))
                    throw new Exception(output);

                else if (error.ToLower().Contains("failed") || error.ToLower().Contains("error"))
                    throw new Exception(error);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LAB exited. " + ex);
                return false;
            }
        }

        public async Task<bool> BuildCurrentBranch(Job job, Messenger messenger)
        {
            bool result = true;
            result &= await NPMBuild(job.NPMBuildPaths, job.NPMBuildCommandSuffix, messenger);

            return result;
        }
    }
}

