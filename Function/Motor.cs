using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXSeriesManager;
using ROBOTIS;
using System.Threading;
using System.Configuration;

namespace SocialRobot.Function
{
    class Motor
    {
        DXSeries EyesControl = new DXSeries();  //DXSeries control eyes motor

        private static Motor _instance = new Motor();

        public static Motor Instance
        {
            get
            {
                return _instance;
            }
        }

        #region Motor Control

        //Dynamixel control neck motor
        public int NeckMotorPortNumber = int.Parse(ConfigurationManager.AppSettings["NECK_PORT_NUM"].ToString());//For adjust com port, go to app config to change.
        public int NeckMotorBaudNumber = 34;
        public int GoalPostionAddress = 30;
        public int SpeedAddress = 32;
        public int PresentPositionAddress = 36;
        //Dynamixel control neck motor

        public void MotorInitialize()
        {
            EyesControl.ConnectToComPort();
            EyesControl.BaudRate = 1000000;

            dynamixel.dxl_initialize(NeckMotorPortNumber, NeckMotorBaudNumber);
            NeckInitialize();
            EyesInitialize();
            LeftArmRest();
            RightArmRest();
        }

        public void MotorWrite_funcEyes(int MotorId, int Speed, int angle)
        {
            EyesControl.ID = MotorId;
            EyesControl.Speed = Speed;
            EyesControl.Angle = angle;
            EyesControl.MotorAction();
        }

        public void MotorWrite_funcNeck(int MotorId, int Speed, int angle)
        {
            dynamixel.dxl_write_word(MotorId, SpeedAddress, Speed);
            dynamixel.dxl_write_word(MotorId, GoalPostionAddress, angle);
        }

        #endregion


        #region Eyes Action

        public int EyesBallID = 10;
        public int LeftEyesID = 12;
        public int RightEyesID = 20;
        public int LeftArmID = 22;
        public int RightArmID = 24;
        public int LeftArmMaxLimit = 663;
        public int LeftArmMinLimit = 370;
        public int RightArmMaxLimit = 640;
        public int RightArmMinLimit = 343;

        public void EyesInitialize()
        {
            for (int i = 0; i < 2; i++)
            {
                MotorWrite_funcEyes(EyesBallID, 100, 512);
                MotorWrite_funcEyes(LeftEyesID, 41, 205);
                MotorWrite_funcEyes(RightEyesID, 41, 821);
                Thread.Sleep(100);
            }
        }

        public void EyesBallInitialize()
        {
            MotorWrite_funcEyes(EyesBallID, 100, 512);
        }

        public void EyesLeft()
        {
            MotorWrite_funcEyes(EyesBallID, 100, 341);
        }

        public void EyesRight()
        {
            MotorWrite_funcEyes(EyesBallID, 100, 682);
        }

        public void EyesOpen()
        {
            MotorWrite_funcEyes(LeftEyesID, 120, 205);
            MotorWrite_funcEyes(RightEyesID, 120, 821);
        }

        public void EyesClose()
        {
            MotorWrite_funcEyes(EyesBallID, 100, 512);
            MotorWrite_funcEyes(LeftEyesID, 120, 465);
            MotorWrite_funcEyes(RightEyesID, 120, 576);
        }

        public void LeftEyesOpen()
        {
            MotorWrite_funcEyes(LeftEyesID, 120, 205);
        }

        public void RightEyesOpen()
        {
            MotorWrite_funcEyes(RightEyesID, 120, 821);
        }

        public void LeftEyesClose()
        {
            MotorWrite_funcEyes(LeftEyesID, 120, 465);
        }

        public void RightEyesClose()
        {
            MotorWrite_funcEyes(RightEyesID, 120, 576);
        }

        public void EyesBlink()
        {
            MotorWrite_funcEyes(RightEyesID, 300, 597);
            Thread.Sleep(10);
            MotorWrite_funcEyes(LeftEyesID, 300, 428);
            Thread.Sleep(300);
            MotorWrite_funcEyes(RightEyesID, 300, 821);
            Thread.Sleep(10);
            MotorWrite_funcEyes(LeftEyesID, 300, 205);
        }

        public void CloseLeftEye()
        {
            MotorWrite_funcEyes(LeftEyesID, 300, 428);
        }

        public void CloseRightEye()
        {
            MotorWrite_funcEyes(RightEyesID, 300, 597);
        }

        public void ArmInitialize()
        {
            MotorWrite_funcEyes(RightArmID, 60, 512);
            MotorWrite_funcEyes(LeftArmID, 60, 512);
        }

