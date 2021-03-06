﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Speech.Synthesis;

namespace SocialRobot.Wit.Vitals.NLP
{
    class NLP_Processing
    {
        private string wit_ai_access_token = "TOTHQJODROK5P4RCQCMHVOKJCUXVFUEJ";
        public SpeechSynthesizer SRE_Speech = new SpeechSynthesizer();

        public Task<string> ProcessWrittenText(string text)
        {
            return Task.Run(() =>
            {
                return ProcessText(text);
            });
        }

        public Task<string> ProcessSpokenText(string file)
        {
            return Task.Run(() =>
            {
                return ProcessSpeech(file);
            });
        }

        // Send the text to the wit.ai API
        private string ProcessText(string text)
        {
            // 20140401 tells the wit.ai engine which version to use, in this case the last version
            // before April 1st, 2014
            string url = "https://api.wit.ai/message?v=20160726&q=" + text;

            WebRequest request = WebRequest.Create(url);

            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer " + wit_ai_access_token;

            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string serverResponse = reader.ReadToEnd();

            return serverResponse;
        }

        // Send the wav file to the wit.ai API
        private string ProcessSpeech(string file)
        {
            FileStream filestream = new FileStream(file, FileMode.Open, FileAccess.Read);
            BinaryReader filereader = new BinaryReader(filestream);
            byte[] BA_AudioFile = filereader.ReadBytes((Int32)filestream.Length);
            filestream.Close();
            filereader.Close();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.wit.ai/speech");

            request.Method = "POST";
            request.Headers["Authorization"] = "Bearer " + wit_ai_access_token;
            request.ContentType = "audio/wav";
            request.ContentLength = BA_AudioFile.Length;
            try
            {
                request.GetRequestStream().Write(BA_AudioFile, 0, BA_AudioFile.Length);
            }
            catch
            {

            }

            // Delete the temp file
            try
            {
                File.Delete(file);
            }
            catch
            {
                MessageBox.Show("Unable to delete the temp file!" + Environment.NewLine + "Please do so yourself: " + file);
            }

            // Process the wit.ai response
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader response_stream = new StreamReader(response.GetResponseStream());
                    return response_stream.ReadToEnd();
                }
                else
                {
                    return "Error: " + response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}
