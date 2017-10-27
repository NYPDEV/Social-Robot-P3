using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Speech.Recognition;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Drawing;
using ROBOTIS;
using System.Configuration;
using System.IO.Ports;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;
using Google.Apis.YouTube.v3.Data;
using WindowsInput;
using WebcamControl;
using System.Windows.Data;
using Microsoft.Expression.Encoder.Devices;
using AForge.Video.DirectShow;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Net.NetworkInformation;

namespace SocialRobot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public static DispatcherTimer ReadNewsTimer = new DispatcherTimer();//The timer used in reading news function
        public static DispatcherTimer GrammarTimer = new DispatcherTimer();//The timer used in reading news function
        public static DispatcherTimer WeatherIconTimer = new DispatcherTimer();//The timer used in weather icon
        public static DispatcherTimer RecordingTimer = new DispatcherTimer();//The timer used in record
        public static DispatcherTimer CoolDownTimer = new DispatcherTimer();//The timer used in conversation cool down
        public static DispatcherTimer SwitchToEnglishTimer = new DispatcherTimer();//The timer used in switch language to English
        public static DispatcherTimer PingerTimer = new DispatcherTimer();//The timer used to constantly check for internet ping (Currently used to ping Google)

        public static bool DemoMode = false;
        public static bool IsListeningFromUser = false;

        string ResultName = null;
        string RuleName = null;
        public static string YesNoStatus = null;

        public bool EnglishNewsMark = true;
        public static bool RecordStartMark = false;
        public static bool FacialExpressionMark = true;
        public static bool FacebookMark = false;
        public static bool VidTrig = false;
        public static bool ChannelTrig = false;
        public static bool PlaylistTrig = false;
        public static bool CloseVid = false;
        public static bool CloseChnl = false;
        public static bool ClosePL = false;

        //Google Speech Async
        public Application.GoogleSpeech googleSpeech = null;
         //googleSpeech = Application.GoogleSpeech GoogleSpeech();
       
        Function.Mic mic = new Function.Mic();

        public Function.Speech_Recognition SR = new Function.Speech_Recognition();

        public Function.Record Record = new Function.Record();

        public Function.WitAnalysis WIT = new Function.WitAnalysis();
        public Application.ReadNews ReadNewsFunction = new Application.ReadNews();
        public Application.GoogleCalender GoogleCalenderFunction = new Application.GoogleCalender();
        public Function.RosBridge.RosBridge RosBridge = new Function.RosBridge.RosBridge();
        public Application.IRKitManager IRKitManager = new Application.IRKitManager();
        public Application.SkypeApp.Skype Skype = null;
        public Application.GetWeather GetWeather = new Application.GetWeather();
        public Application.ReadNews ReadNews = new Application.ReadNews();
        public Function.UIHelper UIHelper;

        Function.Motor Motor = new Function.Motor();
        Function.Speech_Recognition SpeechRecord = new Function.Speech_Recognition();
        Function.MicVolumeDetection volumeDectection = null;
        Application.Facebook facebook;

        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;

        public MainWindow()
        {
            UIHelper = new Function.UIHelper(this);
            //Vision Part Start
            MyCamera.PortName = ConfigurationManager.AppSettings["CAMERA_COM_PORT_NAME"].ToString();//For adjust com port, go to app config to change.
            MyCamera.BaudRate = Convert.ToInt32(9600);
            MyCamera.Handshake = Handshake.None;
            MyCamera.Parity = Parity.None;
            MyCamera.DataBits = 8;
            MyCamera.StopBits = StopBits.One;
            MyCamera.ReadTimeout = 1000;
            MyCamera.WriteTimeout = 50;
            try
            {
                MyCamera.Open();
            }
            catch
            {
                MessageBox.Show("Camera can not be detected! Enter Demo Mode!");
                MainWindow.DemoMode = true;
                try {
                    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                    {
                        DemoModeImage.Visibility = Visibility.Visible;
                    }));
                }
                catch
                {

                }
                OKAOConnected = false;
            }

            //Vision Part End

            ReadNewsTimer.Tick += new EventHandler(ReadNewsTimer_TimeUp);
            ReadNewsTimer.Interval = new TimeSpan(0, 0, 35);
            GrammarTimer.Tick += new EventHandler(GrammarTimer_TimeUp);
            GrammarTimer.Interval = new TimeSpan(0, 0, 30);
            WeatherIconTimer.Tick += new EventHandler(WeatherIconTimer_TimeUp);
            WeatherIconTimer.Interval = new TimeSpan(0, 0, 10);
            RecordingTimer.Tick += new EventHandler(RecordingTimer_TimeUp);
            RecordingTimer.Interval = new TimeSpan(0, 0, 2);
            CoolDownTimer.Tick += new EventHandler(CoolDownTimer_TimeUp);
            CoolDownTimer.Interval = new TimeSpan(0, 0, 15);
            FaceTrackingTimer.Tick += new EventHandler(FaceTrackingTimer_TimesUp);
            FaceTrackingTimer.Interval = TimeSpan.FromMilliseconds(2000);
            SwitchToEnglishTimer.Tick += new EventHandler(SwitchToEnglish);
            SwitchToEnglishTimer.Interval = new TimeSpan(0, 0, 2);
            PingerTimer.Tick += new EventHandler(PingerRecieve_TimeUp);
            PingerTimer.Interval = new TimeSpan(0, 0, 10);

            try
            {
                Function.FaceLED.Instance.Normal();
                if (!DemoMode)
                {
                    FaceTrackingTimer.Start();
                }
            }
            catch
            {
                MessageBox.Show("Error while initialize LED or face tracking timer.");
            }

            Motor.MotorInitialize();

            SR.SpeechRecognized();

            SR.SpeechRecognition.SpeechRecognized += SR_SpeechRecognized;

            SR.SpeechRecognition.AudioLevelUpdated += SR_AudioLevelUpdated;         // English

  //          Record.OnSpeechRecognized += (Text) => UIHelper.UserSpeech(Text, Function.UIHelper.SpeechState.End);

            Function.Record.OnUploadToFacebook += (Text) => UploadToFacebook(Text);

            Function.Record.OnYouTubeSearch += (Text) => AskForYouTubeSearch(Text);

            Function.WitAnalysis.OnSpeechRecognized += (Text) => UIHelper.RobotSpeech(Text);

            Function.WitAnalysis.OnRadioControl += (RadioCommand) => UIHelper.RadioControl(RadioCommand);

            Function.WitAnalysis.OnSwitchToChinese += () => UIHelper.SwitchToChinese();

            Function.WitAnalysis.OnCapture += () => Capture();

            Function.WitAnalysis.OnCloseVideo += () => CloseVideo();

            Function.Text_To_Speech.OnRobotSpeech += (Text) => UIHelper.RobotSpeech(Text);

            Application.GetWeather.OnWeatherTextUpdate += (temp) => UIHelper.WeatherTextUpdate(temp);

            Application.GetWeather.OnWeatherIconUpdate += (WeatherBit) => UIHelper.WeatherIconUpdate(WeatherBit);

            Function.FaceLED.MbedToDemoMode += () => ToDemoMode();

            FaceTrackingTimer.Tick += new EventHandler(UserDataUpdate);

            volumeDectection = new Function.MicVolumeDetection();

            //volumeDectection.VolumeThresholdExceed = (() =>
            //{
            //    if (!IsListeningFromUser)
            //    {
            //        IsListeningFromUser = true;
            //        RecordingTimer.Stop();
            //        Function.FaceLED.Instance.cheekgreen();
            //        Function.Record.RecordingMark = true;
            //        if(CoolDownTimer.IsEnabled)
            //        {
            //            CoolDownTimer.Stop();
            //        }
            //    }
            //});

            //Google Speech Aync
            string lang = "en-US";
            switch (Function.Speech_Recognition.Language)
            {
                case "English": lang = "en-US"; break;
                case "Malay": lang = "ms-MY"; break;
                default: lang = "en-US"; break;
            }
            Function.WitAnalysis WitAI = new Function.WitAnalysis();
            Application.GoogleSpeech.init();
            Function.Mic.StartRecording();
            Function.Mic.DataAvailable += (ss, ee) => Application.GoogleSpeech.WaveInDataAvailable(ss, ee);
            Application.GoogleSpeech.SpeechReceived += (ss, state) =>
            {
                TextBlock UserSpeechTextBlock;
               

                if (state == Function.UIHelper.SpeechState.Partial)
                    Dispatcher.Invoke(() =>
                    {
                        //UserSpeechTextBlock = new TextBlock() { Text = ss + "\r", FontSize = 36, TextWrapping = TextWrapping.Wrap, Foreground = System.Windows.Media.Brushes.LightGray, TextAlignment = TextAlignment.Right };
                        UIHelper.UserSpeech(ss,state);
                       // Console.WriteLine("222");
                    });

                else
                if(state == Function.UIHelper.SpeechState.End)
                {
                    Dispatcher.Invoke(() =>
                    {
                        //partialBox.Clear();
                        //UserSpeechTextBlock = new TextBlock() { Text = ss + "\r\n", FontSize = 36, TextWrapping = TextWrapping.Wrap, Foreground = System.Windows.Media.Brushes.LightGray, TextAlignment = TextAlignment.Right };
                        //UIStackPenal.Children.Add(UserSpeechTextBlock);
                        UIHelper.UserSpeech(ss, state);
                        WitAI.StartProcessing(ss, lang);
                        Console.WriteLine(ss);
                        //finalBox.Text += ss + "\r\n";
                    });
                }
               // Console.WriteLine("123");
             };
            InitializeComponent();

            //Webcam Part Start

            Binding binding_1 = new Binding("SelectedValue");
            binding_1.Source = VideoDevicesComboBox;
            WebcamCtrl.SetBinding(Webcam.VideoDeviceProperty, binding_1);

            Binding binding_2 = new Binding("SelectedValue");
            binding_2.Source = AudioDevicesComboBox;
            WebcamCtrl.SetBinding(Webcam.AudioDeviceProperty, binding_2);

            // Create directory for saving video files
            string videoPath = @"C:\VideoClips";

            if (!Directory.Exists(videoPath))
            {
                Directory.CreateDirectory(videoPath);
            }
            // Create directory for saving image files
            string imagePath = @"C:\WebcamSnapshots";

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            // Set some properties of the Webcam control
            WebcamCtrl.VideoDirectory = videoPath;
            WebcamCtrl.ImageDirectory = imagePath;
            WebcamCtrl.FrameRate = 30;
            WebcamCtrl.FrameSize = new System.Drawing.Size(640, 480);

            // Find available a/v devices
            var vidDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
            var audDevices = EncoderDevices.FindDevices(EncoderDeviceType.Audio);
            VideoDevicesComboBox.ItemsSource = vidDevices;
            AudioDevicesComboBox.ItemsSource = audDevices;
            VideoDevicesComboBox.SelectedIndex = 0;
            AudioDevicesComboBox.SelectedIndex = 0;

            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FinalFrame = new VideoCaptureDevice();

            StartCapture(); 
            //StartCapture_Webcam(); // modified
            //Webcam Part End

            //senseManager.Init();

            Record.RecordStart();
            RecordingTimer.Start();

            Language_text.Text = Function.Speech_Recognition.Language;

            GoogleCalenderFunction.LoadReminder();
