using System.Diagnostics;
using System.IO;

namespace LAB_V2.Worker.Other;

public class BrowserManager
{
    public async Task OpenLink(string appPath)
    {
        string appName = Path.GetFileName(appPath);

        var processInfo = new ProcessStartInfo
        {
            FileName = "cmd",
            RedirectStandardInput = true,
            CreateNoWindow = true
        };

        using (var cmd = Process.Start(processInfo))
        {
            using (var sw = cmd.StandardInput)
            {
                string completeUrl = "https://" + Environment.MachineName + ".tescosw.loc/" + appName + "/client/login";
                cmd.StandardInput.WriteLine($"explorer {completeUrl}");
            }
            cmd.WaitForExitAsync();
            cmd.Close();
        }
    }
}
