using LAB_V2.Jobs;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace LAB_V2.Windows.SettingsSaver
{
    /// <summary>
    /// Interaction logic for JobManager.xaml
    /// </summary>
    public partial class JobManager : Window
    {
        private List<Job> jobs;
        private JobsRoot Root;
        public string jsonPath;
        public event EventHandler? WindowClosed;

        public JobManager()
        {
            InitializeComponent();
            jsonPath = Json();
            Root = jobsRoot();
            jobs = ReadJobsFromFile();
            LoadJobs();
            LoadApps();
            LoadScripts();
        }
        
        public void AddJobs(Object sender, RoutedEventArgs e)
        {
            Job newJob = Job.CreateNew();
            jobs.Add(newJob);

            JobsRoot updatedJobsRoot = new JobsRoot { Jobs = jobs };
            string jobJson = JsonSerializer.Serialize(updatedJobsRoot);

            File.WriteAllText(Json(), jobJson);          
            LoadJobs();
            Save(null, false);
            ListJob.SelectedItem = newJob.Name;
        }
        private string Json()
        {
            string localAppFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string subFolder = Path.Combine(localAppFolder, "LAB");
            string filePath = Path.Combine(subFolder, "Jobs.json");
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(subFolder);
                Save(null, false);
            }
            return filePath;
        }

        private void LoadJobs()
        {
            if (jobs == null || jobs.Count == 0)
            {
                MessageBox.Show("No jobs found. Please add new jobs.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ListJob.ItemsSource = jobs.Select(x => x.Name);
        }

        public void SaveJobs(object sender, RoutedEventArgs e) => Save("Job saved successfully!", true);        
       
        private List<Job> ReadJobsFromFile()
        {
            if (!File.Exists(Json()))
                return new List<Job>();

            if (Root == null)
                return new List<Job>();

            return Root.Jobs;
        }

        public JobsRoot jobsRoot()
        {
            string jobsFileContent = File.ReadAllText(Json());
            return JsonSerializer.Deserialize<JobsRoot>(jobsFileContent);
        }

        private void LoadApps()
        {
            string[] paths = Directory.GetDirectories("C:\\inetpub\\wwwroot");
            foreach (string path in paths)
            {
                LocalAppPath.Items.Add(path);
            }
        }

        private void LoadScripts()
        {
            string localAppFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string subFolder = Path.Combine(localAppFolder, "LAB", "Scripts");
            List<string> files = new List<string>();
            foreach (var file in Directory.GetFiles(subFolder).ToList())
            {
                files.Add(Path.GetFileName(file));
            }

            Scripts.ItemsSource = files;
            
        }

        private void ListJob_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListJob.SelectedItem is string selectedJobName)
            {
                Job? selectedJob = jobs?.FirstOrDefault(j => j.Name == selectedJobName);

                if (selectedJob != null)
                {
                    JobName.Text = selectedJob.Name;
                    LocalRepoPath.Text = selectedJob.LocalRepoPath;
                    DoNPMBuild.IsChecked = selectedJob.DoNPMBuild;
                    PackageJsonPaths.Text = selectedJob.PackageJsonPaths;
                    NPMBuildPaths.Text = selectedJob.NPMBuildPaths;
                    NPMBuildCommandSuffix.Text = selectedJob.NPMBuildCommandSuffix;
                    DoMSBuild.IsChecked = selectedJob.DoMSBuild;
                    UseScript.IsChecked = selectedJob.UseScript;
                    Scripts.SelectedItem = selectedJob.SelectedScript;
                    SolutionPath.Text = selectedJob.SolutionPath;
                    SolutionBuildPath.Text = selectedJob.SolutionBuildPath;
                    LNBPath.Text = selectedJob.LNBPath;
                    CopyBinPath.IsChecked = selectedJob.CopyBinOnly;
                    LocalAppPath.Text = selectedJob.LocalApplicationPath;
                }
            }
        }
        private void DeleteJobs(object sender, RoutedEventArgs e)
        {
            try
            {
                // Read the jobs from the JSON file
                string jobsFilePath = Json(); // Ensure this returns a valid file path
                if (!File.Exists(jobsFilePath))
                {
                    MessageBox.Show("No jobs file found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string jobsFileContent = File.ReadAllText(jobsFilePath);
                JobsRoot? jobsRoot = JsonSerializer.Deserialize<JobsRoot>(jobsFileContent);

                if (jobsRoot == null || jobsRoot.Jobs == null)
                {
                    MessageBox.Show("Failed to load jobs.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (ListJob.SelectedItem == null)
                {
                    MessageBox.Show("No job selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Get the current selected job from the ListBox
                string selectedJobName = ListJob.SelectedItem.ToString();
                Job? currentSelectedJob = jobsRoot.Jobs.FirstOrDefault(j => j.Name == selectedJobName);

                if (currentSelectedJob != null)
                {
                    // Remove the selected job
                    jobsRoot.Jobs.Remove(currentSelectedJob);

                    string jobJson = JsonSerializer.Serialize(new JobsRoot { Jobs = jobsRoot.Jobs });
                    File.WriteAllText(jobsFilePath, jobJson);

                    LoadJobs();
                }
                else MessageBox.Show("Job not found.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting job: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Save(string? text, bool show)
        {
            if (ListJob.SelectedItem is string selectedJobName)
            {
                Job? currentSelectedJob = jobs?.FirstOrDefault(j => j.Name == selectedJobName);

                if (currentSelectedJob != null)
                {
                    // Update the job properties from UI elements
                    currentSelectedJob.Name = JobName.Text;
                    currentSelectedJob.LocalRepoPath = LocalRepoPath.Text;
                    currentSelectedJob.DoNPMBuild = DoNPMBuild.IsChecked ?? false;
                    currentSelectedJob.PackageJsonPaths = PackageJsonPaths.Text;
                    currentSelectedJob.NPMBuildPaths = NPMBuildPaths.Text;
                    currentSelectedJob.NPMBuildCommandSuffix = NPMBuildCommandSuffix.Text;
                    currentSelectedJob.DoMSBuild = DoMSBuild.IsChecked ?? false;
                    currentSelectedJob.UseScript = UseScript.IsChecked ?? false;
                    currentSelectedJob.SelectedScript = Scripts.SelectedItem.ToString();
                    currentSelectedJob.SolutionPath = SolutionPath.Text;
                    currentSelectedJob.SolutionBuildPath = SolutionBuildPath.Text;
                    currentSelectedJob.LNBPath = LNBPath.Text;
                    currentSelectedJob.CopyBinOnly = CopyBinPath.IsChecked ?? false;
                    currentSelectedJob.LocalApplicationPath = LocalAppPath.Text;

                    JobsRoot updatedJobsRoot = new JobsRoot { Jobs = jobs };
                    string jobJson = JsonSerializer.Serialize(updatedJobsRoot);
                    File.WriteAllText(Json(), jobJson);

                    if(show) MessageBox.Show(text, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadJobs();
                }
                else MessageBox.Show("No job selected to save.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
        }

        private void UseScript_Checked(object sender, RoutedEventArgs e)
        {
            ScriptPanel.Visibility = Visibility.Visible;
            MSBuildPanel.Visibility = Visibility.Collapsed;
            DoMSBuild.IsChecked = false;
            LoadScripts();
        }

        private void DoMSBuild_Checked(object sender, RoutedEventArgs e)
        {
            MSBuildPanel.Visibility = Visibility.Visible;
            ScriptPanel.Visibility = Visibility.Collapsed;
            UseScript.IsChecked = false;
        }
    }
}
