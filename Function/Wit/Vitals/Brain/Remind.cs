using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialRobot.Wit.Vitals.Brain
{
    class Remind
    {
        private Objects.O_NLP.RootObject o_NLP = new Objects.O_NLP.RootObject();
        double conf = 0D;

        public string makeSentence(Objects.O_NLP.RootObject _o_NLP, int type)
        {

            string remind = "";
            string obj = "";
            string day = "";
            string timehour = "";
            string timeminute = "";
            string reminder_task = "";

            try
            {
                // Bind to the wit.ai NLP response class
                o_NLP = _o_NLP;
                //conf = (o_NLP.outcomes.confidence * 100);

                string sentence = "";
                //////////////
                string result = "";
                string subresult = "";
                string subday = "";
                string subhour = "";
                string subminute = "";
                /////////////
                sentence += "I'm " + conf.ToString() + "% sure you are setting reminder.";

                // Try {} catch {} are quick fixes to exceptions, code should be made more robust to handle
                // the various types

                // This is also the place to add your custom code to the intent, ie. add the appointment or process the action

                try
                {
                    obj = o_NLP.entities.com_obj[0].value;
                    remind = o_NLP.entities.com_remind[0].value;
                   // timehour = o_NLP.outcomes.entities.datetime[0].value.Hour.ToString();
                   // timeminute = o_NLP.outcomes.entities.datetime[0].value.Minute.ToString();
                    reminder_task = o_NLP.entities.reminder[0].value.ToString();

                    try
                    {
                        day = o_NLP.entities.day[0].value;
                    }
                    catch
                    {
                        day = "today";
                    }


                        try
                        {
                            result = "SGM_FUNC_COMPLEX_SetReminderSuccess";
                            subresult = obj;
                            subday = day;
                            subhour = timehour;
                            subminute = timeminute;
                        }
                        catch
                        {
                            //result = "SGM_FUNC_COMPLEX_SetReminderNotEnoughInfo";
                        }
                    


              //      else
              //      {
                //        sentence += Environment.NewLine + "The obj doesn't matched any conditions: " + remind;
                        //result += "Sorry, i don't quite understand this";                                               //unknown obj
              //      }
                }
                catch
                {
                    //result = "SGM_FUNC_COMPLEX_SetReminderNotEnoughInfo";
                }




                if (type == 1)
                {
                    return result;
                }
                else if (type == 2)
                {
                    return subresult;
                }
                else if (type == 3)
                {
                    return subday;
                }
                else if (type == 4)
                {
                    return subhour.ToString();
                }
                else if (type == 5)
                {
                    return subminute.ToString();
                }

                else
                    return sentence;
            }
            catch (Exception ex)
            {
                if (type == 1)
                {
                    return "Uh oh, something went wrong.";
                }
                else
                    return "Uh oh, something went wrong: " + ex.Message;
            }
        }
    }
}
