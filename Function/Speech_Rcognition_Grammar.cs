using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;
using System.Speech.Synthesis;
using System.Windows;
using System.Threading;
using WindowsInput;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Globalization;
using WindowsMicrophoneMuteLibrary;

namespace SocialRobot.Function
{
    public class Speech_Rcognition_Grammar
    {
        //WindowsMicrophoneMuteLibrary.WindowsMicMute micMute = new WindowsMicrophoneMuteLibrary.WindowsMicMute();
        //micMute.MuteMic();
        //micMute.UnMuteMic();

        public SpeechRecognitionEngine SRE = new SpeechRecognitionEngine();
        public SpeechSynthesizer SRE_Speech = new SpeechSynthesizer();
        public SpeechRecognitionEngine SRECN = new SpeechRecognitionEngine();
        public SpeechSynthesizer SRECN_Speech = new SpeechSynthesizer();
        public SpeechRecognitionEngine SREJP = new SpeechRecognitionEngine();
        public SpeechSynthesizer SREJP_Speech = new SpeechSynthesizer();
        public SpeechRecognitionEngine SRECAN = new SpeechRecognitionEngine();
        public SpeechSynthesizer SRECAN_Speech = new SpeechSynthesizer();

        public Grammar SGM_FUNC_ReadNews;
        public Grammar SGM_FUNC_LanguageOption;
        public Grammar SGM_NewsOption;
        public Grammar SGM_FUNC_NextNews;
        public Grammar SGM_FUNC_StopReadNews;
        public Grammar SGM_ContinueReadNews_YesNo;

        public Grammar SGM_FUNC_AskWhatTimeNow;
        public Grammar SGM_FUNC_AskWhatDayIsToday;
        public Grammar SGM_FUNC_AskWhatDateIsToday;

        public Grammar SGM_FUNC_AskTodayCustomWeather;
        public Grammar SGM_FUNC_AskTomorrowCustomWeather;
        public Grammar SGM_FUNC_GoingOut;

        public Grammar SGM_DIAL_AskCountryPresidentOrPrimeMinister;

        public Grammar SGM_DIAL_Raise_Right_Hand;
        public Grammar SGM_DIAL_Raise_Left_Hand;

        public Grammar SGM_FUNC_COMPLEX_SetReminder;
        public Grammar SGM_FUNC_COMPLEX_SetReminderYesNo;

        public Grammar SGM_FUNC_Call;
        public Grammar SGM_FUNC_CallPerson;
        public Grammar SGM_FUNC_SkypePhoneCallFinished;

        public Grammar SGM_FUNC_Char;
        public Grammar SGM_FUNC_RegisterMode_YesNo;

        public Grammar SGM_FUNC_StartRadioStaion;
        public Grammar SGM_FUNC_StartRadioStationYesNo;
        public Grammar SGM_FUNC_StopRadioStation;
        public Grammar SGM_FUNC_StopRadioStationYesNo;

        public Grammar SGM_FUNC_ControlTV;
        public Grammar SGM_FUNC_ControlProjector;
        public Grammar SGM_FUNC_PowerOnOffTV;
        public Grammar SGM_FUNC_ChangeInputTV;
        public Grammar SGM_FUNC_MuteTV;
        public Grammar SGM_FUNC_upTV;
        public Grammar SGM_FUNC_downTV;
        public Grammar SGM_FUNC_leftTV;
        public Grammar SGM_FUNC_rightTV;
        public Grammar SGM_FUNC_enterTV;
        public Grammar SGM_FUNC_VolumePlusTV;
        public Grammar SGM_FUNC_VolumeMinusTV;
        public Grammar SGM_FUNC_ChannelPlusTV;
        public Grammar SGM_FUNC_ChannelMinusTV;
        public Grammar SGM_FUNC_MenuTV;

        public Grammar SGM_FUNC_ControlRadio;
        public Grammar SGM_FUNC_NextRadio;
        public Grammar SGM_FUNC_PreviousRadio;
        public Grammar SGM_FUNC_VolumeDownRadio;
        public Grammar SGM_FUNC_VolumeUpRadio;


        public Grammar SGM_FUNC_ControlFan;
        public Grammar SGM_FUNC_PowerOnOffFan;
        public Grammar SGM_FUNC_Speed;
        public Grammar SGM_FUNC_Timer;
        public Grammar SGM_FUNC_onUpDown;
        public Grammar SGM_FUNC_onLeftRightFan;

        public Grammar SGM_FUNC_PowerOnLight;
        public Grammar SGM_FUNC_PowerOffLight;

        public Grammar SGM_DIAL_SayHello;
        public Grammar SGM_DIAL_AskIntroduction;
        public Grammar SGM_DIAL_AskRobotHowAreYou;
        public Grammar SGM_DIAL_NiceToMeetYou;
        public Grammar SGM_DIAL_AskWhatIsSocialRobot;
        public Grammar SGM_DIAL_AskWhoDesign;
        public Grammar SGM_DIAL_GoodBye;
        public Grammar SGM_DIAL_Greeting;
        public Grammar SGM_DIAL_ThankYou;
        public Grammar SGM_DIAL_Scold;
        public Grammar SGM_DIAL_Compliment;
        public Grammar SGM_DIAL_AskRobotName;
        public Grammar SGM_DIAL_SwitchLanguageToChinese;
        public Grammar SGM_DIAL_SwitchLanguageToChinese_YesNo;
        public Grammar SGM_DIAL_SwitchLanguageToJapanese;
        public Grammar SGM_DIAL_SwitchLanguageToJapanese_YesNo;
        public Grammar SGM_DIAL_LookAtMe;
        public Grammar SGM_DIAL_Sleep;
        public Grammar SGM_DIAL_Sleep_YesNo;
        public Grammar SGM_DIAL_Emotion;
        public Grammar SGM_DIAL_Help;

        Grammar SGM_FUNC_CHN_时间;
        Grammar SGM_FUNC_CHN_日期;
        Grammar SGM_FUNC_CHN_星期;


        Grammar SGM_DIAL_谢谢;
        Grammar SGM_DIAL_你好;
        Grammar SGM_DIAL_你叫什么名字;
        Grammar SGM_DIAL_早上好;
        Grammar SGM_DIAL_中午好;
        Grammar SGM_DIAL_下午好;
        Grammar SGM_DIAL_晚上好;
        Grammar SGM_DIAL_自我介绍;
        Grammar SGM_DIAL_谁设计了你;
        Grammar SGM_DIAL_功能;
        Grammar SGM_DIAL_英文识别;
        Grammar SGM_DIAL_英文识别_是否;
        Grammar SGM_DIAL_日文识别;
        Grammar SGM_DIAL_日文识别_是否;

        //public Grammar SGM_FUNC_CHN_控制电视;
        //public Grammar SGM_FUNC_CHN_电源;
        //public Grammar SGM_FUNC_CHN_菜单;
        //public Grammar SGM_FUNC_CHN_上;
        //public Grammar SGM_FUNC_CHN_下;
        //public Grammar SGM_FUNC_CHN_左;
        //public Grammar SGM_FUNC_CHN_右;
        //public Grammar SGM_FUNC_CHN_声音加;
        //public Grammar SGM_FUNC_CHN_声音减;
        //public Grammar SGM_FUNC_CHN_频道加;
        //public Grammar SGM_FUNC_CHN_频道减;
        //public Grammar SGM_FUNC_CHN_进入;
        //public Grammar SGM_FUNC_CHN_退出;

        //Grammar SGM_FUNC_计算;
        //Grammar SGM_FUNC_问问题;

        Grammar SGM_FUNC_こんにちは;
        Grammar SGM_FUNC_中国語に切り替え;
        Grammar SGM_FUNC_中国語に切り替え_確認;
        Grammar SGM_FUNC_英語に切り替え;
        Grammar SGM_FUNC_英語に切り替え_確認;
        Grammar SGM_FUNC_天気;
        Grammar SGM_FUNC_時間;
        Grammar SGM_FUNC_名前;
        Grammar SGM_FUNC_おやすみ;
        Grammar SGM_FUNC_おやすみ_確認;
        Grammar SGM_FUNC_自己紹介;
        Grammar SGM_FUNC_曜日;
        Grammar SGM_FUNC_日;
        Grammar SGM_FUNC_元気;

