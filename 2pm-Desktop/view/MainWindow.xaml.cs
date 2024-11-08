﻿using System;
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
using Newtonsoft.Json.Linq; 


using MediaColor = System.Windows.Media.Color;
using Newtonsoft.Json;
using _2pm_Desktop.model;
using _2pm_Desktop.view;

namespace _2pm_Desktop
{
    public partial class MainWindow : Window
    {
        private Thread _connectionCheckThread;
        private bool _isConnected;
        private bool _keepChecking;

        private bool _isReportRequired;

        List<Process> processList = new List<Process>();
        private bool multipleRunCount;
        Process[] processes;
        private bool isScreenshotActive;

        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        private bool isPaused;

        private int dailyReportId = 0;
        private int hourlyReportId = 1;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += Window_Closing;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;



            bool startLoggedIn = ((JToken)ReadUserSetting("StartLoggedIn")).Value<bool>();

            uname.Text = startLoggedIn ? ((JToken)ReadUserSetting("Username")).Value<string>() : string.Empty;
            pwd.Password = startLoggedIn ? ((JToken)ReadUserSetting("Password")).Value<string>() : string.Empty;

            if (startLoggedIn){StayLoggedInCheckbox.IsChecked = true;}else{StayLoggedInCheckbox.IsChecked = false;}
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
                    string host = "www.google.com";
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

            if (StayLoggedInCheckbox.IsChecked == true) {
                UpdateUserSetting("Username", une);
                UpdateUserSetting("Password", pwdd);
                UpdateUserSetting("StartLoggedIn", true);
            }
            else
            {
                UpdateUserSetting("Username", "");
                UpdateUserSetting("Password", "");
                UpdateUserSetting("StartLoggedIn", false);
            }

            string loginResult = await model.requestEngine.logInUser(une, pwdd);

            await Task.Delay(1000);
            if (loginResult == "true")
            {
                loginScreen.Visibility = Visibility.Hidden;
                defaultScreen.Visibility = Visibility.Hidden;
                status.Content = "";

                play.Visibility = Visibility.Visible;
                pause.Visibility = Visibility.Hidden;
                resume.Visibility = Visibility.Hidden;
                stop.Visibility = Visibility.Hidden;

                homeScreen.Visibility = Visibility.Visible;

                _isReportRequired = await model.requestEngine.IsUserInDepartmentAsync();
                //System.Diagnostics.Debug.WriteLine(_isReportRequired);
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

        private async void clickExit(object sender, RoutedEventArgs e)
        {

            loadingScreen.Visibility = Visibility.Hidden;
            loginScreen.Visibility = Visibility.Hidden;
            homeScreen.Visibility = Visibility.Hidden;
            settingScreen.Visibility = Visibility.Hidden;
            reportScreen.Visibility = Visibility.Hidden;
            defaultScreen.Visibility = Visibility.Visible;
            status.Visibility= Visibility.Visible;
            status.Content = "Saving please wait...";
            await Task.Delay(800);
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



                bool startLoggedIn = ((JToken)ReadUserSetting("StartLoggedIn")).Value<bool>();

                uname.Text = startLoggedIn ? ((JToken)ReadUserSetting("Username")).Value<string>() : string.Empty;
                pwd.Password = startLoggedIn ? ((JToken)ReadUserSetting("Password")).Value<string>() : string.Empty;
                if (startLoggedIn) { StayLoggedInCheckbox.IsChecked = true; } else { StayLoggedInCheckbox.IsChecked = false; }



                loginScreen.Visibility = Visibility.Visible;
                homeScreen.Visibility = Visibility.Hidden;
                userScreen.Visibility = Visibility.Hidden;

            }
            else
            {
                status.Content = "Logout failed. Please try again.";
            }
        }

        private async void punchIn_click(object sender, RoutedEventArgs e)
        {
            play.Visibility = Visibility.Hidden;
            pause.Visibility = Visibility.Visible;
            resume.Visibility = Visibility.Hidden;
            stop.Visibility = Visibility.Visible;

            //play.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 77 is 0.3 * 255
            //pause.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 26 is 0.1 * 255
            //resume.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 26 is 0.1 * 255
            //stop.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 26 is 0.1 * 255
            BlinkingEllipse.Fill = new SolidColorBrush(Colors.Green);
            BlinkingEllipse.Visibility = Visibility.Visible;
            subtile.Content = "Recording";

            if (!isPaused)
            {
                elapsedTime = TimeSpan.Zero;
                timeLabel.Content = "00:00:00"; // Reset the clock
            }

            timer.Start();
            string response = await requestEngine.punchin();
            if (response == "true") { System.Diagnostics.Debug.WriteLine("Punch in success!"); } else { System.Diagnostics.Debug.WriteLine("Faild attempt to punch in !"); }
            StartScreenshotProcess(true);
            isPaused = false;

        }

