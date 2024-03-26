using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB_V2.Worker
{
    public class Builder
    {
        public static MainWindow mainWindow = new MainWindow();
        private static string DevPortalBuildPath = "C:\\OmniTool\\DP_TSC_DEV\\DevPortal_FormDesigner\\Src\\Clients\\mwclient"; //FormDesigner/MenuDesigner
        //TODO Do proměnných z JSONu
        public Dictionaries MyDesk = new Dictionaries("MyDesk", "C:\\Git\\MyDesk\\Src\\Clients\\MW_MyDesk\\MyDesk", "C:\\Git\\MyDesk\\Src\\Clients\\MW_MyDesk\\MyDesk\\build\\debug");
        public Dictionaries MultiWeb = new Dictionaries("MultiWebClient", "C:\\Git\\MultiWebClient", "C:\\Git\\MultiWebClient\\MWCore\\wwwroot\\mwclient");
        public Dictionaries FormDesigner = new Dictionaries("DevPortal_FormDesigner", "C:\\OmniTool\\DP_TSC_DEV\\DevPortal_FormDesigner", DevPortalBuildPath + "\\FormDesigner\\build");
        public Dictionaries MenuDesigner = new Dictionaries("MenuDesigner", "C:\\OmniTool\\DP_TSC_DEV\\DevPortal_FormDesigner\\Src\\Clients\\mwclient\\menu-designer", DevPortalBuildPath + "\\menu-designer\\build");

        public async Task NPMBuild(string localPath, string buildCommand)
        {
            var psiNpmRunDist = new ProcessStartInfo
            {
                FileName = "cmd",
                RedirectStandardInput = true,
                WorkingDirectory = localPath,
            };

            using (var pNpmRunDist = Process.Start(psiNpmRunDist))
            {
                if (pNpmRunDist != null)
                {
                    using (var sw = pNpmRunDist.StandardInput)
                    {
                        if (sw.BaseStream.CanWrite)
                        {
                            pNpmRunDist.StandardInput.WriteLine("npm install");
                            pNpmRunDist.StandardInput.WriteLine($"npm run 1-build{buildCommand} & exit");
                        }
                    }
                    pNpmRunDist?.WaitForExit();
                    pNpmRunDist?.Close();
                }

                else
                {
                    Console.WriteLine("Failed to start the npm process.");
                }
            }
        }

        public async Task BuildCurrentBranch(string repository)
        {
            string packageJsonPath = "";
            string buildCommand = "";
            string repo = repository;

            if (repo.Equals(MyDesk.Key)) { packageJsonPath = MyDesk.BuildPath; }
            if (repo.Equals(MultiWeb.Key)) { packageJsonPath = MultiWeb.BuildPath; }
            if (repo.Equals(FormDesigner.Key)) { packageJsonPath = FormDesigner.BuildPath; buildCommand = ":dev"; }
            if (repo.Equals(MenuDesigner.Key)) { packageJsonPath = MenuDesigner.BuildPath; buildCommand = ":dev"; }

            await NPMBuild(packageJsonPath, buildCommand);
        }
    }
}

