using System.Management.Automation;
using System.IO;

namespace LAB_V2.Scripts
{
    public class ScripterWorker
    {
        public string ScriptDirectory(string scriptName)
        {
            string localAppFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string subFolder = Path.Combine(localAppFolder, "LAB", "Scripts");
            string filePath = Path.Combine(subFolder, scriptName);
            if (!File.Exists(filePath))            
                Directory.CreateDirectory(subFolder);            

            return filePath;
        }

        public string GetScript(string directory) => File.ReadAllText(ScriptDirectory(directory));        

        public async Task Execute(string scriptName, string? argument = null)
        {
            var script = GetScript(ScriptDirectory(scriptName));

            var powershell = PowerShell.Create();
            powershell.AddScript($"-Verb RunAs");
            powershell.Invoke();
            powershell.Commands.Clear();

            // Add the actual script
            powershell.AddScript(script);
            if (argument != null)
                InsertParameters(powershell, argument);
            powershell.Invoke();

            foreach (var errorRecord in powershell.Streams.Error)
                Console.WriteLine(errorRecord);
        }

        private void InsertParameters(PowerShell ps, string parameterText)
        {
            var parameters = parameterText.Trim().Split(" -");
            parameters[1] = "-" + parameters[1];
            ps.AddParameters(parameters);
        }

        public List<string> GetScripts()
        {
            string localAppFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string subFolder = Path.Combine(localAppFolder, "LAB", "Scripts");
            List<string> files = new List<string>();
            foreach (var file in Directory.GetFiles(subFolder).ToList())
            {
                files.Add(Path.GetFileName(file));
            }
            return files;
        }
    }
}
