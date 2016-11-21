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

namespace SocialRobot.Application.SkypeApp
{
    /// <summary>
    /// Interaction logic for Skype_Interface.xaml
    /// </summary>
    public partial class Skype_Interface : Window
    {

        public Application.SkypeApp.SkypeApp Skype = new Application.SkypeApp.SkypeApp();

        string country_phone_number;
        string country_number;
        string phone_number;

        public Skype_Interface()
        {
            InitializeComponent();
        }

        private void CallButton_Click(object sender, RoutedEventArgs e)
        {
            if(UserID.Text == null)
            {
            country_phone_number = country_number + phone_number;
            Skype.MakeCall(country_phone_number);
            }
            else if (UserID.Text != null)
            {
                Skype.MakeCall(UserID.Text);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserID.Text == null)
            {
                country_phone_number = country_number + phone_number;
                Skype.SendSMS(country_phone_number, MessageBox.Text);
            }
            else if(UserID.Text != null)
            {
                Skype.SnedMessage(UserID.Text, MessageBox.Text);
            }
        }

        private void ComboBoxItem_China(object sender, RoutedEventArgs e)
        {
            country_number = "+86";
        }

        private void ComboBoxItem_Singapore(object sender, RoutedEventArgs e)
        {
            country_number = "+65";
        }

        private void ComboBoxItem_Japan(object sender, RoutedEventArgs e)
        {
            country_number = "+81";
        }

       
        

  
    }
}
