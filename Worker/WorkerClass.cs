using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB_V2.Worker
{
    public class WorkerClass
    {
        public void Run(string link, bool clientOrBin, string localAppLocation, bool started)
        {
            string branch = Scraper.BranchScraper(link);
            string repo = Scraper.RepositoryScraper(link);

            GitManager gitManager = new GitManager();
            gitManager.FetchAndCheckout(branch, repo);

            Builder builder = new Builder();
            builder.BuildCurrentBranch(repo);

            Copier copier = new Copier();
            copier.CopyAll(branch, repo, clientOrBin, localAppLocation, started);
        }
    }
}
