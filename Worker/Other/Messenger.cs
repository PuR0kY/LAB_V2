namespace LAB_V2.Worker.Other
{
    public class Messenger
    {
        private double progress;
        private MainWindow _mainWindow;

        public Messenger(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void UpdateStatus(string message, int progress)
        {
            SendMessage(message);
            SetProgressBar(progress);
        }

        public void ClearMessage()
        {
            if (System.Windows.Application.Current.Dispatcher.Thread == Thread.CurrentThread)
                _mainWindow.FileName.Content = "";

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _mainWindow.FileName.Content = "";
            }, System.Windows.Threading.DispatcherPriority.Input);

            SetProgressBar(0);
        }

        public void UpdateCopyingProgress(string text, int indexOfPart, int totalNumberOfParts)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => UpdateProgressAndStatusAsync(text, indexOfPart, totalNumberOfParts));
        }

        private void SetProgressBar(int progress)
        {
            if (System.Windows.Application.Current.Dispatcher.Thread == Thread.CurrentThread)
                _mainWindow.pbStatus.Value = progress;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _mainWindow.pbStatus.Value = progress;
            }, System.Windows.Threading.DispatcherPriority.Input);
        }

        private void SendMessage(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (System.Windows.Application.Current.Dispatcher.Thread == Thread.CurrentThread)
                _mainWindow.FileName.Content = text;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _mainWindow.FileName.Content = text;
            }, System.Windows.Threading.DispatcherPriority.Input);
        }

        private void UpdateProgressAndStatusAsync(string text, int indexOfPart, int totalNumberOfParts)
        {
            progress = (uint)Math.Abs(indexOfPart / (float)totalNumberOfParts * 100.0);
            _mainWindow.pbStatus.Value = progress;
            SendMessage(text);
        }
    }
}
