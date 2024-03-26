using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB_V2.Worker
{
    public class Copier
    {
        public static MainWindow mainWindow = new MainWindow();
        public static Builder builder = new Builder();


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
