using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NAudio.Wave;
using System.Windows.Threading;

namespace SocialRobot.Function
{
    class Record
    {
        private IWaveIn waveIn;
        private WaveFileWriter wfWriter;
        private String outputFileName;
        private readonly String outputFolder;

        public bool InvokeRequired { get; }

        public Record()
        {
            outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory+"Wav");//wav文件储存路径
        }

        private void CreateWaveInDevice()
        {
            waveIn = new WaveIn();
            waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;
        }

        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (this.InvokeRequired)
            {
                //Debug.WriteLine("Data Available");
                //Dispatcher.BeginInvoke(new EventHandler<StoppedEventArgs>(OnRecordingStopped), sender, e);
            }
            else
            {
                //Debug.WriteLine("Flushing Data Available");
                wfWriter.Write(e.Buffer, 0, e.BytesRecorded);
                int secondsRecorded = (int)(wfWriter.Length / wfWriter.WaveFormat.AverageBytesPerSecond);

            }
        }

        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                //Dispatcher.BeginInvoke(new EventHandler<StoppedEventArgs>(OnRecordingStopped), sender, e);
            }
            else
            {
                FinalizeWaveFile();

                if (e.Exception != null)
                {
                    System.Windows.MessageBox.Show(String.Format("A problem was encountered during recording {0}",
                                                  e.Exception.Message));
                }
            }
        }

        private void CleanUp()
        {
            if (waveIn != null)
            {
                waveIn.Dispose();
                waveIn = null;
            }
            FinalizeWaveFile();
        }

        private void FinalizeWaveFile()
        {
            if (wfWriter != null)
            {
                wfWriter.Dispose();
                wfWriter = null;
            }
        }

        public void Recordstart()
        {
            CleanUp();
            if (waveIn == null)
            {
                CreateWaveInDevice();
            }
            outputFileName = "Question.wav";
            wfWriter = new WaveFileWriter(Path.Combine(outputFolder, outputFileName), waveIn.WaveFormat);
            waveIn.StartRecording();
        }

        public void Recordstop()
        {
            //Debug.WriteLine("StopRecording");
            if (waveIn != null) waveIn.StopRecording();
        }
    }
}
