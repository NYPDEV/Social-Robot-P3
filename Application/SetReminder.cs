using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Util.Store;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using System.Timers;

namespace SocialRobot.Application
{
    public class SetReminder
    {
        CalendarService calendarConnection;
        Events request = null;
        List<Event> items;
        bool GoogleCalendarEnabled = false;

        string Time_now;
        string Date_now;
        string Day_now;

        public Function.Speech_Rcognition_Grammar Speak = new Function.Speech_Rcognition_Grammar();

        private void bTimer_event(Object source, ElapsedEventArgs e)
        {
            Time_now = DateTime.Now.ToString("hh") + ":" + DateTime.Now.ToString("mm") + " " + DateTime.Now.ToString("tt");
            // Date_now = DateTime.Now.ToString("d") + " of " + DateTime.Now.ToString("MMMM");
            Date_now = DateTime.Now.ToString("M"); //+ DateTime.Now.ToString("m");
            Day_now = DateTime.Now.ToString("dddd");

            int day = DateTime.Now.Day;
            int date = DateTime.Now.Date.Month;
            int minute = DateTime.Now.Minute;
            int hour = DateTime.Now.Hour;
            int sec = DateTime.Now.Second;
            int event_start_at_the_moment = 0;
            int event_end_at_the_moment = 0;

            //MessageBox.Show(day+"\n"+hour);
            if (GoogleCalendarEnabled == true && items.Count != 0)
            {
                foreach (Event test in items)
                {
                    // MessageBox.Show(test.Start.DateTime.Value.Day + "\n" + test.Start.DateTime.Value.Minute);
                    //+"\n"+test.Summary +"\n "+test.End.DateTime+"\n"+test.Description +"\n"+test.Location);

                    if (test.Start.DateTime.Value.Day == day && test.Start.DateTime.Value.Date.Month == date)
                    {
                        //MessageBox.Show("yes" + test.Start.DateTime.Value.Hour);
                        if (test.Start.DateTime.Value.Minute == minute && test.Start.DateTime.Value.Hour == hour && test.Start.DateTime.Value.Second == sec && event_start_at_the_moment == 0)
                        {
                            event_start_at_the_moment++;
                            //   flag_speak_completed = true;
                            Speak.SRE_Speech.SelectVoice("IVONA 2 Amy");
                            //Speak.SRE_Speech.SelectVoice("Microsoft Zira Desktop");
                            Speak.SRE_Speech.SpeakAsync("Excuse me. It's time for " + test.Summary);
                        }
                        else if (event_start_at_the_moment > 0)
                        {
                            event_start_at_the_moment++;
                            //  flag_speak_completed = true;
                            Speak.SRE_Speech.SelectVoice("IVONA 2 Amy");
                            //Speak.SRE_Speech.SelectVoice("Microsoft Zira Desktop");
                            Speak.SRE_Speech.SpeakAsync("and " + test.Summary);
                        }

                        if (test.End.DateTime.Value.Minute == minute && test.End.DateTime.Value.Hour == hour && test.End.DateTime.Value.Second == sec && event_end_at_the_moment == 0)
                        {
                            event_end_at_the_moment++;
                            //  flag_speak_completed = true;
                            Speak.SRE_Speech.SelectVoice("IVONA 2 Amy");
                            // Speak.SRE_Speech.SelectVoice("Microsoft Zira Desktop");
                            Speak.SRE_Speech.SpeakAsync("Don't forget your " + test.Summary);
                        }
                        else if (event_end_at_the_moment > 0)
                        {
                            event_end_at_the_moment++;
                            // flag_speak_completed = true;
                            Speak.SRE_Speech.SelectVoice("IVONA 2 Amy");
                            // Speak.SRE_Speech.SelectVoice("Microsoft Zira Desktop");
                            Speak.SRE_Speech.SpeakAsync("and " + test.Summary);
                        }
                    }
                    /*
                     * Time_now = DateTime.Now.ToString("hh") + ":" + 
                     * DateTime.Now.ToString("mm") + " " +
                     * Date_now = DateTime.Now.ToString("M"); //+ DateTime.Now.ToString("m");
                     * Day_now = DateTime.Now.ToString("dddd");
                     */
                }
            }

        }

