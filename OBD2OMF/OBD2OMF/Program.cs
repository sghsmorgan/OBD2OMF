using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBD2OMF
{
    class Program
    {
        private static volatile bool keepRunning;
        static void Main(string[] args)
        {

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                Program.keepRunning = false;
            };

            while (Program.keepRunning)
            {
                List<OBDMessage> messages = GetOBDMessages();
                Process(messages);

            }
            Console.WriteLine("Collection End");


        }

        private static void Process(List<OBDMessage> messages)
        {
            throw new NotImplementedException();
        }

        private static List<OBDMessage> GetOBDMessages()
        {
            throw new NotImplementedException();
        }


    }

    public static class OMFVars
    {
        public static string _omfEndpoint = @"https://abathon5520.osisoft.int:5470/ingress/messages";
        public static string _producerToken = @"uid=b2f7bfd6-08cf-4ba7-a01c-f2749be34757&crt=20180717195947978&sig=PKn45Ypzlh6Apqr5ZUmEwToCewC//yPlEQzyCujxyZo=";
    }
}
