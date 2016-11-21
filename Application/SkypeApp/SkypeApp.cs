using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SocialRobot.Application.SkypeApp
{
   public class SkypeApp
    {
        private void CallCommand(string CallCmd)
        {
            SkypeCallClass scc = new SkypeCallClass(CallCmd, 5000000);
            scc.PlaceACall();
        }

        public void MakeCall(string Number_ID)
        {
            CallCommand(Number_ID);
           
        }

    

        public void SendSMS(string Number, string SMS)
        {
            SkypeMessageClass SkypeSMS = new SkypeMessageClass(Number, SMS);
            SkypeSMS.SendSMS();
        }

       public void SnedMessage(string ID,string message)
        {
            SkypeMessageClass Skypemessage = new SkypeMessageClass(ID, message);
            Skypemessage.SendSkypeMessage(ID,message);
        }

       

    }
}