        public void RightArmInitialize()
        {
            MotorWrite_funcEyes(RightArmID, 60, 512);
        }

        public void LeftArmInitialize()
        {
            MotorWrite_funcEyes(LeftArmID, 60, 512);
        }

        public void RightArmRest()
        {
            MotorWrite_funcEyes(RightArmID, 60, RightArmMinLimit);
        }

        public void LeftArmRest()
        {
            MotorWrite_funcEyes(LeftArmID, 60, LeftArmMaxLimit);
        }

        public void RightArmRaise()
        {
            MotorWrite_funcEyes(RightArmID, 60, 500);
        }

        public void LeftArmRaise()
        {
            //MotorWrite_funcEyes(LeftArmID, 60, 500);
        }

        public void Dance()
        {
            //MotorWrite_funcEyes(LeftArmID, 30, 200);
            //MotorWrite_funcEyes(RightArmID, 30, 370);
            //MotorWrite_funcNeck(BtmNeckID, 100, 1934);
            //Thread.Sleep(3600);

            //MotorWrite_funcEyes(LeftArmID, 30, 640);
            //MotorWrite_funcEyes(RightArmID, 30, 800);
            //MotorWrite_funcNeck(BtmNeckID, 100, 2616);
            //Thread.Sleep(3600);

            //MotorWrite_funcEyes(LeftArmID, 30, 200);
            //MotorWrite_funcEyes(RightArmID, 30, 365);
            //MotorWrite_funcNeck(BtmNeckID, 100, 1934);
            //Thread.Sleep(3600);

            //MotorWrite_funcEyes(RightArmID, 100, 512);
            //MotorWrite_funcEyes(LeftArmID, 100, 512);

            //MotorWrite_funcEyes(LeftArmID, 30, 640);
            //MotorWrite_funcEyes(RightArmID, 30, 800);
            //MotorWrite_funcNeck(BtmNeckID, 100, 2616);
            //Thread.Sleep(3600);

            //MotorWrite_funcEyes(RightArmID, 60, 512);
            //MotorWrite_funcEyes(LeftArmID, 601, 512);
        }

        public void EyesHalfClose()
        {
            MotorWrite_funcEyes(LeftEyesID, 41, 335);
            MotorWrite_funcEyes(RightEyesID, 41, 698);
        }


        #endregion


        #region Neck Action

        public int TopNeckID = 14;
        public int MidNeckID = 16;
        public int BtmNeckID = 18;
        public int TopPresentPosition = 0;
        public int MidPresentPosition = 0;
        public int BtmPresentPosition = 0;
        public int TopNeckLowLimit = 1800;//The goal position of neck top motor can not lower than this value! Below is the same thing.
        public int TopNeckUpLimit = 2150;
        public int BtmNeckLowLimit = 1024;
        public int BtmNeckUpLimit = 3072;

        public void NeckInitialize()
        {
            MotorWrite_funcNeck(TopNeckID, 30, 2000);
            MotorWrite_funcNeck(MidNeckID, 30, 2048);
            MotorWrite_funcNeck(BtmNeckID, 30, 2048);
        }

        public void RobotSleep()
        {
            MotorWrite_funcNeck(TopNeckID, 30, 2150);
            MotorWrite_funcNeck(MidNeckID, 30, 2048);
            MotorWrite_funcNeck(BtmNeckID, 30, 2048);
        }

        public void TurnLeft()
        {
            MotorWrite_funcNeck(BtmNeckID, 60, 2421);
        }

        public void TurnRight()
        {
            MotorWrite_funcNeck(BtmNeckID, 60, 1675);
        }

        public void MidMotorInitialize()
        {
            MotorWrite_funcNeck(MidNeckID, 60, 2048);
        }

        public void BtmMotorInitialize()
        {
            MotorWrite_funcNeck(BtmNeckID, 60, 2048);
        }

        public void MidMotorTurnLeft()
        {
            MotorWrite_funcNeck(MidNeckID, 60, 1921);
        }

        public void MidMotorTurnRight()
        {
            MotorWrite_funcNeck(MidNeckID, 60, 2179);
        }

        public void NeckShowYes()
        {
            MotorWrite_funcNeck(TopNeckID, 80, 2148);
            Thread.Sleep(300);
            MotorWrite_funcNeck(TopNeckID, 80, 1948);
            Thread.Sleep(600);
            MotorWrite_funcNeck(TopNeckID, 80, 2148);
            Thread.Sleep(600);
            MotorWrite_funcNeck(TopNeckID, 80, 2048);
            Thread.Sleep(300);
        }