        private async void breakIn_click(object sender, RoutedEventArgs e)
        {
            //play.IsEnabled = false;
            //pause.IsEnabled = false;
            //resume.IsEnabled = true;
            //stop.IsEnabled = true;

            play.Visibility = Visibility.Hidden;
            pause.Visibility = Visibility.Hidden;
            resume.Visibility = Visibility.Visible;
            stop.Visibility = Visibility.Hidden;

            //play.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 77 is 0.3 * 255
            //pause.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 26 is 0.1 * 255
            //resume.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 26 is 0.1 * 255
            //stop.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 26 is 0.1 * 255
            BlinkingEllipse.Fill = new SolidColorBrush(Colors.Yellow);
            BlinkingEllipse.Visibility = Visibility.Visible;
            subtile.Content = "Paused";

            timer.Stop();
            string response = await requestEngine.breakin();
            if (response == "true") { System.Diagnostics.Debug.WriteLine("break in success!"); } else { System.Diagnostics.Debug.WriteLine("break in faild!"); }
            StartScreenshotProcess(false);
            isPaused = true;

        }

        private async void punchOut_click(object sender, RoutedEventArgs e)
        {
            //play.IsEnabled = true;
            //pause.IsEnabled = false;
            //resume.IsEnabled = false;
            //stop.IsEnabled = false;

            play.Visibility = Visibility.Visible;
            pause.Visibility = Visibility.Hidden;
            resume.Visibility = Visibility.Hidden;
            stop.Visibility = Visibility.Hidden;


            //play.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255));
            //pause.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255));
            //resume.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255));
            //stop.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255));
            BlinkingEllipse.Fill = new SolidColorBrush(Colors.Gray);
            BlinkingEllipse.Visibility = Visibility.Visible;
            subtile.Content = "Stopped";

            timer.Stop();
            string lastStoppedTime = elapsedTime.ToString(@"hh\:mm\:ss");
            System.Diagnostics.Debug.WriteLine($"Punch Out at: {lastStoppedTime}");

            int totalHours = (int)Math.Ceiling(elapsedTime.TotalHours);
            System.Diagnostics.Debug.WriteLine($"Total Hours (rounded): {totalHours}");
            
            
            System.Diagnostics.Debug.WriteLine($"bool (>>>>): {_isReportRequired}");

            if (_isReportRequired)
            {
                for (int i = 0; i < totalHours; i++)
                {
                    addreportHourly(this); // Add hourly reports
                }

                addreportDaily(this); // Add daily report
            }
            else
            {
                addreportDaily(this); // Only add daily report if no hourly report is required
            }

            StartScreenshotProcess(false);
            elapsedTime = TimeSpan.Zero;
            timeLabel.Content = "00:00:00";
            isPaused = false;

            reportScreen.Visibility = Visibility.Visible;
            homeScreen.Visibility = Visibility.Hidden;


        }

        private void addreportDaily(MainWindow win)
        {
            reportView.Content = reportPnaelView;
            report rp = new report(win);

            // Set unique ID for the daily report
            rp.id = dailyReportId.ToString();



            rp.title = "Daily Report";
            rp.subtitle = "Please fill out what you have done in the provided time frame";
            rp.input = ""; // Input field for the user
            reportPnaelView.Children.Add(rp); // Add to the report panel
        }

        private void addreportHourly(MainWindow win)
        {
            reportView.Content = reportPnaelView;
            report rp = new report(win);

            // Set unique ID for the hourly report
            rp.id = hourlyReportId.ToString();

            // Increment the hourly report ID for the next report
            hourlyReportId++;

            rp.title = "Hour Report";
            rp.subtitle = "Please fill out what you have done in the provided time frame";
            rp.input = ""; // Input field for the user
            reportPnaelView.Children.Add(rp); // Add to the report panel
        }

        private async void breakOut_click(object sender, RoutedEventArgs e)
        {
            //play.IsEnabled = false;
            //pause.IsEnabled = true;
            //resume.IsEnabled = false;
            //stop.IsEnabled = true;

            play.Visibility = Visibility.Hidden;
            pause.Visibility = Visibility.Visible;
            resume.Visibility = Visibility.Hidden;
            stop.Visibility = Visibility.Visible;

            //play.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 77 is 0.3 * 255
            //pause.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 26 is 0.1 * 255
            //resume.Background = new SolidColorBrush(MediaColor.FromArgb(26, 255, 255, 255)); // 26 is 0.1 * 255
            //stop.Background = new SolidColorBrush(MediaColor.FromArgb(77, 255, 255, 255)); // 26 is 0.1 * 255
            BlinkingEllipse.Fill = new SolidColorBrush(Colors.Green);
            BlinkingEllipse.Visibility = Visibility.Visible;
            subtile.Content = "Recording";

            if (isPaused)
            {
                timer.Start();
                StartScreenshotProcess(true);
                string response = await requestEngine.breakout();
                if (response == "true") { System.Diagnostics.Debug.WriteLine("break out success!"); } else { System.Diagnostics.Debug.WriteLine("break out faild!"); }

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

        public void UpdateUserSetting(string key, object newValue)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UserSettings.json");

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Read the existing JSON data
                string json = File.ReadAllText(filePath);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);

                // Update the specified key
                if (key == "Username")
                {
                    jsonObj.Username = newValue;
                }
                else if (key == "Password")
                {
                    jsonObj.Password = newValue;
                }
                else if (key == "StartLoggedIn")
                {
                    jsonObj.Settings.StartLoggedIn = newValue;
                }
                else if (key == "RunOnStartup")
                {
                    jsonObj.Settings.RunOnStartup = newValue;
                }
                else if (key == "RunInBackground")
                {
                    jsonObj.Settings.RunInBackground = newValue;
                }
                else
                {
                    throw new ArgumentException("Invalid key specified.");
                }

                // Serialize back to JSON
                string updatedJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(filePath, updatedJson);
            }
            else
            {
                throw new FileNotFoundException("User settings file not found.");
            }
        }

