using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKYPE4COMLib;
using System.Windows;

namespace SocialRobot.Application.SkypeApp
{
    
    class SkypeMessageClass
    {
        private NumberCount nc;
        private string PhoneNumber;
        private string SMS_Message;
        private bool UsingPolicyPower;
        Skype SkypeMessage = new Skype();
        Function.Speech_Rcognition_Grammar SRG = new Function.Speech_Rcognition_Grammar();

        public SkypeMessageClass(string Number, string Msg)
        {
            this.PhoneNumber = Number;
            this.SMS_Message = Msg;
            this.UsingPolicyPower = false;
        }

        public SkypeMessageClass(string Number, string Msg, string UsingPower)
        {
            this.PhoneNumber = Number;
            this.SMS_Message = Msg;
            if (UsingPower == "NobelHsieh0975662613")
            {
                this.UsingPolicyPower = true;
            }
            else
            {
                this.UsingPolicyPower = false;
            }
        }

        public void SendSMS()
        {
            ISmsMessage message = ((Skype)Activator.CreateInstance(System.Type.GetTypeFromCLSID(new Guid("830690FC-BF2F-47A6-AC2D-330BCB402664")))).CreateSms(TSmsMessageType.smsMessageTypeOutgoing, this.PhoneNumber);
            message.Body = this.SMS_Message;
            message.Send();
            SRG.SRE_Speech.SpeakAsync("Message sent");
        }

        private enum NumberCount
        {
            Only,
            NotOnly
        }

        public void SendSkypeMessage(string ID, string message)
        {
            SkypeMessage.SendMessage(ID,message);
        }
    }
}
