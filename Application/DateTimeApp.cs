using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace SocialRobot.Application
{
    public class DateTimeApp
    {

        public Function.Speech_Rcognition_Grammar Speak = new Function.Speech_Rcognition_Grammar();
        public Application.musicplayer music = new Application.musicplayer();

        string Time_now = "";
        string Date_now = "";
        string Day_now = "";

        string hour_now;
        string minute_now;
        string month_now;
        string date_now;
        string day_now;
        string 星期;
        string 曜日;



        public void timeNow(string language)
        {
            
            if (language == "eng")
            {
                Speak.SRE_Speech.SelectVoice("IVONA 2 Amy");
                //Speak.SRE_Speech.SelectVoice("Microsoft Zira Desktop");
                Time_now = DateTime.Now.ToString("hh") + ":" + DateTime.Now.ToString("mm") + " " + DateTime.Now.ToString("tt");
                Speak.SRE_Speech.SpeakAsync("The time is " + Time_now);
            }

            if (language == "cn")
            {
                Speak.SRECN_Speech.SelectVoice("Microsoft Huihui Desktop");
                //   Speak.SRECN_Speech.SetOutputToDefaultAudioDevice();
                hour_now = DateTime.Now.Hour.ToString();
                minute_now = DateTime.Now.Minute.ToString();
                Speak.SRECN_Speech.SpeakAsync("现在是" + hour_now + "点" + minute_now + "分");
            }

            if (language == "can")
            {
                Speak.SRECAN_Speech.SelectVoice("Microsoft Tracy Desktop");
                //   Speak.SRECAN_Speech.SetOutputToDefaultAudioDevice();
                hour_now = DateTime.Now.Hour.ToString();
                minute_now = DateTime.Now.Minute.ToString();
                Speak.SRECAN_Speech.SpeakAsync("依家" + hour_now + "点" + minute_now + "分");
            }
        }

        public void dateNow(string language)
        {
            if (language == "eng")
            {
                Date_now = DateTime.Now.ToString("M");
                Speak.SRE_Speech.SelectVoice("IVONA 2 Amy");
                //Speak.SRE_Speech.SelectVoice("Microsoft Zira Desktop");
                Speak.SRE_Speech.SpeakAsync("Today is " + Date_now);
            }

            if (language == "cn")
            {
                Speak.SRECN_Speech.SelectVoice("Microsoft Huihui Desktop");
                //   Speak.SRECN_Speech.SetOutputToDefaultAudioDevice();
                month_now = DateTime.Now.Month.ToString();
                Date_now = DateTime.Now.ToString("MM") + "月" + DateTime.Now.ToString("dd") + "日";
                Speak.SRECN_Speech.SpeakAsync("今天是" + Date_now);
            }

            if (language == "can")
            {
                Speak.SRECAN_Speech.SelectVoice("Microsoft Tracy Desktop");
                month_now = DateTime.Now.Month.ToString();
                Date_now = DateTime.Now.ToString("MM") + "月" + DateTime.Now.ToString("dd") + "日";
                Speak.SRECAN_Speech.SpeakAsync("今日是" + Date_now);
            }
        }

        public void dayNow(string language)
        {
            if (language == "eng")
            {
                Day_now = DateTime.Now.ToString("dddd");
                Speak.SRE_Speech.SelectVoice("IVONA 2 Amy");
                //Speak.SRE_Speech.SelectVoice("Microsoft Zira Desktop");
                Speak.SRE_Speech.SpeakAsync("Today is " + Day_now);
            }

            else if (language == "cn")
            {
                Speak.SRECN_Speech.SelectVoice("Microsoft Huihui Desktop");
                //    Speak.SRECN_Speech.SetOutputToDefaultAudioDevice();
                day_now = DateTime.Now.DayOfWeek.ToString();
                switch (day_now)
                {
                    case "Monday":
                        星期 = "星期一";
                        break;

                    case "Tuesday":
                        星期 = "星期二";
                        break;

                    case "Wednesday":
                        星期 = "星期三";
                        break;

                    case "Thursday":
                        星期 = "星期四";
                        break;

                    case "Friday":
                        星期 = "星期五";
                        break;

                    case "Saturday":
                        星期 = "星期六";
                        break;

                    case "Sunday":
                        星期 = "星期天";
                        break;

                }
                Speak.SRECN_Speech.SpeakAsync("今天是" + 星期);
            }

            else if (language == "can")
            {
                Speak.SRECAN_Speech.SelectVoice("Microsoft Tracy Desktop");
                //    Speak.SRECN_Speech.SetOutputToDefaultAudioDevice();
                day_now = DateTime.Now.DayOfWeek.ToString();
                switch (day_now)
                {
                    case "Monday":
                        星期 = "星期一";
                        break;

                    case "Tuesday":
                        星期 = "星期二";
                        break;

                    case "Wednesday":
                        星期 = "星期三";
                        break;

                    case "Thursday":
                        星期 = "星期四";
                        break;

                    case "Friday":
                        星期 = "星期五";
                        break;

                    case "Saturday":
                        星期 = "星期六";
                        break;

                    case "Sunday":
                        星期 = "星期天";
                        break;

                }
                Speak.SRECAN_Speech.SpeakAsync("今日是" + 星期);
            }


            else if (language == "hok")
            {
                //Speak.SRECN_Speech.SelectVoice("Microsoft Huihui Desktop");
                //    Speak.SRECN_Speech.SetOutputToDefaultAudioDevice();
                day_now = DateTime.Now.DayOfWeek.ToString();
                switch (day_now)
                {
                    case "Monday":
                        星期 = "星期一";
                        music.music("今天是星期一");
                        break;

                    case "Tuesday":
                        星期 = "星期二";
                        music.music("今天是星期二");
                        break;

                    case "Wednesday":
                        星期 = "星期三";
                        music.music("今天是星期三");
                        break;

                    case "Thursday":
                        星期 = "星期四";
                        music.music("今天是星期四");
                        break;

                    case "Friday":
                        星期 = "星期五";
                        music.music("今天是星期五");
                        break;

                    case "Saturday":
                        星期 = "星期六";
                        music.music("今天是星期六");
                        break;

                    case "Sunday":
                        星期 = "星期天";
                        music.music("今天是星期天");
                        break;

                }
              
            }

        }
    }

}

