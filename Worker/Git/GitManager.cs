using LAB_V2.Jobs;
using LAB_V2.Worker.Other;
using LibGit2Sharp;
using System.Windows;

namespace LAB_V2.Worker.Git
{
    public class GitManager
    {
        /// <summary>
        /// Checkout pomocí LibGit2Sharp
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="branchName"></param>
        private static bool GitCheckout(string repo, string branchName, Messenger messenger)
        {
            bool result = true;
            messenger.UpdateStatus("Starting Fetch and Checkout...", 15);
            using var repository = new Repository(repo);

            try
            {
                var status = repository.RetrieveStatus();
                if (status.IsDirty)
                {
                    throw new Exception("There are uncommited changes on your Repository. Commit or reset your changes and then try again.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("LAB exited. " + e);
                result = false;
            }

            if (result)
            {
                Commands.Fetch(repository, "origin", new List<string>(), new FetchOptions(), string.Empty);
                Branch branch = repository.Branches.FirstOrDefault(b => b.FriendlyName.EndsWith(branchName, StringComparison.OrdinalIgnoreCase))
                    ?? throw new Exception("Branch no exist");

                if (branch.IsRemote)
                {
                    branch = repository.CreateBranch(branchName, branch.Tip);
                }

                Commands.Checkout(repository, branch);
                try
                {
                    if (branch.FriendlyName == repository.Head.FriendlyName && branch.IsCurrentRepositoryHead)
                    {
                        messenger.UpdateStatus("Fetch and Checkout complete.", 35);
                        result = true;
                    }
                    else
                    {
                        result = false;
                        throw new Exception("Error with Git Fetch and Checkout.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Branch was not checked out successfully. " + ex);
                }
            }
            return result;
        }

        public bool FetchAndCheckout(string prBranch, Job job, Messenger messenger)
        {
            return GitCheckout(job.LocalRepoPath, prBranch, messenger);
        }
    }
}
