namespace LAB_V2.Jobs;

public class Job
{
    public string Name { get; set; }
    public string LocalRepoPath { get; set; }
    public bool DoNPMBuild { get; set; }
    public string PackageJsonPaths { get; set; }
    public string NPMBuildPaths { get; set; }
    public string? NPMBuildCommandSuffix { get; set; }
    public bool DoMSBuild { get; set; }
    public bool UseScript { get; set; }
    public string SelectedScript { get; set; }  
    public string? SolutionPath { get; set; }
    public string? SolutionBuildPath { get; set; }
    public string? LNBPath { get; set; }
    public bool CopyBinOnly { get; set; }
    public string LocalApplicationPath { get; set; }

    public static Job CreateNew()
    {
        return new Job()
        {
            Name = "New Job",
            LocalRepoPath = "",
            DoNPMBuild = true,
            PackageJsonPaths = "",
            NPMBuildPaths = "",
            NPMBuildCommandSuffix = "",
            DoMSBuild = false,
            UseScript = false,
            SelectedScript = "",
            SolutionPath = null,
            SolutionBuildPath = null,
            LNBPath = null,
            CopyBinOnly = false,
            LocalApplicationPath = "",
        };
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