        /*      Grammar SGM_FUNC_Skype起動;
                Grammar SGM_FUNC_SKype電話;
                Grammar SGM_FUNC_Skype電話_国;
                public Grammar SGM_FUNC_Skype電話_電話番号;
                public Grammar SGM_FUNC_Skype電話_電話番号_確認;
                Grammar SGM_FUNC_Skype電話終了;
        */

        Grammar SGM_DIAL_HK_下午好;
        Grammar SGM_DIAL_HK_你叫什么名字;
        Grammar SGM_DIAL_HK_你好;
        Grammar SGM_DIAL_HK_早上好;
        Grammar SGM_DIAL_HK_晚上好;
        Grammar SGM_DIAL_HK_自我介绍;

        Grammar SGM_FUNC_HK_日期;
        Grammar SGM_FUNC_HK_时间;
        Grammar SGM_FUNC_HK_星期;

        Grammar SGM_FUNC_HK_关灯;
        Grammar SGM_FUNC_HK_开灯;
        Grammar SGM_FUNC_HK_开收音机;
        Grammar SGM_FUNC_HK_关闭收音机;
        Grammar SGM_FUNC_HK_开电视;
        Grammar SGM_FUNC_HK_关电视;
        Grammar SGM_FUNC_HK_关风扇;
        Grammar SGM_FUNC_HK_开风扇;

        Grammar SGM_FUNC_HOK_日期;
        Grammar SGM_FUNC_HOK_星期;
        Grammar SGM_FUNC_HOK_开灯;
        Grammar SGM_FUNC_HOK_关灯;
        Grammar SGM_FUNC_HOK_开收音机;
        Grammar SGM_FUNC_HOK_关收音机;
        Grammar SGM_FUNC_HOK_开电视机;
        Grammar SGM_FUNC_HOK_关电视机;
        Grammar SGM_FUNC_HOK_开风扇;
        Grammar SGM_FUNC_HOK_关风扇;

        DictationGrammar dg;
        List<Grammar> Layerone;
        List<Grammar> SubLayer;
        List<Grammar> DIAL;
        List<Grammar> IRCommand;
        List<Grammar> IRCommandFan;
        List<Grammar> IRCommandRadio;
        List<Grammar> 控制;
        List<Grammar> 中文语法;

        bool IRCommand_flag = false;
        bool Game_flag = false;
        public string type = "SRE";

