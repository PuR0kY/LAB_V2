using LAB_V2.Worker;
using LAB_V2.Jobs;
using System.IO;
using System.Windows;
using System.Text.Json;
using LAB_V2.Windows.SettingsSaver;
using LAB_V2.Worker.Other;
using LAB_V2.Scripts;
using LAB_V2.Windows.Settings;
using System.ServiceModel;

namespace LAB_V2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Messenger Messenger => messenger ??= new Messenger(this);
        public string LastNetBuildPath;

        private Messenger messenger;
        private List<Job> jobs;
        private List<FromToJob> fromToJobs;
        private JobManager jobManager = new JobManager();
        private FromToJobManager fromToJobManager = new FromToJobManager();
        private JobsRoot jobsRoot;
        private FromToJobsRoot fromToJobsRoot;

        public MainWindow()
        {
            InitializeComponent();
            jobsRoot = jobManager.jobsRoot();
            jobs = jobsRoot.Jobs;
            fromToJobsRoot = fromToJobManager.fromToJobsRoot();
            LoadJobs();
            FromToLoad();
        }

        #region Checkboxes
        private void FromToCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            FromToPanel.Visibility = Visibility.Visible;
            OpenSettingsFromToButton.Visibility = Visibility.Visible;
            OpenSettingsButton.Visibility = Visibility.Collapsed;
            usePullRequest.IsChecked = false;
            PullRequest.Visibility = Visibility.Collapsed;
            TwoBranchMerger.IsChecked = false;
            TwoBrancher.Visibility = Visibility.Collapsed;
        }

        private void TwoBranchMerger_Checked(object sender, RoutedEventArgs e)
        {
            TwoBrancher.Visibility = Visibility.Visible;
            FromToCheckBox.IsChecked = false;
            FromToPanel.Visibility = Visibility.Collapsed;
            OpenSettingsFromToButton.Visibility = Visibility.Collapsed;
            usePullRequest.IsChecked = false;
            PullRequest.Visibility = Visibility.Collapsed;
            OpenSettingsButton.Visibility = Visibility.Visible;
        }

        private void usePullRequest_Checked(object sender, RoutedEventArgs e)
        {
            PullRequest.Visibility = Visibility.Visible;
            OpenSettingsButton.Visibility = Visibility.Visible;
            OpenSettingsFromToButton.Visibility = Visibility.Collapsed;
            TwoBranchMerger.IsChecked = false;
            TwoBrancher.Visibility = Visibility.Collapsed;
            FromToCheckBox.IsChecked = false;
            FromToPanel.Visibility = Visibility.Collapsed;
        }
        #endregion

        public async void StartButton(object sender, RoutedEventArgs e)
        {
            string? link = PRLink.Text.Equals("") ? null : PRLink.Text;

            Start.IsEnabled = false;
            await Processer.Run(link, jobs[ComboJobs.SelectedIndex], Messenger);
            Start.IsEnabled = true;
        }

        public void OpenSettings(object sender, RoutedEventArgs e)
        {
            jobManager.Show();
            this.Visibility = Visibility.Collapsed;
            LoadJobs();
        }

        private void OpenScripter_Click(object sender, RoutedEventArgs e)
        {
            Scripter scripter = new Scripter();
            scripter.Show();
            this.Visibility = Visibility.Collapsed;
        }

        private void OnJobManagerClosed(object sender, EventArgs e) => LoadJobs(); // Custom method to reload the ListBox      

        private void LoadJobs()
        {
            if (jobsRoot is null)
                throw new Exception("no jobsRoot pyco");
            jobManager.WindowClosed += OnFromToJobManagerClosed;
            ComboJobs.ItemsSource = jobsRoot.Jobs.Select(x => x.Name).ToList();
        }

        private void FromToLoad()
        {
            if(jobsRoot is null)
                throw new Exception("no jobsRoot pyco");
            fromToJobManager.WindowClosed += OnFromToJobManagerClosed;
            FromToJobs.ItemsSource = fromToJobsRoot.FromToJobs.Select(x => x.Name);
        }

        private void OnFromToJobManagerClosed(object sender, EventArgs e) => FromToLoad(); // Custom method to reload the ListBox

        private async void StartFromTo_Click(object sender, RoutedEventArgs e)
        {       
            if (StartFromTo.IsEnabled)
            {
                StartFromTo.IsEnabled = false;
                await Processer.FromToRun(fromToJobs[FromToJobs.SelectedIndex], Messenger);
                FromToJobs.SelectedIndex = -1; //deselect listItemů v jobListu

                messenger.UpdateStatus("All files copied successfully.",100);
            }            
        }

        private void OpenSettingsFromToButton_Click(object sender, RoutedEventArgs e)
        {
            FromToJobs.SelectedIndex = -1;
            FromToJobManager fromToJobManager = new FromToJobManager();
            fromToJobManager.Show();
            this.Visibility = Visibility.Collapsed;
        }

        private void FromToJobs_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (pbStatus.Value == 100)
                messenger.ClearMessage();            
            
            if (FromToJobs.SelectedIndex == -1)
                StartFromTo.IsEnabled = false;
            else if (FromToJobs.SelectedIndex >= 0)
                StartFromTo.IsEnabled = true;
        }

        private async void StartMerger_Click(object sender, RoutedEventArgs e)
        {
            StartMerger.IsEnabled = false;
            await Processer.TwoBranchRun(TargetBranch.Text, BranchToMerge.Text, jobs[ComboJobs.SelectedIndex], Messenger);
        }
    }  
}