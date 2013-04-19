using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SuperCygwin
{
    static class Program
    {
        public static bool dev=false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] Args)
        {
            if (Args.Contains("-version"))
            {
                Console.Write(Application.ProductVersion);
                return;
            }
            if (Args.Contains("-dev"))
                dev = true;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(string.Format("Unhandled Exception: \n {0}", ((Exception)e.ExceptionObject).ToString()), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(string.Format("Thread Exception: \n {0}",e.Exception.ToString()), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