        public void SpeechRecognized()
        {
            //   dg = new DictationGrammar("grammar:dictation#pronunciation");
            //   dg.Name = "Random";
            //  LoadGrammar(dg);  
            try
            {
                SRE = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
                SRECN = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("zh-CN"));
                SREJP = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("ja-JP"));
                SRECAN = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("zh-HK"));
            }
            catch
            {
                SRE_Speech.Speak("Please install Chinese, Japanese and Cantonese's language pack!");
                Environment.Exit(0);
            }
            SRGS_GrammarModels();
          
            try
            {
                SRE.SetInputToDefaultAudioDevice();
                SRECN.SetInputToDefaultAudioDevice();
                SREJP.SetInputToDefaultAudioDevice();
                SRECAN.SetInputToDefaultAudioDevice();
            }
            catch
            {
                SRE_Speech.SpeakAsync("No audio input");
            }
            SRE.RecognizeAsync(RecognizeMode.Multiple);
        }

        public string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;

        public void SRGS_GrammarModels()
        {
            try
            {
                //////////////////////////////////////////////////////////////////////////////////////////
                SGM_FUNC_ReadNews = new Grammar(BaseFolder + @"Grammar\English\ReadNewsFunction\SGM_FUNC_ReadNews.xml");
                LoadGrammar(SGM_FUNC_ReadNews);

                SGM_FUNC_LanguageOption = new Grammar(BaseFolder + @"Grammar\English\ReadNewsFunction\SGM_LanguageOption.xml");

                SGM_NewsOption = new Grammar(BaseFolder + @"Grammar\English\ReadNewsFunction\SGM_NewsOption.xml");

                SGM_FUNC_NextNews = new Grammar(BaseFolder + @"Grammar\English\ReadNewsFunction\SGM_FUNC_NextNews.xml");

                SGM_FUNC_StopReadNews = new Grammar(BaseFolder + @"Grammar\English\ReadNewsFunction\SGM_FUNC_StopReadNews.xml");

                SGM_ContinueReadNews_YesNo = new Grammar(BaseFolder + @"Grammar\English\ReadNewsFunction\SGM_ContinueReadNews_YesNo.xml");
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                SGM_FUNC_AskWhatTimeNow = new Grammar(BaseFolder + @"Grammar\English\DateTimeFunction\SGM_FUNC_AskWhatTimeNow.xml");
                LoadGrammar(SGM_FUNC_AskWhatTimeNow);

                SGM_FUNC_AskWhatDayIsToday = new Grammar(BaseFolder + @"Grammar\English\DateTimeFunction\SGM_FUNC_AskWhatDayIsToday.xml");
                LoadGrammar(SGM_FUNC_AskWhatDayIsToday);

                SGM_FUNC_AskWhatDateIsToday = new Grammar(BaseFolder + @"Grammar\English\DateTimeFunction\SGM_FUNC_AskWhatDateIsToday.xml");
                LoadGrammar(SGM_FUNC_AskWhatDateIsToday);
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                SGM_FUNC_AskTodayCustomWeather = new Grammar(BaseFolder + @"Grammar\English\AskWeather\SGM_FUNC_AskTodayCustomWeather.xml");
                LoadGrammar(SGM_FUNC_AskTodayCustomWeather);

                SGM_FUNC_AskTomorrowCustomWeather = new Grammar(BaseFolder + @"Grammar\English\AskWeather\SGM_FUNC_AskTomorrowCustomWeather.xml");
                LoadGrammar(SGM_FUNC_AskTomorrowCustomWeather);

                SGM_FUNC_GoingOut = new Grammar(BaseFolder + @"Grammar\English\AskWeather\SGM_FUNC_GoingOut.xml");
                LoadGrammar(SGM_FUNC_GoingOut);
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                SGM_DIAL_AskCountryPresidentOrPrimeMinister = new Grammar(BaseFolder + @"Grammar\English\AskPresidentOrPrimeMinister\SGM_DIAL_AskCoutryPresidentOrPrimeMinister.xml");
                LoadGrammar(SGM_DIAL_AskCountryPresidentOrPrimeMinister);
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                SGM_DIAL_Raise_Right_Hand = new Grammar(BaseFolder + @"Grammar\English\Action\SGM_DIAL_Raise_Right_Hand.xml");
                LoadGrammar(SGM_DIAL_Raise_Right_Hand);

                SGM_DIAL_Raise_Left_Hand = new Grammar(BaseFolder + @"Grammar\English\Action\SGM_DIAL_Raise_Left_Hand.xml");
                LoadGrammar(SGM_DIAL_Raise_Left_Hand);
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                SGM_FUNC_COMPLEX_SetReminder = new Grammar(BaseFolder + @"Grammar\English\SetReminder\SGM_FUNC_COMPLEX_SetReminder.xml");
                SGM_FUNC_COMPLEX_SetReminder.Priority = 1;
                SGM_FUNC_COMPLEX_SetReminder.Weight = 1f;
                LoadGrammar(SGM_FUNC_COMPLEX_SetReminder);

                SGM_FUNC_COMPLEX_SetReminderYesNo = new Grammar(BaseFolder + @"Grammar\English\SetReminder\SGM_FUNC_COMPLEX_SetReminderYesNo.xml");
                SGM_FUNC_COMPLEX_SetReminderYesNo.Priority = 1;
                SGM_FUNC_COMPLEX_SetReminderYesNo.Weight = 1f;
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                SGM_FUNC_Call = new Grammar(BaseFolder + @"Grammar\English\Skype\SGM_FUNC_Call.xml");
                LoadGrammar(SGM_FUNC_Call);

                SGM_FUNC_CallPerson = new Grammar(BaseFolder + @"Grammar\English\Skype\SGM_FUNC_CallPerson.xml");

                SGM_FUNC_SkypePhoneCallFinished = new Grammar(BaseFolder + @"Grammar\English\Skype\SGM_FUNC_SkypePhoneCallFinished.xml");
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                SGM_FUNC_Char = new Grammar(BaseFolder + @"Grammar\English\Register\SGM_FUNC_Char.xml");

                SGM_FUNC_RegisterMode_YesNo = new Grammar(BaseFolder + @"Grammar\English\Register\SGM_FUNC_RegisterMode_YesNo.xml");
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                SGM_FUNC_StartRadioStaion = new Grammar(BaseFolder + @"Grammar\English\RadioFunction\SGM_FUNC_StartRadioStaion.xml");
                LoadGrammar(SGM_FUNC_StartRadioStaion);

                SGM_FUNC_StopRadioStation = new Grammar(BaseFolder + @"Grammar\English\RadioFunction\SGM_FUNC_StopRadioStation.xml");

                SGM_FUNC_StartRadioStationYesNo = new Grammar(BaseFolder + @"Grammar\English\RadioFunction\SGM_FUNC_StartRadioStationYesNo.xml");

                SGM_FUNC_StopRadioStationYesNo = new Grammar(BaseFolder + @"Grammar\English\RadioFunction\SGM_FUNC_StopRadioStationYesNo.xml");
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                //SGM_FUNC_ControlTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_ControlTV.xml");
                //LoadGrammar(SGM_FUNC_ControlTV);

                //SGM_FUNC_ControlProjector = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_ControlProjector.xml");
                //LoadGrammar(SGM_FUNC_ControlProjector);

                //SGM_FUNC_PowerOnOffTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_PowerOnOffTV.xml");
                //SGM_FUNC_PowerOnOffTV.Priority = 1;
                //SGM_FUNC_PowerOnOffTV.Weight = 1f;

                //SGM_FUNC_MenuTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_MenuTV.xml");

                //SGM_FUNC_MuteTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_MuteTV.xml");

                //SGM_FUNC_ChangeInputTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_ChangeInputTV.xml");

                //SGM_FUNC_upTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_upTV.xml");

                //SGM_FUNC_downTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_downTV.xml");

                //SGM_FUNC_leftTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_leftTV.xml");

                //SGM_FUNC_rightTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_rightTV.xml");

                //SGM_FUNC_enterTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_enterTV.xml");

                //SGM_FUNC_ChannelPlusTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_ChannelPlusTV.xml");

                //SGM_FUNC_ChannelMinusTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_ChannelMinusTV.xml");

                //SGM_FUNC_VolumePlusTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_VolumePlusTV.xml");

                //SGM_FUNC_VolumeMinusTV = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlTV\SGM_FUNC_VolumeMinusTV.xml");
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                SGM_FUNC_ControlRadio = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlRadio\SGM_FUNC_ControlRadio.xml");
                LoadGrammar(SGM_FUNC_ControlRadio);

                SGM_FUNC_NextRadio = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlRadio\SGM_FUNC_NextRadio.xml");

                SGM_FUNC_PreviousRadio = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlRadio\SGM_FUNC_PreviousRadio.xml");

                SGM_FUNC_VolumeUpRadio = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlRadio\SGM_FUNC_VolumeUpRadio.xml");

                SGM_FUNC_VolumeDownRadio = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlRadio\SGM_FUNC_VolumeDownRadio.xml");
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                //SGM_FUNC_ControlFan = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlFan\SGM_FUNC_ControlFan.xml");
                //LoadGrammar(SGM_FUNC_ControlFan);

                //SGM_FUNC_PowerOnOffFan = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlFan\SGM_FUNC_PowerOnOffFan.xml");

                //SGM_FUNC_onUpDown = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlFan\SGM_FUNC_onUpDown.xml");

                //SGM_FUNC_onLeftRightFan = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlFan\SGM_FUNC_onLeftRightFan.xml");

                //SGM_FUNC_Speed = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlFan\SGM_FUNC_Speed.xml");

                //SGM_FUNC_Timer = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlFan\SGM_FUNC_Timer.xml");
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                //SGM_FUNC_PowerOnLight = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlLight\SGM_FUNC_PowerOnLight.xml");
                //LoadGrammar(SGM_FUNC_PowerOnLight);

                //SGM_FUNC_PowerOffLight = new Grammar(BaseFolder + @"Grammar\English\iRKit\ControlLight\SGM_FUNC_PowerOffLight.xml");
                //LoadGrammar(SGM_FUNC_PowerOffLight);
                //////////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////////////
                SGM_DIAL_SayHello = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_SayHello.xml");
                LoadGrammar(SGM_DIAL_SayHello);

                SGM_DIAL_AskIntroduction = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_AskIntroduction.xml");
                LoadGrammar(SGM_DIAL_AskIntroduction);

                SGM_DIAL_AskRobotHowAreYou = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_AskRobotHowAreYou.xml");
                LoadGrammar(SGM_DIAL_AskRobotHowAreYou);

                SGM_DIAL_NiceToMeetYou = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_NiceToMeetYou.xml");
                LoadGrammar(SGM_DIAL_NiceToMeetYou);

                SGM_DIAL_AskWhatIsSocialRobot = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_AskWhatIsSocialRobot.xml");
                LoadGrammar(SGM_DIAL_AskWhatIsSocialRobot);

                SGM_DIAL_AskWhoDesign = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_AskWhoDesign.xml");
                LoadGrammar(SGM_DIAL_AskWhoDesign);
                               
                SGM_DIAL_GoodBye = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_GoodBye.xml");
                LoadGrammar(SGM_DIAL_GoodBye);

                SGM_DIAL_Greeting = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_Greeting.xml");
                LoadGrammar(SGM_DIAL_Greeting);

                SGM_DIAL_ThankYou = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_ThankYou.xml");
                LoadGrammar(SGM_DIAL_ThankYou);

                SGM_DIAL_Scold = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_Scold.xml");
                LoadGrammar(SGM_DIAL_Scold);

                SGM_DIAL_Compliment = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_Compliment.xml");
                LoadGrammar(SGM_DIAL_Compliment);

                SGM_DIAL_AskRobotName = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_AskRobotName.xml");
                LoadGrammar(SGM_DIAL_AskRobotName);

                SGM_DIAL_SwitchLanguageToChinese = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_SwitchLanguageToChinese.xml");
                LoadGrammar(SGM_DIAL_SwitchLanguageToChinese);

                SGM_DIAL_SwitchLanguageToChinese_YesNo = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_SwitchLanguageToChinese_YesNo.xml");

                SGM_DIAL_SwitchLanguageToJapanese = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_SwitchLanguageToJapanese.xml");
                LoadGrammar(SGM_DIAL_SwitchLanguageToJapanese);

                SGM_DIAL_SwitchLanguageToJapanese_YesNo = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_SwitchLanguageToJapanese_YesNo.xml");

                SGM_DIAL_LookAtMe = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_LookAtMe.xml");
                LoadGrammar(SGM_DIAL_LookAtMe);

                SGM_DIAL_Sleep = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_Sleep.xml");
                LoadGrammar(SGM_DIAL_Sleep);

                SGM_DIAL_Sleep_YesNo = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_Sleep_YesNo.xml");

                SGM_DIAL_Emotion = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_Emotion.xml");
                LoadGrammar(SGM_DIAL_Emotion);

                SGM_DIAL_Help = new Grammar(BaseFolder + @"Grammar\English\Greeting\SGM_DIAL_Help.xml");
                LoadGrammar(SGM_DIAL_Help);
                //////////////////////////////////////////////////////////////////////////////////////////
            }

            catch
            {
                SRE_Speech.SpeakAsync("Load Grammar Error");
            }

        }

        public void SRGS_LoadGrammarModels()
        {
            try
            {
                LoadGrammar(SGM_DIAL_SayHello);

                LoadGrammar(SGM_DIAL_AskIntroduction);

                LoadGrammar(SGM_DIAL_AskRobotHowAreYou);

                LoadGrammar(SGM_DIAL_NiceToMeetYou);

                LoadGrammar(SGM_DIAL_AskWhatIsSocialRobot);

                LoadGrammar(SGM_DIAL_AskWhoDesign);

                LoadGrammar(SGM_DIAL_GoodBye);

                LoadGrammar(SGM_DIAL_Greeting);

                LoadGrammar(SGM_DIAL_ThankYou);

                LoadGrammar(SGM_DIAL_Scold);

                LoadGrammar(SGM_DIAL_Compliment);

                LoadGrammar(SGM_DIAL_AskRobotName);

                LoadGrammar(SGM_DIAL_SwitchLanguageToChinese);

                LoadGrammar(SGM_DIAL_LookAtMe);

                LoadGrammar(SGM_DIAL_Sleep);

                LoadGrammar(SGM_DIAL_SwitchLanguageToJapanese);

                LoadGrammar(SGM_FUNC_ControlFan);

                LoadGrammar(SGM_FUNC_PowerOnLight);

                LoadGrammar(SGM_FUNC_PowerOffLight);

                LoadGrammar(SGM_FUNC_ControlRadio);

                LoadGrammar(SGM_FUNC_StartRadioStaion);

                LoadGrammar(SGM_FUNC_AskWhatTimeNow);

                LoadGrammar(SGM_FUNC_AskWhatDayIsToday);

                LoadGrammar(SGM_FUNC_AskWhatDateIsToday);

                LoadGrammar(SGM_FUNC_AskTodayCustomWeather);

                LoadGrammar(SGM_FUNC_AskTomorrowCustomWeather);

                LoadGrammar(SGM_DIAL_AskCountryPresidentOrPrimeMinister);

                SGM_FUNC_COMPLEX_SetReminder.Priority = 1;
                SGM_FUNC_COMPLEX_SetReminder.Weight = 1f;
                LoadGrammar(SGM_FUNC_COMPLEX_SetReminder);

                SGM_FUNC_COMPLEX_SetReminderYesNo.Priority = 1;
                SGM_FUNC_COMPLEX_SetReminderYesNo.Weight = 1f;

                LoadGrammar(SGM_FUNC_Call);

                LoadGrammar(SGM_DIAL_Raise_Right_Hand);

                LoadGrammar(SGM_DIAL_Raise_Left_Hand);

                LoadGrammar(SGM_FUNC_GoingOut);
            }

            catch
            {
                SRE_Speech.SpeakAsync("Load Grammar Error");
            }

        }

        public void SRGS_UnloadGrammarModels()
        {
            try
            {
                UnloadGrammar(SGM_DIAL_SayHello);

                UnloadGrammar(SGM_DIAL_AskIntroduction);

                UnloadGrammar(SGM_DIAL_AskRobotHowAreYou);

                UnloadGrammar(SGM_DIAL_NiceToMeetYou);

                UnloadGrammar(SGM_DIAL_AskWhatIsSocialRobot);

                UnloadGrammar(SGM_DIAL_AskWhoDesign);

                UnloadGrammar(SGM_DIAL_GoodBye);

                UnloadGrammar(SGM_DIAL_Greeting);

                UnloadGrammar(SGM_DIAL_ThankYou);

                UnloadGrammar(SGM_DIAL_Scold);

                UnloadGrammar(SGM_DIAL_Compliment);

                UnloadGrammar(SGM_DIAL_AskRobotName);

                UnloadGrammar(SGM_DIAL_SwitchLanguageToChinese);

                UnloadGrammar(SGM_DIAL_LookAtMe);

                UnloadGrammar(SGM_DIAL_Sleep);

                UnloadGrammar(SGM_DIAL_SwitchLanguageToJapanese);

                UnloadGrammar(SGM_FUNC_ControlFan);

                UnloadGrammar(SGM_FUNC_PowerOnLight);

                UnloadGrammar(SGM_FUNC_PowerOffLight);

                UnloadGrammar(SGM_FUNC_ControlRadio);

                UnloadGrammar(SGM_FUNC_StartRadioStaion);

                UnloadGrammar(SGM_FUNC_AskWhatTimeNow);

                UnloadGrammar(SGM_FUNC_AskWhatDayIsToday);

                UnloadGrammar(SGM_FUNC_AskWhatDateIsToday);

                UnloadGrammar(SGM_FUNC_AskTodayCustomWeather);

                UnloadGrammar(SGM_FUNC_AskTomorrowCustomWeather);

                UnloadGrammar(SGM_DIAL_AskCountryPresidentOrPrimeMinister);

                SGM_FUNC_COMPLEX_SetReminder.Priority = 1;
                SGM_FUNC_COMPLEX_SetReminder.Weight = 1f;
                UnloadGrammar(SGM_FUNC_COMPLEX_SetReminder);

                SGM_FUNC_COMPLEX_SetReminderYesNo.Priority = 1;
                SGM_FUNC_COMPLEX_SetReminderYesNo.Weight = 1f;

                UnloadGrammar(SGM_FUNC_Call);

                //   UnloadGrammar(SGM_FUNC_SendMessage);

                UnloadGrammar(SGM_DIAL_Raise_Right_Hand);

                UnloadGrammar(SGM_DIAL_Raise_Left_Hand);

                UnloadGrammar(SGM_FUNC_GoingOut);
            }

            catch
            {
                SRE_Speech.SpeakAsync("Unload Grammar Error");
            }

        }
        public void HokkienGrammarLoad()
        {
            try
            {
                SGM_FUNC_HOK_关收音机 = new Grammar(BaseFolder + @"Grammar\hokkian\SGM_FUNC_HOK_关收音机.xml");
                LoadGrammarCN(SGM_FUNC_HOK_关收音机);

                SGM_FUNC_HOK_关灯 = new Grammar(BaseFolder + @"Grammar\hokkian\SGM_FUNC_HOK_关灯.xml");
                LoadGrammarCN(SGM_FUNC_HOK_关灯);

                SGM_FUNC_HOK_关电视机 = new Grammar(BaseFolder + @"Grammar\hokkian\SGM_FUNC_HOK_关电视机.xml");
                LoadGrammarCN(SGM_FUNC_HOK_关电视机);

                SGM_FUNC_HOK_关风扇 = new Grammar(BaseFolder + @"Grammar\hokkian\SGM_FUNC_HOK_关风扇.xml");
                LoadGrammarCN(SGM_FUNC_HOK_关风扇);

                SGM_FUNC_HOK_开收音机 = new Grammar(BaseFolder + @"Grammar\hokkian\SGM_FUNC_HOK_开收音机.xml");
                LoadGrammarCN(SGM_FUNC_HOK_开收音机);

                SGM_FUNC_HOK_开灯 = new Grammar(BaseFolder + @"Grammar\hokkian\SGM_FUNC_HOK_开灯.xml");
                LoadGrammarCN(SGM_FUNC_HOK_开灯);

                SGM_FUNC_HOK_开电视机 = new Grammar(BaseFolder + @"Grammar\hokkian\SGM_FUNC_HOK_开电视机.xml");
                LoadGrammarCN(SGM_FUNC_HOK_开电视机);

                SGM_FUNC_HOK_开风扇 = new Grammar(BaseFolder + @"Grammar\hokkian\SGM_FUNC_HOK_开风扇.xml");
                LoadGrammarCN(SGM_FUNC_HOK_开风扇);

                SGM_FUNC_HOK_日期 = new Grammar(BaseFolder + @"Grammar\hokkian\SGM_FUNC_HOK_日期.xml");
                LoadGrammarCN(SGM_FUNC_HOK_日期);

                SGM_FUNC_HOK_星期 = new Grammar(BaseFolder + @"Grammar\hokkian\SGM_FUNC_HOK_星期.xml");
                LoadGrammarCN(SGM_FUNC_HOK_星期);

            }
            catch
            {
                SRECN_Speech.SpeakAsync("语法装载错误");
            }
        }


        public void ChineseGrammarLoad()
        {
            try
            {
                SGM_DIAL_你好 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_你好.xml");
                LoadGrammarCN(SGM_DIAL_你好);

                SGM_DIAL_谢谢 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_谢谢.xml");
                LoadGrammarCN(SGM_DIAL_谢谢);

                SGM_DIAL_你叫什么名字 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_你叫什么名字.xml");
                LoadGrammarCN(SGM_DIAL_你叫什么名字);

                SGM_DIAL_早上好 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_早上好.xml");
                LoadGrammarCN(SGM_DIAL_早上好);

                SGM_DIAL_中午好 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_中午好.xml");
                LoadGrammarCN(SGM_DIAL_中午好);

                SGM_DIAL_下午好 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_下午好.xml");
                LoadGrammarCN(SGM_DIAL_下午好);

                SGM_DIAL_晚上好 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_晚上好.xml");
                LoadGrammarCN(SGM_DIAL_晚上好);

                SGM_DIAL_自我介绍 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_自我介绍.xml");
                LoadGrammarCN(SGM_DIAL_自我介绍);

                SGM_DIAL_谁设计了你 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_谁设计了你.xml");
                LoadGrammarCN(SGM_DIAL_谁设计了你);

                SGM_DIAL_功能 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_功能.xml");
                LoadGrammarCN(SGM_DIAL_功能);

                SGM_DIAL_英文识别 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_英文识别.xml");
                LoadGrammarCN(SGM_DIAL_英文识别);

                SGM_DIAL_英文识别_是否 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_英文识别_是否.xml");

                SGM_DIAL_日文识别 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_日文识别.xml");
                LoadGrammarCN(SGM_DIAL_日文识别);

                SGM_DIAL_日文识别_是否 = new Grammar(BaseFolder + @"Grammar\Chinese\打招呼\SGM_DIAL_日文识别_是否.xml");

                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                SGM_FUNC_CHN_时间 = new Grammar(BaseFolder + @"Grammar\Chinese\时间\SGM_FUNC_CHN_时间.xml");
                LoadGrammarCN(SGM_FUNC_CHN_时间);

                SGM_FUNC_CHN_日期 = new Grammar(BaseFolder + @"Grammar\Chinese\时间\SGM_FUNC_CHN_日期.xml");
                LoadGrammarCN(SGM_FUNC_CHN_日期);

                SGM_FUNC_CHN_星期 = new Grammar(BaseFolder + @"Grammar\Chinese\时间\SGM_FUNC_CHN_星期.xml");
                LoadGrammarCN(SGM_FUNC_CHN_星期);

                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                //SGM_FUNC_CHN_控制电视 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_控制电视.xml");
                //LoadGrammarCN(SGM_FUNC_CHN_控制电视);

                //SGM_FUNC_CHN_电源 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_电源.xml");

                //SGM_FUNC_CHN_菜单 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_菜单.xml");

                //SGM_FUNC_CHN_上 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_上.xml");

                //SGM_FUNC_CHN_下 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_下.xml");

                //SGM_FUNC_CHN_左 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_左.xml");

                //SGM_FUNC_CHN_右 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_右.xml");

                //SGM_FUNC_CHN_声音加 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_声音加.xml");

                //SGM_FUNC_CHN_声音减 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_声音减.xml");

                //SGM_FUNC_CHN_频道加 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_频道加.xml");

                //SGM_FUNC_CHN_频道减 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_频道减.xml");

                //SGM_FUNC_CHN_进入 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_进入.xml");

                //SGM_FUNC_CHN_退出 = new Grammar(BaseFolder + @"Grammar\Chinese\控制\SGM_FUNC_CHN_退出.xml");

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //SGM_FUNC_问问题 = new Grammar(BaseFolder + @"Grammar\Chinese\XunFei\SGM_FUNC_问问题.xml");
                //LoadGrammarCN(SGM_FUNC_问问题);

                //SGM_FUNC_计算 = new Grammar(BaseFolder + @"Grammar\Chinese\XunFei\SGM_FUNC_计算.xml");
                //LoadGrammarCN(SGM_FUNC_计算);
            }

            catch
            {
                SRECN_Speech.SpeakAsync("语法装载错误");
            }
        }


        public void CantoneseGrammarLoad()
        {
            try
            {
                SGM_DIAL_HK_下午好 = new Grammar(BaseFolder + @"Grammar\Cantonese\打招呼\SGM_DIAL_HK_下午好.xml");
                LoadGrammarCAN(SGM_DIAL_HK_下午好);

                SGM_DIAL_HK_你叫什么名字 = new Grammar(BaseFolder + @"Grammar\Cantonese\打招呼\SGM_DIAL_HK_你叫什么名字.xml");
                LoadGrammarCAN(SGM_DIAL_HK_你叫什么名字);

                SGM_DIAL_HK_你好 = new Grammar(BaseFolder + @"Grammar\Cantonese\打招呼\SGM_DIAL_HK_你好.xml");
                LoadGrammarCAN(SGM_DIAL_HK_你好);

                SGM_DIAL_HK_早上好 = new Grammar(BaseFolder + @"Grammar\Cantonese\打招呼\SGM_DIAL_HK_早上好.xml");
                LoadGrammarCAN(SGM_DIAL_HK_早上好);

                SGM_DIAL_HK_晚上好 = new Grammar(BaseFolder + @"Grammar\Cantonese\打招呼\SGM_DIAL_HK_晚上好.xml");
                LoadGrammarCAN(SGM_DIAL_HK_晚上好);

                SGM_DIAL_HK_自我介绍 = new Grammar(BaseFolder + @"Grammar\Cantonese\打招呼\SGM_DIAL_HK_自我介绍.xml");
                LoadGrammarCAN(SGM_DIAL_HK_自我介绍);

                SGM_FUNC_HK_日期 = new Grammar(BaseFolder + @"Grammar\Cantonese\时间\SGM_FUNC_HK_日期.xml");
                LoadGrammarCAN(SGM_FUNC_HK_日期);

                SGM_FUNC_HK_时间 = new Grammar(BaseFolder + @"Grammar\Cantonese\时间\SGM_FUNC_HK_时间.xml");
                LoadGrammarCAN(SGM_FUNC_HK_时间);

                SGM_FUNC_HK_星期 = new Grammar(BaseFolder + @"Grammar\Cantonese\时间\SGM_FUNC_HK_星期.xml");
                LoadGrammarCAN(SGM_FUNC_HK_星期);

                SGM_FUNC_HK_关灯 = new Grammar(BaseFolder + @"Grammar\Cantonese\控制\SGM_FUNC_HK_关灯.xml");
                LoadGrammarCAN(SGM_FUNC_HK_关灯);


                SGM_FUNC_HK_开灯 = new Grammar(BaseFolder + @"Grammar\Cantonese\控制\SGM_FUNC_HK_开灯.xml");
                LoadGrammarCAN(SGM_FUNC_HK_开灯);

                SGM_FUNC_HK_开收音机 = new Grammar(BaseFolder + @"Grammar\Cantonese\控制\SGM_FUNC_HK_开收音机.xml");
                LoadGrammarCAN(SGM_FUNC_HK_开收音机);

                SGM_FUNC_HK_关闭收音机 = new Grammar(BaseFolder + @"Grammar\Cantonese\控制\SGM_FUNC_HK_关闭收音机.xml");
                LoadGrammarCAN(SGM_FUNC_HK_关闭收音机);

                SGM_FUNC_HK_关电视 = new Grammar(BaseFolder + @"Grammar\Cantonese\控制\SGM_FUNC_HK_关电视.xml");
                LoadGrammarCAN(SGM_FUNC_HK_关电视);

                SGM_FUNC_HK_开电视 = new Grammar(BaseFolder + @"Grammar\Cantonese\控制\SGM_FUNC_HK_开电视.xml");
                LoadGrammarCAN(SGM_FUNC_HK_开电视);

                SGM_FUNC_HK_关风扇 = new Grammar(BaseFolder + @"Grammar\Cantonese\控制\SGM_FUNC_HK_关风扇.xml");
                LoadGrammarCAN(SGM_FUNC_HK_关风扇);

                SGM_FUNC_HK_开风扇 = new Grammar(BaseFolder + @"Grammar\Cantonese\控制\SGM_FUNC_HK_开风扇.xml");
                LoadGrammarCAN(SGM_FUNC_HK_开风扇);
            }

            catch
            {
                SRECN_Speech.SpeakAsync("语法装载错误");
            }

        }
        public void JapaneseGrammarLoad()
        {
            try
            {
                SGM_FUNC_こんにちは = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_こんにちは.xml");
                LoadGrammarJP(SGM_FUNC_こんにちは);

                SGM_FUNC_中国語に切り替え = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_中国語に切り替え.xml");
                LoadGrammarJP(SGM_FUNC_中国語に切り替え);

                SGM_FUNC_中国語に切り替え_確認 = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_中国語に切り替え_確認.xml");

                SGM_FUNC_英語に切り替え = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_英語に切り替え.xml");
                LoadGrammarJP(SGM_FUNC_英語に切り替え);

                SGM_FUNC_英語に切り替え_確認 = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_英語に切り替え_確認.xml");

                SGM_FUNC_天気 = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_天気.xml");
                LoadGrammarJP(SGM_FUNC_天気);

                SGM_FUNC_時間 = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_時間.xml");
                LoadGrammarJP(SGM_FUNC_時間);

                SGM_FUNC_曜日 = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_曜日.xml");
                LoadGrammarJP(SGM_FUNC_曜日);

                SGM_FUNC_名前 = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_名前.xml");
                LoadGrammarJP(SGM_FUNC_名前);

                SGM_FUNC_おやすみ = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_おやすみ.xml");
                LoadGrammarJP(SGM_FUNC_おやすみ);

                SGM_FUNC_おやすみ_確認 = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_おやすみ_確認.xml");

                SGM_FUNC_自己紹介 = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_自己紹介.xml");
                LoadGrammarJP(SGM_FUNC_自己紹介);

                SGM_FUNC_日 = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_日.xml");
                LoadGrammarJP(SGM_FUNC_日);

                SGM_FUNC_元気 = new Grammar(BaseFolder + @"Grammar\Japanese\ご挨拶\SGM_FUNC_元気.xml");
                LoadGrammarJP(SGM_FUNC_元気);

                /*              SGM_FUNC_Skype起動 = new Grammar(@"C:\test\XmlGrammar\Japanese\ご挨拶\SGM_FUNC_Skype起動.xml");
                                LoadGrammarJP(SG M_FUNC_Skype起動);

                                SGM_FUNC_Skype電話 = new Grammar(@"C:\test\XmlGrammar\Japanese\ご挨拶\SGM_FUNC_Skype電話.xml");
                                LoadGrammarJP(SGM_FUNC_Skype電話);

                                SGM_FUNC_Skype電話_国 = new Grammar(@"C:\test\XmlGrammar\Japanese\ご挨拶\SGM_FUNC_Skype電話_国.xml");
                                LoadGrammarJP(SGM_FUNC_Skype電話_国);

                                SGM_FUNC_Skype電話_電話番号 = new Grammar(@"C:\test\XmlGrammar\Japanese\ご挨拶\SGM_FUNC_Skype電話_電話番号.xml");
                                LoadGrammarJP(SGM_FUNC_Skype電話_電話番号);

                                SGM_FUNC_Skype電話_電話番号_確認 = new Grammar(@"C:\test\XmlGrammar\Japanese\ご挨拶\SGM_FUNC_Skype電話_電話番号_確認.xml");

                                SGM_FUNC_Skype電話終了 = new Grammar(@"C:\test\XmlGrammar\Japanese\ご挨拶\SGM_FUNC_Skype電話終了.xml");
                                LoadGrammarJP(SGM_FUNC_Skype電話終了);
                */
            }

            catch
            {
                SRECN_Speech.SpeakAsync("语法装载错误");//Japanese
            }
        }

        public bool MouthMuteMark = false;

        public void LayerGrammarLoadAndUnload(string RuleName, string ResultName)
        {
            IRCommand = new List<Grammar>{
                                      SGM_FUNC_PowerOnOffTV,
                                      SGM_FUNC_MenuTV,
                                      SGM_FUNC_MuteTV,
                                      SGM_FUNC_ChangeInputTV,
                                      SGM_FUNC_upTV,
                                      SGM_FUNC_downTV,
                                      SGM_FUNC_leftTV,
                                      SGM_FUNC_rightTV,
                                      SGM_FUNC_enterTV,
                                      SGM_FUNC_ChannelPlusTV,
                                      SGM_FUNC_ChannelMinusTV ,
                                      SGM_FUNC_VolumePlusTV,
                                      SGM_FUNC_VolumeMinusTV,
                                        };

            IRCommandFan = new List<Grammar>{
                                      SGM_FUNC_PowerOnOffFan,
                                      SGM_FUNC_onUpDown,
                                      SGM_FUNC_onLeftRightFan,
                                      SGM_FUNC_Speed,
                                      SGM_FUNC_Timer,
                                        };

            IRCommandRadio = new List<Grammar>{
                SGM_FUNC_VolumeUpRadio,
                SGM_FUNC_VolumeDownRadio,
                SGM_FUNC_NextRadio,
                SGM_FUNC_PreviousRadio,

            };

            控制 = new List<Grammar>{
                                   //SGM_FUNC_CHN_控制电视,
                                   //SGM_FUNC_CHN_电源,
                                   //SGM_FUNC_CHN_菜单,
                                   //SGM_FUNC_CHN_上,
                                   //SGM_FUNC_CHN_下,
                                   //SGM_FUNC_CHN_左,
                                   //SGM_FUNC_CHN_右,
                                   //SGM_FUNC_CHN_声音加,
                                   //SGM_FUNC_CHN_声音减,
                                   //SGM_FUNC_CHN_频道加,
                                   //SGM_FUNC_CHN_频道减,
                                   //SGM_FUNC_CHN_进入,
                                   //SGM_FUNC_CHN_退出
                                    };


            DIAL = new List<Grammar>{
                                      SGM_DIAL_SayHello,
                                      SGM_DIAL_AskIntroduction,
                                      SGM_DIAL_AskRobotHowAreYou,
                                      SGM_DIAL_NiceToMeetYou,
                                      SGM_DIAL_AskWhatIsSocialRobot,
                                      SGM_DIAL_AskWhoDesign,
                                      SGM_DIAL_GoodBye,
                                      SGM_DIAL_Greeting,
                                      SGM_DIAL_ThankYou,
                                      SGM_DIAL_Scold,
                                      SGM_DIAL_Compliment,
                                      SGM_DIAL_AskRobotName,
                                      SGM_DIAL_LookAtMe,
                                      SGM_DIAL_Sleep,
                                      SGM_DIAL_SwitchLanguageToChinese,
                                      SGM_DIAL_SwitchLanguageToJapanese,
                                      SGM_DIAL_Emotion,
                                     };


            Layerone = new List<Grammar> {

                                           SGM_FUNC_AskWhatTimeNow,
                                           SGM_FUNC_AskWhatDayIsToday,
                                           SGM_FUNC_AskWhatDateIsToday,

                                           SGM_FUNC_AskTodayCustomWeather,
                                           SGM_FUNC_AskTomorrowCustomWeather,


                                           SGM_FUNC_COMPLEX_SetReminder,

                                           SGM_FUNC_ControlTV,
                                           SGM_FUNC_ControlProjector,

                                           SGM_FUNC_StartRadioStaion,

                                           SGM_FUNC_Call




                                        };

            中文语法 = new List<Grammar>
            {
                   SGM_FUNC_CHN_时间,
                   SGM_FUNC_CHN_日期,
                   SGM_FUNC_CHN_星期,
                   SGM_DIAL_谢谢,
                   SGM_DIAL_你好,
                   SGM_DIAL_你叫什么名字,
                   SGM_DIAL_早上好,
                   SGM_DIAL_中午好,
                   SGM_DIAL_下午好,
                   SGM_DIAL_晚上好,
                   SGM_DIAL_自我介绍,
                   SGM_DIAL_谁设计了你,
                   SGM_DIAL_功能,
                   SGM_DIAL_英文识别,
                   //SGM_FUNC_问问题,
                   //SGM_FUNC_计算,
                   //SGM_FUNC_CHN_控制电视

            };

            // Layerone.AddRange(DIAL);

            try
            {
                if (Function.Vision.WakeUp)
                {
                    switch (RuleName)
                    {
                        case "SGM_FUNC_ReadNews":
                            LoadGrammar(SGM_FUNC_LanguageOption);
                            UnloadGrammar(SGM_FUNC_ReadNews);
                            LoadGrammar(SGM_FUNC_StopReadNews);
                            break;

                        case "SGM_FUNC_LanguageOption":
                            if (ResultName == "English news" || ResultName == "English news please" || ResultName == "English")
                            {
                                LoadGrammar(SGM_NewsOption);
                            }
                            else
                            {
                                LoadGrammar(SGM_FUNC_NextNews);
                            }
                            UnloadGrammar(SGM_FUNC_LanguageOption);
                            break;

                        case "SGM_ContinueReadNews_YesNo":
                            UnloadGrammar(SGM_ContinueReadNews_YesNo);
                            MainWindow.GrammarTimer.Stop();
                            break;

                        case "SGM_FUNC_StopReadNews":
                            UnloadGrammar(SGM_FUNC_NextNews);
                            UnloadGrammar(SGM_FUNC_StopReadNews);
                            LoadGrammar(SGM_FUNC_ReadNews);
                            break;

                        //////////////////////////////////////////////

                        case "SGM_FUNC_COMPLEX_SetReminder":
                            //      foreach (Grammar DIALGrammar  in DIAL)
                            //      {
                            //         UnloadGrammar(DIALGrammar);
                            //    }
                            UnloadGrammar(SGM_DIAL_SayHello);
                            UnloadGrammar(SGM_DIAL_LookAtMe);
                            LoadGrammar(SGM_FUNC_COMPLEX_SetReminderYesNo);
                            break;

                        case "SGM_FUNC_COMPLEX_SetReminderYesNo":
                            UnloadGrammar(SGM_FUNC_COMPLEX_SetReminderYesNo);
                            LoadGrammar(SGM_DIAL_SayHello);
                            LoadGrammar(SGM_DIAL_LookAtMe);

                            //   foreach (Grammar DIALGrammar in DIAL)
                            //    {
                            //        LoadGrammar(DIALGrammar);
                            //     }
                            break;

                        //////////////////////////////////////////////

                        //case "SGM_FUNC_ControlTV":

                        //    if (IRCommand_flag == false)
                        //    {
                        //        foreach (Grammar SubLayer_IRCommand in IRCommand)
                        //        {
                        //            LoadGrammar(SubLayer_IRCommand);
                        //        }
                        //        IRCommand_flag = true;
                        //    }
                        //    else if (IRCommand_flag == true)
                        //    {

                        //    }
                        //    break;

                        //case "SGM_FUNC_ControlProjector":
                        //    if (IRCommand_flag == false)
                        //    {
                        //        foreach (Grammar SubLayer_IRCommand in IRCommand)
                        //        {
                        //            LoadGrammar(SubLayer_IRCommand);
                        //        }
                        //        IRCommand_flag = true;
                        //    }
                        //    else if (IRCommand_flag == true)
                        //    {

                        //    }
                        //    break;

                        //case "SGM_FUNC_ControlFan":
                        //    if (IRCommand_flag == false)
                        //    {
                        //        foreach (Grammar SubLayer_IRCommand in IRCommandFan)
                        //        {
                        //            LoadGrammar(SubLayer_IRCommand);
                        //        }
                        //        IRCommand_flag = true;
                        //    }
                        //    else if (IRCommand_flag == true)
                        //    {

                        //    }

                        //    break;

                        //case "SGM_FUNC_ControlRadio":
                        //    if (IRCommand_flag == false)
                        //    {
                        //        foreach (Grammar SubLayer_IRCommand in IRCommandRadio)
                        //        {
                        //            LoadGrammar(SubLayer_IRCommand);
                        //        }
                        //        IRCommand_flag = true;
                        //    }
                        //    else if (IRCommand_flag == true)
                        //    {

                        //    }

                        //    break;

                        //////////////////////////////////////////////

                        case "SGM_FUNC_StartRadioStaion":
                            UnloadGrammar(SGM_FUNC_StartRadioStaion);
                            LoadGrammar(SGM_FUNC_StartRadioStationYesNo);
                            break;

                        case "SGM_FUNC_StartRadioStationYesNo":
                            UnloadGrammar(SGM_FUNC_StartRadioStationYesNo);
                            LoadGrammar(SGM_FUNC_StopRadioStation);
                            break;

                        case "SGM_FUNC_StopRadioStation":
                            UnloadGrammar(SGM_FUNC_StopRadioStation);
                            LoadGrammar(SGM_FUNC_StopRadioStationYesNo);
                            break;

                        case "SGM_FUNC_StopRadioStationYesNo":
                            UnloadGrammar(SGM_FUNC_StopRadioStationYesNo);
                            LoadGrammar(SGM_FUNC_StartRadioStaion);
                            break;

                        //////////////////////////////////////////////

                        case "SGM_FUNC_SkypePhoneCallFinished":
                            if (Function.Vision.UserName != "Unknown")
                            {
                                UnloadGrammar(SGM_FUNC_SkypePhoneCallFinished);
                            }
                            //foreach (Grammar Greeting in DIAL)
                            //{
                            //    LoadGrammar(Greeting);
                            //}
                            //foreach (Grammar Greeting in Layerone)
                            //{
                            //    LoadGrammar(Greeting);
                            //}
                            break;

                        case "SGM_FUNC_Call":
                            //foreach (Grammar Greeting in DIAL)
                            //{
                            //    UnloadGrammar(Greeting);
                            //}
                            //foreach (Grammar Greeting in Layerone)
                            //{
                            //    UnloadGrammar(Greeting);
                            //}
                            //UnloadAllGrammars();
                            LoadGrammar(SGM_FUNC_CallPerson);
                            break;

                        case "SGM_FUNC_CallPerson":
                            UnloadGrammar(SGM_FUNC_CallPerson);
                            LoadGrammar(SGM_FUNC_SkypePhoneCallFinished);
                            MouthMuteMark = true;
                            break;

                        //////////////////////////////////////////////

                        case "SGM_DIAL_SwitchLanguageToChinese":
                            UnloadGrammar(SGM_DIAL_SwitchLanguageToChinese);
                            LoadGrammar(SGM_DIAL_SwitchLanguageToChinese_YesNo);
                            MainWindow.GrammarTimer.Start();
                            break;

                        case "SGM_DIAL_SwitchLanguageToChinese_YesNo":
                            UnloadGrammar(SGM_DIAL_SwitchLanguageToChinese_YesNo);
                            MainWindow.GrammarTimer.Stop();
                            LoadGrammar(SGM_DIAL_SwitchLanguageToChinese);
                            break;

                        case "SGM_DIAL_SwitchLanguageToJapanese":
                            UnloadGrammar(SGM_DIAL_SwitchLanguageToJapanese);
                            LoadGrammar(SGM_DIAL_SwitchLanguageToJapanese_YesNo);
                            MainWindow.GrammarTimer.Start();
                            break;

                        case "SGM_DIAL_SwitchLanguageToJapanese_YesNo":
                            UnloadGrammar(SGM_DIAL_SwitchLanguageToJapanese_YesNo);
                            MainWindow.GrammarTimer.Stop();
                            LoadGrammar(SGM_DIAL_SwitchLanguageToJapanese);
                            break;

                        case "SGM_FUNC_RegisterMode_YesNo":
                            UnloadGrammar(SGM_FUNC_RegisterMode_YesNo);
                            MainWindow.GrammarTimer.Stop();
                            break;

                        case "SGM_DIAL_Sleep":
                            UnloadGrammar(SGM_DIAL_Sleep);
                            LoadGrammar(SGM_DIAL_Sleep_YesNo);
                            MainWindow.GrammarTimer.Start();
                            break;

                        case "SGM_DIAL_Sleep_YesNo":
                            UnloadGrammar(SGM_DIAL_Sleep_YesNo);
                            MainWindow.GrammarTimer.Stop();
                            LoadGrammar(SGM_DIAL_Sleep);
                            break;

                        //////////////////////////////////////////////

                        //case "SGM_FUNC_CHN_控制电视":
                        //    foreach (Grammar 遥控 in 控制)
                        //    {
                        //        LoadGrammarCN(遥控);
                        //    }
                        //    break;

                        case "SGM_DIAL_英文识别":
                            LoadGrammarCN(SGM_DIAL_英文识别_是否);
                            UnloadGrammarCN(SGM_DIAL_英文识别);
                            break;

                        case "SGM_DIAL_英文识别_是否":
                            UnloadGrammarCN(SGM_DIAL_英文识别_是否);
                            LoadGrammarCN(SGM_DIAL_英文识别);
                            break;

                        case "SGM_DIAL_日文识别":
                            LoadGrammarCN(SGM_DIAL_日文识别_是否);
                            UnloadGrammarCN(SGM_DIAL_日文识别);
                            break;

                        case "SGM_DIAL_日文识别_是否":
                            UnloadGrammarCN(SGM_DIAL_日文识别_是否);
                            LoadGrammarCN(SGM_DIAL_日文识别);
                            break;

                        //case "SGM_FUNC_问问题":
                        //    SRECN.UnloadAllGrammars();
                        //    break;

                        //case "SGM_FUNC_计算":
                        //    SRECN.UnloadAllGrammars();
                        //    break;

                        //case "SGM_FUNC_问问题_完成":
                        //    foreach (Grammar 中文 in 中文语法)
                        //    {
                        //        LoadGrammarCN(中文);
                        //    }
                        //    SRECN.RecognizeAsync(RecognizeMode.Multiple);
                        //    break;

                        ////////////////////////////////////////////////////////////////////////////////////////////

                        case "SGM_FUNC_中国語に切り替え":
                            LoadGrammarJP(SGM_FUNC_中国語に切り替え_確認);
                            UnloadGrammarJP(SGM_FUNC_中国語に切り替え);
                            break;

                        case "SGM_FUNC_中国語に切り替え_確認":
                            UnloadGrammarJP(SGM_FUNC_中国語に切り替え_確認);
                            LoadGrammarJP(SGM_FUNC_中国語に切り替え);
                            break;

                        case "SGM_FUNC_英語に切り替え":
                            LoadGrammarJP(SGM_FUNC_英語に切り替え_確認);
                            UnloadGrammarJP(SGM_FUNC_英語に切り替え);
                            break;

                        case "SGM_FUNC_英語に切り替え_確認":
                            UnloadGrammarJP(SGM_FUNC_英語に切り替え_確認);
                            LoadGrammarJP(SGM_FUNC_英語に切り替え);
                            break;

                        case "SGM_FUNC_おやすみ":
                            LoadGrammarJP(SGM_FUNC_おやすみ_確認);
                            UnloadGrammarJP(SGM_FUNC_おやすみ);
                            break;

                        case "SGM_FUNC_おやすみ_確認":
                            UnloadGrammarJP(SGM_FUNC_おやすみ_確認);
                            LoadGrammarJP(SGM_FUNC_おやすみ);
                            break;

                            //////////////////////////////////////////////







                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                            /*                  case "SGM_FUNC_Skype電話_国":
                                                    UnloadGrammarJP(SGM_FUNC_Skype電話_国);
                                                    LoadGrammarJP(SGM_FUNC_Skype電話_電話番号);
                                                    break;

                                                case "SGM_FUNC_Skype電話_電話番号":

                                                    //if (onetimeGrammar1 == false)
                                                    //{
                                                    //    LoadGrammar(SGM_FUNC_SkypeCall_PhoneCall_YesNo);
                                                    //    onetimeGrammar1 = true;
                                                    //}

                                                    break;

                                                case "SGM_FUNC_Skype電話_電話番号_確認":
                                                    LoadGrammarJP(SGM_FUNC_Skype電話終了);
                                                    //UnloadGrammar(SGM_FUNC_SkypeCall_PhoneNumber);
                                                    UnloadGrammarJP(SGM_FUNC_Skype電話_電話番号_確認);
                                                    //onetimeGrammar1 = false;                   
                                                    break;

                                                case "SGM_FUNC_Skype電話_終了":
                                                    UnloadGrammarJP(SGM_FUNC_Skype電話終了);
                                                    break;
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            */
                    }
                }
            }

            catch
            {
                //SRE_Speech.SpeakAsync("Load Unload Grammar Error");
            }
        }

        void AudioLevelUpdate()
        {
            SRE.AudioLevelUpdated += new EventHandler<AudioLevelUpdatedEventArgs>(SRE_AudioLevelUpdated);
        }

        private void SRE_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            SRE.AudioLevelUpdated -= SRE_AudioLevelUpdated;
        }


        //Speak Progress [function]

        public void SRE_Speech_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            MainWindow.FacialExpressionMark = false;
            try
            {
                if (!MouthMuteMark)
                {
                    FaceLED.Instance.Speaking();
                }
            }
            catch { }
            try
            {
                SRE.RecognizeAsyncCancel();
            }
            catch
            {
                SRE_Speech.SpeakAsync("Speech recognition cancel error");
            }

        }

        public void SRE_Speech_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            MainWindow.FacialExpressionMark = true;
            SRE_Speech.SpeakCompleted -= SRE_Speech_SpeakCompleted;
            //flag_speak_completed = true;
            // MessageBox.Show("complete");
            try
            {
                SRE.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch
            {
                //MessageBox.Show("Speech recogition start error");
            }
            SRE_Speech.SpeakCompleted += SRE_Speech_SpeakCompleted;
            MouthMuteMark = false;
        }



        public void SRECN_Speech_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            try
            {
                if (!MouthMuteMark)
                {
                    FaceLED.Instance.Speaking();
                }
            }
            catch { }
            try
            {
                SRECN.RecognizeAsyncCancel();
            }
            catch
            {
                SRECN_Speech.SpeakAsync("识别引擎取消错误");
            }
        }

        public void SRECN_Speech_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            try
            {
                SRECN.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch
            {
                SRECN_Speech.SpeakAsync("识别引擎打开错误");
            }
        }

        public void SREJP_Speech_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            try
            {
                if (!MouthMuteMark)
                {
                    FaceLED.Instance.Speaking();
                }
            }
            catch { }
            try
            {
                SREJP.RecognizeAsyncCancel();
            }
            catch
            {
                SREJP_Speech.SpeakAsync("認識エンジンをキャンセルしたことが失敗しました。");
            }
        }

        public void SREJP_Speech_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            try
            {
                SREJP.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch
            {
                SREJP_Speech.SpeakAsync("認識エンジンをスタットしたことが失敗しました。");
            }
        }

        public void SRECAN_Speech_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            try
            {
                if (!MouthMuteMark)
                {
                    FaceLED.Instance.Speaking();
                }
            }
            catch { }
            try
            {
                SRECAN.RecognizeAsyncCancel();
            }
            catch
            {
                SRECAN_Speech.SpeakAsync("识别引擎取消错误");
            }
        }

        public void SRECAN_Speech_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            try
            {
                SRECAN.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch
            {
                SRECAN_Speech.SpeakAsync("识别引擎打开错误");
            }
        }


        public Grammar SGM_FUNC_Skype電話 { get; set; }

        public void RegisterModeCompleted()
        {
            foreach (Grammar Greeting in DIAL)
            {
                LoadGrammar(Greeting);
            }
            foreach (Grammar Layer1 in Layerone)
            {
                LoadGrammar(Layer1);
            }
        }

        //Grammar Load Unload Manager Part Start
        public void LoadGrammar(Grammar GrammarA)
        {
            if(!GrammarA.Loaded)
            {
                SRE.LoadGrammar(GrammarA);
            }
            else
            {
                //Cannot load the same grammar twice
            }
        }

        public void UnloadGrammar(Grammar GrammarA)
        {
            if(GrammarA.Loaded)
            {
                SRE.UnloadGrammar(GrammarA);
            }
            else
            {
                //Cannot unload the same grammar twice
            }
        }

        public void LoadGrammarCN(Grammar GrammarA)
        {
            if (!GrammarA.Loaded)
            {
                SRECN.LoadGrammar(GrammarA);
            }
            else
            {
                //Cannot load the same grammar twice
            }
        }

        public void UnloadGrammarCN(Grammar GrammarA)
        {
            if (GrammarA.Loaded)
            {
                SRECN.UnloadGrammar(GrammarA);
            }
            else
            {
                //Cannot unload the same grammar twice
            }
        }

        public void LoadGrammarJP(Grammar GrammarA)
        {
            if (!GrammarA.Loaded)
            {
                SREJP.LoadGrammar(GrammarA);
            }
            else
            {
                //Cannot load the same grammar twice
            }
        }

        public void UnloadGrammarJP(Grammar GrammarA)
        {
            if (GrammarA.Loaded)
            {
                SREJP.UnloadGrammar(GrammarA);
            }
            else
            {
                //Cannot unload the same grammar twice
            }
        }

        public void LoadGrammarCAN(Grammar GrammarA)
        {
            if (!GrammarA.Loaded)
            {
                SRECAN.LoadGrammar(GrammarA);
            }
            else
            {
                //Cannot load the same grammar twice
            }
        }

        public void UnloadGrammarCAN(Grammar GrammarA)
        {
            if (GrammarA.Loaded)
            {
                SRECAN.UnloadGrammar(GrammarA);
            }
            else
            {
                //Cannot unload the same grammar twice
            }
        }
        //Grammar Load Unload Manager Part End
    }
}
