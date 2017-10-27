using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Xml;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Speech.Recognition;


namespace SocialRobot.Application
{
   

    public class ReadNews
    {
        public Function.Text_To_Speech TTS = new Function.Text_To_Speech();

        string NewsTitleOrDescription = "";
        public bool flag_StartNewsReading = false;
        int lhzb_choice = 0;
        bool flag_speak_completed = true;
        public static bool NewsTypeSuccess = true;

        public static int i_news = 0;
        public int i_news_CN = 0;
        public void NewsReadingFunction_Eng(string NewsType)
        {
            string query_news = string.Format("http://www.straitstimes.com/news/" + NewsType + "/rss.xml");
            XmlDocument newsdata = new XmlDocument();
            NewsTypeSuccess = true;
            try
            {
                newsdata.Load(query_news);

                XmlNamespaceManager manager_news = new XmlNamespaceManager(newsdata.NameTable);
                manager_news.AddNamespace("media", "http://search.yahoo.com/mrss/");
                XmlNode channel_news = newsdata.SelectSingleNode("rss").SelectSingleNode("channel");
                XmlNodeList nodes_news = newsdata.SelectNodes("/rss/channel/item", manager_news);

                String[,] temprssnewsdata = new String[100, 3];
                System.Xml.XmlNode rssnewsnode;
                rssnewsnode = nodes_news.Item(i_news).SelectSingleNode("title");
                string NewsTitle = null;
                string NewsDescription = null;

                if (rssnewsnode != null && i_news < 30)
                {
                    temprssnewsdata[i_news, 0] = rssnewsnode.InnerText;

                    NewsTitleOrDescription = temprssnewsdata[i_news, 0];

                    NewsTitle = NewsTitleOrDescription;
                    rssnewsnode = nodes_news.Item(i_news).SelectSingleNode("description");
                    try
                    {
                        temprssnewsdata[i_news, 1] = rssnewsnode.InnerText;
                    }
                    catch
                    {
                        i_news++;
                    }
                    NewsTitleOrDescription = " " + temprssnewsdata[i_news, 1] + " ";
                    for (int i = 0; i < NewsTitleOrDescription.Length; i++)
                    {
                        if (NewsTitleOrDescription.Substring(i, 1) == "-")
                        {
                            NewsDescription = NewsTitleOrDescription.Substring(i + 2);
                            break;
                        }
                    }
                    if (NewsTitleOrDescription != " .")
                    {
                        NewsTitleOrDescription = NewsTitle + " \n" + NewsDescription;
                        TTS.Speaking("Here's some " + NewsType + " news. " + NewsTitleOrDescription);
                        NewsTitleOrDescription = null;
                        i_news++;
                        MainWindow.ReadNewsTimer.Start();
                    }
                    else
                    {
                        i_news++;
                        NewsReadingFunction_Eng(NewsType);
                    }
                }
            }
            catch
            {
                if (NewsType != null)
                {
                    TTS.Speaking("Sorry, I cannot get " + NewsType + " news now.");
                }
                else if (i_news >= 30)
                {
                    TTS.Speaking("I have read all of " + NewsType + " news today.");
                }
                else
                {
                    TTS.Speaking("Sorry I didn't hear clearly, can you repeat the news option again?");
                }
            }
        }

