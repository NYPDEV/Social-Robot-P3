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

namespace SocialRobot.Application.iRKit
{
    /// <summary>
    /// Interaction logic for iRKit_interface.xaml
    /// </summary>
    public partial class iRKit_interface : Window
    {

         Application.iRKit.iRKitApp iRKitApp= new Application.iRKit.iRKitApp();

        public iRKit_interface()
        {
            InitializeComponent();
        }

        private void power_Click(object sender, RoutedEventArgs e)
        {
            iRKitApp.iRkitcontrol("projector","power");
        }

        private void changeinput_Click(object sender, RoutedEventArgs e)
        {
            iRKitApp.iRkitcontrol("projector","change input");
        }

        private void up_Click(object sender, RoutedEventArgs e)
        {
            iRKitApp.iRkitcontrol("projector","up");
        }

        private void left_Click(object sender, RoutedEventArgs e)
        {
            iRKitApp.iRkitcontrol("projector","left");
        }

        private void right_Click(object sender, RoutedEventArgs e)
        {
            iRKitApp.iRkitcontrol("projector","right");
        }

        private void down_Click(object sender, RoutedEventArgs e)
        {
            iRKitApp.iRkitcontrol("projector","down");
        }

        private void menus_Click(object sender, RoutedEventArgs e)
        {
            iRKitApp.iRkitcontrol("projector","menu");
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            iRKitApp.iRkitcontrol("projector","enter");
        }

        private void volume_plus_Click(object sender, RoutedEventArgs e)
        {
    //        iRKitApp.iRkitcontrol("volume plus");
        }

        private void volume_minus_Click(object sender, RoutedEventArgs e)
        {
    //        iRKitApp.iRkitcontrol("volume minus");
        }

        private void channel_plus_Click(object sender, RoutedEventArgs e)
        {
     //       iRKitApp.iRkitcontrol("channel plus");
        }

        private void channel_minus_Click(object sender, RoutedEventArgs e)
        {
   //         iRKitApp.iRkitcontrol("channel minus");
        }

        private void mute_Click(object sender, RoutedEventArgs e)
        {
    //        iRKitApp.iRkitcontrol("mute");
        }



       

       

       








    }
}
