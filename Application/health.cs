using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace SocialRobot.Application
{
    class health
    {
        public Process client;//for start and stop client
        string readline = "";    //read the text
        string ClientExe = "C:\\Users\\Shive\\Desktop\\SocialRobot Final - Copy\\Server-client\\ClientExample\\Debug\\ClientExample.exe";//file directory of Client.exe
        string rawtxt = "C:\\Users\\Shive\\Desktop\\SocialRobot Final - Copy\\bin\\Debug\\raw.txt";//file location of raw.txt
       // string[] data;
        //health datas
        //string AirFlow;
        //string Ecg;
        //string Systolic;
        //string Diastolic;
        //string Glucose;
        //string Temperature;
        //string BPM;
        //string SPO2;
        //string Condutance;
        //string Resistance;
        //string Airflow2;
        //string Posititon;
        //string EMG;

        public void download()
        {
            try
            {
                client = Process.Start(ClientExe);
                Thread.Sleep(2000);
                client.Kill();
             }
            catch
            {
                MessageBox.Show("Client.exe was not found");
            }
        }

        public void readApp()
        {
           
            try
            {
                using (StreamReader sr = new StreamReader(rawtxt))
                {
                   
                    


                   // client.Close();
                    readline = sr.ReadLine();
                    
                    
                }
            }
            catch
            {
                MessageBox.Show("Variable rawtxt doesnt' exist or rawtxt cannot be opened");
            }
            
        }

        public string getdata()
        {
            string getdata = " ";
            getdata = readline;
            return getdata;
        }
        //public void seperate()
        //{
        //    string s = readline;
        //    // Split string on spaces.
        //    // ... This will separate all the words.
        //    AirFlow = data[0];
        //    Ecg = data[1];
        //    Systolic = data[2];
        //    Diastolic = data[3];
        //    Glucose = data[4];
        //    Temperature = data[5];
        //    BPM = data[6];
        //    SPO2 = data[7];
        //    Condutance = data[8];
        //    Resistance = data[9];
        //    Airflow2 = data[10];
        //    Posititon = data[11];
        //    EMG = data[12];
        //    }
        

        
    }
}
