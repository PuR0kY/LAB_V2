using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace LAB_V2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        ShutdownMode shutdownMode = ShutdownMode.OnLastWindowClose; 
        public App()
        {
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
