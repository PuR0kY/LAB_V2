using LAB_V2.Scripts;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace LAB_V2.Windows.Settings
{
    /// <summary>
    /// Interaction logic for Scripter.xaml
    /// </summary>
    public partial class Scripter : Window
    {
        private ScripterWorker worker = new ScripterWorker();
        private string? path;

        public Scripter()
        {
            InitializeComponent();
            ScriptList.ItemsSource = worker.GetScripts();
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void RunScript_Click(object sender, RoutedEventArgs e)
        {
            if (ScriptParams.Text is "")            
                await worker.Execute(ScriptList.SelectedItem.ToString());

            else await worker.Execute(ScriptList.SelectedItem.ToString(), $@"{ScriptParams.Text}");
        }

        private void ScriptList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            path = ScriptList.SelectedItem.ToString();
            ScriptValue.Text = worker.GetScript(path);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(path, ScriptValue.Text);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            path = ScriptList.SelectedItem.ToString();
            File.Delete(path);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            File.Create(worker.ScriptDirectory("NewScript.ps1"));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
        }
    }
}