        public void GoogleCalendar()
        {
            GoogleCalendarEnabled = false;
            //           //// check the file exists
            //           //if (!File.Exists(keyFilePath))
            //           //{
            //           //    Console.WriteLine("An Error occurred - Key file does not exist");
            //           //    return null;
            //           //}

            //           string[] scopes = new string[] {
            //               CalendarService.Scope.Calendar, // Manage your calendars
            //               CalendarService.Scope.CalendarReadonly // View your Calendars
            //};

            //           var certificate = new X509Certificate2(@"C:\Users\Shive\Desktop\2-2-15\GoogleCalendar-4bcb43f58d2a.p12", "notasecret", X509KeyStorageFlags.Exportable);




            ClientSecrets secrets = new ClientSecrets
            {
                ClientId = "1057841883430-ml0bjcua59arakl74l57jouq38ul64dm.apps.googleusercontent.com",
                ClientSecret = "HcI89lr3AiWbhUa8spNNjxtZ"
            };



            ///////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////
            //            string keyFilePath = @"C:\Users\Shive\Desktop\10-2-15\GoogleCalendar-4bcb43f58d2a.p12";
            //            string serviceAccountEmail = "1057841883430-0auunec1mbc2gc1cf930oegccmv2kmfc@developer.gserviceaccount.com";
            //                 string[] scopes = new string[] {
            //    CalendarService.Scope.Calendar, // Manage your calendars
            //    CalendarService.Scope.CalendarReadonly // View your Calendars
            // };

            // var certificate = new X509Certificate2(keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);

            //ServiceAccountCredential credential = new ServiceAccountCredential(
            //    new ServiceAccountCredential.Initializer(serviceAccountEmail) {
            //        Scopes = scopes
            //    }.FromCertificate(certificate));
            ///////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////

            try
            {
                //               ServiceAccountCredential credential = new ServiceAccountCredential
                //                   (new ServiceAccountCredential.Initializer("1057841883430-0auunec1mbc2gc1cf930oegccmv2kmfc@developer.gserviceaccount.com")
                //                   {
                //                       Scopes = scopes
                //                   }.FromCertificate(certificate));



                //               // Create the service.
                //               CalendarService service = new CalendarService(new BaseClientService.Initializer()
                //               {
                //                   HttpClientInitializer = credential,
                //                   ApplicationName = "test",
                //               });
                //               //return service;


                //               /*
                //              // BaseClientService.Initializer initializer = new BaseClientService.Initializer();
                //               IList<CalendarListEntry> list = service.CalendarList.List().Execute().Items;//new List<CalendarListEntry>();

                //               DisplayList(list);
                //               foreach (Google.Apis.Calendar.v3.Data.CalendarListEntry calendar in list)
                //               {

                //                   MessageBox.Show(calendar +"");
                //               }
                //               /////////////////////////////////
                //               var calendars = service.CalendarList.List().Execute().Items;
                //               foreach (CalendarListEntry entry in calendars)
                //               {
                //                   MessageBox.Show(entry.Summary + " - " + entry.Id);
                //               }

                //               Events request = null;
                //              // ListRequest lr = service.Events.List("calendar id (email)");

                //               //lr.TimeMin = DateTime.Now.AddDays(-5); //five days in the past
                //               //lr.TimeMax = DateTime.Now.AddDays(5); //five days in the future

                //               //request = lr.Execute();
                //               //////////////////////////////////

                //               */



                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        secrets,
        new string[]
                        { 
                                CalendarService.Scope.Calendar
                        },
        "user",
        CancellationToken.None)
.Result;

                var initializer = new BaseClientService.Initializer();
                initializer.HttpClientInitializer = credential;
                initializer.ApplicationName = "MyProject";
                calendarConnection = new CalendarService(initializer);

                var calendars = calendarConnection.CalendarList.List().Execute().Items;

                /*
                foreach (CalendarListEntry entry in calendars)
                {
                    MessageBox.Show(entry.Summary + " - " + entry.Id);
                }
                */

                Google.Apis.Calendar.v3.EventsResource.ListRequest lr = calendarConnection.Events.List("r3goliavgvj0rtpprmkt2k043c@group.calendar.google.com");

                lr.TimeMin = DateTime.Now.AddDays(-5); //five days in the past
                lr.TimeMax = DateTime.Now.AddDays(5); //five days in the future

                request = lr.Execute();

                items = (List<Event>)request.Items;

                GoogleCalendarEnabled = true;




                //"r3goliavgvj0rtpprmkt2k043c@group.calendar.google.com"


                /*                     IMPORTANT!!!WRITE EVENT
               {
                   Event googleCalendarEvent = new Event();

                   googleCalendarEvent.Start = new EventDateTime();
                   googleCalendarEvent.End = new EventDateTime();

                   //if (AllDay)
                   //{
                       //If you want to create an all day event you don't need
                       //to provide the time, only the date


                   //googleCalendarEvent.Start.Date = DateTime.Now.ToString("2015-2-2");
                   //googleCalendarEvent.End.Date = DateTime.Now.ToString("2015-2-10");

                   //}
                   //else
                   //{

                   googleCalendarEvent.Start.DateTime = new DateTime(2015,2,12,13,3,0);
                   //googleCalendarEvent.Start.DateTime = DateTime.Now;
                   googleCalendarEvent.End.DateTime = new DateTime(2015, 2, 12, 15, 0, 0);
                   //}

                   googleCalendarEvent.Summary = "Its my birthday";
                   googleCalendarEvent.Description = "I'm becoming one year older";
                   googleCalendarEvent.Location = "at home";

                   //Set Remainder
                   googleCalendarEvent.Reminders = new Event.RemindersData();
                   googleCalendarEvent.Reminders.UseDefault = false;
                   EventReminder reminder = new EventReminder();
                   reminder.Method = "popup";
                   reminder.Minutes = 6;

                   //




                   //googleCalendarEvent.Reminders.Overrides = new List();

                   //googleCalendarEvent.Reminders.Overrides.Add(reminder);

                   //Attendees
                   //googleCalendarEvent.Attendees.Add(new EventAttendee()
                   //{
                   //    DisplayName = "Sebastian",
                   //    Email = "my@email.com",
                   //    ResponseStatus = "accepted"
                   //});
                   //googleCalendarEvent.Attendees.Add(new EventAttendee()
                   //{
                   //    DisplayName = "Fiona",
                   //    Email = "fiona@email.com",
                   //    ResponseStatus = "needsAction"
                   //});

                   calendarConnection.Events.Insert(googleCalendarEvent, "r3goliavgvj0rtpprmkt2k043c@group.calendar.google.com").Execute();
               }
               */

            }

