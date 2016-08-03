using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace unFARC
{
    static class Program
    {
        static public List<MessageCode> Messages = new List<MessageCode>();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (File.Exists(Application.StartupPath + @"\msg_code.json"))
            {
                Messages =
                    (JsonConvert.DeserializeObject<List<MessageCode>>(
                        File.ReadAllText(Application.StartupPath + @"\msg_code.json")));
            }
            else
            {
                Messages = new List<MessageCode>();
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        public struct MessageCode
        {
            public string code;
            public int values;
            public string code_name;
            public string code_group;
            public string code_description;
        }
    }
}
