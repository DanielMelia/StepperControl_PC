using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace StepperControl_PC
{
    internal static class Log
    {
        public static string logText { get; set; }

        public static void SendToLog(string text)
        {
            logText += text + Environment.NewLine;
        }

        public static void DisplayLog(ref TextBox textBox)
        {
            if(logText != "")
            {
                logText += Environment.NewLine;
                textBox.Text += logText;
                logText = "";
            }
            
        }

    }
}
