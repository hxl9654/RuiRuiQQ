using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuiRuiQQRobot
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        public static MainForm MainForm;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new MainForm();
            Application.Run(MainForm);
        }
    }
}