        public void NeckShowNo()
        {
            Thread.Sleep(500);
            MotorWrite_funcNeck(BtmNeckID, 80, dynamixel.dxl_read_word(BtmNeckID, PresentPositionAddress) - 100);
            Thread.Sleep(400);
            MotorWrite_funcNeck(BtmNeckID, 80, dynamixel.dxl_read_word(BtmNeckID, PresentPositionAddress) + 200);
            Thread.Sleep(800);
            MotorWrite_funcNeck(BtmNeckID, 80, dynamixel.dxl_read_word(BtmNeckID, PresentPositionAddress) - 200);
            Thread.Sleep(800);
            MotorWrite_funcNeck(BtmNeckID, 80, dynamixel.dxl_read_word(BtmNeckID, PresentPositionAddress) + 100);
        }

        public void NeckRandom()
        {
            Random Position = new Random();
            int RanX = Position.Next(BtmNeckLowLimit, BtmNeckUpLimit);
            int RanY = Position.Next(TopNeckLowLimit, TopNeckUpLimit);
            MotorWrite_funcNeck(BtmNeckID, 50, RanX);
            MotorWrite_funcNeck(TopNeckID, 50, RanY);
        }
        public void necktalk()
        {

            MotorWrite_funcNeck(BtmNeckID, 100, 1488);
            Thread.Sleep(1000);
            MotorWrite_funcNeck(BtmNeckID, 100, 2616);
            Thread.Sleep(1000);
            MotorWrite_funcNeck(BtmNeckID, 100, 1488);
            Thread.Sleep(1000);
            MotorWrite_funcNeck(BtmNeckID, 100, 2616);
            Thread.Sleep(1000);
            MotorWrite_funcNeck(BtmNeckID, 100, 1488);
            Thread.Sleep(1000);
            MotorWrite_funcNeck(BtmNeckID, 100, 2616);
            Thread.Sleep(1000);
        }

        #endregion


        #region FreeMode
        //This mode just for fun. No need to care...
        //By the way, it's a good example of threading.

        Thread subthreadNeck;
        public bool CountUsedInFreeMode = true;
        public void FreeMode()
        {
            if (CountUsedInFreeMode)
            {
                if (subthreadNeck == null)
                {
                    subthreadNeck = new Thread(Free);
                    subthreadNeck.Start();
                }
            }
            else
            {
                subthreadNeck.Abort();
                subthreadNeck = null;
            }
            CountUsedInFreeMode = !CountUsedInFreeMode;
        }

        public void Free()
        {
            int xFree = 1910;
            int yFree = 2048;
            int zFree = 2275;
            int eFree = 512;
            int lFree = LeftArmMaxLimit;
            int rFree = RightArmMinLimit;
            do
            {
                Random example = new Random();
                int ran1 = example.Next(-10, 11);
                int ran2 = example.Next(-10, 11);
                int ran3 = example.Next(-20, 21);
                int ran4 = example.Next(-20, 21);
                int ran5 = example.Next(-10, 11);
                int ran6 = example.Next(-10, 11);

                MotorWrite_funcNeck(TopNeckID, 5, xFree);
                MotorWrite_funcNeck(MidNeckID, 5, yFree);
                MotorWrite_funcNeck(BtmNeckID, 10, zFree);
                MotorWrite_funcEyes(EyesBallID, 20, eFree);
                MotorWrite_funcEyes(LeftArmID, 5, lFree);
                MotorWrite_funcEyes(RightArmID, 5, rFree);
                xFree = xFree + ran1;
                yFree = yFree + ran2;
                zFree = zFree + ran3;
                eFree = eFree + ran4;
                lFree = lFree + ran5;
                rFree = rFree + ran6;
                if (xFree < 1850 || xFree > 2225)
                {
                    xFree = 2048;
                }
                if (yFree < 2024 || yFree > 2072)
                {
                    yFree = 2048;
                }
                if (zFree < 1024 || zFree > 3072)
                {
                    zFree = 2048;
                }
                if (eFree < 400 || eFree > 624)
                {
                    eFree = 512;
                }
                if (lFree < LeftArmMinLimit || lFree > LeftArmMaxLimit)
                {
                    lFree = LeftArmMaxLimit;
                }
                if (rFree < RightArmMinLimit || rFree > RightArmMaxLimit)
                {
                    rFree = RightArmMinLimit;
                }
                Thread.Sleep(100);
            }
            while (true);
        }

        #endregion
    }
}
