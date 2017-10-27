using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Media.Imaging;

namespace SocialRobot.Application
{
    public class GetWeather
    {
        Function.Text_To_Speech TTS = new Function.Text_To_Speech();

        string ApiKey = "b3c579db28aa6eec025acbaaf78c2976";
        string WeatherCondition = null;

        public static event Action<double> OnWeatherTextUpdate;
        public static event Action<BitmapImage> OnWeatherIconUpdate;

        public void GetTheWeather(string Country, string Day)
        {
            List<string> from_weather = new List<string>() { };
            List<string> to_weather = new List<string>() { };
            List<string> temperature = new List<string>() { };
            List<string> rain = new List<string>() { };
            List<string> symbolID = new List<string>() { };
            string rain1;
            //string query = string.Format("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22" + country + "%22)&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
            //XmlDocument wData = new XmlDocument();
            //wData.Load(query);

            //XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
            //manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");
            //XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");


            //temperature = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value;

            //condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;

            //humidity = channel.SelectSingleNode("yweather:atmosphere", manager).Attributes["humidity"].Value;

            //windspeed = channel.SelectSingleNode("yweather:wind", manager).Attributes["speed"].Value;

            //town = channel.SelectSingleNode("yweather:location", manager).Attributes["city"].Value;


            //condition_weather = new List<string>() { };
            //highest_temperature = new List<string>() { };
            //lowest_temperature = new List<string>() { };


            //var fiveDays = channel.SelectSingleNode("item").SelectNodes("yweather:forecast", manager);
            //foreach (XmlNode node in fiveDays)
            //{
            //    var text = node.Attributes["text"].Value;
            //    var high = node.Attributes["high"].Value;
            //    var low = node.Attributes["low"].Value;

            //    condition_weather.Add(text);
            //    highest_temperature.Add(high);
            //    lowest_temperature.Add(low);
            //}
            #region weather
            string query2 = String.Format("http://api.openweathermap.org/data/2.5/forecast?q=" + Country + "&mode=xml&appid=" + ApiKey);
            XmlDocument wData = new XmlDocument();
            try
            {
                wData.Load(query2);
            }
            catch
            {
                Country = "";
            }


            XmlNode channel = wData.SelectSingleNode("weatherdata");
            XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);



