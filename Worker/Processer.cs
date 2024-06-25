
using LAB_V2.Jobs;
using LAB_V2.Scripts;
using LAB_V2.Windows.SettingsSaver;
using LAB_V2.Worker.Build;
using LAB_V2.Worker.Copying;
using LAB_V2.Worker.Git;
using LAB_V2.Worker.Other;
using System.Windows;

namespace LAB_V2.Worker
{
    public class Processer
    {
        #region Instances
        private readonly static GitManager gitManager = new GitManager();
        private readonly static Scraper scraper = new Scraper();
        private readonly static MSBuilder MSBuilder = new MSBuilder();
        private readonly static Builder builder = new Builder();
        private readonly static BrowserManager browserManager = new BrowserManager();
        private readonly static JobManager jobManager = new JobManager();
        private readonly static ScripterWorker scripter = new ScripterWorker();
        #endregion

        public Processer()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static async Task Run(string? link, Job job, Messenger messenger)
        {
            bool task = false;
            if (link != null)
            {
                (string branch, _) = await scraper.ScrapePRLink(link);
                messenger.UpdateStatus("", 0);

                if (job.LNBPath != null)
                {
                    await Task.Run(() => Copier.CopyLNB(job, messenger));
                    messenger.UpdateStatus("Last NET Build copied...", 15);

                    bool fetched = await Task.Run(() => gitManager.FetchAndCheckout(branch, job, messenger));
                    messenger.UpdateStatus("Fetch and Checkout complete...", 25);

                    if (fetched)
                    {
                        if (job.DoMSBuild) //Validovat build výsledek. popř. mazat node-modules + package-lock
                            task = await Task.Run(() => MSBuilder.MSBuild(messenger, job));
                        else if (job.DoNPMBuild)
                            task = await Task.Run(() => builder.BuildCurrentBranch(job, messenger));

                        messenger.UpdateStatus("Branch built...", 50);
                    }
                }
            }

            if (task)
            {
                messenger.UpdateStatus("Starting copying process...", 0);

                if (job.UseScript)
                    await scripter.Execute(job.SelectedScript);
                else                
                    await Task.Run(() => Copier.CopyBuild(job, messenger));
            }

            if (!job.DoMSBuild & task != false)
            {
                await browserManager.OpenLink(job.LocalApplicationPath);

                messenger.UpdateStatus("All files copied successfully!", 100);
                MessageBox.Show("LAB", "Done", MessageBoxButton.OK);
                messenger.ClearMessage();
            }
            else messenger.UpdateStatus("Something went wrong. Work stopped...", 0);                   
        }

        public static async Task FromToRun(FromToJob job, Messenger messenger) => await Task.Run(() => Copier.FromToCopy(job, messenger));

        public static async Task TwoBranchRun(string PR1, string PR2, Job job, Messenger messenger)
        {
            Scraper scraper = new Scraper();
            (string targetBranch, _) = await scraper.ScrapePRLink(PR1);
            (string branchToMerge, _) = await scraper.ScrapePRLink(PR2);

            TwoBranchGitManager twoBranchGitManager = new TwoBranchGitManager(targetBranch, branchToMerge, job, messenger);
            await twoBranchGitManager.TwoBrancher();
        }
    }
}
