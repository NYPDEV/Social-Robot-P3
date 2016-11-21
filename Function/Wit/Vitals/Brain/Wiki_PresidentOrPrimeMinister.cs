using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialRobot.Wit.Vitals.Brain
{
    class Wiki_PresidentOrPrimeMinister
    {
        private Objects.O_NLP.RootObject o_NLP = new Objects.O_NLP.RootObject();
        double conf = 0D;

        public string makeSentence(Objects.O_NLP.RootObject _o_NLP)
        {
            string obj = "Unassigned";
            try
            {
                // Bind to the wit.ai NLP response class
                o_NLP = _o_NLP;
                //conf = (o_NLP.outcomes.confidence * 100);

                string sentence = "";
                string result = "";
                string subresult = "";

                sentence += "I'm " + conf.ToString() + "% sure you are asking for President or Prime Minister.";

                // Try {} catch {} are quick fixes to exceptions, code should be made more robust to handle
                // the various types

                // This is also the place to add your custom code to the intent, ie. add the appointment or process the action

                try
                {
                    obj = o_NLP.entities.role[0].value;
                    sentence += Environment.NewLine + "You want: " + obj;
                    result = "Do you mean" + obj + "?";                                                                 //unknown obj
                }
                catch { }

                string country = "";
                if (obj == "president")
                {
                    try
                    {
                        country = o_NLP.entities.location[0].value;
                        subresult = "Who is the president of " + country;
                    }
                    catch
                    {
                        
                    }
                }

                else if (obj == "prime minister")
                {
                    try
                    {
                        country = o_NLP.entities.location[0].value;
                        subresult = "Who is the prime minister of " + country;
                    }
                    catch
                    {
                        
                    }
                }

                else
                {
                    sentence += Environment.NewLine + "The obj doesn't matched any conditions: " + obj;
                }

               // if (type == 1)
               // {
               //     return result;
               // }
               // else if (type == 2)
             //   {
             //       return subresult;
             //   }
            //    else
                    return sentence;
            } 
            catch (Exception ex)
            {
              //  if (type == 1)
             //   {
                    return "Uh oh, something went wrong.";
              //  }
             //   else
                 //   return "Uh oh, something went wrong: " + ex.Message;
            }
        }
    }
}
    
