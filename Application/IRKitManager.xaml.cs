using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;

namespace SocialRobot.Application
{
    /// <summary>
    /// Interaction logic for IRKitManager.xaml
    /// </summary>
    public partial class IRKitManager : Window
    {
        List<string> data = new List<string>();
        string path = null;
        object lockObj = new object();
        Function.Text_To_Speech TTS = new Function.Text_To_Speech();
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public IRKitManager()
        {
            InitializeComponent();
            listBox.SelectionChanged += (o, e) =>
            {
                if (listBox.SelectedItem != null)
                {
                    Name_Box.Text = listBox.SelectedItem as string;
                    string signalStr = data.Where(d => d.Substring(0, d.IndexOf('@')) == listBox.SelectedItem as string).FirstOrDefault();
                    if (signalStr == null)
                        MessageBox.Show("Error!");
                    else
                        Signal.Text = signalStr.Substring(signalStr.IndexOf('@') + 1, signalStr.Length - signalStr.IndexOf('@') - 1);
                }
            };

            path = AppDomain.CurrentDomain.BaseDirectory + "Database/RemoteControl.txt";
            data = File.ReadAllLines(path, Encoding.UTF8).ToList();
            listBox.Items.Clear();

            foreach (string d in data)
            {
                listBox.Items.Add(d.Substring(0, d.IndexOf('@')));
            }
        }

        private async void GET_Click(object sender, RoutedEventArgs e)
        {
            GET.IsEnabled = false;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add(@"X-Requested-With", "curl");
            client.Timeout = TimeSpan.FromSeconds(5);
            HttpResponseMessage response = null;
            try
            {
                response = await client.GetAsync(new Uri(@"http://" + ConfigurationManager.AppSettings["IRKit_IP"].ToString() + "/messages"));
                if (response.IsSuccessStatusCode)
                    Signal.Text = await response.Content.ReadAsStringAsync();
                else
                    Signal.Text = "Network Error";
            }
            catch (Exception ex)
            {
                Signal.Text = ex.Message + "\n" + ex.InnerException.Message;
                GET.IsEnabled = true;
                return;
            }
            GET.IsEnabled = true;
        }

        private async void POST_Click(object sender, RoutedEventArgs e)
        {
            POST.IsEnabled = false;
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(Signal.Text, Encoding.UTF8, "text/plain");
            client.DefaultRequestHeaders.Add(@"X-Requested-With", "curl");
            client.Timeout = TimeSpan.FromSeconds(5);
            HttpResponseMessage response = null;
            try
            {
                response = await client.PostAsync(new Uri(@"http://" + ConfigurationManager.AppSettings["IRKit_IP"].ToString() + "/messages"), content);
                if (!response.IsSuccessStatusCode)
                    Signal.Text = "Network Error";
            }
            catch (TaskCanceledException ex)
            {
                Signal.Text = "Time Out";
                POST.IsEnabled = true;
                return;
            }
            catch (Exception ex)
            {

                Signal.Text = ex.Message;
                POST.IsEnabled = true;
                return;
            }
            POST.IsEnabled = true;
        }

        private void ADD_Click(object sender, RoutedEventArgs e)
        {
            if (Signal.Text == null ||
                Signal.Text == "" ||
                Name_Box.Text == "" ||
                Name_Box == null)
            {
                MessageBox.Show("Name or Singnal cannot be blank");

            }
            else if (data.Any(d => d.Substring(0, d.IndexOf('@')) == Name_Box.Text))
            {
                MessageBox.Show("Name cannot be duplicated");
            }
            else if (Name_Box.Text.Contains('@'))
            {
                MessageBox.Show("Name cannot contains @");
            }
            else
            {
                data.Add(Name_Box.Text + "@" + Signal.Text);
                listBox.Items.Add(Name_Box.Text);
            }
        }


