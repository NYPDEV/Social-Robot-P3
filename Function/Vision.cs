using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Xml;
using ROBOTIS;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Threading;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Threading;

namespace SocialRobot.Function
{
    class Vision
    {
        Motor Motor = new Motor();
        Random ran = new Random();
        private SerialPort MyCamera = new SerialPort();
        public static bool WakeUp = false;

        public Speech_Rcognition_Grammar SRG = new Speech_Rcognition_Grammar();
        public DispatcherTimer FaceTrackingTimer = new DispatcherTimer();//The timer used in face tracking

        private Vision()
        {
            MyCamera.PortName = ConfigurationManager.AppSettings["CAMERA_COM_PORT_NAME"].ToString();//For adjust com port, go to app config to change.
            MyCamera.BaudRate = Convert.ToInt32(9600);
            MyCamera.Handshake = System.IO.Ports.Handshake.None;
            MyCamera.Parity = Parity.None;
            MyCamera.DataBits = 8;
            MyCamera.StopBits = StopBits.One;
            MyCamera.ReadTimeout = 200;
            MyCamera.WriteTimeout = 50;
            try
            {
                MyCamera.Open();
            }
            catch
            {
                SRG.SRE_Speech.SpeakAsync("Camera can not be detected! Enter Demo Mode!");
                MainWindow.DemoMode = true;
            }

            MyCamera.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(Receive);

            FaceTrackingTimer.Tick += new EventHandler(FaceTrackingTimer_TimesUp);

            SRG.SRE_Speech.SelectVoice("IVONA 2 Amy");
            //SRG.SRE_Speech.SelectVoice("Microsoft Zira Desktop");
        }

        private static Vision _instance = new Vision();

        public static Vision Instance
        {
            get
            {
                return _instance;
            }
        }


        #region FaceTracking

        public void FaceTrackingTimerInitialize()
        {
            //FaceTrackingTimer.AutoReset = true; //The event in timer will happen only once(false), or always happen(true).
            //FaceTrackingTimer.Enabled = true;
            //FaceTrackingTimer.Start();
            FaceTrackingTimer.Interval = TimeSpan.FromMilliseconds(800);
            FaceTrackingTimer.Start();
        }

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
        public string Emotion = "Neutral";
        public int EmotionConfidence = 0;

        public void FaceTrackingTimer_TimesUp(object sender, EventArgs e)
        {
            SendCommand();
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
                    FaceLED.Instance.cheekblank();
                    FaceLED.Instance.Normal();
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
                                Motor.EyesLeft();
                                break;
                            case 2:
                                Motor.EyesRight();
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                        switch (ActionNumber2)
                        {
                            case 1:
                                Motor.MidMotorTurnLeft();
                                break;
                            case 2:
                                Motor.MidMotorTurnRight();
                                break;
                            case 3:
                                break;
                        }
                        switch (ActionNumber3)
                        {
                            case 1:
                                Motor.EyesHalfClose();
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
                                FaceLED.Instance.Happy();
                            }
                            else
                            {
                                FaceLED.Instance.Surprise();
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
                MyCamera.Write(NormalCommand, 0, NormalCommand.Length);
            }
        }

        public void Receive(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (!RegisterMark)
            {
                byte[] DataReceived = new byte[MyCamera.BytesToRead];
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
                                            string MyXMLFilePath = SRG.BaseFolder + @"Database\RegisterData.xml";//the location of register database //Local File
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
                                            SRG.SRE_Speech.SpeakAsync("Error!");
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
                                                Motor.EyesRight();
                                            }
                                            else
                                            {
                                                Motor.EyesLeft();
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
                                    FaceLED.Instance.cheekblue();
                                    NumberOfPerson = DataReceived[8];

                                    #region Microphone Array Facial Detection

                                    if (MainWindow.beamAngleConfidence == 1)
                                    {
                                        DBSSPAPP = 0;
                                        DBSSPAPPmin = 999;
                                        SSPosition = 15.238 * (MainWindow.beamAngleInDeg + 90) - 1051.43;
                                        EyeGazeByte = 32;
                                        for (int i = 0; i < NumberOfPerson; i++)
                                        {
                                            if (EyeGazeByte + 1 < DataReceived.Length)
                                            {
                                                XoPP = DataReceived[EyeGazeByte - 22] + DataReceived[EyeGazeByte - 21] * 256;
                                                DBSSPAPP = System.Math.Abs(XoPP - (int)SSPosition);
                                                if (DBSSPAPP < DBSSPAPPmin)
                                                {
                                                    DBSSPAPPmin = DBSSPAPP;
                                                }
                                                EyeGazeByte = EyeGazeByte + PersonalInformationByteLength;
                                            }
                                        }
                                        EyeGazeByte = 32;
                                        for (int i = 0; i < NumberOfPerson; i++)
                                        {
                                            if (EyeGazeByte + 1 < DataReceived.Length)
                                            {
                                                XoPP = DataReceived[EyeGazeByte - 22] + DataReceived[EyeGazeByte - 21] * 256;
                                                DBSSPAPP = System.Math.Abs(XoPP - (int)SSPosition);
                                                if (DBSSPAPP == DBSSPAPPmin)
                                                {
                                                    X = DataReceived[EyeGazeByte - 22] + DataReceived[EyeGazeByte - 21] * 256;
                                                    Y = DataReceived[EyeGazeByte - 20] + DataReceived[EyeGazeByte - 19] * 256;
                                                }
                                                EyeGazeByte = EyeGazeByte + PersonalInformationByteLength;
                                            }
                                        }
                                        EyeGazeByte = 32;
                                        GazeX = 0;
                                        GazeY = 0;
                                        MinimumGaze = 999;
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
                                                    MinimumGaze = GazeNumber;
                                                }
                                                EyeGazeByte = EyeGazeByte + PersonalInformationByteLength;
                                            }
                                        }
                                        EyeGazeByte = 32;
                                    }

