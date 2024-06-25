using LAB_V2.Jobs;
using LAB_V2.Worker.Build;
using LAB_V2.Worker.Copying;
using LAB_V2.Worker.Other;
using LibGit2Sharp;
using System.Diagnostics;

namespace LAB_V2.Worker.Git
{
    public class TwoBranchGitManager
    {
        #region Constants
        private readonly string _targetBranch; // branch do které se bude mergovat
        private readonly string _branchToMerge; // mergovaná branch
        private readonly Job _job;
        private readonly Repository _repository;
        private readonly Messenger _messenger;
        private readonly Builder builder;
        #endregion

        public TwoBranchGitManager(string branch1, string branch2, Job job, Messenger messenger)
        {
            _targetBranch = branch1;
            _branchToMerge = branch2;
            _job = job;
            _repository = new Repository(job.LocalRepoPath);
            _messenger = messenger;
        }

        public async Task TwoBrancher()
        {
            //První branch LAB
            Branch? targetBranch = _repository.Branches.FirstOrDefault(b => b.FriendlyName.EndsWith(_targetBranch, StringComparison.OrdinalIgnoreCase));
            if (targetBranch.IsRemote)
                targetBranch = _repository.CreateBranch(_targetBranch, targetBranch.Tip);
            Commands.Checkout(_repository, targetBranch);

            await builder.BuildCurrentBranch(_job, _messenger);
            await Task.Run(() => Copier.CopyBuild(_job, _messenger));
            _messenger.UpdateStatus("Building second PR files...", 50);

            //Druhý branch LAB
            Branch? branchToMerge = _repository.Branches.FirstOrDefault(b => b.FriendlyName.EndsWith(_branchToMerge, StringComparison.OrdinalIgnoreCase));
            if (branchToMerge.IsRemote)
                branchToMerge = _repository.CreateBranch(_branchToMerge, branchToMerge.Tip);
            Commands.Checkout(_repository, branchToMerge);

            await builder.BuildCurrentBranch(_job, _messenger);
            await Task.Run(() => Copier.CopyBuild(_job, _messenger));
            _messenger.UpdateStatus("Ready", 100);
        }

        private void ConflictManager(string repoPath) //TBD
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "code",
                Arguments = repoPath,
                UseShellExecute = true,
            };

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
