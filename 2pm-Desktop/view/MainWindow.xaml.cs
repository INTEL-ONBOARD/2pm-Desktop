using System;
using System.Net.NetworkInformation;
using System.Windows;

namespace _2pm_Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var workingArea = System.Windows.SystemParameters.WorkArea;

            double left = workingArea.Right - this.Width - 10;
            double top = workingArea.Bottom - this.Height - 10;

            this.Left = left;
            this.Top = top;

            do
            {
                try
                {
                    Ping myPing = new Ping();
                    string host = "www.nibmworldwide.com";
                    byte[] buffer = new byte[32];
                    int timeout = 1000;
                    PingOptions pingOptions = new PingOptions();
                    PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                    System.Diagnostics.Debug.WriteLine("done");
                    status.Visibility = Visibility.Visible;
                    status.Content = "Checking Connection...";
                    await Task.Delay(3000);

                    status.Visibility = Visibility.Hidden;
                    loadingScreen.Visibility = Visibility.Collapsed;
                    loginScreen.Visibility = Visibility.Visible;
                    await Task.Delay(1000);

                    break;
                }
                catch (Exception)
                {
                    loadingScreen.Visibility = Visibility.Visible;
                    status.Visibility = Visibility.Visible;
                    status.Content = "Something went wrong... Try again!";
                }
            } while (true);
        }

        private async void validNetwork(object sender, RoutedEventArgs e)
        {
            do
            {
                try
                {
                    Ping myPing = new Ping();
                    string host = "www.nibmworldwide.com";
                    byte[] buffer = new byte[32];
                    int timeout = 1000;
                    PingOptions pingOptions = new PingOptions();
                    PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                    System.Diagnostics.Debug.WriteLine("done");
                    status.Visibility = Visibility.Visible;
                    status.Content = "Checking Connection...";
                    await Task.Delay(3000);

                    status.Visibility = Visibility.Hidden;
                    loadingScreen.Visibility = Visibility.Hidden;
                    loginScreen.Visibility = Visibility.Visible;

                    await Task.Delay(1000);

                    break;
                }
                catch (Exception)
                {
                    loadingScreen.Visibility = Visibility.Hidden;
                    status.Visibility = Visibility.Visible;
                    status.Content = "Something went wrong... Try again!";
                }
            } while (true);

        }
    }
}
