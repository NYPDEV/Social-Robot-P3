using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace SocialRobot.Application.Light
{
  public class LightConnect
    {
        HttpClient client = new HttpClient();

        string name;

        public LightConnect(string name)
        {
            this.name = name;
        }


        public async Task PostMessages(string messages)
        {
            string uri = string.Format(@"http://192.168.2.2/api/1f67f865b6dcb6776c743122ee0a93/lights/3/state", name);
            var res = await client.PutAsync(uri, new ByteArrayContent(Encoding.UTF8.GetBytes(messages)));
        }

        public async Task<string> PostKeys()
        {
            string uri = string.Format(@"http://192.168.2.2/api/1f67f865b6dcb6776c743122ee0a93/lights/3/state", name);
            var res = await client.PostAsync(uri, null);
            return await res.Content.ReadAsStringAsync();
        }
    }
}
