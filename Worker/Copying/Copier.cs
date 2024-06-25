using LAB_V2.Jobs;
using LAB_V2.Worker.Other;
using System.IO;

namespace LAB_V2.Worker.Copying
{
    public class Copier
    {
        private static void Copyfiles(string from, string to, Messenger messenger)
        {
            string Src_FOLDER = from;
            string Dest_FOLDER = to;
            string[] originalFiles = Directory.GetFiles(Src_FOLDER, "*", SearchOption.AllDirectories);
            IEnumerable<string[]> chunks;
            if (originalFiles.Length <= 500) { chunks = originalFiles.Chunk(100); }
            else chunks = originalFiles.Chunk(500);

            int totalChunks = chunks.Count();
            int chunksComplete = 0;

            Parallel.ForEach(chunks, chunk =>
            {
                for (int i = 0; i < chunk.Length; i++)
                {
                    string originalFilePath = chunk[i];
                    FileInfo originalFile = new FileInfo(originalFilePath);
                    string sub = originalFile.FullName.Substring(originalFile.FullName.Length - 30, 30);
                    FileInfo destFile = new FileInfo(originalFilePath.Replace(Src_FOLDER, Dest_FOLDER));

                    if (destFile.Exists)
                    {
                        if (originalFile.Length > destFile.Length)
                            destFile.Delete();
                        originalFile.CopyTo(destFile.FullName, true);
                    }
                    else
                    {
                        Directory.CreateDirectory(destFile.DirectoryName);
                        originalFile.CopyTo(destFile.FullName, true);
                    }
                }

                //thread-safe +1
                Interlocked.Increment(ref chunksComplete);

                string message = $"Copied chunks: " + chunksComplete + "/" + totalChunks;
                messenger.UpdateCopyingProgress(message, chunksComplete, totalChunks);
            });
        }

        public static void CopyLNB(Job job, Messenger messenger)
        {
            if (!string.IsNullOrEmpty(job.LNBPath) && !string.IsNullOrEmpty(job.LocalApplicationPath))
            {
                if (!job.CopyBinOnly)
                    Copyfiles(job.LNBPath, job.LocalApplicationPath, messenger);
                else
                    Copyfiles(Path.Combine(job.LNBPath, "bin"), Path.Combine(job.LocalApplicationPath, "bin"), messenger);
            }
        }

        public static void CopyBuild(Job job, Messenger messenger)
        {
            if (job.DoNPMBuild)
                Copyfiles(job.NPMBuildPaths!, Path.Combine(job.LocalApplicationPath, "client"), messenger);

            if (job.DoMSBuild)
                Copyfiles(job.SolutionBuildPath!, Path.Combine(job.LocalApplicationPath, "bin"), messenger);
        }

        public static void FromToCopy(FromToJob job, Messenger messenger)
        {
            messenger.UpdateStatus("Starting to copy files...", 0);
            Copyfiles(job.From, job.To, messenger);
        }
    }
}
