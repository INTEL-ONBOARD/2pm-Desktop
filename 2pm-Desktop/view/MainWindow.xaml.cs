using System;
using System.Configuration;
using System.Net.NetworkInformation;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void txtChanged(object sender, TextChangedEventArgs e)
        {
            //statusLabel.Content = "Attempting to log in again. Please wait...";
            status.Visibility = Visibility.Hidden;
        }

        private void txtChanged(object sender, RoutedEventArgs e)
        {
            //statusLabel.Content = "Attempting to log in again. Please wait...";
            status.Visibility = Visibility.Hidden;
        }


        private async void login(object sender, RoutedEventArgs e)
        {
            loginScreen.Visibility = Visibility.Hidden;
            defaultScreen.Visibility = Visibility.Visible;

            status.Visibility = Visibility.Visible;
            status.Content = "Checking credentials....";
            //statusLabel.Visibility = Visibility.Visible;
            // Assuming model.requestEngine.ValidUser() was intended to call ValidUser()
            String une = uname.Text;
            String pwdd = pwd.Password.ToString();
            string loginResult = await model.requestEngine.logInUser(une, pwdd);

            await Task.Delay(1000);
            if (loginResult == "true")
            {
                loginScreen.Visibility = Visibility.Hidden;
                defaultScreen.Visibility = Visibility.Hidden;
                status.Content = "";
                homeScreen.Visibility = Visibility.Visible;
                //homePane.Visibility = Visibility.Visible;
                //attendencePane.Visibility = Visibility.Hidden;
            }
            else
            {
                loginScreen.Visibility = Visibility.Visible;
                defaultScreen.Visibility = Visibility.Hidden;
                status.Content = "Login failed.Please try again.";
                status.Visibility = Visibility.Visible;
            }
        }

        private void clickExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //private async void logout(object sender, RoutedEventArgs e)
        //{
        //    string logoutResult = await model.requestEngine.logOutUser();

        //    if (logoutResult == "true")
        //    {
        //        uname.Text = "";
        //        pwd.Clear();

        //        panelView.Children.Clear();
        //        if ((pause.Visibility == Visibility.Visible && play.Visibility == Visibility.Hidden) || (pause.Visibility == Visibility.Hidden && play.Visibility == Visibility.Visible))
        //        {
        //            pause.Visibility = Visibility.Hidden;
        //            play.Visibility = Visibility.Visible;
        //            _isRunning = false;
        //            _timer.Stop();
        //            _timeSpan = TimeSpan.Zero;

        //            UpdateTimeLabel();
        //            timerStatus.Content = "Stopped";
        //            StartScreenshotProcess(false);
        //            BlinkingEllipse.Fill = new SolidColorBrush(Colors.Gray);
        //            bgEc.Fill = new SolidColorBrush(Colors.Gray);
        //            await Task.Delay(1000);
        //        }


        //        loginScreen.Visibility = Visibility.Visible;
        //        welcomeScreen.Visibility = Visibility.Hidden;
        //        welcomeBox.Visibility = Visibility.Visible;
        //        stop.Visibility = Visibility.Hidden;

        //        homePane.Visibility = Visibility.Visible;
        //        infoPane.Visibility = Visibility.Hidden;
        //        settingsPane.Visibility = Visibility.Hidden;
        //        homeActive.Visibility = Visibility.Visible;
        //        SettingsActive.Visibility = Visibility.Hidden;
        //        infoActive.Visibility = Visibility.Hidden;
        //    }
        //    else
        //    {
        //        statusLabel.Content = "Logout failed. Please try again.";
        //    }
        //}


    }
}
