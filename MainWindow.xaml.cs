using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LAB_V2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void LoadLNBs()
        {
            string[] arrayOfLNBs = {
                "\\\\fmwk-web13\\wwwroot\\DEV-MWJadroTest",
                "\\\\fmwk-web11\\wwwroot\\2201-MWJadroTest",
                "\\\\fmwk-web13\\DevPortal-Dev\\Debug\\AppService"
            };
            foreach (string lnb in arrayOfLNBs)
            {
                LNBs.Items.Add(lnb);
            }
        }

        private void LoadApps()
        {
            string path = "C:\\inetpub\\wwwroot"; //TODO: Load from Json
            string[] paths = Directory.GetDirectories(path);
            foreach (string path2 in paths)
            {
                LocalApps.Items.Add(path2);
            }
        }
    }
}