        public void NewsReadingFunction_Cn()
        {
            TTS.TextToSpeechCN.SelectVoice("Microsoft Huihui Desktop");
            string query_news = string.Format("http://www.zaobao.com/ssi/rss/news.xml");


            NewsTitleOrDescription = "<speak version=\"1.0\"";
            NewsTitleOrDescription += " xmlns=\"http://www.w3.org/2001/10/synthesis\"";
            NewsTitleOrDescription += " xml:lang=\"zh-cn\">";


            NewsTitleOrDescription += "<prosody rate=\"default\">";


            NewsTitleOrDescription += "让我们听听今天联合早报有什么新闻吧";

            NewsTitleOrDescription += "</prosody><break time=\"1s\"/>";
            NewsTitleOrDescription += "</speak>";

            //flag_speak_completed = false;
            //synthesizer.SpeakSsmlAsync(str);
            
            lhzb_choice = 4;
            //////// you can change to any channel
            switch (lhzb_choice)
            {
                case 0:

                    query_news = string.Format("http://www.zaobao.com/ssi/rss/news.xml");
                    //////即时报道
                    break;
                case 1:
                    query_news = string.Format("http://www.zaobao.com/ssi/rss/zg.xml");
                    break;
                /////中国新闻
                case 2:
                    query_news = string.Format("http://www.zaobao.com/ssi/rss/gj.xml");
                    break;
                ////国际新闻
                case 3:
                    query_news = string.Format("http://www.zaobao.com/ssi/rss/yx.xml");

                    break;
                /////东南亚新闻
                case 4:
                    query_news = string.Format("http://www.zaobao.com/ssi/rss/sp.xml");

                    break;
                /////新加坡新闻
                case 5:
                    query_news = string.Format("http://www.zaobao.com/ssi/rss/yl.xml");

                    break;
                /////今日观点

                case 6:
                    query_news = string.Format("http://www.zaobao.com/ssi/rss/cz.xml");

                    break;
                //////中国财经

                case 7:
                    query_news = string.Format("http://www.zaobao.com/ssi/rss/cs.xml");

                    break;
                /////狮城财经

                case 8:
                    query_news = string.Format("http://www.zaobao.com/ssi/rss/cg.xml");

                    break;
                /////全球财经

                case 9:
                    query_news = string.Format("http://www.zaobao.com/ssi/rss/ty.xml");

                    break;
                    ////早报体育

            }
            XmlDocument newsdata = new XmlDocument();
            newsdata.Load(query_news);
            XmlNamespaceManager manager_news = new XmlNamespaceManager(newsdata.NameTable);
            manager_news.AddNamespace("media", "http://search.yahoo.com/mrss/");
            XmlNode channel_news = newsdata.SelectSingleNode("rss").SelectSingleNode("channel");
            XmlNodeList nodes_news = newsdata.SelectNodes("/rss/channel/item", manager_news);

            String[,] temprssnewsdata = new String[100, 3];

            System.Xml.XmlNode rssnewsnode;
            rssnewsnode = nodes_news.Item(i_news_CN).SelectSingleNode("title");
            if (rssnewsnode != null && flag_StartNewsReading == true && flag_speak_completed == true)
            {
                temprssnewsdata[i_news_CN, 0] = rssnewsnode.InnerText;

                NewsTitleOrDescription = "<speak version=\"1.0\"";
                NewsTitleOrDescription += " xmlns=\"http://www.w3.org/2001/10/synthesis\"";
                NewsTitleOrDescription += " xml:lang=\"zh-cn\">";
                NewsTitleOrDescription += " <voice gender=\"male\">";


                NewsTitleOrDescription += "<prosody rate=\"default\">";



                NewsTitleOrDescription += temprssnewsdata[i_news_CN, 0];

                NewsTitleOrDescription += "</prosody><break time=\"1s\"/>";
                NewsTitleOrDescription += "</voice>";
                NewsTitleOrDescription += "</speak>";
                Function.Text_To_Speech.TextToSpeech.SpeakSsmlAsync(NewsTitleOrDescription);
                rssnewsnode = nodes_news.Item(i_news_CN).SelectSingleNode("description");

                #region desc_cn
                if (rssnewsnode != null && flag_StartNewsReading == true && flag_speak_completed == true)
                {

                    temprssnewsdata[i_news_CN, 1] = rssnewsnode.InnerText;
                    temprssnewsdata[i_news_CN, 1] = Regex.Replace(temprssnewsdata[i_news_CN, 1], @"<[^>]*>", String.Empty, RegexOptions.IgnoreCase).Trim();
                    NewsTitleOrDescription = "<speak version=\"1.0\"";
                    NewsTitleOrDescription += " xmlns=\"http://www.w3.org/2001/10/synthesis\"";
                    NewsTitleOrDescription += " xml:lang=\"zh-cn\">";
                    NewsTitleOrDescription += " <voice gender=\"male\">";


                    NewsTitleOrDescription += "<prosody rate=\"default\">";

                    NewsTitleOrDescription += temprssnewsdata[i_news_CN, 1];

                    NewsTitleOrDescription += "</prosody><break time=\"1s\"/>";
                    NewsTitleOrDescription += "</voice>";
                    NewsTitleOrDescription += "</speak>";

                    Function.Text_To_Speech.TextToSpeech.SpeakSsmlAsync(NewsTitleOrDescription);
                    i_news_CN++;
                    #endregion
                }
            }
            MainWindow.ReadNewsTimer.Start();
        }
    }
}
