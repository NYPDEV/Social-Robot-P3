using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Data;

namespace SocialRobot.Wit.Vitals.NLP
{
    static class Post_NLP_Processing
    {
        public static Objects.O_NLP.RootObject ParseData(string text)
        {

            // HTML-decode the string, in case it has been HTML encoded
            string jsonText = System.Web.HttpUtility.HtmlDecode(text);
            jsonText = jsonText;
            //Since object is reserved, put a _ in front
            jsonText = jsonText.Replace("object", "_object");
            //jsonText = jsonText.Replace("outcomes\" : [ {", "outcome\" : {");
            //jsonText = jsonText.Replace("outcomes", "outcome");
            jsonText = jsonText.Replace(" [ ", " ");
            jsonText = jsonText.Replace(" ]", " ");
            jsonText = jsonText;
            //Deserialize into our class
            var rootObject = JsonConvert.DeserializeObject<Objects.O_NLP.RootObject>(jsonText);

            return rootObject;

        }
    }
}
