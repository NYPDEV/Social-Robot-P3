using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialRobot.Application.Light
{
    public class LightControl
    {
        LightConnect Light = new LightConnect("1f67f865b6dcb6776c743122ee0a93");
        string messages;

        public async void control(string command)
        {
            if (command == "on")
            {
                try
                {
                    messages = "{\"on\":" + "true" + ", \"sat\":254, \"bri\":" + "100" + ",\"hue\":" + "31000" + "}";

                    await Light.PostMessages(messages);
                }

                catch
                {

                }
            }
            else if (command == "off")
            {
                try
                {
                    messages = "{\"on\":" + "false" + ", \"sat\":254, \"bri\":" + "100" + ",\"hue\":" + "31000" + "}";
                    await Light.PostMessages(messages);
                }
                catch
                {

                }
            }
        }
    }
}
