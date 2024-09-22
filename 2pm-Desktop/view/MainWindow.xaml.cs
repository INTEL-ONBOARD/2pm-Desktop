using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Drawing;
using System.Drawing.Imaging;

using MediaColor = System.Windows.Media.Color;

namespace _2pm_Desktop
{
    public partial class MainWindow : Window
    {
        private Thread _connectionCheckThread;
        private bool _isConnected;
        private bool _keepChecking;

        List<Process> processList = new List<Process>();
        private bool multipleRunCount;
        Process[] processes;
        private bool isScreenshotActive;

        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        private bool isPaused;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += Window_Closing;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
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
                            // Convert System.Windows.Media.Color to System.Drawing.Color
                            UpdateConnectionStatus(ConvertMediaColorToDrawingColor(System.Windows.Media.Colors.Green), "Internet Available");
                        }
                    }
                    else
                    {
                        if (_isConnected)
                        {
                            _isConnected = false;
                            UpdateConnectionStatus(ConvertMediaColorToDrawingColor(System.Windows.Media.Colors.Red), "Internet Lost");
                        }
                    }
                }
                catch (Exception)
                {
                    if (_isConnected)
                    {
                        _isConnected = false;
                        UpdateConnectionStatus(ConvertMediaColorToDrawingColor(System.Windows.Media.Colors.Red), "Internet Lost");
                    }
                }

                Thread.Sleep(5000);
            }
        }
        private System.Windows.Media.Color ConvertDrawingColorToMediaColor(System.Drawing.Color drawingColor)
        {
            return System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
        }
        private void UpdateConnectionStatus(System.Drawing.Color color, string message)
        {
            Dispatcher.Invoke(() =>
            {
                // Convert System.Drawing.Color to System.Windows.Media.Color
                var mediaColor = ConvertDrawingColorToMediaColor(color);

                // Apply converted color to SolidColorBrush
                connectionStatus.Fill = new SolidColorBrush(mediaColor);

                System.Diagnostics.Debug.WriteLine(message);
            });
        }

        private System.Drawing.Color ConvertMediaColorToDrawingColor(System.Windows.Media.Color mediaColor)
        {
            return System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
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



                elapsedTime = TimeSpan.Zero;
                isPaused = false;


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

        private async void clickLogout(object sender, RoutedEventArgs e)
        {
            string logoutResult = await model.requestEngine.logOutUser();

            if (logoutResult == "true")
            {
                uname.Text = "";
                pwd.Clear();

                if (!(pause.IsEnabled || resume.IsEnabled || stop.IsEnabled))
                {


                    BlinkingEllipse.Fill = new SolidColorBrush(Colors.Gray);
                    bgEc.Fill = new SolidColorBrush(Colors.Gray);
                    await Task.Delay(1000);
                }

                timer.Stop();
                elapsedTime = TimeSpan.Zero;
                timeLabel.Content = "00:00:00";
                isPaused = false;

                loginScreen.Visibility = Visibility.Visible;
                homeScreen.Visibility = Visibility.Hidden;
            }
            else
            {
                status.Content = "Logout failed. Please try again.";
            }
        }

        private void punchIn_click(object sender, RoutedEventArgs e)
        {
            play.IsEnabled = false;
            pause.IsEnabled = true;
            resume.IsEnabled = false;
            stop.IsEnabled = true;
            play.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 77 is 0.3 * 255
            pause.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 26 is 0.1 * 255
            resume.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 26 is 0.1 * 255
            stop.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 26 is 0.1 * 255
            BlinkingEllipse.Fill = new SolidColorBrush(Colors.Green);
            BlinkingEllipse.Visibility = Visibility.Visible;
            subtile.Content = "Recording";

            if (!isPaused)
            {
                elapsedTime = TimeSpan.Zero;
                timeLabel.Content = "00:00:00"; // Reset the clock
            }

            timer.Start();
            isPaused = false;

        }

        private void breakIn_click(object sender, RoutedEventArgs e)
        {
            play.IsEnabled = false;
            pause.IsEnabled = false;
            resume.IsEnabled = true;
            stop.IsEnabled = true;
            play.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 77 is 0.3 * 255
            pause.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 26 is 0.1 * 255
            resume.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 26 is 0.1 * 255
            stop.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 26 is 0.1 * 255
            BlinkingEllipse.Fill = new SolidColorBrush(Colors.Yellow);
            BlinkingEllipse.Visibility = Visibility.Visible;
            subtile.Content = "Paused";

            timer.Stop();
            isPaused = true;

        }

        private void punchOut_click(object sender, RoutedEventArgs e)
        {
            play.IsEnabled = true;
            pause.IsEnabled = false;
            resume.IsEnabled = false;
            stop.IsEnabled = false;
            play.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 77 is 0.3 * 255
            pause.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 26 is 0.1 * 255
            resume.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 26 is 0.1 * 255
            stop.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 26 is 0.1 * 255
            BlinkingEllipse.Fill = new SolidColorBrush(Colors.Gray);
            BlinkingEllipse.Visibility = Visibility.Visible;
            subtile.Content = "Stopped";

            timer.Stop();
            elapsedTime = TimeSpan.Zero;
            timeLabel.Content = "00:00:00";
            isPaused = false;

        }

        private void breakOut_click(object sender, RoutedEventArgs e)
        {
            play.IsEnabled = false;
            pause.IsEnabled = true;
            resume.IsEnabled = true;
            stop.IsEnabled = true;
            play.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 77 is 0.3 * 255
            pause.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 26 is 0.1 * 255
            resume.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 26 is 0.1 * 255
            stop.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 26 is 0.1 * 255
            BlinkingEllipse.Fill = new SolidColorBrush(Colors.Green);
            BlinkingEllipse.Visibility = Visibility.Visible;
            subtile.Content = "Recording";

            if (isPaused)
            {
                timer.Start();
            }

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            timeLabel.Content = elapsedTime.ToString(@"hh\:mm\:ss");
        }

        private async void StartScreenshotProcess(bool initialState)
        {
            isScreenshotActive = initialState;
            Random random = new Random();


            int randomIntervalSeconds = random.Next(1, 26);
            System.Diagnostics.Debug.WriteLine($"{randomIntervalSeconds} ms");
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(randomIntervalSeconds)
            };

            timer.Tick += async (sender, args) =>
            {
                if (isScreenshotActive)
                {
                    string path = await TakeScreenshot();
                    //update(path);

                    int newRandomIntervalSeconds = random.Next(1, 26);
                    timer.Interval = TimeSpan.FromSeconds(newRandomIntervalSeconds);
                    System.Diagnostics.Debug.WriteLine($"Next interval: {newRandomIntervalSeconds} seconds");


                }
                else
                {
                    timer.Stop();
                    timer.Tick -= Timer_Tick;
                }
            };

            timer.Start();
        }

        private async Task<string> TakeScreenshot()
        {

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string folderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "upload");
            string filename = System.IO.Path.Combine(folderPath, $"Screenshot_{timestamp}.png");


            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            var screenWidth = (int)SystemParameters.PrimaryScreenWidth;
            var screenHeight = (int)SystemParameters.PrimaryScreenHeight;

            using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
            {

                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                }


                await Task.Run(() => bitmap.Save(filename, ImageFormat.Png));
                System.Diagnostics.Debug.WriteLine("Saved :::::" + filename);

            }


            return filename;
        }
    }
}
