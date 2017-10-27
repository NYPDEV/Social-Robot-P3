using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Windows;

namespace SocialRobot.Application
{
    public class GoogleCalender
    {
        static string[] Scopes = { CalendarService.Scope.Calendar };//{ CalendarService.Scope.CalendarReadonly };
        public string RemSum;
        public string RemTime;
        public string AlrmCont;
        public bool NoSum = false;
        public bool RemTime1 = false;
        public bool RemTime2 = false;
        public bool RemTime3 = false;
        public bool resetTrue = false;
        public bool ThrdAbrt = false;
        public static int i = 0;
        static string ApplicationName = "Google Calendar API .NET Quickstart";
        Function.Text_To_Speech TTS = new Function.Text_To_Speech();

        public void CreateAppointment(string ReminderContent, DateTime ReminderTime)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                credPath = System.IO.Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            Event newEvent = new Event()
            {
                Summary = ReminderContent,
                Location = "Singapore",
                Description = ReminderContent,
                Start = new EventDateTime()
                {
                    DateTime = ReminderTime,
                    TimeZone = "Asia/Singapore",
                },
                End = new EventDateTime()
                {
                    DateTime = ReminderTime,//DateTime.Parse("2017-03-01T09:00:00-09:30"),
                    TimeZone = "Asia/Singapore",
                },
                // Recurrence = new String[] { "RRULE:FREQ=DAILY;COUNT=2" },
                
                // Added THEF20170712
                //Attendees = new EventAttendee[] {
                //              new EventAttendee() { Email = "elthef@hotmail.com" },
                //              new EventAttendee() { Email = "elthef@gmail.com" },
                // },\
                Attendees = new EventAttendee[] {
                                 new EventAttendee() { Email = "elthef@gmail.com" },
                 },
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new EventReminder[] {
                      new EventReminder() { Method = "email", Minutes = 24 * 60 },
                        new EventReminder() { Method = "sms", Minutes = 10 },
                 }
                }
            };

            String calendarId = "primary";

            EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
            Event createdEvent = request.Execute();
            Console.WriteLine("Event created: {0}", createdEvent.HtmlLink);

        }



        public void LoadReminder()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                credPath = System.IO.Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            try
            {
                Events events = request.Execute();
                Console.WriteLine("Upcoming events:");
                if (events.Items != null && events.Items.Count > 0)
                {
                    if (i > events.Items.Count)
                    {
                        i = 0;
                        ReminderContent.Clear();
                        ReminderTime.Clear();
                        Thread.Sleep(100);
                        for (int v = 0; v < events.Items.Count; v++)
                        {
                            ReminderContent.Add(events.Items[v].Summary);
                            ReminderTime.Add(events.Items[v].Start.DateTime.Value);
                        }

                    }
                    else if (i == events.Items.Count)
                    {
                        i = 0;
                        ReminderContent.Clear();
                        ReminderTime.Clear();
                        Thread.Sleep(100);
                        for (int v = 0; v < events.Items.Count; v++)
                        {
                            ReminderContent.Add(events.Items[v].Summary);
                            ReminderTime.Add(events.Items[v].Start.DateTime.Value);
                        }
                    }
                    else
                    {
                        ReminderContent.Clear();
                        ReminderTime.Clear();
                        Thread.Sleep(100);
                        for (int v = 0; v < events.Items.Count; v++)
                        {
                            ReminderContent.Add(events.Items[v].Summary);
                            ReminderTime.Add(events.Items[v].Start.DateTime.Value);
                        }
                    }
                    if (i == 0)
                    {
                        string RC = ReminderContent[0];
                        DateTime RT = ReminderTime[0];
                        RemContNow = RC;
                        RemTimeNow = RT;
                    }
                    if (AlarmThread != null)
                    {
                        AlrmReset();
                    }
                    else
                    {
                        SetAlarm();
                    }
                    i++;
                }
                else
                {
                    Console.WriteLine("No upcoming events found.");
                }
                Console.Read();
            }
            catch
            {
                MessageBox.Show("No Internet Connection!");
            }
            
        }

        string RemContNow;
        DateTime RemTimeNow;
        List<string> ReminderContent = new List<string>();
        List<DateTime> ReminderTime = new List<DateTime>();

        public void ReadReminder()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                credPath = System.IO.Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            Thread.Sleep(1000);
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    DateTime when = eventItem.Start.DateTime.Value;
                    //if (String.IsNullOrEmpty(when))
                    //{
                    //    when = eventItem.Start.Date;
                    //}
                    string Time = when.ToString("hh") + ":" + when.ToString("mm") + " " + when.ToString("tt");
                    if (eventItem == events.Items[0] && when.ToString("dd") == DateTime.Now.ToString("dd"))
                    {
                        TTS.Speaking("Today, you should " + eventItem.Summary + " at " + Time);
                    }
                    else if (when.ToString("dd") == DateTime.Now.ToString("dd"))
                    {
                        TTS.Speaking(eventItem.Summary + " at " + Time);
                    }
                }
            }
            else
            {
                TTS.Speaking("Today you don't have any appointment.");
            }
        }

        private void AlrmReset()
        {
            if (AlarmThread != null)
            {
                ThrdAbrt = true;
                //Thread.Sleep(100);
                //AlarmThread.Abort();
                Thread.Sleep(100);
                string RC = ReminderContent[0];
                DateTime RT = ReminderTime[0];
                RemContNow = RC;
                RemTimeNow = RT;
                ThrdAbrt = false;
                AlarmLoop();
                // AlarmThread.Start();
            }
            while (resetTrue != true)
            {
                ;
            }
            if (ReminderContent.Count > 0 && ReminderTime.Count > 0)
            {
                string RC = ReminderContent[0];
                DateTime RT = ReminderTime[0];
                RemContNow = RC;
                RemTimeNow = RT;
                AlarmLoop();
            }
            else
            {

            }
        }

        Thread AlarmThread;

        private void SetAlarm()
        {
            if (AlarmThread != null && ThrdAbrt != true)
            {
                AlrmReset();
            }
            AlarmThread = new Thread(new ThreadStart(AlarmLoop));
            AlarmThread.Start();
        }

        private void AlarmLoop()
        {
            while (DateTime.Now < RemTimeNow)
            {
                ;
                if (ThrdAbrt == true)
                {
                    RemContNow = null;
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            if (RemContNow != null)
            {
                if (RemTime3 == true)
                {

                }
                else
                {
                    AlrmCont = RemContNow;
                    AlrmCont = AlrmCont.Replace(" my ", " your ");
                    AlrmCont = AlrmCont.Replace(" I ", " you ");
                    AlrmCont = AlrmCont.Replace(" me ", " you ");
                    Thread.Sleep(3000);
                    RemTime3 = true;
                    Thread.Sleep(1000);
                    RemTime3 = false;
                    if (ReminderContent.Count == 0 && ReminderTime.Count == 0)
                    {

                    }
                    else if (ReminderContent.Count > 0 && ReminderTime.Count > 0)
                    {
                        ReminderContent.RemoveAt(0);
                        ReminderTime.RemoveAt(0);
                    }
                    resetTrue = true;
                    AlrmReset();
                }
                //uiHelper.RobotSpeech("Excuse me. This is to remind you to " + ReminderContent);
            }
            else
            {
                string RC = ReminderContent[0];
                DateTime RT = ReminderTime[0];
                RemContNow = RC;
                RemTimeNow = RT;
            }
        }
    }
}
