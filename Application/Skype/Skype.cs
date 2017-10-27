using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SKYPE4COMLib;
using System.Threading;

namespace SocialRobot.Application.SkypeApp
{
   public class Skype
    {
        public class UserInfo
        {
            public string Handle;
            public string Name;
        }
        public List<UserInfo> Friends = new List<UserInfo>();
        SKYPE4COMLib.Skype skype = null;
        SKYPE4COMLib.Call CurrentCall = null;
        SKYPE4COMLib.TCallStatus status = TCallStatus.clsUnknown;
        private bool _AutoAnswer = false;
        Function.Text_To_Speech TTS = new Function.Text_To_Speech();
		public static bool CancelMark = true;
		
        public bool AutoAnswer
        {
            set
            {
                if (value && !_AutoAnswer) skype.CallStatus += AutoAnswerFunc;
                else if (!value && _AutoAnswer) skype.CallStatus -= AutoAnswerFunc;
                _AutoAnswer = value;
            }
            get
            {
                return _AutoAnswer;
            }

        }
        public Skype()
        {
            skype = new SKYPE4COMLib.Skype();
            skype.Attach();
            var _Friends = skype.Friends;
            foreach (User f in _Friends)
                Friends.Add(new UserInfo() { Handle = f.Handle, Name = f.FullName });

            skype.CallStatus += (call, status) =>
            {
                CurrentCall = (status == TCallStatus.clsInProgress ||
                                status == TCallStatus.clsRouting ||
                                status == TCallStatus.clsRinging) ? call : null;
                switch(status)
                {
                    case TCallStatus.clsRouting:
                        MainWindow.IsListeningFromUser = true;
                        Function.WitAnalysis.SkypeCallMark = true;
                        break;
                    case TCallStatus.clsRinging:
                        MainWindow.IsListeningFromUser = true;
                        Function.WitAnalysis.SkypeCallMark = true;
                        break;
                    case TCallStatus.clsCancelled:
                        MainWindow.IsListeningFromUser = false;
                        Function.WitAnalysis.SkypeCallMark = false;
                        // if(CancelMark)
                        //{
                        //    CancelMark = false;
                        //    TTS.Speaking("The phone call has been cancelled.");
                        //}
                        //else
                        //{
                        //    CancelMark = true;
                        //}
						TTS.Speaking("The phone call has been cancelled.");
                        break;
                    case TCallStatus.clsRefused:
                        MainWindow.IsListeningFromUser = false;
                        Function.WitAnalysis.SkypeCallMark = false;
                        TTS.Speaking("It seems your call has been refused.");
                        break;
                    case TCallStatus.clsInProgress:
                        MainWindow.IsListeningFromUser = true;
                        Function.WitAnalysis.SkypeCallMark = true;
                        break;
                    case TCallStatus.clsFinished:
                        MainWindow.IsListeningFromUser = false;
                        Function.WitAnalysis.SkypeCallMark = false;
                        break;
                    case TCallStatus.clsFailed:
                        MainWindow.IsListeningFromUser = false;
                        Function.WitAnalysis.SkypeCallMark = false;
                        TTS.Speaking("Sorry I can not make video call for you, please check the network connection.");
                        break;
                    default:
                        status = status;
                        break;
                }
            };
        }

        public string GetHandleFromName(string Name)
        {
            if (Friends.Count != 0)
                return Friends?.Where(item => item.Name == Name)?.First()?.Handle;
            return null;
        }

        public string GetNameFromHandle(string Handle)
        {
            if (Friends.Count != 0)
                return Friends?.Where(item => item.Handle == Handle)?.First()?.Name;
            return null;
        }
        private void AutoAnswerFunc(Call call, TCallStatus status)
        {
            if (status == TCallStatus.clsRinging) call.Answer();
        }

        public void Call(string name)
        {
            new WebBrowser().Navigate("skype:" + name + "?call");
        }
        public void MakeCall(string name)
        {
            //Call call = skype.PlaceCall(name);
            new WebBrowser().Navigate("skype:" + name + "?call&video=true");
            //do
            //{
            //    Thread.Sleep(1);
            //} while (status != TCallStatus.clsInProgress && status != TCallStatus.clsCancelled && status != TCallStatus.clsFailed);
            //if (status == TCallStatus.clsInProgress)
            //{
            //    MainWindow.IsListeningFromUser = true;
            //    Function.WitAnalysis.SkypeCallMark = true;
            //}
            //else
            //{
            //    //TTS.Speaking("Cancel or failed");
            //}
            ////while (call.Status != TCallStatus.clsInProgress) ;
            ////   call.StartVideoSend();
        }

        public void HangUp()
        {
            CurrentCall?.Finish();
        }
    }
}
