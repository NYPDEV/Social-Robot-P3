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
using System.Speech;
using System.Speech.Synthesis;
using System.Speech.Recognition;

namespace SocialRobot
{
    /// <summary>
    /// Interaction logic for keypad.xaml
    /// </summary>
    public partial class keypad : Window
    {
        //form declaration
        SpeechSynthesizer ss = new SpeechSynthesizer();
        PromptBuilder pb = new PromptBuilder();
        SpeechRecognitionEngine SRE = new SpeechRecognitionEngine();
        Choices clist;
        public string result;
        public Application.SkypeApp.SkypeApp Skype = new Application.SkypeApp.SkypeApp(); 


        public keypad()
        {
            InitializeComponent();
        }

        private void backspace(object sender, RoutedEventArgs e)
        {
            int i = result.Length;
            result= result.Substring(0, i - 1);
            phone.Text = result;


        }

        private void NumberPad(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            result = result + b.Content.ToString();
            phone.Text = result;
        }

        private void call(object sender, RoutedEventArgs e)
        {
            Skype.MakeCall(result);
            result = "";
            phone.Text = result;
            this.Hide();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            result = "+";
            phone.Text = result;
        }

        private void Singapore(object sender, RoutedEventArgs e)
        {
            result = "+65";
            phone.Text = result;
        }

        private void Japan(object sender, RoutedEventArgs e)
        {
            result = "+81";
            phone.Text = result;
        }

        private void China(object sender, RoutedEventArgs e)
        {
            result = "+86";
            phone.Text = result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            result = "";
            phone.Text = result;
            this.Hide();
            
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            //!!!
        }
    }
}
