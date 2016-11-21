using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Speech.Synthesis;


namespace SocialRobot.Application
{
   public class WeatherApp
    {
       public Function.Speech_Rcognition_Grammar Speak = new Function.Speech_Rcognition_Grammar();

        string temperature;
        string condition;
        string humidity;
        string windspeed;
        string town;
        
        List<string> condition_weather = new List<string>() { };
        List<string> highest_temperature = new List<string>() { };
        List<string> lowest_temperature = new List<string>() { };

        public void getweather(string country,string day)                   
        {
            string query = string.Format("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22" + country + "%22)&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
            XmlDocument wData = new XmlDocument();
            wData.Load(query);

            XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
            manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");
            XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");


            temperature = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value;

            condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;

            humidity = channel.SelectSingleNode("yweather:atmosphere", manager).Attributes["humidity"].Value;

            windspeed = channel.SelectSingleNode("yweather:wind", manager).Attributes["speed"].Value;

            town = channel.SelectSingleNode("yweather:location", manager).Attributes["city"].Value;

            condition_weather = new List<string>() { };
            highest_temperature = new List<string>() { };
            lowest_temperature = new List<string>() { };


            var fiveDays = channel.SelectSingleNode("item").SelectNodes("yweather:forecast", manager);
            foreach (XmlNode node in fiveDays)
            {
                var text = node.Attributes["text"].Value;
                var high = node.Attributes["high"].Value;
                var low = node.Attributes["low"].Value;

                condition_weather.Add(text);
                highest_temperature.Add(high);
                lowest_temperature.Add(low);
            }

            if (day == "today")
            {
                double temp_temperature_2 = 0;
                temp_temperature_2 = Convert.ToDouble(temperature);
                temp_temperature_2 = ((temp_temperature_2 - 32) / 1.8);
                Speak.srespeech.SelectVoice("Microsoft Zira Desktop");
              Speak.srespeech.SpeakAsync("The weather condition in " + country + ", is " + condition + ", at " + (int)temp_temperature_2 + " degrees celsius. The wind speed is " + windspeed + " miles per hour, and humidity is " + humidity + ".");
            }
            else if(day == "Tomorrow")
            {
                double temp_tfhigh = 0;
                double temp_tflow = 0;
                

                temp_tfhigh = Convert.ToDouble(highest_temperature[1]);
                temp_tflow = Convert.ToDouble(lowest_temperature[1]);

                temp_tfhigh = ((temp_tfhigh - 32) / 1.8);
                temp_tflow = ((temp_tflow - 32) / 1.8);
                Speak.srespeech.SelectVoice("Microsoft Zira Desktop");
              Speak.srespeech.SpeakAsync("Tomorrow's weather condition in " + country + " is " + condition_weather[1] + ", with the temperature range between " + (int)temp_tfhigh + ", to " + (int)temp_tflow + " degree celsius.");
            }
        }
    }
}