        public object ReadUserSetting(string key)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UserSettings.json");

            if (!File.Exists(filePath))
            {
                var defaultSettings = new
                {
                    Username = string.Empty,
                    Password = string.Empty,
                    Settings = new
                    {
                        StartLoggedIn = false,
                        RunOnStartup = false,
                        RunInBackground = false
                    }
                };

                string defaultJson = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented);
                File.WriteAllText(filePath, defaultJson);
            }

            string json = File.ReadAllText(filePath);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            switch (key)
            {
                case "Username":
                    return jsonObj.Username;
                case "Password":
                    return jsonObj.Password;
                case "StartLoggedIn":
                    return jsonObj.Settings.StartLoggedIn;
                case "RunOnStartup":
                    return jsonObj.Settings.RunOnStartup;
                case "RunInBackground":
                    return jsonObj.Settings.RunInBackground;
                default:
                    throw new ArgumentException("Invalid key specified.");
            }
        }

        private void issale_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            reportView.ScrollToHorizontalOffset(reportView.HorizontalOffset - 250);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {

            reportView.ScrollToHorizontalOffset(reportView.HorizontalOffset + 250);
        }

        private async void done(object sender, RoutedEventArgs e)
        {
            defaultScreen.Visibility = Visibility.Visible;

            homeScreen.Visibility = Visibility.Hidden;
            reportScreen.Visibility = Visibility.Hidden;
            status.Content = "checking....";

            try
            {
                string result = await requestEngine.UploadReportData(this);

                if (result.Equals("true"))
                {
                    status.Content = "";
                    homeScreen.Visibility = Visibility.Visible;
                    reportScreen.Visibility = Visibility.Hidden;

                }
                else
                {
                    status.Visibility = Visibility.Visible;
                    status.Content = "Reprot submission faild. Try again!";
                    reportScreen.Visibility = Visibility.Visible;
                    homeScreen.Visibility = Visibility.Hidden;


                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                defaultScreen.Visibility = Visibility.Hidden;
                if (homeScreen.Visibility == Visibility.Visible)
                {
                    status.Content = "";
                    reportPnaelView.Children.Clear();
                }
            }
        }




        private void goSettings(object sender, RoutedEventArgs e)
        {
            defaultScreen.Visibility = Visibility.Hidden;
            settingScreen.Visibility = Visibility.Visible;
            homeScreen.Visibility = Visibility.Hidden;
            reportScreen.Visibility = Visibility.Hidden;
            loginScreen.Visibility = Visibility.Hidden;
        }

        private void goInfo(object sender, RoutedEventArgs e)
        {
            defaultScreen.Visibility = Visibility.Hidden;
            settingScreen.Visibility = Visibility.Hidden;
            homeScreen.Visibility = Visibility.Visible;
            reportScreen.Visibility = Visibility.Hidden;
            loginScreen.Visibility = Visibility.Hidden;
            userScreen.Visibility = Visibility.Hidden;
        }

        private void gouser(object sender, RoutedEventArgs e)
        {
            defaultScreen.Visibility = Visibility.Hidden;
            settingScreen.Visibility = Visibility.Hidden;
            homeScreen.Visibility = Visibility.Hidden;
            reportScreen.Visibility = Visibility.Hidden;
            loginScreen.Visibility = Visibility.Hidden;
            userScreen.Visibility = Visibility.Visible;
        }

        private void clickBackHome(object sender, RoutedEventArgs e)
        {
            defaultScreen.Visibility = Visibility.Hidden;
            settingScreen.Visibility = Visibility.Hidden;
            homeScreen.Visibility = Visibility.Visible;
            reportScreen.Visibility = Visibility.Hidden;
            loginScreen.Visibility = Visibility.Hidden;
            userScreen.Visibility = Visibility.Hidden;
        }

        private void clickBackHome2(object sender, RoutedEventArgs e)
        {
            defaultScreen.Visibility = Visibility.Hidden;
            settingScreen.Visibility = Visibility.Hidden;
            homeScreen.Visibility = Visibility.Visible;
            reportScreen.Visibility = Visibility.Hidden;
            loginScreen.Visibility = Visibility.Hidden;
            userScreen.Visibility = Visibility.Hidden;
            ; ; ; ; ; ; ; ; ;
        }
    }
}

//{
//    "Username": "yourUsername",
//    "Password": "yourPassword",
//    "Settings": {
//        "StartLoggedIn": true,
//        "RunOnStartup": true,
//        "RunInBackground": true
//    }
//}

