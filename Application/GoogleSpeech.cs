using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;
using System.Threading;

namespace SocialRobot.Application
{
    public class GoogleSpeech
    {
        static  public event Action<string, SocialRobot.Function.UIHelper.SpeechState> SpeechReceived;
        static SpeechClient speech = null;
        static SpeechClient.StreamingRecognizeStream streamingCall = null;
        static private object writeLock = new object();
        static private bool writeMore = true;
        static bool recording = true;
        private static bool test;

        //public GoogleSpeech()
        //{


        //}

        static public async void init()
        {
            object obj = await StreamingMicRecognizeAsync();
        }
        public void RecognitionPause()
        {
            recording = false;
        }
        public void RecognitionResume()
        {
            recording = true;
        }

        static public void WaveInDataAvailable(object sender, SocialRobot.Function.Mic.WaveInEventArgs e)
        {
            CancellationTokenSource cancelationTokenSource = new CancellationTokenSource();
            CancellationToken cancelationToken = cancelationTokenSource.Token;

            if (recording)
            {
                lock (writeLock)
                {
                    if (!writeMore) return;
                    try
                    {
                        //if (e.BytesRecorded > 3200) return;
                       // Console.WriteLine("number of bytes" + e.BytesRecorded);
                        streamingCall?.WriteAsync(
                       new StreamingRecognizeRequest()
                       {
                           AudioContent = Google.Protobuf.ByteString
                               .CopyFrom(e.Buffer, 0, e.BytesRecorded)
                       }).Wait();
                    }
                    catch (AggregateException ex)
                    {
                        if (ex.InnerException.HResult == -2146233088)
                        {
                            streamingCall.WriteCompleteAsync();
                            Console.Write(ex.InnerException.Message);
                            init();
                        }
                    }
                   
                }
            }
        }


        static async Task<object> StreamingMicRecognizeAsync()
        {
            speech = SpeechClient.Create();
            streamingCall = speech.StreamingRecognize();
            // Write the initial request with the config.
            await streamingCall.WriteAsync(
                new StreamingRecognizeRequest()
                {
                    StreamingConfig = new StreamingRecognitionConfig()
                    {
                        Config = new RecognitionConfig()
                        {
                            Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                            SampleRateHertz = 16000,
                            LanguageCode = "en",
                        },
                        InterimResults = true,
                    }
                });

            // Invoke responses as they arrive.
            Task invokeResponses = Task.Run(async () =>
            {
                bool x = await streamingCall.ResponseStream.MoveNext(
                    default(CancellationToken));
                while (x)
                {
                    foreach (var result in streamingCall.ResponseStream
                        .Current.Results)
                    {
                        foreach (var alternative in result.Alternatives)
                        {
                            if (result.IsFinal) SpeechReceived?.Invoke(alternative.Transcript, SocialRobot.Function.UIHelper.SpeechState.End);
                            else SpeechReceived?.Invoke(alternative.Transcript, SocialRobot.Function.UIHelper.SpeechState.Partial);
                        }
                    }
                    x = await streamingCall.ResponseStream.MoveNext(
                        default(CancellationToken));
                }
            });


            ManualResetEvent _suspendEvent = new ManualResetEvent(true);
            _suspendEvent.WaitOne(Timeout.Infinite);

            return 0;
        }
    }
}