                                    #endregion

                                    #region Eye Gaze Facial Detection

                                    else
                                    {
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
                                                string MyXMLFilePath = SRG.BaseFolder + @"Database\RegisterData.xml";//the location of register database //Local File
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
                                                SRG.SRE_Speech.SpeakAsync("Error!");
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
                                    FaceLED.Instance.cheekblank();
                                    NobodyCounter++;
                                    if (NobodyCounter == 4)
                                    {
                                        if (dynamixel.dxl_read_word(Motor.RightArmID, Motor.PresentPositionAddress) > 450)
                                        {
                                            Motor.RightArmRest();
                                        }
                                        Motor.LeftArmRest();
                                        FaceLED.Instance.cheekblank();
                                        FaceLED.Instance.Normal();
                                        NobodyCounter = 1;
                                    }
                                    //Motor.EyesBallInitialize();
                                    //Motor.MidMotorInitialize();
                                    if (MainWindow.beamAngleConfidence == 1)
                                    {
                                        float BtmNeckPositionByMic = 2048 + MainWindow.beamAngleInDeg * 1024 / 90;
                                        if (Convert.ToInt32(BtmNeckPositionByMic) < Motor.BtmNeckUpLimit && Convert.ToInt32(BtmNeckPositionByMic) > Motor.BtmNeckLowLimit)
                                        {
                                            Motor.MotorWrite_funcNeck(Motor.BtmNeckID, 50, Convert.ToInt32(BtmNeckPositionByMic));
                                        }
                                        else
                                        {
                                            if (MainWindow.beamAngleInDeg < 0)
                                            {
                                                Motor.TurnRight();
                                            }
                                            else if (MainWindow.beamAngleInDeg > 0)
                                            {
                                                Motor.TurnLeft();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        NeckResetCount++;
                                        if (NeckResetCount == 5)
                                        {
                                            Motor.BtmMotorInitialize();
                                            NeckResetCount = 0;
                                        }
                                    }
                                }
                                #endregion

                            }
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
                                string MyXMLFilePath = SRG.BaseFolder + @"Database\RegisterData.xml";//the location of register database //Local File
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
                                SRG.SRE_Speech.SpeakAsync("Error!");
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
                        SRG.SRE_Speech.SpeakAsync("Hello, " + UserName);
                    }
                }


                #region Motor Control

                XInMotor = (2400 - 4 * X) / 10 * 173 / 100;
                YInMotor = 4 * (Y - 900) / 9;
                Motor.TopPresentPosition = dynamixel.dxl_read_word(Motor.TopNeckID, Motor.PresentPositionAddress);
                Motor.BtmPresentPosition = dynamixel.dxl_read_word(Motor.BtmNeckID, Motor.PresentPositionAddress);

                if (DataReceived.Length >= 48 && Motor.BtmPresentPosition - XInMotor < Motor.BtmNeckUpLimit && Motor.BtmPresentPosition - XInMotor > Motor.BtmNeckLowLimit && Motor.TopPresentPosition + YInMotor < Motor.TopNeckUpLimit && Motor.TopPresentPosition + YInMotor > Motor.TopNeckLowLimit && WakeUp)
                {
                    if (MainWindow.beamAngleConfidence == 1)
                    {
                        if (XInMotor >= 12 || XInMotor <= -12)
                        {
                            Motor.MotorWrite_funcNeck(Motor.BtmNeckID, 70, Motor.BtmPresentPosition + XInMotor);
                        }
                            Motor.MotorWrite_funcNeck(Motor.TopNeckID, 70, Motor.TopPresentPosition + YInMotor);
                        //Thread.Sleep(500);
                        //FaceTrackingTimer.Start();
                    }
                    else
                    {
                        if (MinimumGaze <= 20)
                        {
                            if (XInMotor >= 12 || XInMotor <= -12)
                            {
                                Motor.MotorWrite_funcNeck(Motor.BtmNeckID, 70, Motor.BtmPresentPosition + XInMotor);
                            }
                        }
                        Motor.MotorWrite_funcNeck(Motor.TopNeckID, 70, Motor.TopPresentPosition + YInMotor);
                    }
                }

                XInMotor = 0;
                YInMotor = 0;
                #endregion
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
                        string MyXMLFilePath = SRG.BaseFolder + @"Database\RegisterData.xml";//the location of register database //Local File
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
                        SRG.SRE_Speech.SpeakAsync("Error!");
                        Environment.Exit(0);
                    }

                    //2.add the new user's information into xml database.
                    try
                    {
                        string MyXMLFilePath = SRG.BaseFolder + @"Database\RegisterData.xml";//the location of register database //Local File
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
                        SRG.SRE_Speech.SpeakAsync("Error!");
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
    }

}

