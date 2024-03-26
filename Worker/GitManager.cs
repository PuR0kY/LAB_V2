using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB_V2.Worker
{
    public class GitManager
    {

        private static string DevPortalBuildPath = "C:\\OmniTool\\DP_TSC_DEV\\DevPortal_FormDesigner\\Src\\Clients\\mwclient"; //FormDesigner/MenuDesigner
        //TODO Do proměnných z JSONu
        public Dictionaries MyDesk = new Dictionaries("MyDesk", "C:\\Git\\MyDesk\\Src\\Clients\\MW_MyDesk\\MyDesk", "C:\\Git\\MyDesk\\Src\\Clients\\MW_MyDesk\\MyDesk\\build\\debug");
        public Dictionaries MultiWeb = new Dictionaries("MultiWebClient", "C:\\Git\\MultiWebClient", "C:\\Git\\MultiWebClient\\MWCore\\wwwroot\\mwclient");
        public Dictionaries FormDesigner = new Dictionaries("DevPortal_FormDesigner", "C:\\OmniTool\\DP_TSC_DEV\\DevPortal_FormDesigner", DevPortalBuildPath + "\\FormDesigner\\build");
        public Dictionaries MenuDesigner = new Dictionaries("MenuDesigner", "C:\\OmniTool\\DP_TSC_DEV\\DevPortal_FormDesigner\\Src\\Clients\\mwclient\\menu-designer", DevPortalBuildPath + "\\menu-designer\\build");

        /// <summary>
        /// Checkout pomocí LibGit2Sharp
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="branchName"></param>
        public void GitCheckout(string repo, string branchName)
        {
            var repository = new Repository($"{repo}");
            Commands.Fetch(repository, "origin", new List<string>(), new FetchOptions(), string.Empty);
            Branch? branch = repository.Branches.FirstOrDefault(b => b.FriendlyName.EndsWith(branchName, StringComparison.OrdinalIgnoreCase));
            Commands.Checkout(repository, branch);
        }

        public void FetchAndCheckout(string prBranch, string prRepo)
        {
            string branch = prBranch;
            string repo = prRepo;
            string repository = "";

            if (repo.Equals(MyDesk.Key)) { repository = MyDesk.Path; }
            if (repo.Equals(MultiWeb.Key)) { repository = MultiWeb.Path; }
            if (repo.Equals(FormDesigner.Key)) { repository = FormDesigner.Path; }
            if (repo.Equals(MenuDesigner.Key)) { repository = MenuDesigner.Path; }
            GitCheckout(repository, branch);
        }
    }
}
