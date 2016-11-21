using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKYPE4COMLib;

namespace SocialRobot.Application.SkypeApp
{
    class SkypeCallClass
    {
        private string cmd;
        public Skype sp;
        public SKYPE4COMLib.Command sp_Command;
        private int timeout;
        private bool UsingPolicyPower;

        public SkypeCallClass(string Cmd, int Timeout)
        {
            this.sp = (Skype)Activator.CreateInstance(System.Type.GetTypeFromCLSID(new Guid("830690FC-BF2F-47A6-AC2D-330BCB402664")));
            this.sp_Command = (SKYPE4COMLib.Command)Activator.CreateInstance(System.Type.GetTypeFromCLSID(new Guid("2DBCDA9F-1248-400B-A382-A56D71BF7B15")));
            this.cmd = Cmd;
            this.timeout = Timeout;
          
        }

        public SkypeCallClass(string Cmd, int Timeout, string UsingPower)
        {
            this.sp = (Skype)Activator.CreateInstance(System.Type.GetTypeFromCLSID(new Guid("830690FC-BF2F-47A6-AC2D-330BCB402664")));
            this.sp_Command = (SKYPE4COMLib.Command)Activator.CreateInstance(System.Type.GetTypeFromCLSID(new Guid("2DBCDA9F-1248-400B-A382-A56D71BF7B15")));
            this.cmd = Cmd;
            this.timeout = Timeout;
            if (UsingPower == "AdminPower")
            {
                this.UsingPolicyPower = true;
            }
        }

        public void PlaceACall()
        {
            string str = "call " + this.cmd;
            this.sp_Command.Command = str;
            if (this.timeout <= 0x7530)
            {
                this.sp_Command.Timeout = this.sp.Timeout;
            }
            else
            {
                this.sp_Command.Timeout = this.sp.Timeout;
            }
            this.sp.SendCommand(this.sp_Command);
        }

    }
}