//            RosBridge.Connect();
            CoolDownTimer.Start();
            WakeUp = true;

            PingerTimer.Start();

            #region Radio Icons Visibility
            Kiss92.Visibility = Visibility.Hidden;
            Yes933.Visibility = Visibility.Hidden;
            Class95.Visibility = Visibility.Hidden;
            Love972.Visibility = Visibility.Hidden;
            FM988.Visibility = Visibility.Hidden;
            UFM1003.Visibility = Visibility.Hidden;
            FM924.Visibility = Visibility.Hidden;
            FM987.Visibility = Visibility.Hidden;
            Lush995.Visibility = Visibility.Hidden;
            Gold905.Visibility = Visibility.Hidden;
            Capital958.Visibility = Visibility.Hidden;
            CloseRadio.Visibility = Visibility.Hidden;
            RadioStation.Visibility = Visibility.Hidden;
            CloseRadio_Copy.Visibility = Visibility.Hidden;
            PreviousStation.Visibility = Visibility.Hidden;
            NextStation.Visibility = Visibility.Hidden;
            BackToRadioSelect.Visibility = Visibility.Hidden;
            #endregion Radio Icons Visibility

            ImageBrush brush1 = new ImageBrush();
            brush1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "image/goSilence.png"));
            WakeUpButton.Background = brush1;

            facebook = new Application.Facebook(webBrowser);
        }
        #region CameraInitialize
        public void CameraInitialize()
        {
            MyCamera.PortName = ConfigurationManager.AppSettings["CAMERA_COM_PORT_NAME"].ToString();//For adjust com port, go to app config to change.
            MyCamera.BaudRate = Convert.ToInt32(9600);
            MyCamera.Handshake = Handshake.None;
            MyCamera.Parity = Parity.None;
            MyCamera.DataBits = 8;
            MyCamera.StopBits = StopBits.One;
            MyCamera.ReadTimeout = 1000;
            MyCamera.WriteTimeout = 50;
            try
            {
                MyCamera.Open();
            }
            catch
            {
                MessageBox.Show("Camera can not be detected! Enter Demo Mode!");
                DemoMode = true;
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                {
                    DemoModeImage.Visibility = Visibility.Visible;
                }));
                OKAOConnected = false;
            }
        }
        #endregion CameraInitialize
        public void MaximizeToSecondaryMonitor()
        {
            var secondaryScreen = System.Windows.Forms.Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

            if (secondaryScreen != null)
            {
                var workingArea = secondaryScreen.WorkingArea;
                this.Left = workingArea.Left;
                this.Top = workingArea.Top;
                this.Width = workingArea.Width;
                this.Height = workingArea.Height;

                if (this.IsLoaded)
                {
                    this.WindowState = WindowState.Maximized;
                }
            }
        }

        int LastNum = 8;

        private void CoolDownTimer_TimeUp(object sender, EventArgs e)
        {
            CoolDownTimer.Stop();
            //Emotion = "Sadness";
            //NumberOfPerson = 1;

            if (NumberOfPerson >= 1 && Function.Speech_Recognition.Language == "English")
            {
                Random RanNum = new Random();
                int ConversationNum = RanNum.Next(1, 8);

                if (ConversationNum == LastNum)
                    ConversationNum = RanNum.Next(1, 8);

                //while (ConversationNum == LastNum)
                //{
                //    ConversationNum = RanNum.Next(1, 8);
                //}

                //ConversationNum = 7;
                switch (ConversationNum)
                {
                    case 1:
                        Function.FaceLED.Instance.Happy();
                        Thread.Sleep(500);
                        if(UserName!="Unknown")
                        {
                            WIT.TTS.Speaking("Hello, " + UserName);
                        }
                        else
                        {
                            WIT.TTS.Speaking("Nice to meet you.");
                        }
                        break;
                    case 2:
                        Function.FaceLED.Instance.Happy();
                        Thread.Sleep(500);
                        if (UserName != "Unknown")
                        {
                            WIT.TTS.Speaking("What can I do for you, " + UserName);
                        }
                        else
                        {
                            WIT.TTS.Speaking("What can I do for you.");
                        }
                        break;
                    case 3:
                        string StationUrl;
                        switch (Emotion)
                        {
                            case "Happiness":
                                Function.FaceLED.Instance.Happy();
                                Function.Motor.Instance.EyesBlink();
                                Thread.Sleep(500);
                                Function.Motor.Instance.EyesBlink();
                                Thread.Sleep(500);
                                if (!CaptureMark && CameraConnected)
                                {
                                    WIT.TTS.Speaking("You look so happy now, so I took a photo for you. Do you want to share this photo by Facebook?");
                                    while (Function.Text_To_Speech.SpeakingMark) ;
                                    Capture();
                                }
                                GrammarTimer.Start();// addedd in 20170721
                                break;

                            case "Surprise":
                                Function.FaceLED.Instance.Smile();
                                Function.Motor.Instance.EyesBlink();
                                Thread.Sleep(500);
                                Function.Motor.Instance.EyesBlink();
                                Thread.Sleep(500);
                                WIT.TTS.Speaking("Why are you so Surprise!!!");
                                break;

                            case "Anger":
                                Function.FaceLED.Instance.Fear();
                                if (!Function.WitAnalysis.RadioPlayingMark)
                                {
                                    Thread.Sleep(1000);
                                    WIT.TTS.Speaking("Relax, please relax! Let me play you a radio.");
                                    Function.WitAnalysis.RadioPlayingMark = true;
                                    if (Player.Source != null)
                                    {
                                        Player.LoadedBehavior = MediaState.Manual;
                                        Player.Stop();
                                        Player.Source = null;
                                    }
                                    Player.LoadedBehavior = MediaState.Manual;
                                    RadioStation.Content = "Symphony FM 924";// changed from KISS 92
                                    StationUrl = "http://mediacorp.rastream.com/924fm";
                                    Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
                                    Player.Play();
                                    ShowRadioStation();
                                }
                                break;

                            case "Sadness":
                                Function.FaceLED.Instance.Surprise();
                                if(!Function.WitAnalysis.RadioPlayingMark)
                                {
                                    Thread.Sleep(1000);
                                    WIT.TTS.Speaking("You look upset now, Let me play you a radio.");//, maybe I can tell a joke to you to make you happy?
                                    Function.WitAnalysis.RadioPlayingMark = true;
                                    if (Player.Source != null)
                                    {
                                        Player.LoadedBehavior = MediaState.Manual;
                                        Player.Stop();
                                        Player.Source = null;
                                    }
                                    Player.LoadedBehavior = MediaState.Manual;
                                    RadioStation.Content = "Symphony FM 924";
                                    StationUrl = "http://mediacorp.rastream.com/924fm";
                                    Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
                                    Player.Play();
                                    ShowRadioStation();
                                }
                                break;

                            //case "Contempt"://鄙视
                            //    Function.FaceLED.Instance.Angry();
                            //    Function.Motor.Instance.EyesHalfClose();
                            //    Thread.Sleep(1000);
                            //    switch (language)
                            //    {
                            //        case "English":
                            //            SRG.SRE_Speech.SpeakAsync("Why?");
                            //            break;
                            //        case "Chinese":
                            //            SRG.SRECN_Speech.SpeakAsync("为什么要露出这样的表情");
                            //            break;
                            //        case "Japanese":
                            //            SRG.SREJP_Speech.SpeakAsync("何を！");
                            //            break;
                            //    }
                            //    break;

                            //case "Disgust":
                            //    Function.FaceLED.Instance.Disgust();
                            //    Function.Motor.Instance.EyesHalfClose();
                            //    Thread.Sleep(1000);
                            //    switch (language)
                            //    {
                            //        case "English":
                            //            SRG.SRE_Speech.SpeakAsync("Hey!");
                            //            break;
                            //        case "Chinese":
                            //            SRG.SRECN_Speech.SpeakAsync("你肚子痛吗？");
                            //            break;
                            //        case "Japanese":
                            //            SRG.SREJP_Speech.SpeakAsync("気持ち悪い。");
                            //            break;
                            //    }
                            //    break;

                            //case "Fear":
                            //    Function.FaceLED.Instance.Fear();
                            //    Function.Motor.Instance.EyesHalfClose();
                            //    Thread.Sleep(1000);
                            //    switch (language)
                            //    {
                            //        case "English":
                            //            SRG.SRE_Speech.SpeakAsync("Don't be scared. Let's play a game.");
                            //            break;
                            //        case "Chinese":
                            //            SRG.SRECN_Speech.SpeakAsync("不要怕 不要慌 我不会突然爆炸。");
                            //            break;
                            //        case "Japanese":
                            //            SRG.SREJP_Speech.SpeakAsync("大丈夫だから");
                            //            break;
                            //    }
                            //    break;

                            default:
                                break;
                        }
                        break;
                    case 4:
                        WIT.TTS.Speaking("Do you want me play some radio for you?");

                        while (Function.Text_To_Speech.SpeakingMark) ;

                        IsListeningFromUser = true;

                        SR.LoadGrammar(SR.SGM_AskForYesNo);
                        YesNoStatus = "AskForRadio";
                        GrammarTimer.Start();
                        break;
                    case 5:
                        GetWeather.GetTheWeather("Singapore", "now");
                        break;
                    case 6:
                        WIT.TTS.Speaking("Do you want me read some news for you?");

                        while (Function.Text_To_Speech.SpeakingMark) ;

                        IsListeningFromUser = true;

                        SR.LoadGrammar(SR.SGM_AskForYesNo);
                        YesNoStatus = "AskForNews";
                        GrammarTimer.Start();
                        break;
                    case 7:
                        GoogleCalenderFunction.ReadReminder();
                        break;
                }
                //WIT.TTS.Speaking(ConversationNum.ToString());
                LastNum = ConversationNum;
            }
            CoolDownTimer.Start();
        }

        private void terminate()
        {
            try
            {
                FaceTrackingTimer.Stop();
                FaceTrackingTimer = null;
            }
            catch { }
            Thread.Sleep(100);
            for (int i = 0; i < 2; i++)
            {
                Motor.RobotSleep();
                Thread.Sleep(100);
                Motor.EyesClose();
                Thread.Sleep(100);
                Motor.RightArmRest();
                Thread.Sleep(100);
                Motor.LeftArmRest();
                Thread.Sleep(100);
                try
                {
                    Function.FaceLED.Instance.blank();
                }
                catch
                {

                }
            }
            try
            {
                FinalFrame.Stop();
            }
            catch
            {

            }
            Motor.TopNeckRelease();
            Motor.BtmNeckRelease();
            GC.Collect();
            Environment.Exit(Environment.ExitCode);
        }
        private void Esc_click(object sender, RoutedEventArgs e)
        {
           this.Close();
        }

        private void RecordingTimer_TimeUp(object sender, EventArgs e)
        {
            RecordingTimer.Stop();
            Record.RecordAbandon();
            Thread.Sleep(10);
            Record.RecordStart();
            RecordingTimer.Start();
        }

        private void PingerRecieve_TimeUp(object sender, EventArgs e)
        {
            PingerTimer.Stop();
            PingInt();
            PingerTimer.Start();
        }

        private void PingInt()
        {
            try
            {
                Ping IntPing = new Ping();
                PingReply PngReply = IntPing.Send("www.google.com", 10000);
                if(PngReply != null)
                {
                    Console.WriteLine("Status : " + PngReply.Status + "\n Time : " + PngReply.RoundtripTime.ToString() + "\n Address : " + PngReply.Address);
                    if(PngReply.Status == IPStatus.Success)
                    {
                        if (PngReply.RoundtripTime > 200 && PngReply.RoundtripTime <= 700)
                        {
                            ConnectLabel.Content = "Slow Connection";
                            return;
                        }
                        else if (PngReply.RoundtripTime > 700)
                        {
                            ConnectLabel.Content = "Very Slow Connection";
                            return;
                        }
                        else if (PngReply.RoundtripTime < 10)
                        {
                            ConnectLabel.Content = "Good Connection";
                            return;
                        }
                        if (PngReply.RoundtripTime >= 11 && PngReply.RoundtripTime <= 199)
                        {
                            ConnectLabel.Content = "Average Connection";
                            return;
                        }
                    }
                    else if (PngReply.Status == IPStatus.TimedOut)
                    {
                        Console.WriteLine("Ping showed time out. Please check if internet is stable/Connected");
                    }
                    else
                    {
                        Console.WriteLine("Ping did not return response and it was not due to timeout. Please check internet properly.");
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error in Pinging Check codes please.");
            }
        }

        int i = 0;

        public static bool AnalysisMark = false;

        private void SR_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            SR.SpeechRecognition.AudioLevelUpdated -= SR_AudioLevelUpdated;
            if (Function.Record.RecordingMark  && !AnalysisMark)
            {
                if (e.AudioLevel <= 6)
                {
                    i++;
                }
                else
                {
                    i = 0;
                }
                if (i >= 6)
                {
                    i = 0;
                    IsListeningFromUser = false;
                    AnalysisMark = true;
                    Record.RecordStop();
                    Thread.Sleep(10);
                    Record.RecordStart();
                    RecordingTimer.Start();
                }
            }
            Volume.Value = e.AudioLevel * 5;
            SR.SpeechRecognition.AudioLevelUpdated += SR_AudioLevelUpdated;
        }

        void SR_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            SR.SpeechRecognition.SpeechRecognized -= SR_SpeechRecognized;
            RuleName = e.Result.Grammar.RuleName;
            ResultName = e.Result.Text;
            //if (RuleName == "SGM_FUNC_AskTodayCustomWeather" || RuleName == "SGM_FUNC_AskTomorrowCustomWeather" || RuleName == "SGM_DIAL_AskCountryPresidentOrPrimeMinister" || RuleName == "SGM_FUNC_COMPLEX_SetReminder" || RuleName == "SGM_NewsOption" || RuleName == "SGM_FUNC_COMPLEX_天気")
            //{

            //}
            if (e.Result.Confidence > 0.85)
            {
                ThinkingProcess(RuleName, ResultName);
            }
            SR.SpeechRecognition.SpeechRecognized += SR_SpeechRecognized;
        }

        public void ThinkingProcess(string RuleName, string ResultName)
        {
            if (RuleName == "SGM_AskForYesNo")
            {
                GrammarTimer.Stop();
                SR.UnloadGrammar(SR.SGM_AskForYesNo);
                switch (YesNoStatus)
                {
                    case "ContinueReadNews":
                        if (ResultName == "yes" || ResultName == "yes please")
                        {
                            if (EnglishNewsMark)
                            {
                                WIT.TTS.Speaking("Ok,next news");
                                Thread.Sleep(2000);
                                ReadNewsFunction.flag_StartNewsReading = true;
                                ReadNewsFunction.NewsReadingFunction_Eng(Function.WitAnalysis.NewsType);
                            }
                            else
                            {
                                WIT.TTS.Speaking("Ok,next news");
                                Thread.Sleep(2000);
                                ReadNewsFunction.flag_StartNewsReading = true;
                                ReadNewsFunction.NewsReadingFunction_Cn();
                            }
                        }
                        else
                        {
                            Function.WitAnalysis.NewsReadingMark = false;
                            WIT.TTS.Speaking("Ok, I will stop reading news.");
                            ReadNewsFunction.flag_StartNewsReading = false;
                            CoolDownTimer.Start();
                        }
                        break;
                    case "AskForRadio":
                        if (ResultName == "yes" || ResultName == "yes please")
                        {
                            Function.WitAnalysis.RadioPlayingMark = true;
                            WIT.TTS.Speaking("OK, please choose the station you want.");
                            Kiss92.Visibility = Visibility.Visible;
                            Yes933.Visibility = Visibility.Visible;
                            Class95.Visibility = Visibility.Visible;
                            Love972.Visibility = Visibility.Visible;
                            FM988.Visibility = Visibility.Visible;
                            UFM1003.Visibility = Visibility.Visible;
                            FM924.Visibility = Visibility.Visible;
                            FM987.Visibility = Visibility.Visible;
                            Lush995.Visibility = Visibility.Visible;
                            Gold905.Visibility = Visibility.Visible;
                            Capital958.Visibility = Visibility.Visible;
                            CloseRadio.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            WIT.TTS.Speaking("OK.");
                        }
                        break;
                    case "AskForNews":
                        if (ResultName == "yes" || ResultName == "yes please")
                        {
                            WIT.TTS.Speaking("What kinds of news do you want?");
                        }
                        else
                        {
                            WIT.TTS.Speaking("OK.");
                        }
                        break;
                    case "Facebook":
                        if (ResultName == "yes" || ResultName == "yes please")
                        {
                            if (colorBitmap != null)
                            {
                                WIT.TTS.Speaking("What is description for the photo?");
                                while (Function.Text_To_Speech.SpeakingMark) ;
                                FacebookMark = true;
                                IsListeningFromUser = true;
                                RecordingTimer.Stop();
                                Function.FaceLED.Instance.cheekgreen();
                                Function.Record.RecordingMark = true;
                                if (CoolDownTimer.IsEnabled)
                                {
                                    CoolDownTimer.Stop();
                                }
                            }
                            else
                            {
                                WIT.TTS.Speaking("Sorry, I cannot get the photo.");
                                FacebookMark = false;
                            }
                        }
                        else
                        {
                            WIT.TTS.Speaking("OK");
                        }
                        CaptureMark = false;
                        PhotoImage.Visibility = Visibility.Hidden;
                        //Added in THEF 20170714
                        MainWindow.IsListeningFromUser = true;
                        MainWindow.RecordingTimer.Stop();
                        Function.FaceLED.Instance.cheekgreen();
                        Function.Record.RecordingMark = true;
                        if (MainWindow.CoolDownTimer.IsEnabled)
                        {
                            MainWindow.CoolDownTimer.Stop();
                        }
                        break;
                    case "YouTubeVid":
                        if (ResultName == "yes" || ResultName == "yes please")
                        {
                            YouTubeSearch(YouTubeSearchText);
                        }
                        else
                        {
                            WIT.TTS.Speaking("Please tell me the title of the video.");
                            while (Function.Text_To_Speech.SpeakingMark) ;
                            Function.WitAnalysis.YouTubeMarkVid = true;
                            MainWindow.IsListeningFromUser = true;
                            MainWindow.RecordingTimer.Stop();
                            Function.FaceLED.Instance.cheekgreen();
                            Function.Record.RecordingMark = true;
                            if (MainWindow.CoolDownTimer.IsEnabled)
                            {
                                MainWindow.CoolDownTimer.Stop();
                            }
                        }
                        break;

                    case "YouTubeChnl":
                        if (ResultName == "yes" || ResultName == "yes please")
                        {
                            YouTubeSearch(YouTubeSearchText);
                        }
                        else
                        {
                            WIT.TTS.Speaking("Please tell me the name of the channel.");
                            while (Function.Text_To_Speech.SpeakingMark) ;
                            Function.WitAnalysis.YouTubeMarkChnl = true;
                            MainWindow.IsListeningFromUser = true;
                            MainWindow.RecordingTimer.Stop();
                            Function.FaceLED.Instance.cheekgreen();
                            Function.Record.RecordingMark = true;
                            if (MainWindow.CoolDownTimer.IsEnabled)
                            {
                                MainWindow.CoolDownTimer.Stop();
                            }
                        }
                        break;

                    case "YouTubePL":
                        if (ResultName == "yes" || ResultName == "yes please")
                        {
                            YouTubeSearch(YouTubeSearchText);
                        }
                        else
                        {
                            WIT.TTS.Speaking("Ok then, Whose playlist?");
                            while (Function.Text_To_Speech.SpeakingMark) ;
                            Function.WitAnalysis.YouTubeMarkPL = true;
                            MainWindow.IsListeningFromUser = true;
                            MainWindow.RecordingTimer.Stop();
                            Function.FaceLED.Instance.cheekgreen();
                            Function.Record.RecordingMark = true;
                            if (MainWindow.CoolDownTimer.IsEnabled)
                            {
                                MainWindow.CoolDownTimer.Stop();
                            }
                        }
                        break;
                }
                YesNoStatus = null;
            }
            else if (RuleName == "SGM_FUNC_SkypePhoneCallFinished")
            {
                if (Function.WitAnalysis.SkypeCallMark)
                {
                    Skype = new Application.SkypeApp.Skype();
                    Function.WitAnalysis.SkypeCallMark = false;
                    WIT.TTS.Speaking("ok");
                    IsListeningFromUser = false;
                    Skype.HangUp();
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.MENU);
                    InputSimulator.SimulateKeyPress(VirtualKeyCode.NEXT);
                    Thread.Sleep(100);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.MENU);
                    Thread.Sleep(100);
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.MENU);
                    InputSimulator.SimulateKeyPress(VirtualKeyCode.SPACE);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.MENU);
                    Thread.Sleep(10);
                    InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_N);
                }
            }
        }

        public void UploadToFacebook(string Text)
        {
            facebook.uploadPic(colorBitmap, Text + "\n - Upload From Esther");
            WIT.TTS.Speaking("OK, I have post this photo on your facebook.");
            FacebookMark = false;
        }

        public void Window_Closed(Object sender, EventArgs e)
        {
            try
            {
                FaceTrackingTimer.Stop();
                FaceTrackingTimer = null;
            }
            catch { }
            Thread.Sleep(100);
            for (int i = 0; i < 2; i++)
            {
                Motor.RobotSleep();
                Thread.Sleep(100);
                Motor.EyesClose();
                Thread.Sleep(100);
                Motor.RightArmRest();
                Thread.Sleep(100);
                Motor.LeftArmRest();
                Thread.Sleep(100);
                try
                {
                    Function.FaceLED.Instance.blank();
                }
                catch
                {

                }
            }
            try
            {
                FinalFrame.Stop();
            }
            catch
            {

            }
            Motor.TopNeckRelease();
            Motor.BtmNeckRelease();
            GC.Collect();
            Environment.Exit(Environment.ExitCode);
        }

        //Read News Timer Part Start
        public void ReadNewsTimer_TimeUp(object sender, EventArgs e)
        {
            ReadNewsTimer.Stop();
            WIT.TTS.Speaking("Do you want me to continue reading the news ?");//change THEF2016/11/17
            SR.LoadGrammar(SR.SGM_AskForYesNo);
            YesNoStatus = "ContinueReadNews";
            GrammarTimer.Start();
        }
        //Read News Timer Part End

        #region Grammar Timer

        //Grammar Timer Part Start
        public void GrammarTimer_TimeUp(object sender, EventArgs e)
        {
            GrammarTimer.Stop();
            //else if (SR.SGM_DIAL_SwitchLanguageToChinese_YesNo.Loaded)
            //{
            //    SR.UnloadGrammar(SR.SGM_DIAL_SwitchLanguageToChinese_YesNo);
            //    SR.LoadGrammar(SR.SGM_DIAL_SwitchLanguageToChinese);
            //    WIT.TTS.Speaking("Ok, never mind.");
            //}
            //else if (SR.SGM_DIAL_SwitchLanguageToJapanese_YesNo.Loaded)
            //{
            //    SR.UnloadGrammar(SR.SGM_DIAL_SwitchLanguageToJapanese_YesNo);
            //    SR.LoadGrammar(SR.SGM_DIAL_SwitchLanguageToJapanese);
            //    WIT.TTS.Speaking("Ok, never mind.");
            //}
            //else if (SR.SGM_DIAL_Sleep_YesNo.Loaded)
            //{
            //    SR.UnloadGrammar(SR.SGM_DIAL_Sleep_YesNo);
            //    SR.LoadGrammar(SR.SGM_DIAL_Sleep);
            //    WIT.TTS.Speaking("Ok, never mind.");
            //}
            //else if (SR.SGM_FUNC_RegisterMode_YesNo.Loaded)
            //{
            //    SR.UnloadGrammar(SR.SGM_FUNC_RegisterMode_YesNo);
            //    WIT.TTS.Speaking("Ok, never mind.");
            //    RegisterModeQuit();
            //    RegisterModeSwitch = false;
            //}
            if(SR.SGM_AskForYesNo.Loaded)
            {
                SR.UnloadGrammar(SR.SGM_AskForYesNo);
                switch (YesNoStatus)
                {
                    case "ContinueReadNews":
                        Function.WitAnalysis.NewsReadingMark = false;
                        WIT.TTS.Speaking("I will stop reading news.");
                        ReadNewsFunction.flag_StartNewsReading = false;
                        CoolDownTimer.Start();
                        break;
                    case "AskForRadio":
                        break;
                    case "AskForNews":
                        break;
                    case "Facebook":
                        WIT.TTS.Speaking("Ok, never mind.");
                        CaptureMark = false;
                        PhotoImage.Visibility = Visibility.Hidden;
                        break;
                    case "YouTubeVid":
                        WIT.TTS.Speaking("Never mind, I will stop playing the video.");
                        Function.WitAnalysis.YouTubeMarkVid = false;
                        break;
                    case "YouTubeChnl":
                        WIT.TTS.Speaking("Never mind, I will stop playing the video.");
                        Function.WitAnalysis.YouTubeMarkChnl = false;
                        break;
                    case "YouTubePL":
                        WIT.TTS.Speaking("Never mind, I will stop playing the video.");
                        Function.WitAnalysis.YouTubeMarkPL = false;
                        break;
                }
                YesNoStatus = null;
            }
            Function.FaceLED.Instance.blank();
            IsListeningFromUser = false;
        }
        //Grammar Timer Part End
        #endregion Grammar Timer

        public void WeatherIconTimer_TimeUp(object sender, EventArgs e)
        {
            WeatherIconTimer.Stop();
            WeatherIcon.Source = null;
            WeatherText.Text = null;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (RegisterModeSwitch)
            {
                RegisterModeSwitch = false;
                WIT.TTS.Speaking("Exiting the register mode.");
            }
            else
            {
                RegisterModeSwitch = true;
                WIT.TTS.Speaking("Entering the register mode.");
            }
        }

        public void UserDataUpdate(object sender, EventArgs e)
        {
            if (DataLength >= 48)
            {
                UserData.Text = UserDataText;
            }
            else
            {
                UserData.Text = "Nobody in the view.";
            }
        }

        private void WakeUp_Click(object sender, RoutedEventArgs e)
        {
            if (!WakeUp)
            {
                WakeUp = true;
                ImageBrush brush1 = new ImageBrush();
                brush1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "image/wakeUp.png"));
                WakeUpButton.Background = brush1;
            }
            else
            {
                WakeUp = false;
                ImageBrush brush1 = new ImageBrush();
                brush1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "image/goSilence.png"));
                WakeUpButton.Background = brush1;
            }
        }

        //Vision Part Start
        Random ran = new Random();
        private SerialPort MyCamera = new SerialPort();

        public DispatcherTimer FaceTrackingTimer = new DispatcherTimer();//The timer used in face tracking

        public static bool WakeUp = false;

        #region FaceTracking

        int UserID = -2; //Because 0 is the first user and -1 means not recongized. So I let -2 to be the original value.
        public static string UserName = "Unknown";
        public static int UserNameConfidence = 0;
        public static string UserDataText = null;
        public static int MultipleUserID = -2;
        public static string MultipleUserName = null;
        public static int MultipleUserNameConfidence = 0;
        public static bool RegisterModeSwitch = false;//If want to register, set this bool to true!!!

        int X = 0;//The X position of person in the camera view This person is the one who will be tracked by robot.
        int Y = 0;//The Y position.
        int XInMotor = 0;//The value to change motor's goal position to track user's face.
        int YInMotor = 0;//Same as above.
        int EyeGazeByte = 32;
        int PersonalInformationByteLength = 38;
        int GazeX = 0;
        int GazeY = 0;
        int MinimumGaze = 999;
        int GazeNumber = 0;
        public static bool RegisterMark = false;
        public bool GazeMark = false;
        public bool EyesTurnRight = false;
        public bool NeckTurnDown = false;
        public int SecondLevelTimerCount = 0;//This is for the timer in the 1.5s timer. This timer is 4.5s/time.
        int EyesBlinkCount = 0;
        int EyesBlinkRandKey = 5;
        double SSPosition = 320;//SoundSourcePosition
        public static int NumberOfPerson = 0;
        public static string UserGender = null;
        public static int UserAge = 0;
        int DBSSPAPP = 0;//Distance Between Sound Source Position And Person Position
        int DBSSPAPPmin = 999;//The minimum number of DBSSPAP
        int XoPP = 0;//X of Person Position. For make difference from the indentifier:'X'.
        public static int DataLength = 0;
        int Sensitivity = 6;
        bool InitializeMark = true;//For setting camera angle
        int NeckResetCount = 0;
        int RandomActionMark = 0;
        int NobodyCounter = 0;//0 means have person in front of the view otherwise it's 1 to 4
        int NeutralConfidence = 0;
        int HappinessConfidence = 0;
        int SurpriseConfidence = 0;
        int AngerConfidence = 0;
        int SadnessConfidence = 0;
        int ExpressionDegree = 0;
        public static string Emotion = "Neutral";
        public int EmotionConfidence = 0;

        public void FaceTrackingTimer_TimesUp(object sender, EventArgs e)
        {
            SendCommand();
            VisionDataProcess();
            if (WakeUp)
            {
                if (SecondLevelTimerCount >= Sensitivity)
                {
                    //if (UserName != "Unknown")
                    //{
                    //    SRG.SRE_Speech.SpeakAsync("Hello " + UserName);//For test!!
                    //}

                    //Eye Gaze
                    GazeMark = true;
                    SecondLevelTimerCount = 0;
                    if (dynamixel.dxl_read_word(Motor.RightArmID, Motor.PresentPositionAddress) > 450)
                    {
                        Motor.RightArmRest();
                    }
                    Motor.LeftArmRest();
                    Function.FaceLED.Instance.cheekblank();
                    Function.FaceLED.Instance.Normal();
                }
                //Eyes Blink
                if (EyesBlinkCount == EyesBlinkRandKey)
                {
                    if (RandomActionMark < 4)
                    {
                        if (RandomActionMark == 0)
                        {
                            Motor.EyesInitialize();
                            Motor.MidMotorInitialize();
                        }
                        else
                        {
                            Motor.EyesBlink();
                        }
                        RandomActionMark++;
                    }
                    else if (RandomActionMark == 4)
                    {
                        int ActionNumber1 = ran.Next(1, 5);
                        int ActionNumber2 = ran.Next(1, 4);
                        int ActionNumber3 = ran.Next(1, 4);
                        int ActionNumber4 = ran.Next(1, 3);
                        switch (ActionNumber1)
                        {
                            case 1:
                                //Motor.EyesLeft();
                                break;
                            case 2:
                                //Motor.EyesRight();
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        switch (ActionNumber2)
                        {
                            case 1:
                                //Motor.MidMotorTurnLeft();
                                break;
                            case 2:
                                //Motor.MidMotorTurnRight();
                                break;
                            case 3:
                                break;
                        }
                        switch (ActionNumber3)
                        {
                            case 1:
                                //Motor.EyesHalfClose();
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                        }
                        if ((ActionNumber1 == 3 || ActionNumber1 == 4) && ActionNumber3 != 1 && NobodyCounter == 0)
                        {
                            if (ActionNumber4 == 1)
                            {
                                Function.FaceLED.Instance.Happy();
                            }
                            else
                            {
                                Function.FaceLED.Instance.Surprise();
                            }
                        }
                        RandomActionMark++;
                    }
                    else
                    {
                        RandomActionMark = 0;
                    }
                    EyesBlinkCount = 0;
                    EyesBlinkRandKey = ran.Next(3, 10);
                }
                else
                {
                    EyesBlinkCount++;
                }
            }
            //if (RecordStartMark && Function.Speech_Recognition.Language == "English")
            //{
            //    RecordStartMark = false;
            //    Function.FaceLED.Instance.cheekgreen();
            //    Record.RecordStart();
            //}
        }

        public void SendCommand()
        {
            if (InitializeMark)
            {
                byte[] SetBaudRateCommand = new byte[] { 0Xfe, 0X0e, 0X01, 0X00, 0X05 };
                MyCamera.Write(SetBaudRateCommand, 0, SetBaudRateCommand.Length);
                Thread.Sleep(10);
                MyCamera.BaudRate = Convert.ToInt32(921600);
                MyCamera.DiscardInBuffer();
                Thread.Sleep(10);
                byte[] SetAngleCommand = new byte[] { 0Xfe, 0X01, 0X01, 0X00, 0X03 };
                MyCamera.Write(SetAngleCommand, 0, SetAngleCommand.Length);
                Thread.Sleep(10);
                byte[] SetSaveCommand = new byte[] { 0Xfe, 0X22, 0X00, 0X00 };
                MyCamera.Write(SetSaveCommand, 0, SetSaveCommand.Length);
                InitializeMark = false;
            }
            else
            {
                byte[] NormalCommand = new byte[] { 0Xfe, 0X04, 0X03, 0X00, 0Xfc, 0X03, 0X00 };
                try
                {
                    MyCamera.Write(NormalCommand, 0, NormalCommand.Length);
                }
                catch
                {

                }
            }
        }

        public Task<byte[]> ProcessByteReceived()
        {
            return Task.Run(() =>
            {
                return ProcessByte();
            });
        }

        public byte[] ProcessByte()
        {
            byte[] ByteReceived = null;
            try
            {
                ByteReceived = new byte[MyCamera.BytesToRead];
            }
            catch
            {
                
            }
            return ByteReceived;
        }

        public async void VisionDataProcess()
        {
            try
            {
                if (!RegisterMark)
                {
                    byte[] DataReceived = await ProcessByteReceived();
                    MyCamera.Read(DataReceived, 0, MyCamera.BytesToRead);
                    DataLength = DataReceived.Length;
                    MyCamera.DiscardInBuffer();
                    if (WakeUp)
                    {
                        if (DataLength >= 4)
                        {
                            if (DataReceived[0] == 254 && DataReceived[1] == 0 && (DataLength - 48) % 38 == 0)
                            {
                                if (DataReceived.Length != 6)
                                {
                                    if (DataReceived.Length == 48)//If there is only one person in front of the robot.
                                    {
                                        NumberOfPerson = 1;

                                        UserID = DataReceived[44];

                                        #region Emotion Part

                                        int EmotionData = 0;
                                        int EmotionMark = 0;
                                        for (int i = 0; i < 5; i++)
                                        {
                                            if (DataReceived[i + 38] > EmotionData)
                                            {
                                                EmotionMark = i;
                                                EmotionData = DataReceived[i + 38];
                                                EmotionConfidence = EmotionData;
                                            }
                                        }
                                        switch (EmotionMark)
                                        {
                                            case 0:
                                                Emotion = "Neutral";
                                                break;
                                            case 1:
                                                Emotion = "Happiness";
                                                break;
                                            case 2:
                                                Emotion = "Surprise";
                                                break;
                                            case 3:
                                                Emotion = "Anger";
                                                break;
                                            case 4:
                                                Emotion = "Sadness";
                                                break;
                                            default:
                                                Emotion = "Neutral";
                                                break;
                                        }

                                        #endregion

                                        if (DataReceived[45] != 255)//If robot knew this person.
                                        {
                                            try
                                            {
                                                string MyXMLFilePath = SR.BaseFolder + @"Database\RegisterData.xml";//the location of register database //Local File
                                                XmlDocument MyXmlDoc = new XmlDocument();//Initialize a xml document identifier
                                                MyXmlDoc.Load(MyXMLFilePath);//Load the Register database xml file.（the path of the file）
                                                XmlNode RootNode = MyXmlDoc.SelectSingleNode("Users");//The first node（SelectSingleNode）：the root node of this xml file
                                                XmlNodeList FirstLevelNodeList = RootNode.ChildNodes;//Child node of the root node（Or it is called：the first level child node）
                                                foreach (XmlNode Node in FirstLevelNodeList)
                                                {
                                                    XmlNode SecondLevelNode1 = Node.FirstChild;//Get the child node of the root node
                                                    if (SecondLevelNode1.InnerText == UserID.ToString())//Find the UserID in the database
                                                    {
                                                        XmlNode SecondLevelNode2 = Node.ChildNodes[1];//Get the second child node of the root node(It's not second level, it's second first level. If you don't understand, please check the xml file, just open it and see.)
                                                        UserName = SecondLevelNode2.InnerText;//Give the username which is in the database to identifier 'UserName'
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                MessageBox.Show("Error while getting user's data!");
                                                Environment.Exit(0);
                                            }
                                            UserNameConfidence = DataReceived[46] + DataReceived[47] * 256;
                                        }
                                        else
                                        {
                                            UserName = "Unknown";
                                            UserNameConfidence = 0;
                                        }
                                        SecondLevelTimerCount++;

                                        if (GazeMark)                    //2.Eye Gaze Detection & Action
                                        {
                                            if (GazeX >= 10)
                                            {
                                                if (EyesTurnRight)
                                                {
                                                    //Motor.EyesRight();
                                                }
                                                else
                                                {
                                                    //Motor.EyesLeft();
                                                }
                                                if (NeckTurnDown)
                                                {
                                                    if (GazeY >= 15)
                                                    {
                                                        if (EyesTurnRight)
                                                        {
                                                            Motor.MidMotorTurnRight();
                                                        }
                                                        else
                                                        {
                                                            Motor.MidMotorTurnLeft();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Motor.MidMotorInitialize();
                                                    }
                                                }
                                                else
                                                {
                                                    if (GazeY >= 5)
                                                    {
                                                        if (EyesTurnRight)
                                                        {
                                                            Motor.MidMotorTurnLeft();
                                                        }
                                                        else
                                                        {
                                                            Motor.MidMotorTurnRight();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Motor.MidMotorInitialize();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Motor.EyesBallInitialize();
                                                Motor.MidMotorInitialize();
                                                if (GazeX <= 10 && GazeY <= 10 && !Function.Record.RecordingMark && !Function.Text_To_Speech.SpeakingMark)
                                                {
                                                    Function.Record.RecordingMark = true;
                                                    RecordStartMark = true;
                                                }
                                            }
                                            GazeMark = false;
                                        }
                                        if (DataReceived[29] == 0)
                                        {
                                            UserGender = "Female";
                                        }
                                        else
                                        {
                                            UserGender = "Male";
                                        }
                                        UserAge = DataReceived[26];

                                        HappinessConfidence = DataReceived[39];
                                        SurpriseConfidence = DataReceived[40];
                                        AngerConfidence = DataReceived[41];
                                        SadnessConfidence = DataReceived[42];
                                    }

                                    else
                                    {
                                        UserID = -2;
                                        UserName = "Unknown";
                                        UserNameConfidence = 0;
                                        SecondLevelTimerCount = 0;
                                    }

                                    if (DataReceived.Length >= 48) //Multiple Person Facial Detection
                                    {
                                        Function.FaceLED.Instance.cheekblue();
                                        NumberOfPerson = DataReceived[8];

                                        #region Eye Gaze Facial Detection

                                        GazeX = 0;
                                        GazeY = 0;
                                        MinimumGaze = 999;
                                        DBSSPAPP = 0;
                                        DBSSPAPPmin = 999;
                                        GazeNumber = 0;
                                        for (int i = 0; i < NumberOfPerson; i++)
                                        {
                                            if (EyeGazeByte + 1 < DataReceived.Length)
                                            {
                                                GazeX = DataReceived[EyeGazeByte];
                                                GazeY = DataReceived[EyeGazeByte + 1];
                                                if (GazeX > 90)
                                                {
                                                    GazeX = 256 - GazeX;
                                                    EyesTurnRight = false;
                                                }
                                                else
                                                {
                                                    EyesTurnRight = true;
                                                }
                                                if (GazeY > 90)
                                                {
                                                    GazeY = 256 - GazeY;
                                                    NeckTurnDown = true;
                                                }
                                                else
                                                {
                                                    NeckTurnDown = false;
                                                }
                                                GazeNumber = GazeX + GazeY;
                                                if (GazeNumber < MinimumGaze)
                                                {
                                                    X = DataReceived[EyeGazeByte - 22] + DataReceived[EyeGazeByte - 21] * 256;
                                                    Y = DataReceived[EyeGazeByte - 20] + DataReceived[EyeGazeByte - 19] * 256;
                                                    MinimumGaze = GazeNumber;
                                                }
                                                EyeGazeByte = EyeGazeByte + PersonalInformationByteLength;
                                            }
                                            EyeGazeByte = 32;
                                        }

                                        #endregion

                                        NobodyCounter = 0;

                                        #region Get The Information of Multiple Users
                                        UserDataText = null;
                                        for (int i = 0; i < NumberOfPerson; i++)
                                        {
                                            if (DataReceived[45 + 38 * i] != 255)
                                            {
                                                MultipleUserID = DataReceived[44 + 38 * i];
                                                try
                                                {
                                                    string MyXMLFilePath = SR.BaseFolder + @"Database\RegisterData.xml";//the location of register database //Local File
                                                    XmlDocument MyXmlDoc = new XmlDocument();//Initialize a xml document identifier
                                                    MyXmlDoc.Load(MyXMLFilePath);//Load the Register database xml file.（the path of the file）
                                                    XmlNode RootNode = MyXmlDoc.SelectSingleNode("Users");//The first node（SelectSingleNode）：the root node of this xml file
                                                    XmlNodeList FirstLevelNodeList = RootNode.ChildNodes;//Child node of the root node（Or it is called：the first level child node）
                                                    foreach (XmlNode Node in FirstLevelNodeList)
                                                    {
                                                        XmlNode SecondLevelNode1 = Node.FirstChild;//Get the child node of the root node
                                                        if (SecondLevelNode1.InnerText == MultipleUserID.ToString())//Find the UserID in the database
                                                        {
                                                            XmlNode SecondLevelNode2 = Node.ChildNodes[1];//Get the second child node of the root node(It's not second level, it's second first level. If you don't understand, please check the xml file, just open it and see.)
                                                            MultipleUserName = SecondLevelNode2.InnerText;//Give the username which is in the database to identifier 'UserName'
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                    MessageBox.Show("Error while getting users' data!");
                                                    Environment.Exit(0);
                                                }
                                            }

                                            else
                                            {
                                                MultipleUserName = "Unknown";
                                            }

                                            if (MultipleUserName != "Unknown")
                                            {
                                                MultipleUserNameConfidence = DataReceived[38 * i + 46] + DataReceived[38 * i + 47] * 256;
                                            }

                                            else
                                            {
                                                MultipleUserNameConfidence = 0;
                                            }

                                            string MUNC = ((float)MultipleUserNameConfidence / 10).ToString() + "%";

                                            if (UserDataText == null)
                                            {
                                                UserDataText = "User" + (i + 1).ToString() + ":\n" + MultipleUserName + "\n" + MUNC + "\n" + Emotion + ": " + EmotionConfidence;
                                            }

                                            else
                                            {
                                                UserDataText = UserDataText + "\n" + "User" + (i + 1).ToString() + ":\n" + MultipleUserName + "\n" + MUNC + "\n" + Emotion + ": " + EmotionConfidence;
                                            }

                                            MultipleUserID = -2;
                                            MultipleUserName = null;
                                            MultipleUserNameConfidence = 0;
                                        }
                                        #endregion

                                    }

                                    #region Sound Direction Detection

                                    else//Nobody in the view
                                    {
                                        NumberOfPerson = 0;
                                        Function.FaceLED.Instance.cheekblank();
                                        NobodyCounter++;
                                        if (NobodyCounter == 4)
                                        {
                                            if (dynamixel.dxl_read_word(Motor.RightArmID, Motor.PresentPositionAddress) > 450)
                                            {
                                                Motor.RightArmRest();
                                            }
                                            Motor.LeftArmRest();
                                            Function.FaceLED.Instance.cheekblank();
                                            Function.FaceLED.Instance.Normal();
                                            NobodyCounter = 1;
                                        }
                                        //Motor.EyesBallInitialize();
                                        //Motor.MidMotorInitialize();
                                        NeckResetCount++;
                                        if (NeckResetCount == 5)
                                        {
                                            Motor.BtmMotorInitialize();
                                            NeckResetCount = 0;
                                        }
                                    }
                                    #endregion

                                }
                            }
                            else
                            {
                                int a = DataReceived.Length;
                            }
                        }
                    }
                    else
                    {
                        if (DataReceived.Length == 48)//If there is only one person in front of the robot.
                        {
                            UserID = DataReceived[44];

                            if (DataReceived[45] != 255)//If robot knew this person.
                            {
                                try
                                {
                                    string MyXMLFilePath = SR.BaseFolder + @"Database\RegisterData.xml";//the location of register database //Local File
                                    XmlDocument MyXmlDoc = new XmlDocument();//Initialize a xml document identifier
                                    MyXmlDoc.Load(MyXMLFilePath);//Load the Register database xml file.（the path of the file）
                                    XmlNode RootNode = MyXmlDoc.SelectSingleNode("Users");//The first node（SelectSingleNode）：the root node of this xml file
                                    XmlNodeList FirstLevelNodeList = RootNode.ChildNodes;//Child node of the root node（Or it is called：the first level child node）
                                    foreach (XmlNode Node in FirstLevelNodeList)
                                    {
                                        XmlNode SecondLevelNode1 = Node.FirstChild;//Get the child node of the root node
                                        if (SecondLevelNode1.InnerText == UserID.ToString())//Find the UserID in the database
                                        {
                                            XmlNode SecondLevelNode2 = Node.ChildNodes[1];//Get the second child node of the root node(It's not second level, it's second first level. If you don't understand, please check the xml file, just open it and see.)
                                            UserName = SecondLevelNode2.InnerText;//Give the username which is in the database to identifier 'UserName'
                                        }
                                    }
                                }
                                catch
                                {
                                    MessageBox.Show("Error while getting user's data!");
                                    Environment.Exit(0);
                                }
                            }
                            else
                            {
                                UserName = "Unknown";
                                UserNameConfidence = 0;
                            }
                            SecondLevelTimerCount++;
                        }
                        if (UserName != "Unknown")
                        {
                            WakeUp = true;
                            Motor.MotorInitialize();
                            WIT.TTS.Speaking("Hello, " + UserName);
                        }
                    }


                    #region Motor Control

                    XInMotor = (2400 - 4 * X) / 10 * 173 / 100;
                    YInMotor = 4 * (Y - 900) / 9;
                    Motor.TopPresentPosition = dynamixel.dxl_read_word(Motor.TopNeckID, Motor.PresentPositionAddress);
                    Motor.BtmPresentPosition = dynamixel.dxl_read_word(Motor.BtmNeckID, Motor.PresentPositionAddress);

                    if (DataReceived.Length >= 48 && Motor.BtmPresentPosition + XInMotor < Motor.BtmNeckUpLimit && Motor.BtmPresentPosition + XInMotor > Motor.BtmNeckLowLimit && Motor.TopPresentPosition + YInMotor < Motor.TopNeckUpLimit && Motor.TopPresentPosition + YInMotor > Motor.TopNeckLowLimit && WakeUp)
                    {
                        if (MinimumGaze <= 20)
                        {
                            if (XInMotor >= 12 || XInMotor <= -12)
                            {
                                Motor.MotorWrite_funcNeck(Motor.BtmNeckID, 30, Motor.BtmPresentPosition + XInMotor);
                            }
                        }
                        Motor.MotorWrite_funcNeck(Motor.TopNeckID, 30, Motor.TopPresentPosition + YInMotor);
                    }

                    XInMotor = 0;
                    YInMotor = 0;
                    #endregion
                }
            }
            catch
            {

            }

        }

        #endregion


        #region Registration

        int NewUserID = -2;
        public string NewUserName = "";

        public void RegisterMode()
        {
            FaceTrackingTimer.Stop();
            Thread.Sleep(200);
            RegisterMark = true;
            Thread.Sleep(200);
        }

        public void Command(string Character)
        {
            switch (Character)
            {
                case "finish":
                    //reload previous Grammar

                    //1.Get the new ID of new user.
                    try
                    {
                        string MyXMLFilePath = SR.BaseFolder + @"Database\RegisterData.xml";//the location of register database //Local File
                        XmlDocument MyXmlDoc = new XmlDocument();//Initialize a xml document identifier
                        MyXmlDoc.Load(MyXMLFilePath);//Load the Register database xml file.（the path of the file）
                        XmlNode RootNode = MyXmlDoc.SelectSingleNode("Users");//The first node（SelectSingleNode）：the root node of this xml file
                        XmlNodeList FirstLevelNodeList = RootNode.ChildNodes;//Child node of the root node（Or it is called：the first level child node）
                        foreach (XmlNode Node in FirstLevelNodeList)
                        {
                            XmlNode SecondLevelNode1 = Node.FirstChild;//Get the child node of the root node
                            if (int.Parse(SecondLevelNode1.InnerText) >= NewUserID)//This is for get the NewUserID which is equal to the last ID in database plus one
                            {
                                NewUserID = int.Parse(SecondLevelNode1.InnerText);
                            }
                        }
                        NewUserID++;//Plus one
                    }
                    catch
                    {
                        MessageBox.Show("Error while saving user's data!");
                        Environment.Exit(0);
                    }

                    //2.add the new user's information into xml database.
                    try
                    {
                        string MyXMLFilePath = SR.BaseFolder + @"Database\RegisterData.xml";//the location of register database //Local File
                        XmlDocument MyXmlDoc = new XmlDocument();//Initialize a xml document identifier
                        MyXmlDoc.Load(MyXMLFilePath);//Load the Register database xml file.（the path of the file）
                        XmlNode RootNode = MyXmlDoc.SelectSingleNode("Users");//The first node（SelectSingleNode）：the root node of this xml file

                        XmlElement newElement1 = MyXmlDoc.CreateElement("User");
                        RootNode.AppendChild(newElement1);

                        XmlElement newElementChild1 = MyXmlDoc.CreateElement("ID");
                        newElementChild1.InnerText = NewUserID.ToString();//the new id sent back by omron
                        newElement1.AppendChild(newElementChild1);

                        XmlElement newElementChild2 = MyXmlDoc.CreateElement("Name");
                        newElementChild2.InnerText = NewUserName;
                        newElement1.AppendChild(newElementChild2);

                        MyXmlDoc.Save(MyXMLFilePath);
                    }
                    catch
                    {
                        MessageBox.Show("Error while saving user's data!");
                        Environment.Exit(0);
                    }

                    //3.Set the register data to OMRON.
                    byte UserIDinByte = 0;
                    UserIDinByte = (byte)(UserIDinByte | NewUserID);
                    byte[] RegistCommand = new byte[] { 0Xfe, 0X10, 0X03, 0X00, UserIDinByte, 0X00, 0X00 };
                    MyCamera.Write(RegistCommand, 0, RegistCommand.Length);

                    NewUserID = -2;
                    NewUserName = "";
                    RegisterMark = false;
                    Thread.Sleep(1000);
                    FaceTrackingTimer.Start();
                    break;

                case "No":
                    if (NewUserName != "")
                    {
                        NewUserName = NewUserName.Substring(0, NewUserName.Length - 1);
                    }
                    else
                    {
                        RegisterModeQuit();
                    }

                    break;

                case "space":
                    NewUserName = NewUserName + " ";

                    break;

                default:
                    NewUserName = NewUserName + Character;
                    break;
            }

        }

        public void RegisterModeQuit()
        {
            RegisterMark = false;
            Thread.Sleep(1000);
            FaceTrackingTimer.Start();
        }
        #endregion
        //Vision Part End

        #region Switch Language & Chinese Part

        public static bool record = true;
        Function.xfSpeech.iFlyISR isr;

        private void SwitchLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (Function.Speech_Recognition.Language == "English")
            {
                if(CoolDownTimer.IsEnabled)
                {
                    CoolDownTimer.Stop();
                }
                Function.Speech_Recognition.Language = "Chinese";
                IsListeningFromUser = true;
                WakeUp = true;
                ImageBrush brush1 = new ImageBrush();
                brush1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "image/goSilence.png"));
                WakeUpButton.Background = brush1;
                if (record)
                {
                    record = false;
                    string c1 = "server_url=dev.voicecloud.cn,appid=57f619a4,timeout=10000";
                    string c2 = "sub=iat,domain=iat,ssm=1,auf=audio/L16;rate=16000,aue=speex,ent=sms16k,sch=1,rst=json,rse=utf8,nlp_version=2.0";
                    isr = new Function.xfSpeech.iFlyISR(c1, c2);
                    isr.DataArrived += new EventHandler<Function.xfSpeech.iFlyISR.DataArrivedEventArgs>(isr_DataAvailable);
                    isr.iatStart();
                }
            }
            else if (Function.Speech_Recognition.Language == "Chinese")
            {
                Function.Speech_Recognition.Language = "Malay";
                IsListeningFromUser = false;
                if (!record)
                {
                    record = true;
                    isr.iatStop();
                }
            }
             else if (Function.Speech_Recognition.Language == "Malay")
            {
                Function.Speech_Recognition.Language = "English";
                IsListeningFromUser = false;
                if (!record)
                {
                    record = true;
                    isr.iatStop();
                }
            }
            Language_text.Text = Function.Speech_Recognition.Language;
        }

        void isr_DataAvailable(object sender, Function.xfSpeech.iFlyISR.DataArrivedEventArgs e)
        {
            string Text = isr.AnsText;
            //Funcional Request!!!!!
            if(Text=="讲英文。"|| Text == "讲英语。" || Text == "说英语。" || Text == "说英文。" || Text == "切换回英文。" || Text == "切换至英文。" || Text == "切换到英文。" || Text == "你会讲英文吗。")
            {
                WIT.TTS.Speaking("OK");
                SwitchToEnglishTimer.Start();
            }
            else if (e.response != null)
            {
                WIT.TTS.SpeakingCN(e.response);
            }
            else
            {
                WIT.TTS.SpeakingCN("抱歉，不是很懂。");
            }
        }

        void SwitchToEnglish(object sender, EventArgs e)
        {
            SwitchToEnglishTimer.Stop();
            Function.Speech_Recognition.Language = "English";
            IsListeningFromUser = false;
            if (!record)
            {
                record = true; 
                isr.iatStop();
            }
            Language_text.Text = Function.Speech_Recognition.Language;
        }
        #endregion

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IRKitManager.Show();
            }
            catch
            {
                IRKitManager.Close();
            }
            //IRKitManager.SendCommand("Power");
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            GoogleCalenderFunction.ReadReminder();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            RosBridge.Show();
        }

        private Double VerticalOffset { get; set; }
        private System.Windows.Point Point { get; set; }

        private void ScrollViewer_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            VerticalOffset = (sender as ScrollViewer).VerticalOffset;
            Point = e.GetTouchPoint(sender as ScrollViewer).Position;
        }

        private void ScrollViewer_PreviewTouchMove(object sender, System.Windows.Input.TouchEventArgs e)
        {

            var point = e.GetTouchPoint(sender as ScrollViewer);

            var dy = point.Position.Y - Point.Y;

            (sender as ScrollViewer).ScrollToVerticalOffset(VerticalOffset - dy);
        }

        private void Esc_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImageBrush brush1 = new ImageBrush();
            brush1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory+ "image/Power off_.png"));
            Esc.Background = brush1;
        }

        private void Esc_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImageBrush brush1 = new ImageBrush();
            brush1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "image/Power off.png"));
            Esc.Background = brush1;
        }

        private void MainGrid_SizeChanged(object sender, EventArgs e)
        {
            CalculateScale();
        }

        private void CalculateScale()
        {
            double yScale = ActualHeight / 720f;
            double xScale = ActualWidth / 1280f;
            double value = Math.Min(xScale, yScale);
            ScaleValue = (double)OnCoerceScaleValue(myMainWindow, value);
        }

        #region ScaleValue Depdency Property
        public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.Register("ScaleValue", typeof(double), typeof(MainWindow), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnScaleValueChanged), new CoerceValueCallback(OnCoerceScaleValue)));

        private static object OnCoerceScaleValue(DependencyObject o, object value)
        {
            MainWindow mainWindow = o as MainWindow;
            if (mainWindow != null)
                return mainWindow.OnCoerceScaleValue((double)value);
            else
                return value;
        }

        private static void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MainWindow mainWindow = o as MainWindow;
            if (mainWindow != null)
                mainWindow.OnScaleValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceScaleValue(double value)
        {
            if (double.IsNaN(value))
                return 1.0f;

            value = Math.Max(0.1, value);
            return value;
        }

        protected virtual void OnScaleValueChanged(double oldValue, double newValue)
        {

        }

        public double ScaleValue
        {
            get
            {
                return (double)GetValue(ScaleValueProperty);
            }
            set
            {
                SetValue(ScaleValueProperty, value);
            }
        }
        #endregion

        private void myMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MaximizeToSecondaryMonitor();
        }

        #region Radio

        private void Kiss92_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.LoadedBehavior = MediaState.Manual;
                Player.Stop();
                Player.Source = null;
            }
            Player.LoadedBehavior = MediaState.Manual;
            string StationUrl = "http://sph.rastream.com/sph-kiss92";
            RadioStation.Content = "KISS 92";
            ShowRadioStation();
            Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
            Player.Play();
        }

        private void Yes933_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.LoadedBehavior = MediaState.Manual;
                Player.Stop();
                Player.Source = null;
            }
            Player.LoadedBehavior = MediaState.Manual;
            string StationUrl = "http://mediacorp.rastream.com/933fm";
            RadioStation.Content = "YES 933";
            ShowRadioStation();
            Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
            Player.Play();
        }

        private void Class95_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.LoadedBehavior = MediaState.Manual;
                Player.Stop();
                Player.Source = null;
            }
            Player.LoadedBehavior = MediaState.Manual;
            string StationUrl = "http://mediacorp.rastream.com/950fm";
            RadioStation.Content = "CLASS 95";
            ShowRadioStation();
            Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
            Player.Play();
        }

        private void Love972_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.LoadedBehavior = MediaState.Manual;
                Player.Stop();
                Player.Source = null;
            }
            Player.LoadedBehavior = MediaState.Manual;
            string StationUrl = "http://mediacorp.rastream.com/972fm";
            RadioStation.Content = "LOVE 972";
            ShowRadioStation();
            Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
            Player.Play();
        }

        private void FM988_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.LoadedBehavior = MediaState.Manual;
                Player.Stop();
                Player.Source = null;
            }
            Player.LoadedBehavior = MediaState.Manual;
            string StationUrl = "http://starrfm.rastream.com/starrfm-988";
            RadioStation.Content = "FM 988";
            ShowRadioStation();
            Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
            Player.Play();
        }

        private void UFM1003_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.LoadedBehavior = MediaState.Manual;
                Player.Stop();
                Player.Source = null;
            }
            Player.LoadedBehavior = MediaState.Manual;
            string StationUrl = "http://sph.rastream.com/1003fm";
            RadioStation.Content = "UFM 1003";
            ShowRadioStation();
            Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
            Player.Play();
        }

        private void FM924_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.LoadedBehavior = MediaState.Manual;
                Player.Stop();
                Player.Source = null;
            }
            Player.LoadedBehavior = MediaState.Manual;
            string StationUrl = "http://mediacorp.rastream.com/924fm";
            RadioStation.Content = "SYMPHONY FM 924";
            ShowRadioStation();
            Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
            Player.Play();
        }
        private void FM987_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.LoadedBehavior = MediaState.Manual;
                Player.Stop();
                Player.Source = null;
            }
            Player.LoadedBehavior = MediaState.Manual;
            string StationUrl = "http://mediacorp.rastream.com/987fm";
            RadioStation.Content = "FM 987";
            ShowRadioStation();
            Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
            Player.Play();
        }
        private void Lush995_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.LoadedBehavior = MediaState.Manual;
                Player.Stop();
                Player.Source = null;
            }
            Player.LoadedBehavior = MediaState.Manual;
            string StationUrl = "http://mediacorp.rastream.com/995fm";
            RadioStation.Content = "LUSH 995";
            ShowRadioStation();
            Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
            Player.Play();
        }
        private void Gold905_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.LoadedBehavior = MediaState.Manual;
                Player.Stop();
                Player.Source = null;
            }
            Player.LoadedBehavior = MediaState.Manual;
            string StationUrl = "http://mediacorp.rastream.com/905fm";
            RadioStation.Content = "GOLD 905";
            ShowRadioStation();
            Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
            Player.Play();
        }
        private void Capital958_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.LoadedBehavior = MediaState.Manual;
                Player.Stop();
                Player.Source = null;
            }
            Player.LoadedBehavior = MediaState.Manual;
            string StationUrl = "http://mediacorp.rastream.com/958fm";
            RadioStation.Content = "CAPITAL 958";
            ShowRadioStation();
            Player.Source = new Uri(StationUrl, UriKind.RelativeOrAbsolute);
            Player.Play();
        }
        private void NextStation_Click(object sender, RoutedEventArgs e)
        {
            UIHelper.RadioControl("NextStation");
        }

        private void PreviousStation_Click(object sender, RoutedEventArgs e)
        {
            UIHelper.RadioControl("PreviousStation");
        }

        private void BackToRadioSelect_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.LoadedBehavior = MediaState.Manual;
                Player.Stop();
                Player.Source = null;
            }
            Kiss92.Visibility = Visibility.Visible;
            Yes933.Visibility = Visibility.Visible;
            Class95.Visibility = Visibility.Visible;
            Love972.Visibility = Visibility.Visible;
            FM988.Visibility = Visibility.Visible;
            UFM1003.Visibility = Visibility.Visible;
            FM924.Visibility = Visibility.Visible;
            FM987.Visibility = Visibility.Visible;
            Lush995.Visibility = Visibility.Visible;
            Gold905.Visibility = Visibility.Visible;
            Capital958.Visibility = Visibility.Visible;
            CloseRadio.Visibility = Visibility.Visible;
            RadioStation.Visibility = Visibility.Hidden;
            CloseRadio_Copy.Visibility = Visibility.Hidden;
            NextStation.Visibility = Visibility.Hidden;
            PreviousStation.Visibility = Visibility.Hidden;
            BackToRadioSelect.Visibility = Visibility.Hidden;
        }
        private void ShowRadioStation()
        {
            Kiss92.Visibility = Visibility.Hidden;
            Yes933.Visibility = Visibility.Hidden;
            Class95.Visibility = Visibility.Hidden;
            Love972.Visibility = Visibility.Hidden;
            FM988.Visibility = Visibility.Hidden;
            UFM1003.Visibility = Visibility.Hidden;
            FM924.Visibility = Visibility.Hidden;
            FM987.Visibility = Visibility.Hidden;
            Lush995.Visibility = Visibility.Hidden;
            Gold905.Visibility = Visibility.Hidden;
            Capital958.Visibility = Visibility.Hidden;
            CloseRadio.Visibility = Visibility.Hidden;
            RadioStation.Visibility = Visibility.Visible;
            CloseRadio_Copy.Visibility = Visibility.Visible;
            NextStation.Visibility = Visibility.Visible;
            PreviousStation.Visibility = Visibility.Visible;
            BackToRadioSelect.Visibility = Visibility.Visible;
        }
        private void CloseRadio_Click(object sender, RoutedEventArgs e)
        {
            Function.WitAnalysis.RadioPlayingMark = false;
            Player.LoadedBehavior = MediaState.Manual;
            Player.Stop();
            Player.Source = null;
            RadioStation.Content = null;

            Kiss92.Visibility = Visibility.Hidden;
            Yes933.Visibility = Visibility.Hidden;
            Class95.Visibility = Visibility.Hidden;
            Love972.Visibility = Visibility.Hidden;
            FM988.Visibility = Visibility.Hidden;
            UFM1003.Visibility = Visibility.Hidden;
            FM924.Visibility = Visibility.Hidden;
            FM987.Visibility = Visibility.Hidden;
            Lush995.Visibility = Visibility.Hidden;
            Gold905.Visibility = Visibility.Hidden;
            Capital958.Visibility = Visibility.Hidden;
            CloseRadio.Visibility = Visibility.Hidden;
            RadioStation.Visibility = Visibility.Hidden;
            CloseRadio_Copy.Visibility = Visibility.Hidden;
            NextStation.Visibility = Visibility.Hidden;
            PreviousStation.Visibility = Visibility.Hidden;
            BackToRadioSelect.Visibility = Visibility.Hidden;

        }

        #endregion

        #region Webcam

        public static bool CaptureMark = false;

        Bitmap colorBitmap;

        public void Capture()
        {
            CaptureMark = true;

            ScreenShot();

            PhotoImage.Visibility = Visibility.Visible;

            IsListeningFromUser = true;

            SR.LoadGrammar(SR.SGM_AskForYesNo);
            YesNoStatus = "Facebook";
            GrammarTimer.Start();
        }

        private void StartCapture_Webcam()
        {
            WebcamCtrl.Visibility = Visibility.Visible;
            if (WebcamCtrl.VideoDevice == null)
            {
                MessageBox.Show("No Device");
            }
            else
            {
                try
                {
                    // Display webcam video
                    WebcamCtrl.StartPreview();
                }
                catch (Microsoft.Expression.Encoder.SystemErrorException ex)
                {
                    MessageBox.Show("Device is in use by another application");
                }
            }
        }

        private void StopCapture_Webcam()
        {
            // Stop the display of webcam video.
            WebcamCtrl.StopPreview();
        }

        private void StartRecording()
        {
            // Start recording of webcam video to harddisk.
            WebcamCtrl.StartRecording();
        }

        private void StopRecording()
        {
            // Stop recording of webcam video to harddisk.
            WebcamCtrl.StopRecording();
        }

        private void TakeSnapshot(object sender, RoutedEventArgs e)
        {
            // Take snapshot of webcam video.
            WebcamCtrl.TakeSnapshot();
        }

        private void StopCapture()
        {
            FinalFrame.Stop();
            image.Visibility = Visibility.Hidden;
        }

        Bitmap ScreenShotIMAGE = null;
    //    Thread WebcamDisconnectionDetection;
    //    Thread WebcamReconnectionDetection;

        private void StartCapture()
        {
            try
            {
                FinalFrame = new VideoCaptureDevice(CaptureDevice[1].MonikerString);// Device 1 for Webcam
            }
            catch
            {
                MessageBox.Show("Camera cannot be detected.");
            }
            finally {

                CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (CaptureDevice.Count >1)
                {
                    FinalFrame = new VideoCaptureDevice(CaptureDevice[1].MonikerString);// Device 1 for Webcam

                    FinalFrame.NewFrame += (s, ee) =>
                    {
                        System.Drawing.Image img = (Bitmap)ee.Frame.Clone();
                        ScreenShotIMAGE = (Bitmap)img;
                        MemoryStream ms = new MemoryStream();
                        img.Save(ms, ImageFormat.Bmp);
                        ms.Seek(0, SeekOrigin.Begin);
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = ms;
                        bi.EndInit();
                        bi.Freeze();
                        Dispatcher.BeginInvoke(new ThreadStart(delegate
                        {
                            image.Source = bi;
                        }));
                    };
                    FinalFrame.Start();
                }
            }
            //WebcamReconnectionDetection = new Thread(WebcamReconnectDetect);
            //WebcamReconnectionDetection.Start();
           // Console.WriteLine(CaptureDevice[0].Name + CaptureDevice[1].MonikerString);
           
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
            {
                image.Visibility = Visibility.Visible;
            }));
        }

        //private void WebcamDisconnectDetect()
        //{
        //    while (FinalFrame.IsRunning) ;
        //    FinalFrame.Stop();
        //    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
        //    {
        //        image.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "image/disconnect.png"));
        //    }));
        //    WebcamReconnectionDetection = new Thread(WebcamReconnectDetect);
        //    WebcamReconnectionDetection.Start();
        //}

        private void WebcamReconnectDetect()
        {
            while (CaptureDevice.Count == 1) CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice); ;
            StartCapture();
        }

        private void ScreenShot()
        {
            colorBitmap = ScreenShotIMAGE;
            PhotoImage.Source = BitmapToBitmapImage(ScreenShotIMAGE);
        }

        public BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            BitmapImage bit3 = new BitmapImage();
            bit3.BeginInit();
            bit3.StreamSource = ms;
            bit3.EndInit();
            return bit3;
        }

        #endregion

        #region YouTube
        Application.YoutubeSearcher searcher = new Application.YoutubeSearcher();
        SearchListResponse response;

        private void ThumbNail1_Click(object sender, RoutedEventArgs e)
        {
            switch (response.Items[0].Id.Kind)
            {
                case "youtube#video":
                    if (VidTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/v/" + response.Items[0].Id.VideoId + @"?version=3" + @"&autoplay=1");
                    }
                    break;

                case "youtube#channel":
                    if (ChannelTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/channel/" + response.Items[0].Id.ChannelId);
                    }
                    break;

                case "youtube#playlist":
                    if(PlaylistTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/view_play_list?p=" + response.Items[0].Id.PlaylistId + "&playnext=1&playnext_from=PL");
                    }
                    break;
            }
        }

        private void ThumbNail2_Click(object sender, RoutedEventArgs e)
        {
            switch (response.Items[1].Id.Kind)
            {
                case "youtube#video":
                    if (VidTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/v/" + response.Items[1].Id.VideoId + @"?version=3" + @"&autoplay=1");
                    }
                    break;

                case "youtube#channel":
                    if (ChannelTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/channel/" + response.Items[1].Id.ChannelId);
                    }
                    break;

                case "youtube#playlist":
                    if (PlaylistTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/view_play_list?p=" + response.Items[1].Id.PlaylistId + "&playnext=1&playnext_from=PL");
                    }
                    break;
            }
        }

        private void ThumbNail3_Click(object sender, RoutedEventArgs e)
        {
            switch (response.Items[2].Id.Kind)
            {
                case "youtube#video":
                    if (VidTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/v/" + response.Items[2].Id.VideoId + @"?version=3" + @"&autoplay=1");
                    }
                    break;

                case "youtube#channel":
                    if (ChannelTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/channel/" + response.Items[2].Id.ChannelId);
                    }
                    break;

                case "youtube#playlist":
                    if (PlaylistTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/view_play_list?p=" + response.Items[2].Id.PlaylistId + "&playnext=1&playnext_from=PL");
                    }
                    break;
            }
        }

        private void ThumbNail4_Click(object sender, RoutedEventArgs e)
        {
            switch (response.Items[3].Id.Kind)
            {
                case "youtube#video":
                    if (VidTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/v/" + response.Items[3].Id.VideoId + @"?version=3" + @"&autoplay=1");
                    }
                    break;

                case "youtube#channel":
                    if (ChannelTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/channel/" + response.Items[3].Id.ChannelId);
                    }
                    break;

                case "youtube#playlist":
                    if (PlaylistTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/view_play_list?p=" + response.Items[3].Id.PlaylistId + "&playnext=1&playnext_from=PL");
                    }
                    break;
            }
        }

        private void ThumbNail5_Click(object sender, RoutedEventArgs e)
        {
            switch (response.Items[4].Id.Kind)
            {
                case "youtube#video":
                    if (VidTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/v/" + response.Items[4].Id.VideoId + @"?version=3" + @"&autoplay=1");
                    }
                    break;

                case "youtube#channel":
                    if (ChannelTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/channel/" + response.Items[4].Id.ChannelId);
                    }
                    break;

                case "youtube#playlist":
                    if (PlaylistTrig == true)
                    {
                        webBrowser.Visibility = Visibility.Visible;
                        webBrowser.Navigate(@"https://www.youtube.com/view_play_list?p=" + response.Items[4].Id.PlaylistId + "&playnext=1&playnext_from=PL");
                    }
                    break;
            }
        }

        public static string YouTubeSearchText = null;

        private void AskForYouTubeSearch(string Text)
        {
            YouTubeSearchText = Text;
            WIT.TTS.Speaking("Do you mean " + YouTubeSearchText + "?");
            while (Function.Text_To_Speech.SpeakingMark) ;
            IsListeningFromUser = true;
            SR.LoadGrammar(SR.SGM_AskForYesNo);
            if (VidTrig == true)
            {
                YesNoStatus = "YouTubeVid";
            }
            else if (ChannelTrig == true)
            {
                YesNoStatus = "YouTubeChnl";
            }
            else if (PlaylistTrig == true)
            {
                YesNoStatus = "YouTubePL";
            }
            GrammarTimer.Start();
        }

        private async void YouTubeSearch(string Text)//After Video title recording!!!!!
        {
            if (VidTrig == true)
            {
                Function.WitAnalysis.YouTubeMarkVid = false;
                CloseVid = true;
            }
            else if (ChannelTrig == true)
            {
                Function.WitAnalysis.YouTubeMarkChnl = false;
                CloseChnl = true;
            }
            else if (PlaylistTrig == true)
            {
                Function.WitAnalysis.YouTubeMarkPL = false;
                ClosePL = true;
            }
            response = await searcher.SearchAsync(Text);
            textBlock1.Text = response.Items[0].Snippet.Title.ToString();
            textBlock2.Text = response.Items[1].Snippet.Title.ToString();
            textBlock3.Text = response.Items[2].Snippet.Title.ToString();
            textBlock4.Text = response.Items[3].Snippet.Title.ToString();
            textBlock5.Text = response.Items[4].Snippet.Title.ToString();

            BitmapImage bitmap1 = new BitmapImage();
            bitmap1.BeginInit();
            bitmap1.UriSource = new Uri(response.Items[0].Snippet.Thumbnails.Default__.Url, UriKind.Absolute);
            bitmap1.EndInit();
            ImageBrush brush1 = new ImageBrush();
            brush1.ImageSource = bitmap1;
            ThumbNail1.Background = brush1;

            BitmapImage bitmap2 = new BitmapImage();
            bitmap2.BeginInit();
            bitmap2.UriSource = new Uri(response.Items[1].Snippet.Thumbnails.Default__.Url, UriKind.Absolute);
            bitmap2.EndInit();
            ImageBrush brush2 = new ImageBrush();
            brush2.ImageSource = bitmap2;
            ThumbNail2.Background = brush2;

            BitmapImage bitmap3 = new BitmapImage();
            bitmap3.BeginInit();
            bitmap3.UriSource = new Uri(response.Items[2].Snippet.Thumbnails.Default__.Url, UriKind.Absolute);
            bitmap3.EndInit();
            ImageBrush brush3 = new ImageBrush();
            brush3.ImageSource = bitmap3;
            ThumbNail3.Background = brush3;

            BitmapImage bitmap4 = new BitmapImage();
            bitmap4.BeginInit();
            bitmap4.UriSource = new Uri(response.Items[3].Snippet.Thumbnails.Default__.Url, UriKind.Absolute);
            bitmap4.EndInit();
            ImageBrush brush4 = new ImageBrush();
            brush4.ImageSource = bitmap4;
            ThumbNail4.Background = brush4;

            BitmapImage bitmap5 = new BitmapImage();
            bitmap5.BeginInit();
            bitmap5.UriSource = new Uri(response.Items[4].Snippet.Thumbnails.Default__.Url, UriKind.Absolute);
            bitmap5.EndInit();
            ImageBrush brush5 = new ImageBrush();
            brush5.ImageSource = bitmap5;
            ThumbNail5.Background = brush5;

            ThumbNail1.Visibility = Visibility.Visible;
            ThumbNail2.Visibility = Visibility.Visible;
            ThumbNail3.Visibility = Visibility.Visible;
            ThumbNail4.Visibility = Visibility.Visible;
            ThumbNail5.Visibility = Visibility.Visible;
            textBlock1.Visibility = Visibility.Visible;
            textBlock2.Visibility = Visibility.Visible;
            textBlock3.Visibility = Visibility.Visible;
            textBlock4.Visibility = Visibility.Visible;
            textBlock5.Visibility = Visibility.Visible;
            VideoBackground.Visibility = Visibility.Visible;
            if (VidTrig == true)
            {
                WIT.TTS.Speaking("Here are the video I found for " + Text + ". Please choose the video you want to watch.");
            }
            else if (ChannelTrig == true)
            {
                WIT.TTS.Speaking("Here are the channels I found for " + Text + ". Please choose the channel you want to view.");
            }
            else if (PlaylistTrig == true)
            {
                WIT.TTS.Speaking("Here are the playlists I found for " + Text + ". Please choose the playlists you want to watch.");
            }
            while (Function.Text_To_Speech.SpeakingMark) ;
            IsListeningFromUser = false;
            if (VidTrig == true)
            {
                Function.WitAnalysis.VideoPlayingMark = true;
            }
            else if (ChannelTrig == true)
            {
                Function.WitAnalysis.ChnlViewMark = true;
            }
            else if (PlaylistTrig == true)
            {
                Function.WitAnalysis.PLPlayingMark = true;
            }
        }

        private void CloseVideo()
        {
            webBrowser.Navigate(@"https://www.google.com.sg");
            ThumbNail1.Visibility = Visibility.Hidden;
            ThumbNail2.Visibility = Visibility.Hidden;
            ThumbNail3.Visibility = Visibility.Hidden;
            ThumbNail4.Visibility = Visibility.Hidden;
            ThumbNail5.Visibility = Visibility.Hidden;
            textBlock1.Visibility = Visibility.Hidden;
            textBlock2.Visibility = Visibility.Hidden;
            textBlock3.Visibility = Visibility.Hidden;
            textBlock4.Visibility = Visibility.Hidden;
            textBlock5.Visibility = Visibility.Hidden;
            webBrowser.Visibility = Visibility.Hidden;
            VideoBackground.Visibility = Visibility.Hidden;
        }
        #endregion

        public static bool CameraConnected = true;

        public void CameraDisconnected()
        {
            CameraConnected = false;
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
            {
                WebcamCtrl.Visibility = Visibility.Hidden;
            }));
        }
        #region USB Status Detection

        public static bool OKAOConnected = true;
        public static bool MbedConnected = true;

        private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {

            if (msg == UsbNotification.WmDevicechange)
            {
                UsbNotification.DEV_BROADCAST_HDR hdr;
                UsbNotification.DEV_BROADCAST_DEVICEINTERFACE deviceInterface;
                string DeviceName = null;
                string DeviceID = null;
                string[] Device_Name = null;

                switch ((int)wparam)
                {
                    case UsbNotification.DbtDeviceremovecomplete:
                        hdr = (UsbNotification.DEV_BROADCAST_HDR)Marshal.PtrToStructure(lparam, typeof(UsbNotification.DEV_BROADCAST_HDR));
                        deviceInterface = (UsbNotification.DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lparam, typeof(UsbNotification.DEV_BROADCAST_DEVICEINTERFACE));
                        Console.WriteLine("Device Name:" + UsbNotification.GetDeviceName(deviceInterface));
                        Console.WriteLine("Device ID:" + UsbNotification.GetDeviceID(deviceInterface));
                        DeviceName = UsbNotification.GetDeviceName(deviceInterface);
                        Device_Name = DeviceName.Split(Convert.ToChar(";"));
                        DeviceName = Device_Name[1];
                        DeviceID = UsbNotification.GetDeviceID(deviceInterface);
                        switch (DeviceName)
                        {
                            case "Logitech USB Camera (HD Pro Webcam C920)":
                                while (FinalFrame.IsRunning) ;
                                FinalFrame.Stop();
                                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                                {
                                    image.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "image/disconnect.png"));
                                 }));
                                break;
                            case "OMRON Serial Converter":
                                DemoMode = true;
                                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                                {
                                    DemoModeImage.Visibility = Visibility.Visible;
                                }));
                                OKAOConnected = false;
                                FaceTrackingTimer.Stop();
                                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                                {
                                    disConImage1.Visibility = Visibility.Visible;
                                    disConImage2.Visibility = Visibility.Visible;
                                }));
                                break;
                            case "mbed Serial Port":
                                DemoMode = true;
                                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                                {
                                    DemoModeImage.Visibility = Visibility.Visible;
                                }));
                                MbedConnected = false;
                                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                                {
                                    disConImage3.Visibility = Visibility.Visible;
                                    disConImage4.Visibility = Visibility.Visible;
                                }));
                                break;
                        }
                        break;
                    case UsbNotification.DbtDevicearrival:
                        hdr = (UsbNotification.DEV_BROADCAST_HDR)Marshal.PtrToStructure(lparam, typeof(UsbNotification.DEV_BROADCAST_HDR));
                        deviceInterface = (UsbNotification.DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lparam, typeof(UsbNotification.DEV_BROADCAST_DEVICEINTERFACE));
                        Console.WriteLine("Device Name:" + UsbNotification.GetDeviceName(deviceInterface));
                        Console.WriteLine("Device ID:" + UsbNotification.GetDeviceID(deviceInterface));
                        DeviceName = UsbNotification.GetDeviceName(deviceInterface);
                        Device_Name = DeviceName.Split(Convert.ToChar(";"));
                        DeviceName = Device_Name[1];
                        DeviceID = UsbNotification.GetDeviceID(deviceInterface);
                        switch (DeviceName)
                        {
                            case "Logitech USB Camera (HD Pro Webcam C920)":
                                Thread.Sleep(1000);
                                StartCapture();
                                break;
                            case "OMRON Serial Converter":
                                OKAOConnected = true;
                                Thread.Sleep(1000);
                                CameraInitialize();
                                FaceTrackingTimer.Start();
                                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                                {
                                    disConImage1.Visibility = Visibility.Hidden;
                                    disConImage2.Visibility = Visibility.Hidden;
                                }));
                                break;
                            case "mbed Serial Port":
                                MbedConnected = true;
                                Thread.Sleep(1000);
                                //LED port open!!!!!
                                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                                {
                                    disConImage3.Visibility = Visibility.Hidden;
                                    disConImage4.Visibility = Visibility.Hidden;
                                }));
                                break;
                            case "HID - compliant consumer control device":
                                MbedConnected = true;
                                Thread.Sleep(1000);
                                //LED port open!!!!!
                                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                                {
                                    disConImage3.Visibility = Visibility.Hidden;
                                    disConImage4.Visibility = Visibility.Hidden;
                                }));
                                break;
                        }

                        if (OKAOConnected && MbedConnected)
                        {
                            DemoMode = false;
                            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                            {
                                DemoModeImage.Visibility = Visibility.Hidden;
                            }));
                        }
                        break;
                }

            }

            handled = false;
            return IntPtr.Zero;
        }


        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr windowHandle;
            WindowInteropHelper interop = new WindowInteropHelper(this);
            HwndSource source = HwndSource.FromHwnd(interop.Handle);
            if (source != null)
            {
                windowHandle = source.Handle;
                source.AddHook(HwndHandler);
                UsbNotification.RegisterUsbDeviceNotification(windowHandle);
            }
        }


        public void ToDemoMode()
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
            {
                DemoModeImage.Visibility = Visibility.Visible;
            }));
        }
        #endregion USB Status Detection

        private void Demo_Click(object sender, RoutedEventArgs e)
        {
            Function.Motor.Instance.LeftArmRaiseRest(2000);
            Function.FaceLED.Instance.cheekblue();
            if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
               WIT.TTS.Speaking("Good morning ladies and gentlement");
            else
                if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
                    WIT.TTS.Speaking("Good afternoon ladies and gentlement");

            WIT.TTS.Speaking("Welcome to NYP lifestyle and social robotics lab");
            Function.Motor.Instance.RightArmRaise();
            WIT.TTS.Speaking("Let me introduce myself, I am Ruth, a social robot build by students from NYP and Kitakyushu National College of Technology");
            Function.FaceLED.Instance.Bluek();
            WIT.TTS.Speaking("I am not able to talk to you today as my speech engine is currently going through a revamp.");
            //Function.Motor.Instance.LeftArmRest();
             WIT.TTS.Speaking("however there is a similar system which we can demo to you.");
           // Function.Motor.Instance.RightArmRest();
        }
    }
}