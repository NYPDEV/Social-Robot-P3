using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Xml;
using System.Text.RegularExpressions;



namespace SocialRobot.Application
{
    public class WikiApp
    {
        //SpeechSynthesizer SRE_Speech = new SpeechSynthesizer();
        public Function.Speech_Rcognition_Grammar Speak = new Function.Speech_Rcognition_Grammar();





        List<string> answer_list = new List<string>() { };
        string wiki_answer = "";

        public async void AskPresidentOrPrimeMinister_Async(string input)
        {
            Speak.SRE_Speech.SelectVoice("IVONA 2 Amy");
            Speak.SRE_Speech.SpeakAsync("Hold on.");
            await Task.Run(() => askquestion(input));
        }

        private void askquestion(string question_asking)//yihao
        {
            Speak.SRE_Speech.SelectVoice("IVONA 2 Amy");
            //Speak.SRE_Speech.SelectVoice("Microsoft Zira Desktop");
            //question_choice = "knowledge";
            string query = string.Format("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20google.search%20where%20q%20%3D%20%22" + question_asking + "%22&diagnostics=true&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
            XmlDocument wData = new XmlDocument();
            wData.Load(query);
            string google_question_search_url;
            XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
            try
            {
                XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("results");


                google_question_search_url = channel.SelectSingleNode("url").InnerText;
                if (google_question_search_url == "http://en.wikipedia.org/wiki/List_of_premiers_of_China")
                {
                    google_question_search_url = "http://en.wikipedia.org/wiki/Premier_of_the_People%27s_Republic_of_China";
                }

                google_question_search_url = Regex.Replace(google_question_search_url, @"/", "%2F", RegexOptions.IgnoreCase);
                google_question_search_url = Regex.Replace(google_question_search_url, @":", "%3A", RegexOptions.IgnoreCase);
                google_question_search_url = Regex.Replace(google_question_search_url, @"%27", "%2527", RegexOptions.IgnoreCase);
                //switch(question_choice)
                //{ 
                //    case"government": 
                String query_2 = string.Format("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20html%20where%20url%3D'" + google_question_search_url + "'%20and%20xpath%3D'%2F%2Ftable%2F*%5Bcontains(.%2C%22Incumbent%22)%5D%2F%2Fa'&diagnostics=true&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
                XmlDocument search_answer_wikiW = new XmlDocument();
                search_answer_wikiW.Load(query_2);
                channel = search_answer_wikiW.SelectSingleNode("query").SelectSingleNode("results");
                wiki_answer = channel.InnerText;

                wiki_answer = Regex.Replace(wiki_answer, @"<[^>]*>", String.Empty, RegexOptions.IgnoreCase).Trim();
                answer_list = new List<string>() { };


                //
                var allresults_in_wiki = channel.SelectNodes("a");
                foreach (XmlNode node in allresults_in_wiki)
                {
                    if (node.InnerText == "")
                    {

                    }
                    else
                    {
                        var title = node.InnerText;
                        answer_list.Add(title);
                    }
                }
                wiki_answer = answer_list[0];


                Speak.SRE_Speech.SpeakAsync("Is it " + wiki_answer);
            }
            catch
            {
                Speak.SRE_Speech.SpeakAsync("Sorry, I can't find it, can you try another one please?");
            }
            //return wiki_answer;

            ////        break;
            ////            case "knowledge":
            //        String query_3 = string.Format("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20html%20where%20url%3D%27http%3A%2F%2Fen.wikipedia.org%2Fwiki%2F"+question_wiki_single_word+"%27&diagnostics=true&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
            //        XmlDocument search_answer_wikiW_single_word = new XmlDocument();
            //        search_answer_wikiW_single_word.Load(query_3);
            //        XmlNamespaceManager manager_single_word_knowledge = new XmlNamespaceManager(search_answer_wikiW_single_word.NameTable);
            //        manager_single_word_knowledge.AddNamespace("class", "mw-body-content");
            //        XmlNamespaceManager manager_single_word_knowledge_2 = new XmlNamespaceManager(search_answer_wikiW_single_word.NameTable);
            //        manager_single_word_knowledge_2.AddNamespace("class", "mw-content-ltr");
            //      //  channel = search_answer_wikiW_single_word.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("body").SelectSingleNode("div",manager_single_word_knowledge);
            //      //  channel = search_answer_wikiW_single_word.SelectSingleNode("/query/results/body");
            //               //"//span[@class='date'
            //        channel = search_answer_wikiW_single_word.SelectSingleNode("/query/results/body/div[@class='mw-body']");
            //       // XmlNodeList nodes_knowledge = search_answer_wikiW_single_word.SelectNodes("/query/results/body/div[@class='mw-body']");
            //        var allresults_in_wiki_knowledge = channel.SelectNodes("p");
            //        foreach(XmlNode node_knowledge in allresults_in_wiki_knowledge)
            //        {
            //            if(node_knowledge.InnerText=="")
            //            { }
            //        }
            //        wiki_answer = channel.SelectSingleNode("//div[@id='bodyContent'").InnerText;

            //        break;
            //    }
        }


        //private void question_knowledge(string character_question)
        //{
        //    string url = "http://en.wikipedia.org/wiki/" + character_question;
        //            var t = new NReadability.NReadabilityWebTranscoder();
        //            bool b;
        //            string page = t.Transcode(url, out b);

        //            if (b)
        //            {
        //                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        //                doc.LoadHtml(page);


        //             //  var spanclass = doc.DocumentNode.SelectSingleNode("//span[@class='date']").InnerText;
        //           //   spanclass += doc.DocumentNode.SelectSingleNode("//span[@class='time-text']").InnerText;
        //        //     spanclass += doc.DocumentNode.SelectSingleNode("//span[@class='time']").InnerText;
        //                var spanclass2 = doc.DocumentNode.SelectSingleNode("//h1").InnerText;
        //                var mainText = doc.DocumentNode.SelectSingleNode("//div[@id='readInner']").InnerText;
        //               //// mainText = Regex.Replace(mainText, spanclass, String.Empty, RegexOptions.IgnoreCase).Trim();
        //                mainText = Regex.Replace(mainText, spanclass2, String.Empty, RegexOptions.IgnoreCase).Trim();
        //                mainText = mainText.Replace(".", ". ");




        //           }
        //}
    }
}
