using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LAB_V2.Worker
{
    public class Copier
    {
        public MainWindow mainWindow;
        public Builder builder;
        public GitManager gitManager;

        private static string DevPortalBuildPath = "C:\\OmniTool\\DP_TSC_DEV\\DevPortal_FormDesigner\\Src\\Clients\\mwclient"; //FormDesigner/MenuDesigner
        //TODO Do proměnných z JSONu
        public static Dictionaries MyDesk = new Dictionaries("MyDesk", "C:\\Git\\MyDesk\\Src\\Clients\\MW_MyDesk\\MyDesk", "C:\\Git\\MyDesk\\Src\\Clients\\MW_MyDesk\\MyDesk\\build\\debug");
        public static Dictionaries MultiWeb = new Dictionaries("MultiWebClient", "C:\\Git\\MultiWebClient", "C:\\Git\\MultiWebClient\\MWCore\\wwwroot\\mwclient");
        public static Dictionaries FormDesigner = new Dictionaries("DevPortal_FormDesigner", "C:\\OmniTool\\DP_TSC_DEV\\DevPortal_FormDesigner", DevPortalBuildPath + "\\FormDesigner\\build");
        public static Dictionaries MenuDesigner = new Dictionaries("MenuDesigner", "C:\\OmniTool\\DP_TSC_DEV\\DevPortal_FormDesigner\\Src\\Clients\\mwclient\\menu-designer", DevPortalBuildPath + "\\menu-designer\\build");


        public static void Copyfiles(string from, string to)
        {
            string Src_FOLDER = from;
            string Dest_FOLDER = to;
            string[] originalFiles = Directory.GetFiles(Src_FOLDER, "*", SearchOption.AllDirectories);
            foreach (string originalFileLocation in originalFiles)
            {
                FileInfo originalFile = new FileInfo(originalFileLocation);
                FileInfo destFile = new FileInfo(originalFileLocation.Replace(Src_FOLDER, Dest_FOLDER));
                // Update UI with progress

                if (destFile.Exists)
                {
                    if (originalFile.Length > destFile.Length)
                    {
                        originalFile.CopyTo(destFile.FullName, true);
                    }
                }
                else
                {
                    Directory.CreateDirectory(destFile.DirectoryName);
                    originalFile.CopyTo(destFile.FullName, false);
                }
            }
        }

        public void CopyAll(string branch, string repository, bool clientOrBin, string localAppPath, bool StartButton)
        {
            string? whereToBuildPath = null;
            string? repo = repository;
            if (StartButton)
            {
                try
                {
                    if (!string.IsNullOrEmpty(mainWindow.LastNetBuildPath) && !string.IsNullOrEmpty(localAppPath))
                    {
                        if (mainWindow.AllCheckbox.IsChecked == true) { Copyfiles(mainWindow.LastNetBuildPath, localAppPath); }

                        else if (mainWindow.JustBinCheckbox.IsChecked == true) { Copyfiles(mainWindow.LastNetBuildPath + "\\bin", localAppPath + "\\bin"); }
                    }


                    if (repository != null)
                    {
                        gitManager.FetchAndCheckout(branch, repository);


                        builder.BuildCurrentBranch(repository);


                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            mainWindow.FileName.Content = "App changes built...";
                            mainWindow.pbStatus.Value = 75;
                            mainWindow.FileName.Content = "Starting to Copy files to desired destination...";
                        }));

                    }

                    if (repo != null)
                    {
                        if (repo.Equals(MyDesk.Key)) { whereToBuildPath = MyDesk.BuildPath; };
                        if (repo.Equals(MultiWeb.Key)) { whereToBuildPath = MultiWeb.BuildPath; };
                        if (repo.Equals(FormDesigner.Key)) { whereToBuildPath = FormDesigner.BuildPath; };
                        if (repo.Equals(MenuDesigner.Key)) { whereToBuildPath = MenuDesigner.BuildPath; };
                        {
                            if (clientOrBin) { Copyfiles(whereToBuildPath, localAppPath + "\\client"); }

                            else if (!clientOrBin) { Copyfiles(whereToBuildPath, localAppPath + "\\bin"); }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }
    }
    public class Dictionaries
    {
        #region build paths to copy
        private static string DevPortalBuildPath = "C:\\OmniTool\\DP_TSC_DEV\\DevPortal_FormDesigner\\Src\\Clients\\mwclient"; //FormDesigner/MenuDesigner
        private string MWBuildPath = "C:\\Git\\MultiWebClient\\MWCore\\wwwroot\\mwclient";  //musí do clienta
        private string FormDesignerBuildPath = DevPortalBuildPath + "\\FormDesigner\\build";         //musí do clienta
        private string MenuDesignerBuildPath = DevPortalBuildPath + "\\menu-designer\\build";        //musí do clienta
        private string MyDeskBuildPath = "C:\\Git\\MyDesk\\Src\\Clients\\MW_MyDesk\\MyDesk\\build\\debug"; //musí do clienta
        private string AppServer = "C:\\Git\\TEAF\\Server\\TescoSW\\Build\\NET_4.0\\Debug\\AppService"; //bin
        #endregion

        // Key-value pairs for different repositories
        private static Dictionary<string, string> repositories = new Dictionary<string, string>()
        {
            { "MyDesk", "C:\\Git\\MyDesk\\Src\\Clients\\MW_MyDesk\\MyDesk" },
            { "Multiweb", "C:\\Git\\MultiWebClient\\MWCore" },
            { "FormDesigner", "C:\\OmniTool\\DP_TSC_DEV\\DevPortal_FormDesigner\\Src\\Clients\\mwclient\\FormDesigner" },
            { "MenuDesigner", "C:\\OmniTool\\DP_TSC_DEV\\DevPortal_FormDesigner\\Src\\Clients\\mwclient\\menu-designer" }
        };

        // Constructor
        public Dictionaries(string key, string repoPath, string buildPath)
        {
            if (key == "MyDesk") { buildPath = MyDeskBuildPath; }
            if (key == "MultiWeb") { buildPath = MWBuildPath; }
            if (key == "FormDesigner") { buildPath = FormDesignerBuildPath; }
            if (key == "MultiWeb") { buildPath = MenuDesignerBuildPath; }

            Key = key;
            Path = repoPath;
            BuildPath = buildPath;
        }

        // Properties
        public string Key { get; private set; }
        public string Path { get; private set; }
        public string BuildPath { get; private set; }
    }
}
