using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Configuration;

namespace SocialRobot.Function
{
    class FaceLED
    {
        private SerialPort LED = new SerialPort();
        Function.Speech_Rcognition_Grammar SRG = new Speech_Rcognition_Grammar();
        private FaceLED()
        {
            LED.PortName = ConfigurationManager.AppSettings["MBED_COM_PORT_NAME"].ToString();//For adjust com port, go to app config to change.
            LED.BaudRate = Convert.ToInt32(9600);
            LED.Handshake = System.IO.Ports.Handshake.None;
            LED.Parity = Parity.None;
            LED.DataBits = 8;
            LED.StopBits = StopBits.One;
            LED.ReadTimeout = 200;
            LED.WriteTimeout = 50;
            try
            {
                LED.Open(); 
               
            }
            catch
            {
                SRG.SRE_Speech.SpeakAsync("LED can not be detected! Enter Demo Mode!");
                MainWindow.DemoMode = true;
            }
        }

        private static FaceLED _instance = new FaceLED();

        public static FaceLED Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Angry()
        {
           LED.WriteLine("angry");
        }

        public void Bluek()
        {
           LED.WriteLine("bluek");
        }

        public void Delicious()
        {
           LED.WriteLine("delicious");
        }

        public void Disgust()
        {
            LED.WriteLine("disgust");
        }

        public void Fear()
        {
            LED.WriteLine("fear");
        }

        public void Happy()
        {
            LED.WriteLine("happy");
        }

        public void Horny()
        {
            LED.WriteLine("horny");
        }

        public void Normal()
        {
            LED.WriteLine("normal");
        }

        public void Sad()
        {
            LED.WriteLine("sad");
        }

        public void Sleep()
        {
            LED.WriteLine("sleep");
        }

        public void Smile()
        {
            LED.WriteLine("smile");
        }

        public void Surprise()
        {
            LED.WriteLine("surprise");
        }

        public void Speaking()
        {
            LED.WriteLine("mouth");
            Thread.Sleep(50);
            LED.WriteLine("mouth1");
            Thread.Sleep(50);
            LED.WriteLine("mouth2");
            Thread.Sleep(50);
            LED.WriteLine("mouth3");
            Thread.Sleep(50);
            LED.WriteLine("mouth2");
            Thread.Sleep(50);
            LED.WriteLine("mouth1");
            Thread.Sleep(50);
            LED.WriteLine("mouth");
        }
        
        public void blank()
        {
            LED.WriteLine("blank");
        }

        public void cheekblank()
        {
            LED.WriteLine("cheekblank");
        }

        public void cheekgreen()
        {
            LED.WriteLine("blue");
        }

        public void cheekblue()
        {
            LED.WriteLine("green");
        }

        public void cheekred()
        {
            LED.WriteLine("red");
        }
    }
}
