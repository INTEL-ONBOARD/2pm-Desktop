using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace _2pm_Desktop
{
    public partial class MainWindow : Window
    {
        private Thread _connectionCheckThread;
        private bool _isConnected;
        private bool _keepChecking;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += Window_Closing;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            var workingArea = System.Windows.SystemParameters.WorkArea;
            double left = workingArea.Right - this.Width - 10;
            double top = workingArea.Bottom - this.Height - 10;
            this.Left = left;
            this.Top = top;

            _keepChecking = true;
            _connectionCheckThread = new Thread(CheckInternetConnection)
            {
                IsBackground = true 
            };
            _connectionCheckThread.Start();

            loadingScreen.Visibility = Visibility.Visible;
            status.Visibility = Visibility.Visible;
            status.Content = "Checking Connection...";
            await Task.Delay(3000);


            CheckInitialConnection();
        }

        private void CheckInitialConnection()
        {
            do
            {
                try
                {
                    Ping myPing = new Ping();
                    string host = "www.nibmworldwide.com";
                    PingReply reply = myPing.Send(host, 1000);
                    if (reply.Status == IPStatus.Success)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            System.Diagnostics.Debug.WriteLine("Initial Connection Successful!");
                            status.Visibility = Visibility.Hidden;
                            status.Content = "";
                            loadingScreen.Visibility = Visibility.Collapsed;
                            loginScreen.Visibility = Visibility.Visible;
                        });
                        break;
                    }
                }
                catch (Exception)
                {
                    Dispatcher.Invoke(() =>
                    {
                        loadingScreen.Visibility = Visibility.Visible;
                        status.Visibility = Visibility.Visible;
                        status.Content = "Something went wrong... Try again!";
                    });
                }
            } while (true);
        }

        private void CheckInternetConnection()
        {
            while (_keepChecking)
            {
                try
                {
                    Ping myPing = new Ping();
                    string host = "www.nibmworldwide.com";
                    PingReply reply = myPing.Send(host, 1000);
                    if (reply.Status == IPStatus.Success)
                    {
                        if (!_isConnected)
                        {
                            _isConnected = true;
                            UpdateConnectionStatus(Colors.Green, "Internet Available");
                        }
                    }
                    else
                    {
                        if (_isConnected)
                        {
                            _isConnected = false;
                            UpdateConnectionStatus(Colors.Red, "Internet Lost");
                        }
                    }
                }
                catch (Exception)
                {
                    if (_isConnected)
                    {
                        _isConnected = false;
                        UpdateConnectionStatus(Colors.Red, "Internet Lost");
                    }
                }

                Thread.Sleep(5000); 
            }
        }

        private void UpdateConnectionStatus(Color color, string message)
        {
            Dispatcher.Invoke(() =>
            {
                connectionStatus.Fill = new SolidColorBrush(color);
                System.Diagnostics.Debug.WriteLine(message);
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _keepChecking = false; 
            if (_connectionCheckThread != null && _connectionCheckThread.IsAlive)
            {
                _connectionCheckThread.Join(); 
            }
        }
    }
}