        private void Open_Click(object sender, RoutedEventArgs e)
        {

            path = AppDomain.CurrentDomain.BaseDirectory + "Database/RemoteControl.txt";
            data = File.ReadAllLines(path, Encoding.UTF8).ToList();
            listBox.Items.Clear();

            foreach (string d in data)
            {
                listBox.Items.Add(d.Substring(0, d.IndexOf('@')));
            }

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (path == null || path == "")
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = AppDomain.CurrentDomain.BaseDirectory+ "Database/RemoteControl.txt";
                dlg.DefaultExt = ".text";
                dlg.Filter = "Text documents (.txt)|*.txt";
                //bool? result = dlg.ShowDialog();
                path = dlg.FileName;
            }
            File.WriteAllLines(path, data);
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".text";
            dlg.Filter = "Text documents (.txt)|*.txt";
            bool? result = dlg.ShowDialog();
            path = dlg.FileName;
            File.WriteAllLines(path, data);
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DELETE_Click(object sender, RoutedEventArgs e)
        {
            data.Remove(data.Where(d => d.Substring(0, d.IndexOf('@')) == Name_Box.Text).FirstOrDefault());
            if (listBox.SelectedItem != null)
            {
                data.Remove(data.Where(d => d.Substring(0, d.IndexOf('@')) == listBox.SelectedItem as string).FirstOrDefault());
                listBox.Items.Remove(listBox.SelectedItem);
            }

            else
            {
                Signal.Text = "Please select an item to delete.";
            }
        }

        private void Name_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).IsFocused)
            {
                listBox.SelectedItem = null;
            }

        }

        private void Signal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).IsFocused)
            {
                listBox.SelectedItem = null;
            }
        }

        public async void SendCommand(string Command)
        {
            POST.IsEnabled = false;
            string CommandData = null;
            path = AppDomain.CurrentDomain.BaseDirectory + "Database/RemoteControl.txt";
            data = File.ReadAllLines(path, Encoding.UTF8).ToList();
            listBox.Items.Clear();
            if(Command=="TempUp")
            {
                ConfigurationManager.AppSettings["ACTemp"] = (Convert.ToInt32(ConfigurationManager.AppSettings["ACTemp"]) + 1).ToString();
                if (Convert.ToInt32(ConfigurationManager.AppSettings["ACTemp"]) > 30)
                {
                    TTS.Speaking("Sorry, I cannot increase the temperature anymore.");
                    ConfigurationManager.AppSettings["ACTemp"] = "30";
                    Function.AppConfigHelper.ModifyAppSettings("ACTemp", ConfigurationManager.AppSettings["ACTemp"]);
                    Command = "Temp" + ConfigurationManager.AppSettings["ACTemp"];
                    return;
                }
                else
                {
                    Function.AppConfigHelper.ModifyAppSettings("ACTemp", ConfigurationManager.AppSettings["ACTemp"]);
                    Command = "Temp" + ConfigurationManager.AppSettings["ACTemp"];
                }

            }
            else if(Command=="TempDown")
            {
                ConfigurationManager.AppSettings["ACTemp"] = (Convert.ToInt32(ConfigurationManager.AppSettings["ACTemp"]) - 1).ToString();
                if (Convert.ToInt32(ConfigurationManager.AppSettings["ACTemp"]) < 18)
                {
                    TTS.Speaking("Sorry, I cannot decrease the temperature anymore.");
                    ConfigurationManager.AppSettings["ACTemp"] = "18";
                    Function.AppConfigHelper.ModifyAppSettings("ACTemp", ConfigurationManager.AppSettings["ACTemp"]);
                    Command = "Temp" + ConfigurationManager.AppSettings["ACTemp"];
                    return;
                }
                else
                {
                    Function.AppConfigHelper.ModifyAppSettings("ACTemp", ConfigurationManager.AppSettings["ACTemp"]);
                    Command = "Temp" + ConfigurationManager.AppSettings["ACTemp"];
                }
            }
            foreach (string d in data)
            {
                if(d.Substring(0, d.IndexOf('@'))==Command)
                {
                    CommandData = d.Substring(d.IndexOf('@') + 1, d.Length - d.IndexOf('@') - 1);
                }
            }

            if(CommandData!=null)
            {
                HttpClient client = new HttpClient();
                StringContent content = new StringContent(CommandData, Encoding.UTF8, "text/plain");
                client.DefaultRequestHeaders.Add(@"X-Requested-With", "curl");
                client.Timeout = TimeSpan.FromSeconds(5);
                HttpResponseMessage response = null;
                try
                {
                    TTS.Speaking("OK");
                    response = await client.PostAsync(new Uri(@"http://" + ConfigurationManager.AppSettings["IRKit_IP"].ToString() + "/messages"), content);
                    if (!response.IsSuccessStatusCode)
                        Signal.Text = "Network Error";
                }
                catch (TaskCanceledException ex)
                {
                    TTS.Speaking("Sorry, I cannot control it for you now.");
                    return;
                }
                catch (Exception ex)
                {

                    Signal.Text = ex.Message;
                    return;
                }
            }
            else
            {
                TTS.Speaking("Sorry, I don't see it connected.");
            }

            POST.IsEnabled = true;
        }
    }
}
