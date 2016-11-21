using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.IO;

namespace SocialRobot
{
    /// <summary>
    /// Interaction logic for HealthPage.xaml
    /// </summary>
   public partial class HealthPage : Window
    {
        Application.health Health = new Application.health();//delare health cs
       
        string rawdata;//before split
        string[] data;//after split
        string rawfile ="C:\\Users\\Shive\\Desktop\\SocialRobot Final - Copy\\bin\\Debug\\raw.txt";//txt file of text received 
        
        
        public HealthPage()
        {
            InitializeComponent();
        }

        public void download()
        {
            int check = 0;
            Health.download();
            while (check != 1)
            {
                if (File.Exists(rawfile))
                {
                    Thread.Sleep(1000);//file write is too slow, a delay is needed
                    Health.readApp();
                    RawData.Text = Health.getdata();//for text box
                    rawdata = Health.getdata();//for splitting

                    check = 1;
                }
                else
                {
                    //MessageBox.Show("rawfile not exist");
                }
            }
            File.Delete(rawfile);
        }
       
        private void Button_Click_1(object sender, RoutedEventArgs e)//download button
        {
            download();
            
        }


        public void split()
        {
            string s = rawdata; 
            // Split string on spaces.
            // ... This will separate all the words.
            data = s.Split('#');
            //foreach (string word in words)
            //{
            //    //Main.Text = word;
            //    data[i] = word;
            //    i++;
            //}
            Main.Text = data[0] + " " + data[1] + " " + data[2] + " " + data[3] + " " + data[4] + " " + data[5] + " " + data[6] + " " + data[7] + " " + data[8] + " " + data[9] + " " + data[10] + " " + data[11] + " " + data[12];

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)//split button
        {

            split();
            
        
        }

        public void showhealth()
        {
           // if (AirFlow.Text == "0.0")
                AirFlow.Text = data[0];
           // else
            //    data[0] = AirFlow.Text;

           // if (Ecg.Text == "0.0")
                Ecg.Text = data[1];
           // else
           //     data[1] = Ecg.Text;

            //if (Systolic.Text == "0.0")
                Systolic.Text = data[2];
           // else
                //data[2] = Systolic.Text;

          // if (Diastolic.Text == "0.0")
              Diastolic.Text = data[3];
           // else
              //  data[3] = Diastolic.Text;

           // if (Glucose.Text == "0.0")
                Glucose.Text = data[4];
            //else
             //   data[4] = Glucose.Text;

            //if (Temperature.Text == "0.0")
                Temperature.Text = data[5] + "degCel";
            //else
            //    data[5] = Temperature.Text;

           // if (BPM.Text == "0.0")
                BPM.Text = data[6] + "Bpm";
           // else
              //  data[6] = BPM.Text;

            //if (SPO2.Text == "0.0")
                SPO2.Text = data[7];
            //else
             //   data[7] = SPO2.Text;

            //if (Condutance.Text == "0.0")
                Condutance.Text = data[8];
          //  else
             //   data[8] = Condutance.Text;

           // if (Resistance.Text == "0.0")
                Resistance.Text = data[9];
          //  else
            //    data[9] = Resistance.Text;

          //  if (Airflow2.Text == "0.0")
                Airflow2.Text = data[10];
          //  else
           //     data[10] = Airflow2.Text;

         //   if (Posititon.Text == "0.0")
               Posititon.Text = data[11];
                //1 == Supine position.

                //2 == Left lateral decubitus.

                //3 == Right lateral decubitus.

                //4 == Prone position.

                //5 == Stand or sit position.
          //  else
             //   data[11] = Posititon.Text;

          //  if (EMG.Text == "0")
                EMG.Text = data[12];
           // else
             //   data[12] = EMG.Text;


        }

        public void analyze()
        {
            int bpm= int.Parse(data[6]);
            float temp = float.Parse(data[5]);
            int Systolic = 600;//int.Parse(data[2]);
            int Diastolic = 200;// int.Parse(data[3]);
            float BP = Systolic/Diastolic; 
            int SPO2 = int.Parse(data[7]);

            string bpmstatus;
            string tempstatus;
            string BPstatus;
            string SPO2status;
            if ((bpm<100)&&(bpm>60))
            {
                bpmstatus = " and it is within healthy range";
            }
            else
            {
                bpmstatus = " and it is within unhealthy range";
            }
            if ((temp < 37.7) && (temp > 35.5))
            {
                tempstatus = " in healthy range";
            }
            else
            {
                tempstatus = " in unhealthy range";
            }
            if (BP<(120/80))
            {
                BPstatus = " Your Blood pressure is within normal range";
            }
            else
            {
                BPstatus = (" Your Blood pressure is not in range");
            }
            if (SPO2>95)
            {
                SPO2status = " You have normal blood oxygen saturation levels";
            }
            else
            {
                SPO2status = " Below normal level of blood oxygen saturation";
            }
            Analyze.Text = "your BPM is:" + bpm + bpmstatus + ".\n" + "Your temperature is:" + temp + tempstatus + ".\n" + "Your Blood pressure is:" + BP+BPstatus+".\n" +"Your oxygen saturation level is:"+ SPO2 + SPO2status + ".\n";
        }
        private void Go_back(object sender, RoutedEventArgs e)
        {
            this.Hide();

        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            showhealth();
        }

        private void Analyzebutton(object sender, RoutedEventArgs e)
        {
            //analyze();
        }

        
    }
}