            catch (Exception ex)
            {
                //MessageBox.Show(ex.InnerException + "");
                //return null;
            }
        }

        public void WriteGoogleCalendarReminder(string Calendartask, int startyear, int startmonth, int startdate, int starthour, int startminute)
        {

            GoogleCalendar();
            Event googleCalendarEvent = new Event();

            googleCalendarEvent.Start = new EventDateTime();
            googleCalendarEvent.End = new EventDateTime();

            //if (AllDay)
            //{
            //If you want to create an all day event you don't need
            //to provide the time, only the date


            //googleCalendarEvent.Start.Date = DateTime.Now.ToString("2015-2-2");
            //googleCalendarEvent.End.Date = DateTime.Now.ToString("2015-2-10");

            //}
            //else
            //{

            googleCalendarEvent.Start.DateTime = new DateTime(startyear, startmonth, startdate, starthour, startminute, 0);
            //googleCalendarEvent.Start.DateTime = DateTime.Now;

            int enddate = startdate, endhour = starthour, endminute = startminute;
            if (startminute < 55)
            {
                endminute = startminute + 5;
            }
            else if (startminute == 55)
            {
                endminute = 0;
                if (starthour < 24)
                {
                    endhour = starthour + 1;
                }
                else if (starthour == 24)
                {
                    endhour = 0;
                    enddate = startdate + 1;
                }
            }
            else if (startminute > 55)
            {
                endminute = startminute + 5;
                endminute = endminute - 60;
                if (starthour < 24)
                {
                    endhour = starthour + 1;
                }
                else if (starthour == 24)
                {
                    endhour = 0;
                    enddate = startdate + 1;
                }
            }
            else
            {
                enddate = startdate;
                endhour = starthour;
                endminute = startminute;
            }

            googleCalendarEvent.End.DateTime = new DateTime(startyear, startmonth, enddate, endhour, endminute, 0);
            //}
            Calendartask = Calendartask;
            googleCalendarEvent.Summary = Calendartask;

            //googleCalendarEvent.Description = "I'm becoming one year older";
            // googleCalendarEvent.Location = "at home";
            /*
            //Set Remainder
            googleCalendarEvent.Reminders = new Event.RemindersData();
            googleCalendarEvent.Reminders.UseDefault = false;
            EventReminder reminder = new EventReminder();
            reminder.Method = "popup";
            reminder.Minutes = 6;
            */
            //




            //googleCalendarEvent.Reminders.Overrides = new List();

            //googleCalendarEvent.Reminders.Overrides.Add(reminder);

            //Attendees
            //googleCalendarEvent.Attendees.Add(new EventAttendee()
            //{
            //    DisplayName = "Sebastian",
            //    Email = "my@email.com",
            //    ResponseStatus = "accepted"
            //});
            //googleCalendarEvent.Attendees.Add(new EventAttendee()
            //{
            //    DisplayName = "Fiona",
            //    Email = "fiona@email.com",
            //    ResponseStatus = "needsAction"
            //});
            try
            {
                calendarConnection.Events.Insert(googleCalendarEvent, "r3goliavgvj0rtpprmkt2k043c@group.calendar.google.com").Execute();
            }
            catch
            {
                Speak.SRE_Speech.SpeakAsync("Sorry, I did not hear clearly, please try again!");
            }
           // GoogleCalendar();
        }

        private static void DisplayList(IList<CalendarListEntry> list)
        {
            foreach (CalendarListEntry item in list)
            {
                //MessageBox.Show(item.Id + "");
            }
        }
    }
}