            try
            {
                var forecast = channel.SelectSingleNode("forecast");
                foreach (XmlNode node in forecast)
                {
                    var temp = node.SelectSingleNode("temperature").Attributes["value"].Value;
                    if (Convert.ToDouble(temp) >= 100)
                    {
                        double tempInt = Convert.ToDouble(temp) / 10;
                        temp = tempInt.ToString();
                    }
                    temperature.Add(temp);

                    //check if there is rain
                    try
                    {
                        var pep = node.SelectSingleNode("precipitation").Attributes["type"].Value;
                        rain.Add(pep);
                    }

                    catch
                    {
                        rain1 = "No rain";
                        rain.Add(rain1);
                    }
                    //symbol number
                    var ID = node.SelectSingleNode("symbol").Attributes["number"].Value;
                    symbolID.Add(ID);
                }
                try
                {
                    if (Day == "today" || Day == "Today")
                    {

                        double temp = 0;
                        temp = Convert.ToDouble(temperature[1]);

                        if (rain[1] == "No rain")
                        {
                            weathericon(symbolID[1]);
                            TTS.Speaking("the weather condition in " + Country + " for " + Day + " is " + (int)temp + " degrees celsius with " + WeatherCondition); Country = "";
                        }
                        else
                        {
                            weathericon(symbolID[1]);
                            TTS.Speaking("the weather condition in " + Country + " for " + Day + " is " + (int)temp + " degrees celsius and " + WeatherCondition); Country = "";
                        }
                        OnWeatherTextUpdate?.Invoke(temp);


                    }

                    else if (Day == "tomorrow" || Day == "Tomorrow")
                    {
                        double temp = 0;
                        temp = Convert.ToDouble(temperature[8]);
                        if (rain[8] == "No rain")
                        {
                            weathericon(symbolID[8]);
                            TTS.Speaking("the weather condition in " + Country + " for " + Day + " is " + (int)temp + " degrees celsius. with " + WeatherCondition); Country = "";
                        }
                        else
                        {
                            weathericon(symbolID[8]);
                            TTS.Speaking("the weather condition in " + Country + " for " + Day + " is " + (int)temp + " degrees celsius and " + WeatherCondition); Country = "";
                        }
                        OnWeatherTextUpdate?.Invoke(temp);
                    }
                    else if (Day == "goingout")
                    {
                        try
                        {// double temp = 0;
                         //    temp = Convert.ToDouble(temperature[0]);
                            if (rain[1] == "No rain")
                            {

                                weathericon(symbolID[0]);
                                TTS.Speaking("Bon voyage.");
                            }
                            else
                            {
                                weathericon(symbolID[0]);
                                TTS.Speaking("Bon voyage, By the way please remember to take an umbrella."); Country = "";
                            }
                        }
                        catch

                        {
                            TTS.Speaking("Bon vovage!");
                        }

                    }
                    else if (Day == "now")
                    {
                        double temp = 0;
                        temp = Convert.ToDouble(temperature[0]);

                        if (rain[1] == "No rain")
                        {
                            weathericon(symbolID[1]);
                            TTS.Speaking("the weather condition in " + Country + " for " + Day + " is " + (int)temp + " degrees celsius with " + WeatherCondition); Country = "";
                        }
                        else
                        {
                            weathericon(symbolID[1]);
                            TTS.Speaking("the weather condition in " + Country + " for " + Day + " is " + (int)temp + " degrees celsius and " + WeatherCondition + " now."); Country = "";
                        }
                        OnWeatherTextUpdate?.Invoke(temp);
                    }
                }
                catch
                {
                    TTS.Speaking("It seems I have trouble obtaining the weather forecast for " + Country + ". please try another city"); Country = "";
                }
            }
            catch
            {
                if (Day == "now")
                {
                    TTS.Speaking("Bon voyage!");
                }
                else
                {
                    if (Country != "")
                    {
                        TTS.Speaking("sorry I can not get the weather condition in " + Country + " now."); Country = "";
                    }
                    else
                    {
                        TTS.Speaking("sorry I did not hear clearly, please try again!");
                    }
                }
            }

            #endregion weather
        }

        BitmapImage WeatherBit;

