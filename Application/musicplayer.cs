using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;
using System.Windows.Media;
namespace SocialRobot.Application
{
    public class musicplayer
    {
        WMPLib.WindowsMediaPlayer WinMediaPlayer = new WMPLib.WindowsMediaPlayer();

        public void music(string selection)
        {
           switch(selection)
            {
                case "stop":
                    WinMediaPlayer.controls.stop();
                    break;

                case "今天是3月28号":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\今天是3月28号.mp3";
                    WinMediaPlayer.controls.play();

                    break;

                case "今天是星期一":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\今天是星期一.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "今天是星期二":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\今天是星期二.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "今天是星期三":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\今天是星期三.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "今天是星期四":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\今天是星期四.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "今天是星期五":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\今天是星期五.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "今天是星期六":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\今天是星期六.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "今天是星期天":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\今天是星期天.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "你好":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\你好.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "好的，稍等":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\好.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "正在关闭":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\正在关闭.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                         case "正在打开":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\正在打开.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點20分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點20分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點21分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點21分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點22分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點22分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點23分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點23分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點24分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點24分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點25分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點25分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點26分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點26分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點27分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點27分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點28分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點28分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點29分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點29分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點30分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點30分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點31分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點31分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點32分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點32分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點33分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點33分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點34分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點34分.mp3";
                    WinMediaPlayer.controls.play();
                    break;

                case "現在是12點35分":
                    WinMediaPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + @"Hok\現在是12點35分.mp3";
                    WinMediaPlayer.controls.play();
                    break;
            }
        }
    }
}
