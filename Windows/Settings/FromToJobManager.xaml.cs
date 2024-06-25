using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using LAB_V2.Jobs;

namespace LAB_V2.Windows.SettingsSaver
{
    /// <summary>
    /// Interaction logic for FromToJobManager.xaml
    /// </summary>
    public partial class FromToJobManager : Window
    {
        private List<FromToJob> fromToJobs;
        private FromToJobsRoot Root;
        public event EventHandler? WindowClosed;

        public FromToJobManager()
        {
            InitializeComponent();
            Root = fromToJobsRoot();
            fromToJobs = JobsFromFile();
            LoadApps();
            LoadJobs();
        }

        public string FromToJson()
        {
            string localAppFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string subFolder = Path.Combine(localAppFolder, "LAB");
            string filePath = Path.Combine(subFolder, "FromToJobs.json");

            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(subFolder);
                Save(null, false);
            }
            return filePath;
        }

        private void LoadJobs()
        {

            if (fromToJobs == null || fromToJobs.Count == 0)
            {
                MessageBox.Show("No jobs found. Please add new jobs.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            FromToListJob.ItemsSource = fromToJobs.Select(x => x.Name);
        }

        private List<FromToJob> JobsFromFile()
        {
            if (!File.Exists(FromToJson()))
                return new List<FromToJob>();

            if (Root == null)
                return new List<FromToJob>();

            return Root.FromToJobs;
        }

        public FromToJobsRoot fromToJobsRoot()
        {
            string jobsFileContent = File.ReadAllText(FromToJson());
            return JsonSerializer.Deserialize<FromToJobsRoot>(jobsFileContent);
        }

        private void Save(string? text, bool show)
        {
            if (FromToListJob.SelectedItem is string selectedJobName)
            {
                FromToJob? currentSelectedJob = fromToJobs.FirstOrDefault(j => j.Name == selectedJobName);
                if (currentSelectedJob != null)
                {
                    currentSelectedJob.Name = FromToJobName.Text;
                    currentSelectedJob.From = FromToJobFrom.Text;
                    currentSelectedJob.To = LocalAppPath.Text;

                    FromToJobsRoot updatedJobsRoot = new FromToJobsRoot { FromToJobs = fromToJobs };
                    string jobJson = JsonSerializer.Serialize(updatedJobsRoot);

                    File.WriteAllText(FromToJson(), jobJson);
                    if (show) MessageBox.Show(text, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadJobs();
                }
            }
        }

        private void LoadApps()
        {
            string[] paths = Directory.GetDirectories("C:\\inetpub\\wwwroot");
            foreach (string path in paths)
            {
                LocalAppPath.Items.Add(path);
            }
        }

        private void FromToListJob_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FromToListJob.SelectedItem is string selectedJobName)
            {
                FromToJob? selectedJob = fromToJobs?.FirstOrDefault(j => j.Name == selectedJobName);

                if (selectedJob != null)
                {
                    FromToJobName.Text = selectedJob.Name;
                    FromToJobFrom.Text = selectedJob.From;
                    LocalAppPath.Text = selectedJob.To;
                }
            }
        }

        private void FromToSaveJob_Click(object sender, RoutedEventArgs e) => Save("Job saved successfully!", true);       

        private void FromToDeleteJob_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Read the jobs from the JSON file
                string jobsFilePath = FromToJson(); // Ensure this returns a valid file path
                if (!File.Exists(jobsFilePath))
                {
                    MessageBox.Show("No jobs file found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (Root == null || fromToJobs == null)
                {
                    MessageBox.Show("Failed to load jobs.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (FromToListJob.SelectedItem == null)
                {
                    MessageBox.Show("No job selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Get the current selected job from the ListBox
                string? selectedJobName = FromToListJob.SelectedItem.ToString();
                FromToJob? currentSelectedJob = fromToJobs.FirstOrDefault(j => j.Name == selectedJobName);

                if (currentSelectedJob != null)
                {
                    // Remove the selected job
                    Root.FromToJobs.Remove(currentSelectedJob);
                    string jobJson = JsonSerializer.Serialize(new FromToJobsRoot { FromToJobs = JobsFromFile() });
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

        private void FromToAdd_Click(object sender, RoutedEventArgs e)
        {
            FromToJob newJob = FromToJob.CreateNew();
            fromToJobs.Add(newJob);

            FromToJobsRoot updatedJobsRoot = new FromToJobsRoot { FromToJobs = fromToJobs };
            string jobJson = JsonSerializer.Serialize(updatedJobsRoot);

            File.WriteAllText(FromToJson(), jobJson);
            LoadJobs();
            Save(null, false);
        }

        private void FromToWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
        }
    }
}