        public void weathericon(string icon)
        {
            WeatherBit = new BitmapImage();

            switch (icon)
            {
                #region thunderstorm
                case "200":
                    WeatherCondition = "there will be a thunderstorm with light rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "201":
                    WeatherCondition = "there will be a thunderstorm with rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "202":
                    WeatherCondition = "there will be a thunderstorm with heavy rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\5.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "210":
                    WeatherCondition = "there will be a light thunderstorm";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "211":
                    WeatherCondition = "there will be a thunderstorm";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "212":
                    WeatherCondition = "there will be a heavy thunderstorm";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "221":
                    WeatherCondition = "there will be a ragged thunderstorm";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "230":
                    WeatherCondition = "there will be a thunderstorm with light drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "231":
                    WeatherCondition = "there will be a thunderstorm with drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "232":
                    WeatherCondition = "there will be a thunderstorm with heavy drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\PM Thunderstorms.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;
                #endregion thunderstorm
                #region drizzle
                case "300":
                    WeatherCondition = "there will be a light intensity drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "301":
                    WeatherCondition = "there will be a drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "302":
                    WeatherCondition = "there will be a heavy intensity drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "310":
                    WeatherCondition = "there will be a light intensity drizzle rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "311":
                    WeatherCondition = "there will be a drizzle rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "312":
                    WeatherCondition = "there will be a heavy intensity drizzle rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "313":
                    WeatherCondition = "there will be a shower rain and drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "314":
                    WeatherCondition = "there will be a heavy shower rain and drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "321":
                    WeatherCondition = "there will be a shower drizzle";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;
                #endregion drizzle
                #region rain
                case "500":
                    WeatherCondition = "there will be a light rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "501":
                    WeatherCondition = "there will be a moderate rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "502":
                    WeatherCondition = "there will be a heavy intensity rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\5.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "503":
                    WeatherCondition = "there will be a very heavy rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\5.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "504":
                    WeatherCondition = "there will be a extreme rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\5.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "511":
                    WeatherCondition = "there will be a freezing rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\11.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "520":
                    WeatherCondition = "there will be a light intensity shower rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "521":
                    WeatherCondition = "there will be a shower rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "522":
                    WeatherCondition = "there will be a heavy intensity shower rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\5.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "531":
                    WeatherCondition = "there will be a ragged shower rain";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\5.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;
                #endregion rain
                #region snow
                case "600":
                    WeatherCondition = "there will be a light snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\11.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "601":
                    WeatherCondition = "there will be a snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\10.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "602":
                    WeatherCondition = "there will be a heavy snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\10.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "611":
                    WeatherCondition = "there will be a sleet";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\10.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "612":
                    WeatherCondition = "there will be a shower sleet";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\10.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "615":
                    WeatherCondition = "there will be a light rain and snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\11.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "616":
                    WeatherCondition = "there will be a rain and snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\11.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "620":
                    WeatherCondition = "there will be a light shower snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Showers.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "621":
                    WeatherCondition = "there will be a shower snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\10.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "622":
                    WeatherCondition = "there will be a heavy shower snow";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\10.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;
                #endregion snow
                #region atmosphere
                case "701":
                    WeatherCondition = "there will be a mist";
                    break;

                case "711":
                    WeatherCondition = "it will be smoky";
                    break;

                case "721":
                    WeatherCondition = "it will be hazy haze";
                    break;

                case "731":
                    WeatherCondition = "there will be sand, dust whirls";
                    break;

                case "741":
                    WeatherCondition = "it will be foggy";
                    break;

                case "751":
                    WeatherCondition = "It will be sandy ";
                    break;

                case "761":
                    WeatherCondition = "it will be dusty";
                    break;

                case "762":
                    WeatherCondition = "there will be volcanic ash";
                    break;

                case "771":
                    WeatherCondition = "there will be squalls";
                    break;

                case "781":
                    WeatherCondition = "there will be a tornado";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\16.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;
                #endregion atmosphere
                #region clouds
                case "800":
                    WeatherCondition = "clear sky";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Sunny.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "801":
                    WeatherCondition = "few clouds";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Partly Cloudy.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "802":
                    WeatherCondition = "scattered clouds";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Partly Cloudy.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "803":
                    WeatherCondition = "broken clouds";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Fair.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "804":
                    WeatherCondition = "overcast clouds";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\Partly Cloudy.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;
                #endregion clouds
                #region extreme
                case "900":
                    WeatherCondition = "there will be a tornado";
                    WeatherBit.BeginInit();
                    WeatherBit.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"weather-icons-set\16.png");
                    WeatherBit.EndInit();
                    OnWeatherIconUpdate?.Invoke(WeatherBit);
                    break;

                case "901":
                    WeatherCondition = "there will be a tropical storm";
                    break;

                case "902":
                    WeatherCondition = "there will be a hurricane";
                    break;

                case "903":
                    WeatherCondition = "the weather will be extremely cold";
                    break;

                case "904":
                    WeatherCondition = "the weather will be extremely hot";
                    break;

                case "905":
                    WeatherCondition = "the weather will be extremely windy";
                    break;

                case "906":
                    WeatherCondition = "there will be hail";
                    break;
                #endregion extreme
                #region additional
                case "951":
                    break;

                case "952":
                    break;

                case "953":
                    break;

                case "954":
                    break;

                case "955":
                    break;

                case "956":
                    break;

                case "957":
                    break;

                case "958":
                    break;

                case "959":
                    break;

                case "960":
                    break;

                case "961":
                    break;

                case "962":
                    break;
                    #endregion additional

            }
            MainWindow.WeatherIconTimer.Start();
        }
    }
}
