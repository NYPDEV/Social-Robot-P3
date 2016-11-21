using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Speech.Recognition;
using System.Diagnostics;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Threading;
using WindowsInput;
using iFly;
using System.Web.Script.Serialization;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;
using SKYPE4COMLib;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Drawing;

namespace SocialRobot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        //Kinect Part Start

        #region defineVar
        /// <summary>
        /// Number of samples captured from Kinect audio stream each millisecond.
        /// </summary>
        private const int SamplesPerMillisecond = 16;

        /// <summary>
        /// Number of bytes in each Kinect audio stream sample (32-bit IEEE float).
        /// </summary>
        private const int BytesPerSample = sizeof(float);

        /// <summary>
        /// Number of audio samples represented by each column of pixels in wave bitmap.
        /// </summary>
        private const int SamplesPerColumn = 40;

        /// <summary>
        /// Minimum energy of audio to display (a negative number in dB value, where 0 dB is full scale)
        /// </summary>
        private const int MinEnergy = -90;

        /// <summary>
        /// Width of bitmap that stores audio stream energy data ready for visualization.
        /// </summary>
        private const int EnergyBitmapWidth = 780;

        /// <summary>
        /// Height of bitmap that stores audio stream energy data ready for visualization.
        /// </summary>
        private const int EnergyBitmapHeight = 195;

        /// <summary>
        /// Rectangle representing the entire energy bitmap area. Used when drawing background
        /// for energy visualization.
        /// </summary>
        private readonly Int32Rect fullEnergyRect = new Int32Rect(0, 0, EnergyBitmapWidth, EnergyBitmapHeight);

        /// <summary>
        /// Array of background-color pixels corresponding to an area equal to the size of whole energy bitmap.
        /// </summary>
        private readonly byte[] backgroundPixels = new byte[EnergyBitmapWidth * EnergyBitmapHeight];

        /// <summary>
        /// Will be allocated a buffer to hold a single sub frame of audio data read from audio stream.
        /// </summary>
        private readonly byte[] audioBuffer = null;

        /// <summary>
        /// Buffer used to store audio stream energy data as we read audio.
        /// We store 25% more energy values than we strictly need for visualization to allow for a smoother
        /// stream animation effect, since rendering happens on a different schedule with respect to audio
        /// capture.
        /// </summary>
        private readonly float[] energy = new float[(uint)(EnergyBitmapWidth * 1.25)];

        /// <summary>
        /// Object for locking energy buffer to synchronize threads.
        /// </summary>
        private readonly object energyLock = new object();

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Reader for audio frames
        /// </summary>
        private AudioBeamFrameReader reader = null;

        /// <summary>
        /// Last observed audio beam angle in radians, in the range [-pi/2, +pi/2]
        /// </summary>
        private float beamAngle = 0;

        /// <summary>
        /// Last observed audio beam angle confidence, in the range [0, 1]
        /// </summary>
        public static float beamAngleInDeg = 0;
        public static float beamAngleConfidence = 0;

        /// <summary>
        /// Sum of squares of audio samples being accumulated to compute the next energy value.
        /// </summary>
        private float accumulatedSquareSum;

        /// <summary>
        /// Number of audio samples accumulated so far to compute the next energy value.
        /// </summary>
        private int accumulatedSampleCount;

        /// <summary>
        /// Index of next element available in audio energy buffer.
        /// </summary>
        private int energyIndex;

        /// <summary>
        /// Number of newly calculated audio stream energy values that have not yet been
        /// displayed.
        /// </summary>
        private int newEnergyAvailable;
        //ProjectOxford Part End
        public static DispatcherTimer ReadNewsTimer = new DispatcherTimer();//The timer used in reading news function
        public static DispatcherTimer GrammarTimer = new DispatcherTimer();//The timer used in reading news function
        public static DispatcherTimer WeatherIconTimer = new DispatcherTimer();//The timer used in weather icon

        #endregion defineVar

        public static bool DemoMode = false;

        string ResultName = "";
        string RuleName = "";
        string Country = "";
        string Day = "";
        string Duration = "";
        string question = "";
        string subday = "";
        int subday_num = 0;
        string subhour = "";
        string subhour_12 = "";
        string subminute = "";
        string reminder_task = "";
        string text_test;
        string president = "";
        string ContactName;
        string homecountry = "singapore";
        string NewsType = null;

        string alarm;
        DateTime alarmTime;
        Thread alarmThread;
        System.DateTime EventTime;

        BitmapImage b = new BitmapImage();





        string fileLoc = @"C:\test\SMS.txt";

        string device;

        string country_number = "";
        string phone_number = "";
        string country_phone_number = "";
        int i = 1;
        int CurrentGenreIndex;
        string current_MusicOrSongGenre = "";
        string number;
        string SMSContent;
        public static string language = "English";
        public bool EnglishNewsMark = true;
        bool pass = true;
        bool OneTimeMark = false;

        string remind_time = null;
        int substringminute = 60;
        //public SocialRobot.Application.WeatherApp WeatherFunction = new SocialRobot.Application.WeatherApp();
        public SocialRobot.Application.WikiApp WifiFunction = new SocialRobot.Application.WikiApp();
        public Application.SetReminder SetReminderFunction = new Application.SetReminder();
        public static SocialRobot.Function.Speech_Rcognition_Grammar SRG = new SocialRobot.Function.Speech_Rcognition_Grammar();
        public Application.ReadNewsApp ReadNewsFunction = new Application.ReadNewsApp();
        public Application.DateTimeApp DateTimeFunction = new Application.DateTimeApp();
        public Application.SkypeApp.SkypeApp Skype = new Application.SkypeApp.SkypeApp();
        Application.musicplayer mp3 = new Application.musicplayer();
        Application.health Health = new Application.health();
        //public Application.SkypeApp.Skype_Interface SkypeInterface = new Application.SkypeApp.Skype_Interface();
        public Application.iRKit.iRKitApp iRKit = new Application.iRKit.iRKitApp();
        public Application.Light.LightControl LightCon = new Application.Light.LightControl();

        public SocialRobot.Setting Setting_Windows = new Setting();
        public SocialRobot.HealthPage HealthWindows = new HealthPage();
        public SocialRobot.keypad dialpad = new keypad();


        // O_NLP.RootObject is a class that contains the data interpreted from wit.ai
        SocialRobot.Wit.Objects.O_NLP.RootObject oNLP = new SocialRobot.Wit.Objects.O_NLP.RootObject();

        // NLP_Processing is the code that processes the response from wit.ai
        SocialRobot.Wit.Vitals.NLP.NLP_Processing vitNLP = new SocialRobot.Wit.Vitals.NLP.NLP_Processing();

        // Winmm.dll is used for recording speech
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);


        // Variables used for speech recording
        //private bool recording = false;
        //private string speechfilename = "";
        // Set a timer to make sure recording doesn't exceed 10 seconds
        //private System.Timers.Timer speechTimer = new System.Timers.Timer();
        private System.Timers.Timer PhoneCallTimer = new System.Timers.Timer();

        public static bool FacialExpressionMark = true;

        string XunFei_result;

        Function.Motor Motor = new Function.Motor();
        Function.Record Record = new Function.Record();

        public static int HelloCount = 0;

        public bool VideoCallFlag = false;



        public MainWindow()
        {
            SetReminderFunction.GoogleCalendar();

            //SpeechRcognitionSystem.SRGS_GrammarModels();
            InitializeComponent();
            // VisionDisplay visiondisplay = new VisionDisplay();

            ReadNewsTimer.Tick += new EventHandler(ReadNewsTimer_TimeUp);
            ReadNewsTimer.Interval = new TimeSpan(0, 0, 35);
            GrammarTimer.Tick += new EventHandler(GrammarTimer_TimeUp);
            GrammarTimer.Interval = new TimeSpan(0, 0, 10);
            WeatherIconTimer.Tick += new EventHandler(WeatherIconTimer_TimeUp);
            WeatherIconTimer.Interval = new TimeSpan(0, 0, 10);

            try
            {
                Function.FaceLED.Instance.Normal();
            }
            catch
            {

            }
            try
            {
                if (!DemoMode)
                {
                    Function.Vision.Instance.FaceTrackingTimerInitialize();
                }
            }
            catch
            {

            }

            Motor.MotorInitialize();

            SRG.SpeechRecognized();
            // SRG.SRE.SpeechDetected
            SRG.SRE.SpeechRecognized += SRE_SpeechRecognized;
            SRG.SRECN.SpeechRecognized += SRE_SpeechRecognized;
            SRG.SREJP.SpeechRecognized += SRE_SpeechRecognized;
            SRG.SRECAN.SpeechRecognized += SRE_SpeechRecognized;
            //Audio Level Detected
            SRG.SRE.AudioLevelUpdated += SRE_AudioLevelUpdated;         // English
            SRG.SRECN.AudioLevelUpdated += SRE_AudioLevelUpdated;         // Chinese
            SRG.SREJP.AudioLevelUpdated += SRE_AudioLevelUpdated;         // Japanese
            SRG.SRECAN.AudioLevelUpdated += SRE_AudioLevelUpdated;         // Cantnonse

            #region Stop and previous speech
            // variable to stop SpeakAsync
            var current = SRG.SRE_Speech.GetCurrentlySpokenPrompt();
            if (current != null)
                SRG.SRE_Speech.SpeakAsyncCancel(current);

            current = SRG.SRECN_Speech.GetCurrentlySpokenPrompt();
            if (current != null)
                SRG.SRECN_Speech.SpeakAsyncCancel(current);

            current = SRG.SREJP_Speech.GetCurrentlySpokenPrompt();
            if (current != null)
                SRG.SREJP_Speech.SpeakAsyncCancel(current);

            current = SRG.SRECAN_Speech.GetCurrentlySpokenPrompt();
            if (current != null)
                SRG.SRECAN_Speech.SpeakAsyncCancel(current);

            #endregion Stop and previous speech

            // SRG.SRECN_Speech.GetInstalledVoices(new System.Globalization.CultureInfo("zh-CN"));
            try
            {
                SRG.SRE_Speech.SelectVoice("IVONA 2 Amy");
                //SRG.SRE_Speech.SelectVoice("Microsoft Zira Desktop");
                 SRG.SRE_Speech.SetOutputToDefaultAudioDevice();

                SRG.SRECN_Speech.SelectVoice("Microsoft Huihui Desktop");
                SRG.SRECN_Speech.SetOutputToDefaultAudioDevice();

                SRG.SREJP_Speech.SelectVoice("VW Misaki");
                //SRG.SREJP_Speech.SelectVoice("Microsoft Haruka Desktop");
                SRG.SREJP_Speech.SetOutputToDefaultAudioDevice();

                SRG.SRECAN_Speech.SelectVoice("Microsoft Tracy Desktop");
                SRG.SRECAN_Speech.SetOutputToDefaultAudioDevice();
            }
            catch
            {
                SRG.SRE_Speech.Speak("Please install Chinese and Japanese language pack!");
                Environment.Exit(0);
            }

            Function.Vision.Instance.FaceTrackingTimer.Tick += new EventHandler(UserDataUpdate);

            SRG.SRECN_Speech.SpeakProgress += SRG.SRECN_Speech_SpeakProgress;
            SRG.SRE_Speech.SpeakProgress += SRG.SRE_Speech_SpeakProgress;
            SRG.SREJP_Speech.SpeakProgress += SRG.SREJP_Speech_SpeakProgress;
            SRG.SRECAN_Speech.SpeakProgress += SRG.SRECAN_Speech_SpeakProgress;

            DateTimeFunction.Speak.SRE_Speech.SpeakProgress += SRG.SRE_Speech_SpeakProgress;
            DateTimeFunction.Speak.SRECN_Speech.SpeakProgress += SRG.SRECN_Speech_SpeakProgress;
            DateTimeFunction.Speak.SREJP_Speech.SpeakProgress += SRG.SREJP_Speech_SpeakProgress;
            DateTimeFunction.Speak.SRECAN_Speech.SpeakProgress += SRG.SRECAN_Speech_SpeakProgress;


            ReadNewsFunction.SRG.SRE_Speech.SpeakProgress += SRG.SRE_Speech_SpeakProgress;

            try
            {
                if (!DemoMode)
                {
                    Function.Vision.Instance.SRG.SRE_Speech.SpeakProgress += SRG.SRE_Speech_SpeakProgress;
                }
            }
            catch
            {

            }

            SRG.SRECN_Speech.SpeakCompleted += SRG.SRECN_Speech_SpeakCompleted;
            SRG.SRE_Speech.SpeakCompleted += SRG.SRE_Speech_SpeakCompleted;
            SRG.SREJP_Speech.SpeakCompleted += SRG.SREJP_Speech_SpeakCompleted;
            SRG.SRECAN_Speech.SpeakCompleted += SRG.SRECAN_Speech_SpeakCompleted;

            DateTimeFunction.Speak.SRE_Speech.SpeakCompleted += SRG.SRE_Speech_SpeakCompleted;
            DateTimeFunction.Speak.SRECN_Speech.SpeakCompleted += SRG.SRECN_Speech_SpeakCompleted;
            DateTimeFunction.Speak.SREJP_Speech.SpeakCompleted += SRG.SREJP_Speech_SpeakCompleted;
            DateTimeFunction.Speak.SRECAN_Speech.SpeakCompleted += SRG.SRECAN_Speech_SpeakCompleted;


            //WeatherFunction.Speak.SRE_Speech.SpeakCompleted += SRG.SRE_Speech_SpeakCompleted;
            ReadNewsFunction.SRG.SRE_Speech.SpeakCompleted += SRG.SRE_Speech_SpeakCompleted;

            try
            {
                if (!DemoMode)
                {
                    Function.Vision.Instance.SRG.SRE_Speech.SpeakCompleted += SRG.SRE_Speech_SpeakCompleted;
                }
            }
            catch
            {

            }


            //speechTimer = new System.Timers.Timer();
            //speechTimer.Elapsed += new ElapsedEventHandler(OnTimedSpeechEvent);
            //speechTimer.Interval = 4000; //6 seconds

            //!!!!!Xunfei = new Wave();
            //!!!!!Xunfei.ErrorEvent += new ErrorEventHandle(Xunfei_ErrorEvent);
            //!!!!!Xunfei.SavedFile = AppDomain.CurrentDomain.BaseDirectory + "aaa.wav";
            //!!!!!Xunfei.RecordQuality = Quality.Height;

            Language_text.Text = language;
            //Record_State.Text = recording.ToString();
            //WitKeyIn.Focus();
            //bv  PhoneCallTimer.Enabled = true;

            try
            {
                loadxml();
                setalarm();
            }

            catch
            {
            }
            //SRG.SRE_Speech.SpeakAsync("Hi, I'm Ruth.   Your personal companion.");!!
        }

        private void SRE_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            SRG.SRE.AudioLevelUpdated -= SRE_AudioLevelUpdated;

            Volume.Value = e.AudioLevel * 5;
            Setting_Windows.Audio_Level.Value = e.AudioLevel * 15;
            Setting_Windows.tb_audiolevel.Text = e.AudioLevel.ToString();
            SRG.SRE.AudioLevelUpdated += SRE_AudioLevelUpdated;
        }

        //private void tbYou_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter && WitKeyIn.Text.Length > 0)
        //    {
        //        WitKeyIn.IsEnabled = false;
        //        StartProcessing(WitKeyIn.Text, 0);
        //        WitKeyIn.Text = "";
        //        WitFeedBack.Text = "Hold on..";
        //        WitKeyIn.Focus();
        //        SRG.LayerGrammarLoadAndUnload(RuleName, ResultName);
        //    }
        //}

        //void SRECN_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        // {
        //     SRG.SRE.SpeechRecognized -= SRE_SpeechRecognized;
        //     tb_confidence.Text = "Confidence: " + e.Result.Confidence;
        //     RuleName = e.Result.Grammar.RuleName;
        //     ResultName = e.Result.Text;

        //     if (e.Result.Confidence > 0.9)
        //     {
        //         tb_result_text.Text = tb_result_text.Text + "\n\n" + e.Result.Text + "\n" + e.Result.Grammar.RuleName + "\n";
        //         tb_result_text.ScrollToEnd();
        //         SRG.LayerGrammarLoadAndUnload(RuleName, ResultName);
        //         ThinkingProcess(RuleName, ResultName);
        //     }
        // }

        void SRE_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            SRG.SRE.SpeechRecognized -= SRE_SpeechRecognized;
            Setting_Windows.tb_confidence.Text = "Confidence: " + e.Result.Confidence;
            // tb_confidence.Text = "Confidence: " + e.Result.Confidence;
            RuleName = e.Result.Grammar.RuleName;
            ResultName = e.Result.Text;
            if (RuleName == "SGM_FUNC_AskTodayCustomWeather" || RuleName == "SGM_FUNC_AskTomorrowCustomWeather" || RuleName == "SGM_DIAL_AskCountryPresidentOrPrimeMinister" || RuleName == "SGM_FUNC_COMPLEX_SetReminder" || RuleName == "SGM_NewsOption" || RuleName == "SGM_FUNC_COMPLEX_天気")
            {
                RecognizedAudio audio = e.Result.Audio;
                TimeSpan start = new TimeSpan(0);
                TimeSpan duration = audio.Duration - start;
                // Add code to verify and persist the audio.

                string path = SRG.BaseFolder + @"Wav\nameAudio.wav";
                using (Stream outputStream = new FileStream(path, FileMode.Create))
                {
                    RecognizedAudio nameAudio = audio.GetRange(start, duration);
                    nameAudio.WriteToWaveStream(outputStream);
                    outputStream.Close();
                    Setting_Windows.WitFeedBack.Text = "Hold on please";
                    if (RuleName == "SGM_FUNC_AskTodayCustomWeather" || RuleName == "SGM_FUNC_AskTomorrowCustomWeather" || RuleName == "SGM_DIAL_AskCountryPresidentOrPrimeMinister" || RuleName == "SGM_FUNC_COMPLEX_SetReminder" || RuleName == "SGM_FUNC_COMPLEX_天気")
                    {
                        SRG.SRE_Speech.SpeakAsync("Hold on please");
                    }
                    StartProcessing(path, 1);
                }
            }
            if (e.Result.Confidence > 0.85)
            {
                tb_result_text.Text = e.Result.Text + "\n";
                Setting_Windows.Setting_Result.Text = Setting_Windows.Setting_Result.Text + "\n\n" + e.Result.Text + "\n" + e.Result.Grammar.RuleName + "\n";
                SRG.LayerGrammarLoadAndUnload(RuleName, ResultName);
                ThinkingProcess(RuleName, ResultName);
            }
            else
            {
                //added in THEF 20131116
                //SRG.SRE_Speech.SpeakAsync("Sorry I don't understand, can you repeat again?");
            }


            SRG.SRE.SpeechRecognized += SRE_SpeechRecognized;
        }



        public async void StartProcessing(string text, int type)
        {
            try
            {
                string modtext = SocialRobot.Wit.Vitals.NLP.Pre_NLP_Processing.preprocessText(text);

                string nlp_text = "";

                if (type == 0)
                {
                    nlp_text = await vitNLP.ProcessWrittenText(modtext);
                }
                else
                {
                    nlp_text = await vitNLP.ProcessSpokenText(text);
                }

                // If the audio file doesn't contain anything, or wit.ai doesn't understand it, a code 400 will be returned
                if (nlp_text.Contains("The remote server returned an error: (400) Bad Request"))
                {
                    Setting_Windows.WitFeedBack.Text = "Sorry, didn't get that. Could you please repeat yourself?";
                    // WitKeyIn.IsEnabled = true;
                    Setting_Windows.WitRaw.Text = nlp_text;
                    return;
                }

                Setting_Windows.WitRaw.Text = nlp_text;

                oNLP = SocialRobot.Wit.Vitals.NLP.Post_NLP_Processing.ParseData(nlp_text);

                // This codeblock dynamically casts the intent to the corresponding class
                // Check README.txt in Vitals.Brain
                Assembly objAssembly;
                objAssembly = Assembly.GetExecutingAssembly();

                Type classType = objAssembly.GetType("SocialRobot.Wit.Vitals.Brain." + oNLP.entities.intent);

                //object obj = Activator.CreateInstance(classType);

                // MethodInfo mi = classType.GetMethod("makeSentence");

                object[] parameters = new object[1];
                parameters[0] = oNLP;

                //   mi = classType.GetMethod("makeSentence");
                //    string sentence = "";
                //   sentence = (string)mi.Invoke(obj, parameters);


                if (RuleName == "SGM_FUNC_AskTodayCustomWeather" || RuleName == "SGM_FUNC_AskTomorrowCustomWeather"/* || RuleName == "SGM_FUNC_天気"*/)
                {
                    Day = oNLP.entities.day[0].value;
                    Country = oNLP.entities.location[0].value;
                }

                if (RuleName == "SGM_DIAL_AskCountryPresidentOrPrimeMinister")
                {
                    question = "who is the " + oNLP.entities.role[0].value + " of " + oNLP.entities.location[0].value;
                }

                if (RuleName == "SGM_FUNC_COMPLEX_SetReminder" || RuleName == "")
                {
                    subday = oNLP.entities.datetime[0].value.Day.ToString();
                    if (subday == DateTime.Now.Date.Day.ToString())
                    {
                        subday_num = DateTime.Now.Date.Day;
                        subday = "today";
                    }
                    else if (subday != DateTime.Now.Date.Day.ToString())
                    {
                        subday = "tomorrow";
                        subday_num = DateTime.Now.Date.Day + 1;
                    }
                    subhour = oNLP.entities.datetime[0].value.Hour.ToString();
                    subminute = oNLP.entities.datetime[0].value.Minute.ToString();
                    reminder_task = oNLP.entities.reminder[0].value;
                    remind_time = oNLP.entities.datetime[0].value.ToString();
                }

                if (RuleName == "SGM_NewsOption")
                {
                    try
                    {
                        NewsType = oNLP.entities.news[0].value;
                    }
                    catch
                    {
                        NewsType = null;
                    }
                }

                // Show what was deducted from the sentence
                //tbI.Text = sentence;           
                //  WitKeyIn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Setting_Windows.WitFeedBack.Text = "Sorry, I didn't get that. Could you please repeat yourself?";
                //   WitKeyIn.IsEnabled = true;
                //   WitFeedBack.Text = "Sorry, no idea what's what. Try again later please!" + Environment.NewLine + Environment.NewLine + "I bumped onto this error: " + ex.Message;
            }

            if (RuleName == "SGM_FUNC_AskTodayCustomWeather")
            {
                // if (Country != "")
                SRG.SRE.RecognizeAsyncCancel();
                getweather(Country, Day);
                // else
                //UI.Text = Country;
                //UI.Text = Day;
                // SRG.SRE_Speech.SpeakAsync("Please repeat");

            }
            //if (RuleName == "SGM_FUNC_天気")
            //{
                //getweather(Country, Day);
            //}
            if (RuleName == "SGM_FUNC_AskTomorrowCustomWeather")
            {
                SRG.SRE.RecognizeAsyncCancel();
                getweather(Country, Day);
            }

            if (RuleName == "SGM_DIAL_AskCountryPresidentOrPrimeMinister")
            {
                WifiFunction.AskPresidentOrPrimeMinister_Async(question);
                //president = WifiFunction.getdata();
                //SRG.SRE_Speech.SpeakAsync(president);
            }

            if (RuleName == "SGM_FUNC_COMPLEX_SetReminder" || RuleName == "")
            {
                try
                {
                    if (Convert.ToInt32(subhour) > 12)
                    {
                        int temp_12 = 0;
                        temp_12 = Convert.ToInt32(subhour) - 12;
                        subhour_12 = temp_12.ToString();
                    }
                    else subhour_12 = subhour;
                    if (subminute == "0")
                    {
                        SRG.SRE_Speech.SpeakAsync("Set reminder for  " + reminder_task + " at " + subhour_12 + "o'clock ," + " ," + subday + " ?");
                    }
                    else if (subminute != "0")
                    {
                        SRG.SRE_Speech.SpeakAsync("Set reminder for  " + reminder_task + " at " + subhour_12 + " ," + subminute + " ," + subday + " ?");
                    }
                }
                catch
                {
                    SRG.SRE_Speech.SpeakAsync("Sorry I can't set the reminder, can you repeat again?");
                }
            }

            if (RuleName == "SGM_NewsOption")
            {
                SRG.UnloadGrammar(SRG.SGM_NewsOption);
                SRG.LoadGrammar(SRG.SGM_FUNC_NextNews);
                EnglishNewsMark = true;
                ReadNewsFunction.flag_StartNewsReading = true;
                ReadNewsFunction.NewsReadingFunction_Eng(NewsType);
                if(!Application.ReadNewsApp.NewsTypeSuccess)
                {
                    if(NewsType!=null)
                    {
                        SRG.SRE_Speech.SpeakAsync("Sorry, I cannot get " + NewsType + " news now.");
                        SRG.UnloadGrammar(SRG.SGM_NewsOption);
                        SRG.LoadGrammar(SRG.SGM_FUNC_ReadNews);
                        SRG.UnloadGrammar(SRG.SGM_FUNC_NextNews);
                        SRG.UnloadGrammar(SRG.SGM_FUNC_StopReadNews);
                        MainWindow.GrammarTimer.Stop();
                    }
                    else
                    {
                        SRG.SRE_Speech.SpeakAsync("Sorry I didn't hear clearly, can you repeat the news option again?");
                        SRG.LoadGrammar(SRG.SGM_NewsOption);
                        SRG.UnloadGrammar(SRG.SGM_FUNC_ReadNews);
                        SRG.UnloadGrammar(SRG.SGM_FUNC_NextNews);
                        SRG.UnloadGrammar(SRG.SGM_FUNC_StopReadNews);
                        MainWindow.GrammarTimer.Stop();
                    }
                }
            }
        }


        public void ThinkingProcess(string RuleName, string ResultName)
        {
            string 曜日;
            Random RanNum = new Random();
            int RandomNumber = RanNum.Next(1, 101);
            if (Function.Vision.WakeUp)
            {
                switch (RuleName)
                {

                    case "SGM_DIAL_NiceToMeetYou":
                        //robotEmoton_show("happiness");
                        // flag_speak_completed = false;
                        UI.Text = "Nice to meet you too!";
                        try
                        {
                            Function.FaceLED.Instance.Happy();
                        }
                        catch { }
                        Thread.Sleep(500);
                        if (Function.Vision.UserName == "Unknown")
                        {
                            SRG.SRE_Speech.SpeakAsync("Nice to meet you too!");
                        }
                        else
                        {
                            SRG.SRE_Speech.SpeakAsync("Nice to meet you too!" + Function.Vision.UserName);
                        }
                        if (Function.Vision.Instance.EmotionConfidence >= 70 && Function.Vision.Instance.Emotion != "Neutral")
                        {
                            SRG.SRE_Speech.SpeakAsync("By the way, ");
                            EmotionDetection();
                        }
                        break;

                    case "SGM_DIAL_AskIntroduction":
                        // robotEmoton_show("neutral");
                        //  flag_speak_completed = false;
                        if (RandomNumber <= 90)
                        {
                            if (ResultName == "introduce yourself")
                            {
                                Function.FaceLED.Instance.Happy();
                                Thread.Sleep(1000);
                                SRG.SRE_Speech.SpeakAsync("My name is Ruth, I am a social robot designed by students from Nanyung Polytechnic and Kitakyushu National College of Technology. I have the abilities to recognize human emotions and natural language. I can also perform a variety of function such as news reading, weather broadcast, skype call or even music playing.");
                            }
                            else
                            {
                                Function.FaceLED.Instance.Happy();
                                Thread.Sleep(1000);
                                SRG.SRE_Speech.SpeakAsync("I can perform lots of functions. I can turn on the lights by remote controlling. I can recognize human's facial expression and tell when you are happy or when you are sad. I can set reminders for taking medicine, appointments. I can also call for help when the elderly is in danger by making phone calls using Skype.");
                            }
                        }
                        else
                        {
                            Function.FaceLED.Instance.Sad();
                            Thread.Sleep(1000);
                            SRG.SRE_Speech.SpeakAsync("I'm so tired.");
                        }
                        break;


                    case "SGM_DIAL_AskRobotName":
                        //robotEmoton_show("neutral");

                        //   if (flag_registered == false)
                        //   {
                        //       flag_speak_completed = false;
                        //       synthesizer.SpeakAsync("My name is Eva. What about you?");
                        //    }
                        //   else
                        //    {
                        //        flag_speak_completed = false;
                        //        synthesizer.SpeakAsync("I'm Eva, my dear " + user_username + ". Do you forget me?");
                        //        flag_registered = false;
                        //    }
                        Function.FaceLED.Instance.Happy();
                        Thread.Sleep(800);
                        if (Function.Vision.UserName == "Unknown")
                        {
                            SRG.SRE_Speech.SpeakAsync("My name is Ruth. What about you?");
                        }
                        else
                        {
                            SRG.SRE_Speech.SpeakAsync("I'm Ruth, " + Function.Vision.UserName + ". you forgot me?");
                        }
                        UI.Text = "I'm Ruth";
                        break;

                    //    case "SGM_DIAL_AskFunctions":
                    //    robotEmoton_show("neutral");
                    //    flag_speak_completed = false;
                    //    SRG.SRE_Speech.SpeakAsync("I can perform a variety of functions.");

                    //break;
                    case "SGM_DIAL_AskWhoDesign":
                        // robotEmoton_show("neutral");
                        // flag_speak_completed = false;
                        if (ResultName == "how old are you")
                        {
                            Motor.ArmInitialize();
                            SRG.SRE_Speech.SpeakAsync("I was created in 2014. So, does that means I'm " + (System.DateTime.Now.Year - 2014) + " years old?");
                            Motor.LeftArmRest();
                            Motor.RightArmRest();
                            SRG.SRE.RecognizeAsync(RecognizeMode.Multiple);
                        }
                        else
                        {
                            Motor.ArmInitialize();
                            SRG.SRE_Speech.SpeakAsync("I was designed by students from Nanyang Polytechnic and Kitakyushu National College of Technology.");
                            Motor.LeftArmRest();
                            Motor.RightArmRest();
                            SRG.SRE.RecognizeAsync(RecognizeMode.Multiple);
                        }
                        break;

                    case "SGM_DIAL_AskWhatIsSocialRobot":
                        //robotEmoton_show("neutral");
                        // flag_speak_completed = false;
                        Motor.ArmInitialize();
                        SRG.SRE_Speech.Speak("Sure. According to Wikipedia, a social robot is an autonomous robot that interacts and communicates with humans or other autonomous physical agents by following social behaviors and rules attached to its role.");
                        Motor.LeftArmRest();
                        Motor.RightArmRest();
                        SRG.SRE.RecognizeAsync(RecognizeMode.Multiple);
                        break;

                    case "SGM_DIAL_AskRobotHowAreYou":
                        //robotEmoton_show("happiness");
                        //flag_speak_completed = false;
                        UI.Text = "I'm fine!";
                        if (RandomNumber < 25)
                        {
                            try
                            {
                                Function.FaceLED.Instance.Happy();
                            }
                            catch { }
                            Thread.Sleep(500);
                            SRG.SRE_Speech.SpeakAsync("I am fine, thank you.");
                        }
                        else if (RandomNumber < 50)
                        {
                            try
                            {
                                Function.FaceLED.Instance.Happy();
                            }
                            catch { }
                            Thread.Sleep(500);
                            if (Function.Vision.UserName == "Unknown")
                            {
                                SRG.SRE_Speech.SpeakAsync("Excellent. How are you?");
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("Excellent. How are you? " + Function.Vision.UserName);
                            }
                        }
                        else if (RandomNumber < 75)
                        {
                            if (Function.Vision.UserName == "Unknown")
                            {
                                SRG.SRE_Speech.SpeakAsync("Not bad.");
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("Not bad. " + Function.Vision.UserName);
                            }
                        }
                        else
                        {
                            try
                            {
                                Function.FaceLED.Instance.Happy();
                            }
                            catch { }
                            Thread.Sleep(500);
                            SRG.SRE_Speech.SpeakAsync("Pretty good.");
                        }
                        if (Function.Vision.Instance.EmotionConfidence >= 70 && Function.Vision.Instance.Emotion != "Neutral")
                        {
                            SRG.SRE_Speech.SpeakAsync("By the way, ");
                            EmotionDetection();
                        }
                        break;

                    case "SGM_DIAL_Compliment":
                        //robotEmoton_show("happiness");
                        //flag_speak_completed = false;
                        UI.Text = "Thank you!";
                        try
                        {
                            Function.FaceLED.Instance.Smile();
                        }
                        catch { }
                        Motor.ArmInitialize();
                        Thread.Sleep(500);
                        if (Function.Vision.UserName == "Unknown")
                        {
                            SRG.SRE_Speech.SpeakAsync("Thank you very much.");
                        }
                        else
                        {
                            SRG.SRE_Speech.SpeakAsync("Thank you, " + Function.Vision.UserName);
                        }
                        break;

                    case "SGM_DIAL_Scold":
                        // robotEmoton_show("sadness");
                        //  flag_speak_completed = false;
                        if (ResultName == "fuck you" && RandomNumber <= 30)
                        {
                            UI.Text = "Angry";
                            try
                            {
                                Function.FaceLED.Instance.Angry();
                                Motor.EyesHalfClose();
                            }
                            catch { }
                            Thread.Sleep(500);
                        }
                        else
                        {
                            UI.Text = "I'm so sorry";
                            try
                            {
                                Function.FaceLED.Instance.Sad();
                            }
                            catch { }
                            Thread.Sleep(500);
                            if (Function.Vision.UserName == "Unknown")
                            {
                                SRG.SRE_Speech.SpeakAsync("I am so sorry.");
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("I'm sorry, " + Function.Vision.UserName);
                            }
                        }
                        break;

                    case "SGM_DIAL_ThankYou":
                        //robotEmoton_show("happiness");
                        //flag_speak_completed = false;
                        UI.Text = "You are welcome";
                        try
                        {
                            Function.FaceLED.Instance.Happy();
                        }
                        catch { }
                        Thread.Sleep(500);
                        if (RandomNumber < 33)
                        {
                            SRG.SRE_Speech.SpeakAsync("You are welcome.");
                        }
                        else if (RandomNumber < 66)
                        {
                            if (Function.Vision.UserName == "Unknown")
                            {
                                SRG.SRE_Speech.SpeakAsync("My pleasure.");
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("My pleasure. " + Function.Vision.UserName);
                            }
                        }
                        else
                        {
                            SRG.SRE_Speech.SpeakAsync("No problem.");
                        }
                        // SRG.LayerGrammarLoadAndUnload("SGM_DIAL_ThankYou", "Initialize");
                        break;

                    case "SGM_FUNC_GoingOut":
                        switch (ResultName)
                        {
                            case "how is the weather":
                                getweather(homecountry, "today");
                                break;

                            case "What is the weather":
                                getweather(homecountry, "today");
                                break;

                            case "I am going out now":
                                getweather(homecountry, "goingout");
                                break;

                            case "how is the weather today":
                                getweather(homecountry, "today");
                                break;
                            case "how is the weather tomorrow":
                                getweather(homecountry, "tomorrow");
                                break;
                            case "how is the weather for today":
                                getweather(homecountry, "today");
                                break;
                            case "how is the weather for tomorrow":
                                getweather(homecountry, "tomorrow");
                                break;
                            case "how is the weather now":
                                getweather(homecountry, "now");
                                break;

                        }
                        break;

                    case "SGM_DIAL_SayHello":
                        //robotEmoton_show("happiness");
                        // flag_speak_completed = false;
                        // if (ProcessValid == false)
                        // {
                        //     synthesizer.SpeakAsync(ResultText);
                        // }
                        //  else
                        //  {
                        //       synthesizer.SpeakAsync("Hello there");
                        //   }
                        //   break;
                        if (!DemoMode)
                        {
                            if (Function.Vision.DataLength < 27)//Nobody in the view
                            {
                                //if (!OneTimeMark)
                                //{
                                //    UI.Text = "Hi!";
                                //    SRG.SRE_Speech.SpeakAsync("Sorry, where are you?");
                                //    OneTimeMark = true;
                                //}
                                //else
                                //{
                                //    UI.Text = "Hi!";
                                //    if (RandomNumber <= 50)
                                //    {
                                //        SRG.SRE_Speech.SpeakAsync("Hi.");
                                //    }
                                //    else
                                //    {
                                //        SRG.SRE_Speech.SpeakAsync("Hello.");
                                //    }
                                //}
                                //Motor.NeckRandom();
                            }
                            else if (Function.Vision.UserName != "Unknown")
                            {
                                UI.Text = "Hi!";
                                if (RandomNumber <= 50)
                                {
                                    SRG.SRE_Speech.SpeakAsync("Hi " + Function.Vision.UserName);
                                }
                                else
                                {
                                    SRG.SRE_Speech.SpeakAsync("Hello " + Function.Vision.UserName);
                                }
                                OneTimeMark = false;
                                Motor.RightArmRaise();
                                if (Function.Vision.Instance.EmotionConfidence >= 70 && Function.Vision.Instance.Emotion != "Neutral")
                                {
                                    SRG.SRE_Speech.SpeakAsync("By the way, ");
                                    EmotionDetection();
                                }
                            }
                            else if (Function.Vision.RegisterModeSwitch && Function.Vision.DataLength == 27)
                            {
                                Function.Vision.Instance.RegisterMode();
                                UI.Text = "Hi!";
                                SRG.SRE_Speech.SpeakAsync("Sorry, it seems I never seen you before. Do you want to register in my database?");
                                SRG.LoadGrammar(SRG.SGM_FUNC_RegisterMode_YesNo);
                                GrammarTimer.Start();
                                OneTimeMark = false;
                                Motor.RightArmRaise();

                            }
                            else
                            {
                                if (RandomNumber <= 50)
                                {
                                    SRG.SRE_Speech.SpeakAsync("Hi.");
                                }
                                else
                                {
                                    SRG.SRE_Speech.SpeakAsync("Hello.");
                                }
                                OneTimeMark = false;
                                Motor.RightArmRaise();
                                if (Function.Vision.Instance.EmotionConfidence >= 70 && Function.Vision.Instance.Emotion != "Neutral")
                                {
                                    SRG.SRE_Speech.SpeakAsync("By the way, ");
                                    EmotionDetection();
                                }
                            }
                        }
                        else
                        {
                            if (RandomNumber <= 50)
                            {
                                SRG.SRE_Speech.SpeakAsync("Hi.");
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("Hello.");
                            }
                            Motor.RightArmRaise();
                        }
                        break;

                    case "SGM_DIAL_GoodBye":  // function uncommented and deleted away old code added in by thef 20160811
                        UI.Text = "Goodbye";
                        Function.FaceLED.Instance.Happy();
                        Thread.Sleep(100);
                        if (Function.Vision.UserName == "Unknown")
                        {
                            SRG.SRE_Speech.SpeakAsync("Goodbye.");// added in by thef 20160811
                        }
                        else
                        {
                            SRG.SRE_Speech.SpeakAsync("Goodbye." + ", " + Function.Vision.UserName);// added in by thef 20160811
                        }
                         Thread.Sleep(100);
                         SRG.SRE_Speech.SpeakAsync("Nice talking to you.");// added in by thef 20160811
                         Thread.Sleep(100);
                         SRG.SRE_Speech.SpeakAsync("Hope to see you soon.");// added in by thef 20160811
                                               
                        break;

                    case "SGM_DIAL_Greeting":
                        //good morning evening datetime
                        string CurrentGreeting = null;
                        if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
                        {
                            CurrentGreeting = "Good Morning";
                        }
                        else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
                        {
                            CurrentGreeting = "Good Afternoon";
                        }
                        else if (DateTime.Now.Hour >= 18 && DateTime.Now.Hour < 24)
                        {
                            CurrentGreeting = "Good Evening";
                        }
                        else
                        {
                            CurrentGreeting = "Good Night";
                        }

                        if (ResultName == CurrentGreeting)
                        {
                            if (Function.Vision.UserName == "Unknown")
                            {
                                SRG.SRE_Speech.SpeakAsync(CurrentGreeting);
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync(CurrentGreeting + ", " + Function.Vision.UserName);
                            }
                        }
                        else
                        {
                            if (Function.Vision.UserName == "Unknown")
                            {
                                SRG.SRE_Speech.SpeakAsync("I think you should have said " + CurrentGreeting);
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("I think you should have said " + CurrentGreeting + ", " + Function.Vision.UserName);
                            }
                        }
                        break;

                    case "SGM_DIAL_SwitchLanguageToChinese":
                        UI.Text = "Switch to Chinese?";
                        SRG.SRE_Speech.SpeakAsync("Do you want me to speak Chinese?");
                        break;

                    case "SGM_DIAL_SwitchLanguageToChinese_YesNo":
                        if (ResultName == "yes")
                        {
                            UI.Text = "切换至中文";
                            SRG.SRECN_Speech.SpeakAsync("好的，切换至中文语音识别");
                            SwitchToChinese();
                        }
                        else
                        {
                            UI.Text = "Ok, nevermind";
                            SRG.SRE_Speech.SpeakAsync("Ok, never mind");
                        }
                        break;

                    case "SGM_DIAL_SwitchLanguageToJapanese":
                        UI.Text = "Switch to Japanese?";
                        SRG.SRE_Speech.SpeakAsync("Do you want to switch to Japanese speech recgnition?");
                        break;

                    case "SGM_DIAL_SwitchLanguageToJapanese_YesNo":
                        if (ResultName == "yes")
                        {
                            UI.Text = "日本語に切り替えました。";
                            SRG.SREJP_Speech.SpeakAsync("はい、日本語に切り替えました。");
                            SwitchToJapanese();
                        }
                        else
                        {
                            UI.Text = "Ok, nevermind";
                            SRG.SRE_Speech.SpeakAsync("Ok, never mind");
                        }
                        break;

                    case "SGM_DIAL_LookAtMe":
                        if (ResultName == "Look at me")
                        {
                            Motor.EyesBallInitialize();
                            if (RandomNumber <= 50)
                            {
                                Function.FaceLED.Instance.Happy();
                            }
                            else
                            {
                                Function.FaceLED.Instance.Surprise();
                            }
                            if (Function.Vision.Instance.EmotionConfidence >= 30 && Function.Vision.Instance.Emotion != "Neutral")
                            {
                                EmotionDetection();
                            }
                        }
                        else if (ResultName == "ruth, can you smile" || ResultName == "say, cheese")
                        {
                            Function.FaceLED.Instance.Happy();
                        }
                        else if (ResultName == "ruth, do you want me to take a photo" || ResultName == "ruth, let's take a photo" || ResultName == "ruth, let's take a photo together")
                        {
                            Function.FaceLED.Instance.Happy();
                            Function.Motor.Instance.RightArmRaise();
                            Thread.Sleep(800);
                            SRG.SRE_Speech.SpeakAsync("Sure, why not.");
                            Function.FaceLED.Instance.Happy();
                        }
                        else if (ResultName == "ruth I'm here")
                        {
                            if (Convert.ToInt32(2048 + beamAngleInDeg * 1024 / 90) < Motor.BtmNeckUpLimit && Convert.ToInt32(2048 + beamAngleInDeg * 1024 / 90) > Motor.BtmNeckLowLimit)
                            {
                                Motor.MotorWrite_funcNeck(Motor.BtmNeckID, 50, Convert.ToInt32(2048 + beamAngleInDeg * 1024 / 90));
                            }
                            if (RandomNumber <= 50)
                            {
                                SRG.SRE_Speech.SpeakAsync("Oh, Hi.");
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("Oh, Hello.");
                            }
                            Motor.RightArmRaise();
                        }
                        else if (ResultName == "Open your eyes")
                        {
                            Motor.EyesOpen();
                        }
                        else if (ResultName == "ruth are you okay?")
                        {
                            Motor.MidMotorInitialize();
                            SRG.SRE_Speech.SpeakAsync("Oh, I'm OK.");
                        }
                        else if (ResultName == "who is the CEO of Microsoft, ruth?")
                        {
                            SRG.SRE_Speech.SpeakAsync("The CEO of microsoft is Mr Satya Nadella.");
                        }
                        else if (ResultName == "do you know me?" || ResultName == "what's my name?" || ResultName == "who am i, ruth")
                        {
                            if (Function.Vision.UserName != "Unknown")
                            {
                                SRG.SRE_Speech.SpeakAsync("You are " + Function.Vision.UserName);
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("You are my friend.");
                            }
                        }
                        break;

                    case "SGM_DIAL_Sleep":
                        if (ResultName == "have a rest ruth")
                        {
                            if (Function.Vision.UserName == "Unknown")
                            {
                                SRG.SRE_Speech.SpeakAsync("OK, see you next time.");
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("OK, see you next time. " + Function.Vision.UserName);
                            }
                            Thread.Sleep(1000);
                            this.Close();
                        }
                        else
                        {
                            SRG.SRE_Speech.SpeakAsync("Do you want me to go to sleep?");
                        }
                        break;

                    case "SGM_DIAL_Sleep_YesNo":
                        switch (ResultName)
                        {
                            case "yes":
                                if (Function.Vision.UserName == "Unknown")
                                {
                                    SRG.SRE_Speech.SpeakAsync("OK, see you next time.");
                                }
                                else
                                {
                                    SRG.SRE_Speech.SpeakAsync("OK, see you next time. " + Function.Vision.UserName);
                                }
                                this.Close();
                                break;

                            case "no":
                                SRG.SRE_Speech.SpeakAsync("Ok, never mind");
                                break;
                        }

                        break;

                    ////////////////////////////////////////////////////////////////////////////////////////////
                    ////////////////////////////////////////////////////////////////////////////////////////////

                    case "SGM_FUNC_StartRadioStaion":
                        SRG.SRE_Speech.SpeakAsync("Do you want me to start playing radio?");
                        //  SRG.LayerGrammarLoadAndUnload("SGM_FUNC_StartRadioStaion", "");
                        break;

                    case "SGM_FUNC_StopRadioStation":
                        SRG.SRE_Speech.SpeakAsync("Do you want to stop playing radio?");
                        //  SRG.LayerGrammarLoadAndUnload("SGM_FUNC_StopRadioStation", "");
                        break;

                    case "SGM_FUNC_StartRadioStationYesNo":
                        if (ResultName == "yes")
                        {
                            UI.Text = "Ok, start playing radio.";
                            SRG.SRE_Speech.SpeakAsync("Ok, start playing radio.");
                            Process.Start("http://streema.com/radios/play/43058");
                            //  SRG.LayerGrammarLoadAndUnload("SGM_FUNC_StartRadioStationYesNo", "");
                        }
                        else if (ResultName == "no")
                        {
                            UI.Text = "Ok, nevermind";
                            SRG.SRE_Speech.SpeakAsync("Ok, never mind");
                        }

                        break;

                    case "SGM_FUNC_StopRadioStationYesNo":
                        if (ResultName == "yes")
                        {
                            UI.Text = "Ok, close radio";
                            SRG.SRE_Speech.SpeakAsync("Ok, close radio");
                            Process[] processNames = Process.GetProcessesByName("chrome");
                            foreach (Process item in processNames)
                            {
                                item.Kill();
                            }
                            SRG.LayerGrammarLoadAndUnload("SGM_FUNC_StopRadioStationYesNo", "");
                        }

                        else if (ResultName == "no")
                        {
                            UI.Text = "Ok,nevermind";
                            SRG.SRE_Speech.SpeakAsync("Ok,never mind");
                        }
                        break;
                    #region Remote Control
                    //////////////////////////////////////////////////////////////////////////////////////////
                    ////Remote Control Start
                    //case "SGM_FUNC_ControlTV":
                    //    UI.Text = "Control TV";
                    //    SRG.SRE_Speech.SpeakAsync("Ok,I can help you to control the TV");
                    //    device = "TV";
                    //    //SRG.LayerGrammarLoadAndUnload("SGM_FUNC_ControlTV", "");
                    //    break;

                    //case "SGM_FUNC_ControlProjector":
                    //    UI.Text = "Control Projector";
                    //    SRG.SRE_Speech.SpeakAsync("Ok, I will help you to control the projector");
                    //    device = "projector";
                    //    //SRG.LayerGrammarLoadAndUnload("SGM_FUNC_ControlProjector", "");
                    //    break;

                    //case "SGM_FUNC_PowerOnOffTV":
                    //    UI.Text = "Power";
                    //    SRG.SRE_Speech.SpeakAsync("Ok");
                    //    iRKit.iRkitcontrol(device, "power");
                    //    break;

                    //case "SGM_FUNC_MenuTV":
                    //    UI.Text = "Menu";
                    //    iRKit.iRkitcontrol(device, "menu");
                    //    break;

                    //case "SGM_FUNC_MuteTV":
                    //    UI.Text = "Mute";
                    //    iRKit.iRkitcontrol(device, "mute");
                    //    break;

                    //case "SGM_FUNC_ChangeInputTV":
                    //    UI.Text = "Change Input";
                    //    iRKit.iRkitcontrol(device, "change input");
                    //    break;

                    //case "SGM_FUNC_upTV":
                    //    UI.Text = "Up";
                    //    iRKit.iRkitcontrol(device, "up");
                    //    break;

                    //case "SGM_FUNC_downTV":
                    //    UI.Text = "Down";
                    //    iRKit.iRkitcontrol(device, "down");
                    //    break;

                    //case "SGM_FUNC_leftTV":
                    //    UI.Text = "Left";
                    //    iRKit.iRkitcontrol(device, "left");
                    //    break;

                    //case "SGM_FUNC_rightTV":
                    //    UI.Text = "Right";
                    //    iRKit.iRkitcontrol(device, "right");
                    //    break;

                    //case "SGM_FUNC_enterTV":
                    //    UI.Text = "Enter";
                    //    iRKit.iRkitcontrol(device, "enter");
                    //    break;

                    //case "SGM_FUNC_ChannelPlusTV":
                    //    UI.Text = "Next Channel";
                    //    iRKit.iRkitcontrol(device, "channel plus");
                    //    break;

                    //case "SGM_FUNC_ChannelMinusTV":
                    //    UI.Text = "Previous Channel";
                    //    iRKit.iRkitcontrol(device, "channel minus");
                    //    break;

                    //case "SGM_FUNC_VolumePlusTV":
                    //    UI.Text = "Volume Plus";
                    //    iRKit.iRkitcontrol(device, "volume plus");
                    //    break;

                    //case "SGM_FUNC_VolumeMinusTV":
                    //    UI.Text = "Volume Minus";
                    //    iRKit.iRkitcontrol(device, "volume minus");
                    //    break;
                    ///////////////////////////////////////////////////////////////////////////////////////


                    //case "SGM_FUNC_ControlFan":
                    //    UI.Text = "Control Fan";
                    //    SRG.SRE_Speech.SpeakAsync("Ok, I will help you to control the Fan");
                    //    device = "fan";
                    //    break;

                    //case "SGM_FUNC_PowerOnOffFan":
                    //    UI.Text = "Power";
                    //    SRG.SRE_Speech.SpeakAsync("Ok");
                    //    iRKit.iRkitcontrol(device, "power on");
                    //    break;

                    //case "SGM_FUNC_onLeftRightFan":
                    //    UI.Text = "Left Right";
                    //    iRKit.iRkitcontrol(device, "left right");
                    //    break;

                    //case "SGM_FUNC_onUpDown":
                    //    UI.Text = "Up Down";
                    //    iRKit.iRkitcontrol(device, "up down");
                    //    break;

                    //case "SGM_FUNC_Speed":
                    //    UI.Text = "Speed";
                    //    iRKit.iRkitcontrol(device, "speed");
                    //    break;

                    //case "SGM_FUNC_Timer":
                    //    UI.Text = "Timer";
                    //    iRKit.iRkitcontrol(device, "timer");
                    //    break;

                    //case "SGM_FUNC_ControlRadio":
                    //    UI.Text = "Control Radio";
                    //    SRG.SRE_Speech.SpeakAsync("Ok, I will help you to control the Radio");
                    //    device = "radio";
                    //    break;

                    //case "SGM_FUNC_PowerOnOffRadio":
                    //    UI.Text = "Power";
                    //    SRG.SRE_Speech.SpeakAsync("Ok, power on");
                    //    iRKit.iRkitcontrol(device, "power on");
                    //    break;


                    //case "SGM_FUNC_NextRadio":
                    //    UI.Text = "Next";
                    //    SRG.SRE_Speech.SpeakAsync("Ok,next channel");
                    //    iRKit.iRkitcontrol(device, "next channel");
                    //    break;

                    //case "SGM_FUNC_PreviousRadio":
                    //    UI.Text = "Previous";
                    //    SRG.SRE_Speech.SpeakAsync("Ok, previous channel");
                    //    iRKit.iRkitcontrol(device, "previous channel");
                    //    break;

                    //case "SGM_FUNC_VolumeUpRadio":
                    //    UI.Text = "Volume Up";
                    //    iRKit.iRkitcontrol(device, "volume pluse");
                    //    break;

                    //case "SGM_FUNC_VolumeDownRadio":
                    //    UI.Text = "Volume Down";
                    //    iRKit.iRkitcontrol(device, "volume minus");
                    //    break;


                    //case "SGM_FUNC_PowerOnLight":
                    //    UI.Text = "Light";
                    //    LightCon.control("on");
                    //    SRG.SRE_Speech.SpeakAsync("Ok, light on");
                    //    break;

                    //case "SGM_FUNC_PowerOffLight":
                    //    UI.Text = "Light";
                    //    LightCon.control("off");
                    //    SRG.SRE_Speech.SpeakAsync("Ok, light off");
                    //    break;

                    ////Remote Control End
                    ///////////////////////////////////////////////////////////////////////////////////////
                    #endregion Remote Control

                    case "SGM_FUNC_COMPLEX_SetReminderYesNo":
                        if (ResultName == "yes")
                        {
                            //SetReminderFunction.WriteGoogleCalendarReminder(reminder_task, DateTime.Now.Year, DateTime.Now.Month, Convert.ToInt32(subday_num), Convert.ToInt32(subhour), Convert.ToInt32(subminute));
                            xmlmaker();
                            loadxml();
                            UI.Text = "Ok, reminder set.";
                            if (Function.Vision.UserName == "Unknown")
                            {
                                SRG.SRE_Speech.SpeakAsync("Ok, reminder set.");
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("OK, I've set reminder for you, " + Function.Vision.UserName);
                            }
                            setalarm();
                        }
                        else if (ResultName == "no")
                        {
                            UI.Text = "Ok, nevermind";
                            SRG.SRE_Speech.SpeakAsync("Ok, never mind");
                        }
                        break;
                    #region read news SGM_ReadNews
                    //Read News Part Start
                    case "SGM_FUNC_ReadNews":
                        SRG.SRE_Speech.SpeakAsync("English news or Chinese news?");
                        break;

                    case "SGM_FUNC_LanguageOption":
                        if (ResultName == "English news"||ResultName == "English news please" || ResultName == "English")
                        {                            
                            SRG.SRE_Speech.Speak("I can read for you News from Stratis Time.");
                            SRG.SRE_Speech.Speak("At the moment I can read you Top of the News,");
                            SRG.SRE_Speech.Speak("SINGAPORE News");
                            SRG.SRE_Speech.Speak("BUSINESS, OPINION");
                            SRG.SRE_Speech.Speak("ASIA, LIFESTYLE,FORUM");
                            SRG.SRE_Speech.Speak("MULTIMEDIA, SPORT, TECH, WORLD,POLITICS");
                            SRG.SRE_Speech.SpeakAsync("What kinds of news do you want me to read ?");
                            GrammarTimer.Start();
                        }

                        else if (ResultName == "Chinese news" || ResultName == "Chinese news please" || ResultName == "Chinese")
                        {
                            SRG.SRE_Speech.SpeakAsync("Ok");
                            Thread.Sleep(1000);
                            EnglishNewsMark = false;
                            ReadNewsFunction.flag_StartNewsReading = true;
                            ReadNewsFunction.NewsReadingFunction_Cn();
                        }
                        break;

                    case "SGM_ContinueReadNews_YesNo":
                        if (ResultName == "yes" || ResultName == "yes please")
                        {
                            if (EnglishNewsMark)
                            {
                                SRG.SRE_Speech.SpeakAsync("Ok,next news");
                                Thread.Sleep(2000);
                                ReadNewsFunction.flag_StartNewsReading = true;
                                ReadNewsFunction.NewsReadingFunction_Eng(NewsType);
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("Ok,next news");
                                Thread.Sleep(2000);
                                ReadNewsFunction.flag_StartNewsReading = true;
                                ReadNewsFunction.NewsReadingFunction_Cn();
                            }
                        }
                        else
                        {
                            SRG.SRE_Speech.SpeakAsync("Ok, I will stop reading news.");
                            ReadNewsFunction.flag_StartNewsReading = false;
                            SRG.LoadGrammar(SRG.SGM_FUNC_ReadNews);
                        }
                        break;

                    case "SGM_FUNC_NextNews":
                        ReadNewsTimer.Stop();
                        SRG.SRE_Speech.SpeakAsync("Ok,next news");
                        Thread.Sleep(2000);
                        ReadNewsFunction.flag_StartNewsReading = true;
                        if (EnglishNewsMark)
                        {
                            ReadNewsFunction.NewsReadingFunction_Eng(NewsType);
                        }
                        else
                        {
                            ReadNewsFunction.NewsReadingFunction_Cn();
                        }
                        break;

                    case "SGM_FUNC_StopReadNews":
                        ReadNewsTimer.Stop();
                        SRG.SRE_Speech.SpeakAsync("Ok, I will stop reading news.");
                        ReadNewsFunction.flag_StartNewsReading = false;
                        break;
                    //Read News Part End
                #endregion  read news SGM_ReadNews
                    case "SGM_FUNC_AskWhatTimeNow":
                        UI.Text = DateTime.Now.ToShortTimeString();
                        DateTimeFunction.timeNow("eng");
                        break;

                    case "SGM_FUNC_AskWhatDayIsToday":
                        UI.Text = DateTime.Now.ToString("dddd");
                        DateTimeFunction.dayNow("eng");
                        break;

                    case "SGM_FUNC_AskWhatDateIsToday":
                        UI.Text = DateTime.Now.ToShortDateString();
                        DateTimeFunction.dateNow("eng");
                        break;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    case "SGM_FUNC_RegisterMode_YesNo":
                        if (ResultName == "yes")
                        {
                            SRG.SRE.UnloadAllGrammars();//Unsafe!!
                            SRG.SRE_Speech.SpeakAsync("Please spell your name to me. If finish then just speak finish. If the character is wrong please speak no");
                            SRG.LoadGrammar(SRG.SGM_FUNC_Char);
                        }
                        else
                        {
                            SRG.SRE_Speech.SpeakAsync("Ok, never mind");
                            Function.Vision.Instance.RegisterModeQuit();
                            Function.Vision.RegisterModeSwitch = false;
                        }
                        break;

                    case "SGM_FUNC_Char":
                        if (ResultName == "finish")
                        {
                            SRG.UnloadGrammar(SRG.SGM_FUNC_Char);
                            SRG.SRE_Speech.SpeakAsync("So, I will call you: " + Function.Vision.Instance.NewUserName);
                            Function.Vision.Instance.Command(ResultName);
                            SRG.SRE_Speech.SpeakAsync("Successfully register");
                            SRG.RegisterModeCompleted();
                        }
                        else
                        {
                            Function.Vision.Instance.Command(ResultName);
                            if (ResultName == "No")
                            {
                                if (Function.Vision.Instance.NewUserName == "")
                                {
                                    SRG.UnloadGrammar(SRG.SGM_FUNC_Char);
                                    SRG.RegisterModeCompleted();
                                }

                                else
                                {
                                    SRG.SRE_Speech.SpeakAsync("backspace");
                                }
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync(ResultName);
                            }
                        }
                        break;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    case "SGM_DIAL_Raise_Right_Hand":
                        Motor.RightArmInitialize();
                        break;
                    case "SGM_DIAL_Raise_Left_Hand":
                        Motor.LeftArmInitialize();
                        break;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    case "SGM_FUNC_Call":
                        if (Function.Vision.UserName == "Unknown")
                        {
                            SRG.SRE_Speech.SpeakAsync("OK, tell me who you want to call.");
                        }
                        else
                        {
                            SRG.SRE_Speech.SpeakAsync("OK, " + Function.Vision.UserName + ". tell me who you want to call.");
                        }
                        if (ResultName == "i want to make phone call")
                        {
                            VideoCallFlag = false;
                        }
                        else
                        {
                            VideoCallFlag = true;
                        }
                        break;

                    case "SGM_FUNC_CallPerson":
                        if (!VideoCallFlag)
                        {
                            string PhoneNumber = null;
                            try
                            {
                                string MyXMLFilePath = SRG.BaseFolder + @"Database\ContactsData.xml";
                                XmlDocument MyXmlDoc = new XmlDocument();
                                MyXmlDoc.Load(MyXMLFilePath);
                                XmlNode RootNode = MyXmlDoc.SelectSingleNode("Users");
                                XmlNodeList FirstLevelNodeList = RootNode.ChildNodes;
                                foreach (XmlNode Node in FirstLevelNodeList)
                                {
                                    XmlNode SecondLevelNode1 = Node.FirstChild;
                                    if (SecondLevelNode1.InnerText == ResultName)
                                    {
                                        XmlNode SecondLevelNode2 = Node.ChildNodes[1];
                                        PhoneNumber = SecondLevelNode2.InnerText;
                                    }
                                }
                            }
                            catch
                            {
                                SRG.SRE_Speech.SpeakAsync("Error! I am going to sleep");
                                Environment.Exit(0);
                            }
                            Motor.RightArmRaise();
                            Function.FaceLED.Instance.Happy();
                            SRG.SRE_Speech.SpeakAsync("OK, I'm calling " + ResultName + " for you.");
                            Thread.Sleep(2000);
                            if (PhoneNumber != null)
                            {
                                Skype.MakeCall("+" + PhoneNumber);
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("Sorry I can not make video call for this person.");
                                SRG.UnloadGrammar(SRG.SGM_FUNC_SkypePhoneCallFinished);
                            }
                        }
                        else
                        {
                            string ContactsID = null;
                            try
                            {
                                string MyXMLFilePath = SRG.BaseFolder + @"Database\VideoContactsData.xml";
                                XmlDocument MyXmlDoc = new XmlDocument();
                                MyXmlDoc.Load(MyXMLFilePath);
                                XmlNode RootNode = MyXmlDoc.SelectSingleNode("Users");
                                XmlNodeList FirstLevelNodeList = RootNode.ChildNodes;
                                foreach (XmlNode Node in FirstLevelNodeList)
                                {
                                    XmlNode SecondLevelNode1 = Node.FirstChild;
                                    if (SecondLevelNode1.InnerText == ResultName)
                                    {
                                        XmlNode SecondLevelNode2 = Node.ChildNodes[1];
                                        ContactsID = SecondLevelNode2.InnerText;
                                    }
                                }
                            }
                            catch
                            {
                                SRG.SRE_Speech.SpeakAsync("Error!");
                                Environment.Exit(0);
                            }
                            Motor.RightArmRaise();
                            Function.FaceLED.Instance.Happy();
                            SRG.SRE_Speech.SpeakAsync("OK, I'm calling " + ResultName + " for you.");
                            Thread.Sleep(2000);
                            if (ContactsID != null)
                            {
                                Skype skype;
                                skype = new SKYPE4COMLib.Skype();
                                string SkypeID = ContactsID;
                                Call call = skype.PlaceCall(SkypeID);
                                do
                                {
                                    System.Threading.Thread.Sleep(1);
                                } while (call.Status != TCallStatus.clsInProgress);
                                call.StartVideoSend();
                            }
                            else
                            {
                                SRG.SRE_Speech.SpeakAsync("Sorry I can not make video call for this person.");
                                SRG.UnloadGrammar(SRG.SGM_FUNC_SkypePhoneCallFinished);
                            }
                        }
                        break;

                    case "SGM_FUNC_SkypePhoneCallFinished":
                        if (Function.Vision.UserName != "Unknown")
                        {
                            SRG.SRE_Speech.SpeakAsync("ok");
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
                        break;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    case "SGM_DIAL_Emotion":
                        if (Function.Vision.Instance.Emotion != "Neutral")
                        {
                            EmotionDetection();
                        }
                        else
                        {
                        SRG.SRE_Speech.SpeakAsync("You look fine now.");
                        }
                        break;

                    case "SGM_DIAL_Help":
                        //SRG.SRE_Speech.SpeakAsync("OK, I will call for help.");
                        //Skype skype1;
                        //skype1 = new SKYPE4COMLib.Skype();
                        //string SkypeID1 = "elthef";
                        //Call call1 = skype1.PlaceCall(SkypeID1);
                        //do
                        //{
                        //    System.Threading.Thread.Sleep(1);
                        //} while (call1.Status != TCallStatus.clsInProgress);
                        //call1.StartVideoSend();
                        //for(;;)
                        //{
                        //    SRG.SRE_Speech.SpeakAsync("Mr. Tay is in dangerous, please help.");
                        //}
                        break;
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #region Chinese
                    case "SGM_DIAL_你好":
                        UI.Text = "你好";
                        SRG.SRECN_Speech.SelectVoice("Microsoft Huihui Desktop");
                        SRG.SRECN_Speech.SpeakAsync("你好");
                        break;

                    case "SGM_DIAL_谢谢":
                        UI.Text = "不谢";
                        SRG.SRECN_Speech.SpeakAsync("不谢");
                        break;

                    case "SGM_DIAL_早上好":
                        UI.Text = "早上好";
                        SRG.SRECN_Speech.SpeakAsync("早上好");
                        break;

                    case "SGM_DIAL_中午好":
                        UI.Text = "中午好";
                        SRG.SRECN_Speech.SpeakAsync("中午好");
                        break;

                    case "SGM_DIAL_下午好":
                        UI.Text = "下午好";
                        SRG.SRECN_Speech.SpeakAsync("下午好");
                        break;

                    case "SGM_DIAL_晚上好":
                        UI.Text = "晚上好";
                        SRG.SRECN_Speech.SpeakAsync("晚上好");
                        break;

                    case "SGM_DIAL_你叫什么名字":
                        UI.Text = "我的名字叫做露丝";
                        SRG.SRECN_Speech.SpeakAsync("我的名字叫做露丝");
                        break;


                    case "SGM_DIAL_自我介绍":
                        SRG.SRECN_Speech.SpeakAsync("好的，我是由新加坡南洋理工学院和日本北九州高专联合设计和生产的。 这个项目的目的是制造一个智能社交机器人去理解人类的语言，从而成为他们的助手");
                        break;

                    case "SGM_DIAL_谁设计了你":
                        SRG.SRECN_Speech.SpeakAsync("好的，我是由新加坡南洋理工学院和日本北九州高专的学生联合设计的");
                        break;

                    case "SGM_DIAL_功能":
                        //SRG.SRECN_Speech.SpeakAsync("我有一系列的功能");
                        break;

                    case "SGM_DIAL_英文识别":
                        UI.Text = "需要切换至英文语音识别吗？";
                        SRG.SRECN_Speech.SpeakAsync("需要切换至英文语音识别吗？");
                        break;

                    case "SGM_DIAL_英文识别_是否":
                        if (ResultName == "是的" || ResultName == "是")
                        {
                            UI.Text = "Ok, switch to English speech recognition";
                            SRG.SRE_Speech.SpeakAsync("Ok, switch to English speech recognition");
                            SwitchToEnglish();
                        }
                        else
                        {
                            UI.Text = "好吧";
                            SRG.SRECN_Speech.SpeakAsync("好吧");
                        }
                        break;

                    case "SGM_DIAL_日文识别":
                        UI.Text = "需要切换至日文语音识别吗？";
                        SRG.SRECN_Speech.SpeakAsync("需要切换至日文语音识别吗？");
                        break;

                    case "SGM_DIAL_日文识别_是否":
                        if (ResultName == "是的" || ResultName == "是")
                        {
                            UI.Text = "日本語に切り替えました。";
                            SRG.SREJP_Speech.SpeakAsync("はい、日本語に切り替えました。");
                            SwitchToJapanese();
                        }
                        else
                        {
                            UI.Text = "好吧";
                            SRG.SRECN_Speech.SpeakAsync("好吧");
                        }
                        break;

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion Chinese
                    case "SGM_FUNC_CHN_时间":
                        UI.Text = DateTime.Now.ToShortTimeString();
                        DateTimeFunction.timeNow("cn");
                        break;

                    case "SGM_FUNC_CHN_日期":
                        UI.Text = DateTime.Now.ToShortDateString();
                        DateTimeFunction.dateNow("cn");
                        break;

                    case "SGM_FUNC_CHN_星期":
                        UI.Text = DateTime.Now.ToString("dddd");
                        DateTimeFunction.dayNow("cn");
                        break;

                    ///////////////////////////////////////////////////////////////////////////////////////

                    case "SGM_FUNC_CHN_控制电视":
                        SRG.SRECN_Speech.SpeakAsync("好的，我可以帮你控制电视机");
                        device = "TV";
                        break;

                    case "SGM_FUNC_CHN_电源":
                        SRG.SRECN_Speech.SpeakAsync("好的");
                        iRKit.iRkitcontrol(device, "power");
                        break;

                    case "SGM_FUNC_CHN_菜单":
                        iRKit.iRkitcontrol(device, "menu");
                        break;

                    case "SGM_FUNC_CHN_上":
                        iRKit.iRkitcontrol(device, "up");
                        break;

                    case "SGM_FUNC_CHN_下":
                        iRKit.iRkitcontrol(device, "down");
                        break;

                    case "SGM_FUNC_CHN_左":
                        iRKit.iRkitcontrol(device, "left");
                        break;

                    case "SGM_FUNC_CHN_右":
                        iRKit.iRkitcontrol(device, "right");
                        break;

                    case "SGM_FUNC_CHN_声音加":
                        iRKit.iRkitcontrol(device, "volume plus");
                        break;

                    case "SGM_FUNC_CHN_声音减":
                        iRKit.iRkitcontrol(device, "volume minus");
                        break;

                    case "SGM_FUNC_CHN_频道加":
                        iRKit.iRkitcontrol(device, "channel plus");
                        break;

                    case "SGM_FUNC_CHN_频道减":
                        iRKit.iRkitcontrol(device, "channel minus");
                        break;

                    case "SGM_FUNC_CHN_进入":
                        iRKit.iRkitcontrol(device, "enter");
                        break;

                    ///////////////////////////////////////////////////////////////////////////////////

                    //case "SGM_FUNC_问问题":
                    //    SRG.SRECN_Speech.Speak("好的， 请说");
                    //    recording = true;
                    //    speechTimer.Enabled = true;
                    //    Record.Recordstart();
                    //    break;


                    //case "SGM_FUNC_计算":
                    //    SRG.SRECN_Speech.Speak("好的， 请说");
                    //    recording = true;
                    //    speechTimer.Enabled = true;
                    //    Record.Recordstart();
                    //    break;

                    //////////////////////////////////////////////////////////////////////////////////////////////
                    //////////////////////////////////////////////////////////////////////////////////////////////
                    //////////////////////////////////////////////////////////////////////////////////////////////
                    #region Japanses
                    case "SGM_FUNC_こんにちは":
                        if (ResultName == "こんにちは")
                        {
                            if (!MainWindow.DemoMode)
                            {
                                UI.Text = "こんにちは!";
                                SRG.SREJP_Speech.SpeakAsync("こんにちは!");
                            }
                            else
                            {
                                UI.Text = "こんにちは!";
                                SRG.SREJP_Speech.SpeakAsync("こんにちは!");
                            }
                        }
                        else if (ResultName == "おはようございます" || ResultName == "おはよう")
                        {
                            UI.Text = "おはようございます!";
                            SRG.SREJP_Speech.SpeakAsync("おはようございます!");
                        }
                        else if (ResultName == "こんばんは")
                        {
                            if (DateTime.Now.Hour > 18 || DateTime.Now.Hour < 3)
                            {
                                UI.Text = "こんばんは!";
                                SRG.SREJP_Speech.SpeakAsync("こんばんは!");
                            }
                            else
                            {
                                UI.Text = "今は夜じゃないよ!";
                                SRG.SREJP_Speech.SpeakAsync("今は夜じゃないよ!");
                            }
                        }
                        else if (ResultName == "初めまして")
                        {
                            if (Function.Vision.UserName == "Unknown")
                            {
                                UI.Text = "初めまして!どうぞよろしくお願いします。";
                                SRG.SREJP_Speech.SpeakAsync("初めまして!どうぞよろしくお願いします。");
                            }
                            else
                            {
                                UI.Text = "初めてじゃないでしょう？";
                                SRG.SREJP_Speech.SpeakAsync("初めてじゃないでしょう？");
                            }
                        }
                        else if (ResultName == "いい天気ですね")
                        {
                            UI.Text = "そうですね。いい天気ですね。";
                            SRG.SREJP_Speech.SpeakAsync("そうですね。いい天気ですね。");
                        }
                        break;

                    case "SGM_FUNC_中国語に切り替え":
                        UI.Text = "中国語に切り替えますか?";
                        SRG.SREJP_Speech.SpeakAsync("中国語に切り替えますか?");
                        break;

                    case "SGM_FUNC_中国語に切り替え_確認":
                        if (ResultName == "はい" || ResultName == "そうです")
                        {
                            UI.Text = "切换至中文";
                            SRG.SRECN_Speech.SpeakAsync("好的，切换至中文语音识别");
                            SwitchToChinese();
                        }
                        else
                        {
                            UI.Text = "じゃあ、そうしましょう。";
                            SRG.SREJP_Speech.SpeakAsync("じゃあ、そうしましょう。");
                        }
                        break;

                    case "SGM_FUNC_英語に切り替え":
                        UI.Text = "英語に切り替えますか?";
                        SRG.SREJP_Speech.SpeakAsync("英語に切り替えますか?");
                        break;

                    case "SGM_FUNC_英語に切り替え_確認":
                        if (ResultName == "はい" || ResultName == "そうです")
                        {
                            UI.Text = "Ok, switch to English speech recognition";
                            SRG.SRE_Speech.SpeakAsync("Ok, switch to English speech recognition");
                            SwitchToEnglish();
                        }
                        else
                        {
                            UI.Text = "じゃあ、そうしましょう。";
                            SRG.SREJP_Speech.SpeakAsync("じゃあ、そうしましょう。");
                        }
                        break;

                    case "SGM_FUNC_時間":
                        UI.Text = "今の時間は" + DateTime.Now.Hour + "時" + DateTime.Now.Minute + "分です。";
                        if (DateTime.Now.Hour > 12)
                        {
                            int HourInAfternoon = DateTime.Now.Hour - 12;
                            SRG.SREJP_Speech.SpeakAsync("今の時間は 午後 " + HourInAfternoon + "時" + DateTime.Now.Minute + "分です。");
                        }
                        else
                        {
                            SRG.SREJP_Speech.SpeakAsync("今の時間は 午前 " + DateTime.Now.Hour + "時" + DateTime.Now.Minute + "分です。");
                        }
                        break;

                    case "SGM_FUNC_曜日":

                        //DateTimeFunction.dayNow("jp");
                        曜日 = DateTime.Now.DayOfWeek.ToString();
                        switch (曜日)
                        {
                            case "Monday":
                                曜日 = "月曜日";
                                break;

                            case "Tuesday":
                                曜日 = "火曜日";
                                break;

                            case "Wednesday":
                                曜日 = "水曜日";
                                break;

                            case "Thursday":
                                曜日 = "木曜日";
                                break;

                            case "Friday":
                                曜日 = "金曜日";
                                break;

                            case "Saturday":
                                曜日 = "土曜日";
                                break;

                            case "Sunday":
                                曜日 = "日曜日";
                                break;

                        }
                        //Speak.SREJP_Speech.SelectVoice("VW Misaki");
                        SRG.SREJP_Speech.SpeakAsync(曜日 + "です");
                        UI.Text = 曜日;


                        break;

                    case "SGM_FUNC_名前":
                        UI.Text = "私の名前は,ルース.です！";
                        SRG.SREJP_Speech.SpeakAsync("私の名前はルースです!");
                        break;

                    case "SGM_FUNC_自己紹介":
                        UI.Text = "自己紹介";
                        SRG.SREJP_Speech.SpeakAsync("私の名前はルースです。");
                        SRG.SREJP_Speech.SpeakAsync("ナンヤンポリテクニックと 北九州高専の共同開発によって日々機能の拡張のための研究が続けられています.");
                        SRG.SREJP_Speech.SpeakAsync("みなさんのお役に立つことが私の夢です");
                        break;

                    case "SGM_FUNC_おやすみ":
                        UI.Text = "終了しますか？";
                        SRG.SREJP_Speech.SpeakAsync("終了しますか？");
                        break;

                    case "SGM_FUNC_おやすみ_確認":
                        if (ResultName == "はい" || ResultName == "そうです")
                        {
                            UI.Text = "おやすみなさいませ";
                            SRG.SREJP_Speech.SpeakAsync("おやすみなさいませ");
                            this.Close();
                        }
                        else
                        {
                            UI.Text = "かしこまりました。";
                            SRG.SREJP_Speech.SpeakAsync("かしこまりました。");
                        }
                        break;

                    case "SGM_FUNC_日":
                        SRG.SREJP_Speech.SpeakAsync("今日は" + DateTime.Now.ToString("MM") + "月" + DateTime.Now.ToString("dd") + "日" + "です");
                        break;

                    case "SGM_FUNC_元気":
                        UI.Text = "元気です";
                        SRG.SREJP_Speech.SpeakAsync("元気だよう、ありがとう");
                        break;


                    /*
                     //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                                        case "SGM_FUNC_Skype起動":
                                        UI.Text = "はい、あります。";
                                        SRG.SREJP_Speech.SpeakAsync("はいあります");
                                        break;

                                    case "SGM_FUNC_Skype電話":
                                        UI.Text = "どこの国にかけますか？";
                                        SRG.SREJP_Speech.SpeakAsync("どこの国にかけますか？");
                                        //  SRG.LayerGrammarLoadAndUnload("SGM_FUNC_Skype電話","");
                                        break;

                                    case "SGM_FUNC_Skype電話_国":
                                        if (ResultName == "日本")
                                        {
                                            UI.Text = "日本";
                                            country_number = "+81";
                                        }

                                        if (ResultName == "シンガポール")
                                        {
                                            UI.Text = "シンガポール";
                                            country_number = "+65";
                                        }
                                        else if (ResultName == "中国")
                                        {
                                            UI.Text = "中国";
                                            country_number = "+86";
                                        }
                                        //SREJP.RecognizeAsyncCancel();


                                        SRG.SREJP_Speech.SpeakAsync("番号を教えてください");
                                        //  SRG.LayerGrammarLoadAndUnload("SGM_FUNC_Skype電話_国", "");
                                        break;

                                    case "SGM_FUNC_Skype電話_電話番号":
                                        if (country_number == "+65")
                                        {
                                            if (i < 8)
                                            {
                                                if (ResultName == "backspace")
                                                {
                                                    phone_number = phone_number.Substring(0, phone_number.Length - 1);
                                                    UI.Text = phone_number;
                                                    //SkypeInterface.PhoneNumber.Text = phone_number;
                                                    i = i - 1;                     
                                                }   
                                                else if(ResultName == "いち")
                                                {
                                                    number = "1";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "に")
                                                {
                                                    number = "2";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "さん")
                                                {
                                                    number = "3";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "よん" || ResultName == "し")
                                                {
                                                    number = "4";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "ご")
                                                {
                                                    number = "5";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "ろく")
                                                {
                                                    number = "6";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "なな" || ResultName == "しち")
                                                {
                                                    number = "7";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "はち")
                                                {
                                                    number = "8";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "きゅう")
                                                {
                                                    number = "9";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "ぜろ" || ResultName == "れい")
                                                {
                                                    number = "0";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                            }
                                            else if (i == 8)
                                            {
                                                SRG.UnloadGrammarJP(SRG.SGM_FUNC_Skype電話_電話番号);
                                                if(ResultName == "いち")
                                                {
                                                    number = "1";

                                                }
                                                else if (ResultName == "に")
                                                {
                                                    number = "2";

                                                }
                                                else if (ResultName == "さん")
                                                {
                                                    number = "3";

                                                }
                                                else if (ResultName == "よん" || ResultName == "し")
                                                {
                                                    number = "4";

                                                }
                                                else if (ResultName == "ご")
                                                {
                                                    number = "5";

                                                }
                                                else if (ResultName == "ろく")
                                                {
                                                    number = "6";

                                                }
                                                else if (ResultName == "なな" || ResultName == "しち")
                                                {
                                                    number = "7";

                                                }
                                                else if (ResultName == "はち")
                                                {
                                                    number = "8";

                                                }
                                                else if (ResultName == "きゅう")
                                                {
                                                    number = "9";

                                                }
                                                else if (ResultName == "ぜろ" || ResultName == "れい")
                                                {
                                                    number = "0";

                                                }
                                                phone_number = phone_number + number;
                                                UI.Text = phone_number;
                                                Thread.Sleep(1000);
                                                //SRG.LayerGrammarLoadAndUnload("SGM_FUNC_Skype電話_電話番号", "");
                                                country_phone_number = country_number + phone_number;
                                                char[] code_2 = phone_number.ToCharArray();
                                                SRG.SREJP_Speech.SpeakAsync(code_2[0] + " " + code_2[1] + " " + code_2[2] + " " + code_2[3] + " " + code_2[4] + " " + code_2[5] + " " + code_2[6] + " " + code_2[7] +"に電話をかけますか?");
                                                i = 0;
                                                SRG.LoadGrammarJP(SRG.SGM_FUNC_Skype電話_電話番号_確認);
                                            }

                                        }

                                        else if (country_number == "+81")
                                        {
                                            if (i < 11)
                                            {
                                                if (ResultName == "backspace")
                                                {
                                                    phone_number = phone_number.Substring(0, phone_number.Length - 1);
                                                    UI.Text = phone_number;
                                                    //SkypeInterface.PhoneNumber.Text = phone_number;
                                                    i = i - 1;
                                                }
                                                else if (ResultName == "いち")
                                                {
                                                    number = "1";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "に")
                                                {
                                                    number = "2";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "さん")
                                                {
                                                    number = "3";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "よん" || ResultName == "し")
                                                {
                                                    number = "4";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "ご")
                                                {
                                                    number = "5";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "ろく")
                                                {
                                                    number = "6";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "なな" || ResultName == "しち")
                                                {
                                                    number = "7";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "はち")
                                                {
                                                    number = "8";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "きゅう")
                                                {
                                                    number = "9";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                                else if (ResultName == "ぜろ" || ResultName == "れい")
                                                {
                                                    number = "0";
                                                    phone_number = phone_number + number;
                                                    UI.Text = phone_number;
                                                    i++;
                                                }
                                            }

                                            else if (i == 11)
                                            {
                                                SRG.UnloadGrammarJP(SRG.SGM_FUNC_Skype電話_電話番号);
                                                if (ResultName == "いち")
                                                {
                                                    number = "1";

                                                }
                                                else if (ResultName == "に")
                                                {
                                                    number = "2";

                                                }
                                                else if (ResultName == "さん")
                                                {
                                                    number = "3";

                                                }
                                                else if (ResultName == "よん" || ResultName == "し")
                                                {
                                                    number = "4";

                                                }
                                                else if (ResultName == "ご")
                                                {
                                                    number = "5";

                                                }
                                                else if (ResultName == "ろく")
                                                {
                                                    number = "6";

                                                }
                                                else if (ResultName == "なな" || ResultName == "しち")
                                                {
                                                    number = "7";

                                                }
                                                else if (ResultName == "はち")
                                                {
                                                    number = "8";

                                                }
                                                else if (ResultName == "きゅう")
                                                {
                                                    number = "9";

                                                }
                                                else if (ResultName == "ぜろ" || ResultName == "れい")
                                                {
                                                    number = "0";

                                                }
                                                phone_number = phone_number + number;
                                                UI.Text = phone_number;
                                                Thread.Sleep(1000);
                                                //SRG.LayerGrammarLoadAndUnload("SGM_FUNC_Skype電話_電話番号", "");
                                                country_phone_number = country_number + phone_number;
                                                SRG.SREJP_Speech.SpeakAsync("この番号に電話をかけますか？");
                                                i = 0;
                                                SRG.LoadGrammarJP(SRG.SGM_FUNC_Skype電話_電話番号_確認);
                                            }
                                        }
                                        break;

                                    case "SGM_FUNC_Skype電話_電話番号_確認":
                                        if (ResultName == "はい" || ResultName == "そうです")
                                        {
                                            Skype.MakeCall(country_phone_number);

                                        }
                                        else if (ResultName == "いいえ" || ResultName == "ちがいます")
                                        {
                                            SRG.SREJP_Speech.SpeakAsync("かしこまりました");
                                        }
                                        break;

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    */
                    #endregion Japanese
                    #region cantonese
                    case "SGM_DIAL_HK_下午好":
                        SRG.SRECAN_Speech.SpeakAsync("下午好");
                        break;

                    case "SGM_DIAL_HK_你叫什么名字":
                        SRG.SRECAN_Speech.SpeakAsync("我是ruth");
                        break;

                    case "SGM_DIAL_HK_你好":
                        SRG.SRECAN_Speech.SpeakAsync("你好！");
                        break;

                    case "SGM_DIAL_HK_早上好":
                        SRG.SRECAN_Speech.SpeakAsync("早上好");
                        break;

                    case "SGM_DIAL_HK_晚上好":
                        SRG.SRECAN_Speech.SpeakAsync("晚上好");
                        break;

                    case "SGM_DIAL_HK_自我介绍":
                        SRG.SRECAN_Speech.SpeakAsync("我是ruth!你系边个？");
                        break;


                    case "SGM_FUNC_HK_时间":
                        UI.Text = DateTime.Now.ToShortTimeString();
                        DateTimeFunction.timeNow("can");
                        break;

                    case "SGM_FUNC_HK_日期":
                        UI.Text = DateTime.Now.ToShortDateString();
                        DateTimeFunction.dateNow("can");
                        break;

                    case "SGM_FUNC_HK_星期":
                        UI.Text = DateTime.Now.ToString("dddd");
                        DateTimeFunction.dayNow("can");
                        break;

                    case "SGM_FUNC_HK_关灯":
                        SRG.SRECAN_Speech.SpeakAsync("正在关闭");
                        LightCon.control("off");
                        break;

                    case "SGM_FUNC_HK_开灯":
                        SRG.SRECAN_Speech.SpeakAsync("正在打开");
                        LightCon.control("on");
                        break;

                    case "SGM_FUNC_HK_开收音机":
                        SRG.SRECAN_Speech.SpeakAsync("正在打开");
                        break;
                    case "SGM_FUNC_HK_关闭收音机":
                        SRG.SRECAN_Speech.SpeakAsync("正在关闭");
                        break;

                    case "SGM_FUNC_HK_关电视":
                        iRKit.iRkitcontrol("TV", "power");
                        SRG.SRECAN_Speech.SpeakAsync("正在关闭");
                        break;

                    case "SGM_FUNC_HK_开电视":
                        iRKit.iRkitcontrol("TV", "power");
                        SRG.SRECAN_Speech.SpeakAsync("正在打开");

                        break;


                    case "SGM_FUNC_HK_开风扇":
                        iRKit.iRkitcontrol("fan", "power on");
                        SRG.SRECAN_Speech.SpeakAsync("正在打开");
                        break;

                    case "SGM_FUNC_HK_关风扇":
                        iRKit.iRkitcontrol("fan", "power on");
                        SRG.SRECAN_Speech.SpeakAsync("正在关闭");
                        break;
                        #endregion cantonese
                        #region Hokkien
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //Hokkian Part
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //case "SGM_FUNC_HOK_日期":
                        //    UI.Text = DateTime.Now.ToShortDateString();
                        //    mp3.music("今天是3月28号");
                        //    //Thread.Sleep(700);                   
                        //    break;

                        //case "SGM_FUNC_HOK_星期":
                        //    UI.Text = DateTime.Now.ToString("dddd");
                        //    DateTimeFunction.dayNow("hok");
                        //    break;

                        //case "SGM_FUNC_HOK_开灯":
                        //    mp3.music("好的，稍等");
                        //    LightCon.control("on");
                        //    break;

                        //case "SGM_FUNC_HOK_关灯":
                        //    mp3.music("好的，稍等");
                        //    LightCon.control("off");
                        //    break;

                        //case "SGM_FUNC_HOK_开收音机":
                        //    mp3.music("好的，稍等");
                        //    break;

                        //case "SGM_FUNC_HOK_关收音机":
                        //    mp3.music("好的，稍等");

                        //    break;

                        //case "SGM_FUNC_HOK_开电视机":
                        //    mp3.music("好的，稍等");
                        //    iRKit.iRkitcontrol("TV", "power");
                        //    break;

                        //case "SGM_FUNC_HOK_关电视机":
                        //    mp3.music("好的，稍等");
                        //    iRKit.iRkitcontrol("TV", "power");
                        //    break;

                        //case "SGM_FUNC_HOK_开风扇":
                        //    iRKit.iRkitcontrol("fan", "power on");
                        //    mp3.music("好的，稍等");
                        //    break;

                        //case "SGM_FUNC_HOK_关风扇":
                        //    iRKit.iRkitcontrol("fan", "power on");
                        //    mp3.music("好的，稍等");
                        //    break;
                        #endregion Hokkien
                }
            }
        }



        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd);
        String ProcWindow_TextDocument = "SMS-WordPad";
        //String ProcWindow_robot = "vshost32.exe";

        private void SwitchWindow_TextDocument()
        {
            Process[] procs = Process.GetProcessesByName(ProcWindow_TextDocument);
            ProcessStartInfo start_TextDocument = new ProcessStartInfo();
            start_TextDocument.FileName = @"C:\test\SMS.txt";
            if (procs.Length > 0)
            {
                foreach (Process proc in procs)
                {
                    //switch to process by name
                    SwitchToThisWindow(proc.MainWindowHandle);
                }
            }
            else
            {
                Process.Start(start_TextDocument);
                Thread.Sleep(3000);
            }

        }



        public void Window_Closed(Object sender, EventArgs e)
        {
            try
            {
                Function.Vision.Instance.FaceTrackingTimer.Stop();
                Function.Vision.Instance.FaceTrackingTimer = null;
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
                catch { }
            }
            GC.Collect();
            Environment.Exit(Environment.ExitCode); // Added by THEF 20161110
//            Environment.Exit(0);
        }

        public void OnExiting(Object sender, EventArgs e)
        {
            try
            {
                Function.Vision.Instance.FaceTrackingTimer.Stop();
                Function.Vision.Instance.FaceTrackingTimer = null;
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
                catch { }
            }
            GC.Collect();
            Environment.Exit(Environment.ExitCode); // Added by THEF 20161110
//            Environment.Exit(0);
        }

        public void SwitchToChinese()
        {
            switch (language)
            {
                case "English":
                    language = "Chinese";
                    SRG.type = "SRECN";
                    Language_text.Text = language;
                    SRG.SRE.RecognizeAsyncCancel();
                    SRG.SRE.UnloadAllGrammars();
                    SRG.ChineseGrammarLoad();
                    SRG.SRECN.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Japanese":
                    language = "Chinese";
                    SRG.type = "SRECN";
                    Language_text.Text = language;
                    SRG.SREJP.RecognizeAsyncCancel();
                    SRG.SREJP.UnloadAllGrammars();
                    SRG.ChineseGrammarLoad();
                    SRG.SRECN.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Cantonese":
                    language = "Chinese";
                    SRG.type = "SRECN";
                    Language_text.Text = language;
                    SRG.SRECAN.RecognizeAsyncCancel();
                    SRG.SRECAN.UnloadAllGrammars();
                    SRG.ChineseGrammarLoad();
                    SRG.SRECN.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Hokkien":
                    language = "Chinese";
                    SRG.type = "SRECN";
                    Language_text.Text = language;
                    SRG.SRECN.RecognizeAsyncCancel();
                    SRG.SRECN.UnloadAllGrammars();
                    SRG.ChineseGrammarLoad();
                    SRG.SRECN.RecognizeAsync(RecognizeMode.Multiple);
                    break;


            }
        }

        public void SwitchToJapanese()
        {
            switch (language)
            {
                case "English":
                    language = "Japanese";
                    SRG.type = "SRECN";
                    Language_text.Text = language;
                    SRG.SRE.RecognizeAsyncCancel();
                    SRG.SRE.UnloadAllGrammars();
                    SRG.JapaneseGrammarLoad();
                    SRG.SREJP.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Chinese":
                    language = "Japanese";
                    SRG.type = "SRECN";
                    Language_text.Text = language;
                    SRG.SRECN.RecognizeAsyncCancel();
                    SRG.SRECN.UnloadAllGrammars();
                    SRG.JapaneseGrammarLoad();
                    SRG.SREJP.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Cantonese":
                    language = "Japanese";
                    SRG.type = "SRECN";
                    Language_text.Text = language;
                    SRG.SRECAN.RecognizeAsyncCancel();
                    SRG.SRECAN.UnloadAllGrammars();
                    SRG.JapaneseGrammarLoad();
                    SRG.SREJP.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Hokkien":
                    language = "Japanese";
                    SRG.type = "SRECN";
                    Language_text.Text = language;
                    SRG.SRECN.RecognizeAsyncCancel();
                    SRG.SRECN.UnloadAllGrammars();
                    SRG.JapaneseGrammarLoad();
                    SRG.SREJP.RecognizeAsync(RecognizeMode.Multiple);
                    break;

            }
        }

        public void SwitchToEnglish()
        {
            switch (language)
            {
                case "Chinese":
                    language = "English";
                    SRG.type = "SRE";
                    Language_text.Text = language;
                    SRG.SRECN.RecognizeAsyncCancel();
                    SRG.SRECN.UnloadAllGrammars();
                    SRG.SRGS_GrammarModels();
                    SRG.SRE.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Japanese":
                    language = "English";
                    SRG.type = "SRE";
                    Language_text.Text = language;
                    SRG.SREJP.RecognizeAsyncCancel();
                    SRG.SREJP.UnloadAllGrammars();
                    SRG.SRGS_GrammarModels();
                    SRG.SRE.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Cantonese":
                    language = "English";
                    SRG.type = "SRE";
                    Language_text.Text = language;
                    SRG.SRECAN.RecognizeAsyncCancel();
                    SRG.SRECAN.UnloadAllGrammars();
                    SRG.SRGS_GrammarModels();
                    SRG.SRE.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Hokkien":
                    language = "English";
                    SRG.type = "SRE";
                    Language_text.Text = language;
                    SRG.SRECN.RecognizeAsyncCancel();
                    SRG.SRECN.UnloadAllGrammars();
                    SRG.SRGS_GrammarModels();
                    SRG.SRE.RecognizeAsync(RecognizeMode.Multiple);
                    break;
            }
        }

        public void SwitchToCantonese()
        {
            switch (language)
            {
                case "English":
                    language = "Cantonese";
                    SRG.type = "SRECAN";
                    Language_text.Text = language;
                    SRG.SRE.RecognizeAsyncCancel();
                    SRG.SRE.UnloadAllGrammars();
                    // SRG.ChineseGrammarLoad();
                    SRG.SRECAN.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Japanese":
                    language = "Cantonese";
                    SRG.type = "SRECAN";
                    Language_text.Text = language;
                    SRG.SREJP.RecognizeAsyncCancel();
                    SRG.SREJP.UnloadAllGrammars();

                    SRG.CantoneseGrammarLoad();
                    SRG.SRECAN.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Chinese":
                    language = "Cantonese";
                    SRG.type = "SRECAN";
                    Language_text.Text = language;
                    SRG.SRECN.RecognizeAsyncCancel();
                    SRG.SRECN.UnloadAllGrammars();
                    SRG.CantoneseGrammarLoad();
                    SRG.SRECAN.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Hokkien":
                    language = "Cantonese";
                    SRG.type = "SRECAN";
                    Language_text.Text = language;
                    SRG.SRECN.RecognizeAsyncCancel();
                    SRG.SRECN.UnloadAllGrammars();
                    SRG.CantoneseGrammarLoad();
                    SRG.SRECAN.RecognizeAsync(RecognizeMode.Multiple);
                    break;

            }
        }

        public void SwitchTohokkien()
        {
            switch (language)
            {
                case "English":
                    language = "Hokkien";
                    SRG.type = "SRECN";
                    Language_text.Text = language;
                    SRG.SRE.RecognizeAsyncCancel();
                    SRG.SRE.UnloadAllGrammars();
                    SRG.HokkienGrammarLoad();
                    SRG.SRECN.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Japanese":
                    language = "Hokkien";
                    SRG.type = "SRECN";
                    Language_text.Text = language;
                    SRG.SREJP.RecognizeAsyncCancel();
                    SRG.SREJP.UnloadAllGrammars();
                    SRG.HokkienGrammarLoad();
                    SRG.SRECN.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Chinese":
                    language = "Hokkien";
                    SRG.type = "SRECN";
                    Language_text.Text = language;
                    SRG.SRECN.RecognizeAsyncCancel();
                    SRG.SRECN.UnloadAllGrammars();
                    SRG.HokkienGrammarLoad();
                    SRG.SRECN.RecognizeAsync(RecognizeMode.Multiple);
                    break;

                case "Cantonese":
                    language = "Hokkien";
                    SRG.type = "SRECN";
                    Language_text.Text = language;
                    SRG.SRECAN.RecognizeAsyncCancel();
                    SRG.SRECAN.UnloadAllGrammars();
                    SRG.HokkienGrammarLoad();
                    SRG.SRECN.RecognizeAsync(RecognizeMode.Multiple);
                    break;
            }

        }



        //private void SwitchWindow_toSocialRobotics()
        //{
        //    Process[] proc_socialrobot = Process.GetProcessesByName(ProcWindow_robot);
        //    foreach (Process proc_sb in proc_socialrobot)
        //    {
        //        SwitchToThisWindow(proc_sb.MainWindowHandle);
        //    }

        //}

        private void Xunfei_ErrorEvent(Exception e, string error)
        {
            SRG.SRE_Speech.SpeakAsync(e.Message);
        }

        //private void ButtonRec_Click(object sender, RoutedEventArgs e)
        //{
        //    if (btnRecord.Content.ToString() == "录音")
        //    {
        //        recording = true;
        //        btnRecord.Content = "停止";
        //        Xunfei.Start();
        //    }
        //    else
        //    {
        //        recording = false;
        //        btnRecord.Content = "转换中";
        //        Xunfei.Stop();
        //        try
        //        {
        //            string c1 = "server_url=dev.voicecloud.cn,appid=556b30ff,timeout=10000";
        //            string c2 = "sub=iat,ssm=1,sch=1,auf=audio/L16;rate=16000,aue=speex,ent=sms16k,ptt=0,rst=json,rse=gb2312,nlp_version=2.0";
        //            iFlyASR asr = new iFlyASR(c1, c2);
        //            XunFei_result = asr.Audio2Txt(AppDomain.CurrentDomain.BaseDirectory + "aaa.wav");
        //            Msg.Text = XunFei_result;
        //        }

        //        catch (Exception)
        //        {
        //            Msg.Text = "无法识别";
        //        }

        //        btnRecord.Content = "录音";
        //        Thread.Sleep(500);
        //        //  JsonMy(result);
        //    }
        //}



        //private void OnTimedSpeechEvent(object source, ElapsedEventArgs e)
        //{
        //    recording = false;
        //    speechTimer.Enabled = false;
        //    Record.Recordstop();
        //    SRG.SRECN_Speech.Speak("正在处理，请稍等");
        //    //   Thread.Sleep(2000);


        //    this.Dispatcher.Invoke((Action)(() =>
        //    {
        //        try
        //        {
        //            string c1 = "server_url=dev.voicecloud.cn,appid=556addae,timeout=10000";
        //            string c2 = "sub=iat,ssm=1,sch=1,auf=audio/L16;rate=16000,aue=speex,ent=sms16k,ptt=0,rst=json,rse=gb2312,nlp_version=2.0";
        //            iFlyASR asr = new iFlyASR(c1, c2);
        //            XunFei_result = asr.Audio2Txt(SRG.BaseFolder + @"Wav\Question.wav");
        //            Setting_Windows.XunFeiRaw.Text = XunFei_result;
        //            pass = true;
        //        }

        //        catch (Exception)
        //        {
        //            Setting_Windows.XunFeiRaw.Text = "无法识别";
        //            pass = false;
        //            SRG.SRECN_Speech.Speak("对不起， 我没有听清楚，可以重复一遍吗");
        //            recording = true;
        //            speechTimer.Enabled = true;
        //            Record.Recordstart();
        //        }
        //    }));

        //    Thread.Sleep(500);
        //    if (pass == true)
        //    {
        //        if (RuleName == "SGM_FUNC_问问题" || RuleName == "SGM_FUNC_计算")
        //        {
        //            JsonAns(XunFei_result);
        //            SRG.LayerGrammarLoadAndUnload("SGM_FUNC_问问题_完成", "");
        //        }
        //    }
        //}

        public struct ToJson_Ans
        {
            public string re { get; set; }
            public string operation { get; set; }
            public string service { get; set; }
            public string text { get; set; }
            public Answer answer { get; set; }
        }

        public struct Answer
        {
            public string text { get; set; }
            public string type { get; set; }
        }



        public struct ToJson_Call
        {
            public Semantic semantic { get; set; }
            public string operation { get; set; }
            public string service { get; set; }
            public string text { get; set; }
        }

        public struct Semantic
        {
            public Slots slots { get; set; }
        }

        public struct Slots
        {
            public string code { get; set; }
            public string content { get; set; }
        }


        public struct ToJson_SMS
        {
            public Semantic semantic { get; set; }
            public string operation { get; set; }
            public string service { get; set; }
            public string text { get; set; }
        }




        public void JsonAns(string ANS)
        {
            string json = ANS;
            JavaScriptSerializer js = new JavaScriptSerializer();   //实例化一个能够序列化数据的类
            ToJson_Ans list = js.Deserialize<ToJson_Ans>(json);    //将json数据转化为对象类型并赋值给list
            string re = list.re;
            string operation = list.operation;
            string service = list.service;
            string text = list.text;
            string answer_text = list.answer.text;
            string answer_type = list.answer.type;
            SRG.SRECN_Speech.SpeakAsync(answer_text);

        }

        //string temperature;
        //string condition;
        //string humidity;
        //string windspeed;
        //string town;
        string apikey = "b3c579db28aa6eec025acbaaf78c2976";
        //List<string> condition_weather = new List<string>() { };
        //List<string> highest_temperature = new List<string>() { };
        //List<string> lowest_temperature = new List<string>() { };

        string weather_condition;

        BitmapImage WeatherBit;

        public void weathericon(string icon)
        {
            WeatherBit = new BitmapImage();
            
            switch (icon)
            {
                #region thunderstorm
                case "200":
                    weather_condition = "there will be a thunderstorm with light rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "201":
                    weather_condition = "there will be a thunderstorm with rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "202":
                    weather_condition = "there will be a thunderstorm with heavy rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\5.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "210":
                    weather_condition = "there will be a light thunderstorm";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "211":
                    weather_condition = "there will be a thunderstorm";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "212":
                    weather_condition = "there will be a heavy thunderstorm";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "221":
                    weather_condition = "there will be a ragged thunderstorm";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "230":
                    weather_condition = "there will be a thunderstorm with light drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "231":
                    weather_condition = "there will be a thunderstorm with drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "232":
                    weather_condition = "there will be a thunderstorm with heavy drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;
                #endregion thunderstorm
                #region drizzle
                case "300":
                    weather_condition = "there will be a light intensity drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "301":
                    weather_condition = "there will be a drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "302":
                    weather_condition = "there will be a heavy intensity drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "310":
                    weather_condition = "there will be a light intensity drizzle rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "311":
                    weather_condition = "there will be a drizzle rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "312":
                    weather_condition = "there will be a heavy intensity drizzle rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "313":
                    weather_condition = "there will be a shower rain and drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "314":
                    weather_condition = "there will be a heavy shower rain and drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "321":
                    weather_condition = "there will be a shower drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;
                #endregion drizzle
                #region rain
                case "500":
                    weather_condition = "there will be a light rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "501":
                    weather_condition = "there will be a moderate rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "502":
                    weather_condition = "there will be a heavy intensity rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\5.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "503":
                    weather_condition = "there will be a very heavy rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\5.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "504":
                    weather_condition = "there will be a extreme rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\5.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "511":
                    weather_condition = "there will be a freezing rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\11.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "520":
                    weather_condition = "there will be a light intensity shower rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "521":
                    weather_condition = "there will be a shower rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "522":
                    weather_condition = "there will be a heavy intensity shower rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\5.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "531":
                    weather_condition = "there will be a ragged shower rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\5.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;
                #endregion rain
                #region snow
                case "600":
                    weather_condition = "there will be a light snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\11.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "601":
                    weather_condition = "there will be a snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\10.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "602":
                    weather_condition = "there will be a heavy snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\10.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "611":
                    weather_condition = "there will be a sleet";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\10.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "612":
                    weather_condition = "there will be a shower sleet";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\10.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "615":
                    weather_condition = "there will be a light rain and snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\11.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "616":
                    weather_condition = "there will be a rain and snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\11.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "620":
                    weather_condition = "there will be a light shower snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "621":
                    weather_condition = "there will be a shower snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\10.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "622":
                    weather_condition = "there will be a heavy shower snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\10.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;
                #endregion snow
                #region atmosphere
                case "701":
                    weather_condition = "there will be a mist";
                    break;

                case "711":
                    weather_condition = "it will be smoky";
                    break;

                case "721":
                    weather_condition = "it will be hazy haze";
                    break;

                case "731":
                    weather_condition = "there will be sand, dust whirls";
                    break;

                case "741":
                    weather_condition = "it will be foggy";
                    break;

                case "751":
                    weather_condition = "It will be sandy ";
                    break;

                case "761":
                    weather_condition = "it will be dusty";
                    break;

                case "762":
                    weather_condition = "there will be volcanic ash";
                    break;

                case "771":
                    weather_condition = "there will be squalls";
                    break;

                case "781":
                    weather_condition = "there will be a tornado";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\16.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;
                #endregion atmosphere
                #region clouds
                case "800":
                    weather_condition = "clear sky";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Sunny.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "801":
                    weather_condition = "few clouds";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Partly Cloudy.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "802":
                    weather_condition = "scattered clouds";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Partly Cloudy.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "803":
                    weather_condition = "broken clouds";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Fair.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "804":
                    weather_condition = "overcast clouds";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Partly Cloudy.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;
                #endregion clouds
                #region extreme
                case "900":
                    weather_condition = "there will be a tornado";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\16.png");
                    WeatherBit.EndInit();
                    WeatherIcon.Source = WeatherBit;
                    break;

                case "901":
                    weather_condition = "there will be a tropical storm";
                    break;

                case "902":
                    weather_condition = "there will be a hurricane";
                    break;

                case "903":
                    weather_condition = "the weather will be extremely cold";
                    break;

                case "904":
                    weather_condition = "the weather will be extremely hot";
                    break;

                case "905":
                    weather_condition = "the weather will be extremely windy";
                    break;

                case "906":
                    weather_condition = "there will be hail";
                    break;
                #endregion extreme
                #region additional
                case "951":
                    break;

                case "952":
                    break;

                case "953":
                    break;

                case "954":
                    break;

                case "955":
                    break;

                case "956":
                    break;

                case "957":
                    break;

                case "958":
                    break;

                case "959":
                    break;

                case "960":
                    break;

                case "961":
                    break;

                case "962":
                    break;
                    #endregion additional

            }
            WeatherIconTimer.Start();
        }
        public void getweather(string country, string day)
        {
            List<string> from_weather = new List<string>() { };
            List<string> to_weather = new List<string>() { };
            List<string> temperature = new List<string>() { };
            List<string> rain = new List<string>() { };
            List<string> symbolID = new List<string>() { };
            string rain1;
            //string query = string.Format("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22" + country + "%22)&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
            //XmlDocument wData = new XmlDocument();
            //wData.Load(query);

            //XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
            //manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");
            //XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");


            //temperature = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value;

            //condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;

            //humidity = channel.SelectSingleNode("yweather:atmosphere", manager).Attributes["humidity"].Value;

            //windspeed = channel.SelectSingleNode("yweather:wind", manager).Attributes["speed"].Value;

            //town = channel.SelectSingleNode("yweather:location", manager).Attributes["city"].Value;


            //condition_weather = new List<string>() { };
            //highest_temperature = new List<string>() { };
            //lowest_temperature = new List<string>() { };


            //var fiveDays = channel.SelectSingleNode("item").SelectNodes("yweather:forecast", manager);
            //foreach (XmlNode node in fiveDays)
            //{
            //    var text = node.Attributes["text"].Value;
            //    var high = node.Attributes["high"].Value;
            //    var low = node.Attributes["low"].Value;

            //    condition_weather.Add(text);
            //    highest_temperature.Add(high);
            //    lowest_temperature.Add(low);
            //}
            #region weather
            string query2 = String.Format("http://api.openweathermap.org/data/2.5/forecast?q=" + country + "&mode=xml&appid=" + apikey);
            XmlDocument wData = new XmlDocument();
            try
            {
                wData.Load(query2);
            }
            catch
            {
                country = "";
            }


            XmlNode channel = wData.SelectSingleNode("weatherdata");
            XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);



            try
            {
                var forecast = channel.SelectSingleNode("forecast");
                foreach (XmlNode node in forecast)
                {
                    // get the hours
                    //var from = node.Attributes["from"].Value;
                    ////var to = node.Attributes["to"].Value;
                    //from_weather.Add(from);
                    ////to_weather.Add(to);
                    //manager.AddNamespace("from", from);

                    //wind attributes
                    //var windsp = node.SelectSingleNode("windSpeed").Attributes["mps"].Value;
                    //windspeed.Add(windsp);
                    //var windcon = node.SelectSingleNode("windSpeed").Attributes["name"].Value;
                    //windcondition.Add(windcon);
                    ////humdity
                    //var humid = node.SelectSingleNode("humidity").Attributes["value"].Value;
                    //humidity.Add(humid);
                    ////cloud condition
                    //var cloudcon = node.SelectSingleNode("clouds").Attributes["value"].Value;
                    //cloud.Add(cloudcon);
                    //temperature
                    var temp = node.SelectSingleNode("temperature").Attributes["value"].Value;
                    temperature.Add(temp);

                    //check if there is rain
                    try
                    {
                        var pep = node.SelectSingleNode("precipitation").Attributes["type"].Value;
                        rain.Add(pep);
                    }

                    catch
                    {
                        rain1 = "No rain";
                        rain.Add(rain1);
                    }
                    //symbol number
                    var ID = node.SelectSingleNode("symbol").Attributes["number"].Value;
                    symbolID.Add(ID);
                }
                try
                {
                    if (day == "today" || day == "Today")
                    {

                        double temp = 0;
                        temp = Convert.ToDouble(temperature[1]);

                        if (rain[1] == "No rain")
                        {
                            weathericon(symbolID[1]);
                            SRG.SRE_Speech.SpeakAsync("the weather condition in " + country + " for " + day + " is " + (int)temp + " degrees celsius with " + weather_condition); country = "";
                        }
                        else
                        {
                            weathericon(symbolID[1]);
                            SRG.SRE_Speech.SpeakAsync("the weather condition in " + country + " for " + day + " is " + (int)temp + " degrees celsius and " + weather_condition); country = "";
                        }
                        WeatherText.Text = (int)temp + " °C";


                    }

                    else if (day == "tomorrow" || day == "Tomorrow")
                    {
                        double temp = 0;
                        temp = Convert.ToDouble(temperature[8]);
                        if (rain[8] == "No rain")
                        {
                            weathericon(symbolID[8]);
                            SRG.SRE_Speech.SpeakAsync("the weather condition in " + country + " for " + day + " is " + (int)temp + " degrees celsius. with " + weather_condition); country = "";
                        }
                        else
                        {
                            weathericon(symbolID[8]);
                            SRG.SRE_Speech.SpeakAsync("the weather condition in " + country + " for " + day + " is " + (int)temp + " degrees celsius and " + weather_condition); country = "";
                        }
                        WeatherText.Text = (int)temp + " °C";
                    }
                    else if (day == "goingout")
                    {
                        try
                        {// double temp = 0;
                         //    temp = Convert.ToDouble(temperature[0]);
                            if (rain[1] == "No rain")
                            {

                                weathericon(symbolID[0]);
                                SRG.SRE_Speech.SpeakAsync("Bon voyage.");
                            }
                            else
                            {
                                weathericon(symbolID[0]);
                                SRG.SRE_Speech.SpeakAsync("Bon voyage, By the way please remember to take an umbrella."); country = "";
                            }
                        }
                        catch

                        {
                            SRG.SRE_Speech.SpeakAsync("Bon vovage!");
                        }

                    }
                    else if (day == "now")
                    {
                        double temp = 0;
                        temp = Convert.ToDouble(temperature[0]);

                        if (rain[1] == "No rain")
                        {
                            weathericon(symbolID[1]);
                            SRG.SRE_Speech.SpeakAsync("the weather condition in " + country + " for " + day + " is " + (int)temp + " degrees celsius with " + weather_condition); country = "";
                        }
                        else
                        {
                            weathericon(symbolID[1]);
                            SRG.SRE_Speech.SpeakAsync("the weather condition in " + country + " for " + day + " is " + (int)temp + " degrees celsius and " + weather_condition + " now."); country = "";
                        }
                        WeatherText.Text = (int)temp + " °C";
                    }
                }
                catch
                {
                    SRG.SRE_Speech.SpeakAsync("It seems I have trouble obtaining the weather forecast for " + country + ". please try another city"); country = "";
                }
            }
            catch
            {
                if (day == "now")
                {
                    SRG.SRE_Speech.SpeakAsync("Bon voyage!");
                }
                else
                {
                    if(country!="")
                    {
                        SRG.SRE_Speech.SpeakAsync("sorry I can not get the weather condition in "+country+" now."); country = "";
                    }
                    else
                    {
                        SRG.SRE_Speech.SpeakAsync("sorry I did not hear clearly, please try again!");
                    }
                }
            }

            #endregion weather
            #region old weather
            //    if (day == "today")
            //    {
            //        double temp_temperature_2 = 0;
            //        temp_temperature_2 = Convert.ToDouble(temperature);
            //        temp_temperature_2 = ((temp_temperature_2 - 32) / 1.8);
            //        //  SRG.SRE_Speech.SelectVoice("IVONA Amy");
            //        SRG.SRE_Speech.SpeakAsync("The weather condition in " + country + ", is " + condition + ", at " + (int)temp_temperature_2 + " degrees celsius. The wind speed is " + windspeed + " miles per hour, and humidity is " + humidity + ".");
            //        switch (condition)
            //        {
            //            case "Thunder in the Vicinity":
            //                b.BeginInit();
            //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Thunderstorms.png");
            //                b.EndInit();
            //                WeatherIcon.Source = b;
            //                break;


            //            case "Mostly Cloudy":
            //                b.BeginInit();
            //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Mostly Cloudy.png");
            //                b.EndInit();
            //                WeatherIcon.Source = b;
            //                break;

            //            case "Partly Cloudy":
            //                b.BeginInit();
            //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Partly Cloudy.png");
            //                b.EndInit();
            //                WeatherIcon.Source = b;
            //                break;

            //            case "Fair":
            //                b.BeginInit();
            //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Fair.png");
            //                b.EndInit();
            //                WeatherIcon.Source = b;
            //                break;

            //            case "Sunny":
            //                b.BeginInit();
            //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Sunny.png");
            //                b.EndInit();
            //                WeatherIcon.Source = b;
            //                break;

            //            case "Showers in the Vicinity":
            //                b.BeginInit();
            //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
            //                b.EndInit();
            //                WeatherIcon.Source = b;
            //                break;

            //            case "Showers":
            //                b.BeginInit();
            //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
            //                b.EndInit();
            //                WeatherIcon.Source = b;
            //                break;

            //            case "PM Thunderstorms":
            //                b.BeginInit();
            //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\PM Thunderstorms.png");
            //                b.EndInit();
            //                WeatherIcon.Source = b;
            //                break;

            //            case "Thunderstorms":
            //                b.BeginInit();
            //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Thunderstorms.png");
            //                b.EndInit();
            //                WeatherIcon.Source = b;
            //                break;

            //            case "Light Rain with Thunder":
            //                b.BeginInit();
            //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Thunderstorms.png");
            //                b.EndInit();
            //                WeatherIcon.Source = b;


            //                break;
            //        }
            //        WeatherText.Text = (int)temp_temperature_2 + " °C";


            //    }
            //    else if (day == "Tomorrow")
            //    {
            //        double temp_tfhigh = 0;
            //        double temp_tflow = 0;


            //        temp_tfhigh = Convert.ToDouble(highest_temperature[1]);
            //        temp_tflow = Convert.ToDouble(lowest_temperature[1]);

            //        temp_tfhigh = ((temp_tfhigh - 32) / 1.8);
            //        temp_tflow = ((temp_tflow - 32) / 1.8);
            //        //  SRG.SRE_Speech.SelectVoice("IVONA Amy");
            //        SRG.SRE_Speech.SpeakAsync("Tomorrow's weather condition in " + country + " is " + condition_weather[1] + ", with the temperature range between " + (int)temp_tfhigh + ", to " + (int)temp_tflow + " degree celsius.");
            //    }
            #endregion
        }

        //public void getweather2(string country, string day)
        //{
        //    string query = string.Format("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22" + country + "%22)&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
        //    XmlDocument wData = new XmlDocument();
        //    wData.Load(query);

        //    XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
        //    manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");
        //    XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");


        //    temperature = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value;

        //    condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;

        //    humidity = channel.SelectSingleNode("yweather:atmosphere", manager).Attributes["humidity"].Value;

        //    windspeed = channel.SelectSingleNode("yweather:wind", manager).Attributes["speed"].Value;

        //    town = channel.SelectSingleNode("yweather:location", manager).Attributes["city"].Value;

        //    condition_weather = new List<string>() { };
        //    highest_temperature = new List<string>() { };
        //    lowest_temperature = new List<string>() { };


        //    var fiveDays = channel.SelectSingleNode("item").SelectNodes("yweather:forecast", manager);
        //    foreach (XmlNode node in fiveDays)
        //    {
        //        var text = node.Attributes["text"].Value;
        //        var high = node.Attributes["high"].Value;
        //        var low = node.Attributes["low"].Value;

        //        condition_weather.Add(text);
        //        highest_temperature.Add(high);
        //        lowest_temperature.Add(low);
        //    }

        //    if (day == "today")
        //    {
        //        double temp_temperature_2 = 0;
        //        temp_temperature_2 = Convert.ToDouble(temperature);
        //        temp_temperature_2 = ((temp_temperature_2 - 32) / 1.8);
        //        //  SRG.SRE_Speech.SelectVoice("IVONA Amy");
        //        SRG.SRE_Speech.SpeakAsync("The weather condition in " + country + ", is " + condition + ", at " + (int)temp_temperature_2 + " degrees celsius. The wind speed is " + windspeed + " miles per hour, and humidity is " + humidity + ".");
        //        switch (condition)
        //        {
        //            case "Thunder in the Vicinity":
        //                b.BeginInit();
        //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Thunderstorms.png");
        //                b.EndInit();
        //                WeatherIcon.Source = b;
        //                break;


        //            case "Mostly Cloudy":
        //                b.BeginInit();
        //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Mostly Cloudy.png");
        //                b.EndInit();
        //                WeatherIcon.Source = b;
        //                break;

        //            case "Partly Cloudy":
        //                b.BeginInit();
        //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Partly Cloudy.png");
        //                b.EndInit();
        //                WeatherIcon.Source = b;
        //                break;

        //            case "Fair":
        //                b.BeginInit();
        //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Fair.png");
        //                b.EndInit();
        //                WeatherIcon.Source = b;
        //                break;

        //            case "Sunny":
        //                b.BeginInit();
        //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Sunny.png");
        //                b.EndInit();
        //                WeatherIcon.Source = b;
        //                break;

        //            case "Showers in the Vicinity":
        //                b.BeginInit();
        //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
        //                b.EndInit();
        //                WeatherIcon.Source = b;
        //                break;

        //            case "Showers":
        //                b.BeginInit();
        //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Showers.png");
        //                b.EndInit();
        //                WeatherIcon.Source = b;
        //                break;

        //            case "PM Thunderstorms":
        //                b.BeginInit();
        //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\PM Thunderstorms.png");
        //                b.EndInit();
        //                WeatherIcon.Source = b;
        //                break;

        //            case "Thunderstorms":
        //                b.BeginInit();
        //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Thunderstorms.png");
        //                b.EndInit();
        //                WeatherIcon.Source = b;
        //                break;

        //            case "Light Rain with Thunder":
        //                b.BeginInit();
        //                b.UriSource = new Uri(SRG.BaseFolder + @"weather-icons-set\Thunderstorms.png");
        //                b.EndInit();
        //                WeatherIcon.Source = b;


        //                break;
        //        }
        //        WeatherText.Text = (int)temp_temperature_2 + " °C";


        //    }
        //    else if (day == "Tomorrow")
        //    {
        //        double temp_tfhigh = 0;
        //        double temp_tflow = 0;


        //        temp_tfhigh = Convert.ToDouble(highest_temperature[1]);
        //        temp_tflow = Convert.ToDouble(lowest_temperature[1]);

        //        temp_tfhigh = ((temp_tfhigh - 32) / 1.8);
        //        temp_tflow = ((temp_tflow - 32) / 1.8);
        //        //  SRG.SRE_Speech.SelectVoice("IVONA Amy");
        //        SRG.SRE_Speech.SpeakAsync("Tomorrow's weather condition in " + country + " is " + condition_weather[1] + ", with the temperature range between " + (int)temp_tfhigh + ", to " + (int)temp_tflow + " degree celsius.");
        //    }
        //}





        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            Setting_Windows.Show();
        }

        private void Esc_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SwitchLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (language == "English")
            {
                SwitchToChinese();
            }
            else if (language == "Chinese")
            {
                SwitchToJapanese();
            }
            else if (language == "Japanese")
            {
                SwitchToCantonese();
            }

            else if (language == "Cantonese")
            {
                SwitchTohokkien();
            }

            else if (language == "Hokkien")
            {
                SwitchToEnglish();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            HealthWindows.Show();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            dialpad.Show();
        }





        static Thread MotorShowYes;

        public static void ShowYes()
        {
            Function.Vision.Instance.FaceTrackingTimer.Stop();
            Function.Motor.Instance.NeckShowYes();
            Function.Vision.Instance.FaceTrackingTimer.Start();
            MotorShowYes.Abort();
            MotorShowYes = null;
        }

        static Thread MotorShowNo;

        private static void ShowNo()
        {
            Function.Vision.Instance.FaceTrackingTimer.Stop();
            Function.Motor.Instance.NeckShowNo();
            Function.Vision.Instance.FaceTrackingTimer.Start();
            MotorShowNo.Abort();
            MotorShowNo = null;
        }

        //Read News Timer Part Start
        public void ReadNewsTimer_TimeUp(object sender, EventArgs e)
        {
            ReadNewsTimer.Stop();
            SRG.SRE_Speech.Speak("Do you want me to continue reading the news ?");//change THEF2016/11/17
            SRG.LoadGrammar(SRG.SGM_ContinueReadNews_YesNo);
            GrammarTimer.Start();
        }
        //Read News Timer Part End
        #region Grammar Timer
        //Grammar Timer Part Start
        public void GrammarTimer_TimeUp(object sender, EventArgs e)
        {
            GrammarTimer.Stop();
            if (SRG.SGM_ContinueReadNews_YesNo.Loaded)
            {
                SRG.UnloadGrammar(SRG.SGM_ContinueReadNews_YesNo);
                SRG.SRE_Speech.SpeakAsync("I will stop reading news.");
                ReadNewsFunction.flag_StartNewsReading = false;
                SRG.UnloadGrammar(SRG.SGM_FUNC_NextNews);
                SRG.UnloadGrammar(SRG.SGM_FUNC_StopReadNews);
                SRG.LoadGrammar(SRG.SGM_FUNC_ReadNews);
            }
            else
                if (SRG.SGM_DIAL_SwitchLanguageToChinese_YesNo.Loaded)
                {
                    SRG.UnloadGrammar(SRG.SGM_DIAL_SwitchLanguageToChinese_YesNo);
                    SRG.LoadGrammar(SRG.SGM_DIAL_SwitchLanguageToChinese);
                    UI.Text = "Ok, nevermind";
                    SRG.SRE_Speech.SpeakAsync("Ok, never mind");
                }
                else 
                    if (SRG.SGM_DIAL_SwitchLanguageToJapanese_YesNo.Loaded)
                    {
                        SRG.UnloadGrammar(SRG.SGM_DIAL_SwitchLanguageToJapanese_YesNo);
                        SRG.LoadGrammar(SRG.SGM_DIAL_SwitchLanguageToJapanese);
                        UI.Text = "Ok, nevermind";
                        SRG.SRE_Speech.SpeakAsync("Ok, never mind");
                    }
                    else
                        if (SRG.SGM_DIAL_Sleep_YesNo.Loaded)
                        {
                            SRG.UnloadGrammar(SRG.SGM_DIAL_Sleep_YesNo);
                            SRG.LoadGrammar(SRG.SGM_DIAL_Sleep);
                            SRG.SRE_Speech.SpeakAsync("Ok, never mind");
                        }
                        else
                            if (SRG.SGM_FUNC_RegisterMode_YesNo.Loaded)
                            {
                                SRG.UnloadGrammar(SRG.SGM_FUNC_RegisterMode_YesNo);
                                SRG.SRE_Speech.SpeakAsync("Ok, never mind");
                                Function.Vision.Instance.RegisterModeQuit();
                                Function.Vision.RegisterModeSwitch = false;
                            }
                            else
                                if(SRG.SGM_NewsOption.Loaded)
                                {
                                    SRG.UnloadGrammar(SRG.SGM_NewsOption);
                                    SRG.LoadGrammar(SRG.SGM_FUNC_ReadNews);
                                    SRG.UnloadGrammar(SRG.SGM_FUNC_StopReadNews);
                                }
        }
        //Grammar Timer Part End
        #endregion Grammar Timer

        public void WeatherIconTimer_TimeUp(object sender, EventArgs e)
        {
            WeatherIconTimer.Stop();
            WeatherIcon.Source = null;
            WeatherBit = null;
            WeatherText.Text = null;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (Function.Vision.RegisterModeSwitch)
            {
                Function.Vision.RegisterModeSwitch = false;
                SRG.SRE_Speech.SpeakAsync("Exiting the register mode.");
            }
            else
            {
                Function.Vision.RegisterModeSwitch = true;
                SRG.SRE_Speech.SpeakAsync("Entering the register mode.");
            }
        }
        private void Free_Click(object sender, RoutedEventArgs e)
        {
            Motor.FreeMode();
        }



        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //Motor.ArmInitialize();
        //Motor.TurnLeft();
        //SRG.SRE_Speech.Speak("The School of Engineering offers an exciting range of quality engineering");

        //Motor.TurnRight();
        //SRG.SRE_Speech.Speak("science and technology courses that is designed for your success");

        //Motor.TurnLeft();
        //SRG.SRE_Speech.Speak("Learn and grow with our innovation-based curriculum and extensive industry partnerships");


        //Motor.TurnRight();
        //SRG.SRE_Speech.Speak("Go beyond the classroom to gain a well-rounded and highly relevant education!");

        //Motor.TurnLeft();
        //SRG.SRE_Speech.Speak("Experience a vibrant student life and learning experiences full of endless possibilities");

        //Motor.TurnRight();
        //SRG.SRE_Speech.Speak("as we prepare you for both exciting careers and higher studies at reputable local and overseas universities");

        //Motor.LeftArmRest();
        //Motor.RightArmRest();
        //}


        private void loadxml()
        {

            try
            {

                string MyXMLDataPath = SRG.BaseFolder + @"Database\ReminderData.xml";
                XmlDocument MyXmlData = new XmlDocument();
                MyXmlData.Load(MyXMLDataPath);
                XmlNode RootNode = MyXmlData.SelectSingleNode("Reminders");//The first node（SelectSingleNode）：the root node of this xml file
                XmlNodeList FirstLevelNodeList = RootNode.ChildNodes;
                foreach (XmlNode Node in FirstLevelNodeList)
                {
                    try
                    {
                        XmlNode SecondLevelNode1 = Node.ChildNodes[0];
                        XmlNode SecondLevelNode2 = Node.ChildNodes[1];//Get the child node of the root node
                        XmlNode SecondLevelNode3 = Node.ChildNodes[2];
                        XmlNode SecondLevelNode4 = Node.ChildNodes[3];
                        if (Int32.Parse(SecondLevelNode2.InnerText) >= DateTime.Now.Hour)
                        {
                            EventTime = Convert.ToDateTime(SecondLevelNode4.InnerText.ToString());
                            reminder_task = SecondLevelNode1.InnerText.ToString();
                            substringminute = Convert.ToInt32(SecondLevelNode3.InnerText.ToString());
                        }
                    }
                    catch
                    {

                    }

                }
                MyXmlData.Save(MyXMLDataPath);

            }
            catch
            {
                Environment.Exit(0);
            }
        }

        private void xmlmaker()
        {
            try
            {
                if (reminder_task != "")
                {
                    string MyXMLDataPath = SRG.BaseFolder + @"Database\ReminderData.xml";
                    XmlDocument MyXmlData = new XmlDocument();
                    MyXmlData.Load(MyXMLDataPath);
                    XmlNode RootNode = MyXmlData.SelectSingleNode("Reminders");
                    XmlElement newElement1 = MyXmlData.CreateElement("Reminder");
                    RootNode.AppendChild(newElement1);

                    XmlElement newElementChild1 = MyXmlData.CreateElement("Event");
                    newElementChild1.InnerText = reminder_task;
                    newElement1.AppendChild(newElementChild1);

                    XmlElement newElementChild2 = MyXmlData.CreateElement("SubHour");
                    newElementChild2.InnerText = subhour;
                    newElement1.AppendChild(newElementChild2);

                    XmlElement newElementChild3 = MyXmlData.CreateElement("SubMin");
                    newElementChild3.InnerText = subminute;
                    newElement1.AppendChild(newElementChild3);

                    XmlElement newElementChild4 = MyXmlData.CreateElement("Time");
                    newElementChild4.InnerText = remind_time;
                    newElement1.AppendChild(newElementChild4);

                    MyXmlData.Save(MyXMLDataPath);
                }
                else
                {
                    SRG.SRE_Speech.SpeakAsync("Sorry I don't understand, can you repeat again?");
                }


            }
            catch
            {

            }

        }

        private void setalarm()
        {
            this.alarmTime = EventTime;
            if (alarmThread != null)
            {
                if (alarmThread.IsAlive)
                {
                    alarmThread.Abort();
                }
            }

            alarmThread = new Thread(new ThreadStart(alarmLoop));
            alarmThread.Start();
            //AlarmStatusLabel.Text = "Alarm set for " + this.alarmTime.ToString("HH:mm");

        }


        private void alarmLoop()
        {
            while (DateTime.Now < alarmTime)
            {
                ;
            }

            Dispatcher.BeginInvoke(new Action(delegate
            {

                if (DateTime.Now.Minute == substringminute)
                {
                    SRG.SRE_Speech.SpeakAsync("excuse me sir. It's time for you to " + reminder_task);
                }
                else if (DateTime.Now.Minute > substringminute)
                {
                    if (reminder_task != "")
                    {
                        Motor.MotorInitialize();
                        SRG.SRE_Speech.SpeakAsync("excuse me sir. You have missed your appointment to " + reminder_task);
                    }
                }
                string MyXMLDataPath = SRG.BaseFolder + @"Database\ReminderData.xml";
                XmlDocument MyXmlData = new XmlDocument();
                MyXmlData.Load(MyXMLDataPath);
                XmlNode RootNode = MyXmlData.SelectSingleNode("Reminders");
                RootNode.RemoveAll();

                MyXmlData.Save(MyXMLDataPath);


            }));

        }

        public void UserDataUpdate(object sender, EventArgs e)
        {
            if (Function.Vision.DataLength >= 48)
            {
                UserData.Text = Function.Vision.UserDataText;
            }
            else
            {
                UserData.Text = "Nobody in the view.";
            }
        }

        private void WakeUp_Click(object sender, RoutedEventArgs e)
        {
            if (!Function.Vision.WakeUp)
            {
                Function.Vision.WakeUp = true;
                WakeUp.Content = "Go Silence";
            }
            else
            {
                Function.Vision.WakeUp = false;
                WakeUp.Content = "Wake Up";
            }
        }

        private void EmotionDetection()
        {
            if (Function.Vision.Instance.Emotion != "Neutral")
            {
                switch (Function.Vision.Instance.Emotion)
                {
                    case "Happiness":
                        Function.FaceLED.Instance.Happy();
                        Function.Motor.Instance.EyesBlink();
                        Thread.Sleep(500);
                        Function.Motor.Instance.EyesBlink();
                        Thread.Sleep(500);
                        switch (language)
                        {
                            case "English":
                                SRG.SRE_Speech.SpeakAsync("You look so happy now.");
                                break;
                            case "Chinese":
                                SRG.SRECN_Speech.SpeakAsync("你看起来好像很开心。发生了什么吗?");
                                break;
                            case "Japanese":
                                SRG.SREJP_Speech.SpeakAsync("あなたは今すごく幸せそうに見えます。何が起こったか?");
                                break;
                        }
                        break;

                    case "Surprise":
                        Function.FaceLED.Instance.Smile();
                        Function.Motor.Instance.EyesBlink();
                        Thread.Sleep(500);
                        Function.Motor.Instance.EyesBlink();
                        Thread.Sleep(500);
                        switch (language)
                        {
                            case "English":
                                SRG.SRE_Speech.SpeakAsync("Surprise!!!");
                                break;
                            case "Chinese":
                                SRG.SRECN_Speech.SpeakAsync("哈哈?");
                                break;
                            case "Japanese":
                                SRG.SREJP_Speech.SpeakAsync("びっくりしたか?");
                                break;
                        }
                        break;

                    case "Anger":
                        Function.FaceLED.Instance.Fear();
                        Thread.Sleep(1000);
                        switch (language)
                        {
                            case "English":
                                SRG.SRE_Speech.SpeakAsync("Relax, please relax!");
                                break;
                            case "Chinese":
                                SRG.SRECN_Speech.SpeakAsync("我觉得你应该放轻松！");
                                break;
                            case "Japanese":
                                SRG.SREJP_Speech.SpeakAsync("怖い顔ですね。");
                                break;
                        }
                        break;

                    case "Sadness":
                        Function.FaceLED.Instance.Surprise();
                        Thread.Sleep(1000);
                        switch (language)
                        {
                            case "English":
                                SRG.SRE_Speech.SpeakAsync("You look upset now.");//, maybe I can tell a joke to you to make you happy?
                                break;
                            case "Chinese":
                                SRG.SRECN_Speech.SpeakAsync("你看起来似乎有些不开心呢 呵呵");
                                break;
                            case "Japanese":
                                SRG.SREJP_Speech.SpeakAsync("何が…起こりましたか？");
                                break;
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
            }
        }
    }
}