using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;



namespace SocialRobot
{
    class VisionDisplay
    {
        public VisionDisplay()
        {
            Process.Start(@"C:\HVC-P_EvaluationKit_Software_Specifications_v1.2.0\EvaluationSoftware\EvaluationSoftware_v1.0.4\HVC-PDemoE.exe");
            Console.ReadKey();
        }
    }
}
