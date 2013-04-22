using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using System.Net;

namespace SuperCygwin
{
    public partial class ProcessContainer : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        static DockPanel dp;
        Process process;

        public static void RegisterNewProcessHandler(DockPanel dp, EventManager em)
        {
            em.NewProcess += new EventManager.NewProcessEventHandler(em_NewProcess);
            ProcessContainer.dp = dp;
        }

        static void em_NewProcess(object sender, EventManager.NewProcessEventArgs e)
        {
            //MessageBox.Show(e.Process.Arguments);
            if(e.Process.Arguments.StartsWith("/usr/bin/"))
                if (!File.Exists(e.Process.Arguments.Split(' ')[0].Replace("/usr/bin/", @"C:\cygwin\bin\")) &&
                    !File.Exists(e.Process.Arguments.Split(' ')[0].Replace("/usr/bin/", @"C:\cygwin\bin\") + ".exe")) 
                {
                    if (!File.Exists("cygwin_setup.exe"))
                    {
                        WebClient wc = new WebClient();
                        try{
                            wc.DownloadFile("http://www.cygwin.com/setup.exe", "cygwin_setup.exe");
                        }catch(Exception ex){
                            MessageBox.Show("Could not download cygwin setup. Please connect to the internet or install " + e.Process.Arguments.Split(' ')[0].Replace("/usr/bin/", ""));
                            return;
                        }
                    }
                    MessageBox.Show("Please install " + e.Process.Arguments.Split(' ')[0].Replace("/usr/bin/", "") + " before attempting to open this preset.");
                    ProcessStartInfo psi = new ProcessStartInfo("cygwin_setup.exe");
                    Create(dp,psi);
                    return;
                }
            Create(dp, e.Process);
        }

        public static ProcessContainer Create(DockPanel dp, string Path, string args = "")
        {
            ProcessContainer pc = new ProcessContainer(Path, args);
            pc.Show(dp);
            return pc;
        }
        public static ProcessContainer Create(DockPanel dp, Process p)
        {
            ProcessContainer pc = new ProcessContainer(p);
            pc.Show(dp);
            return pc;
        }
        public static ProcessContainer Create(DockPanel dp, ProcessStartInfo p)
        {
            ProcessContainer pc = new ProcessContainer(p);
            pc.Show(dp);
            return pc;
        }

        private ProcessContainer()
        {
            InitializeComponent();
        }
        public ProcessContainer(string Path, string args = "")
        {
            Process wnd = new Process();
            wnd.EnableRaisingEvents = true;
            ProcessStartInfo psi = new ProcessStartInfo(Path);
            psi.WindowStyle = ProcessWindowStyle.Minimized;
            if (args != null)
                psi.Arguments = args;
            psi.UseShellExecute = true;
            wnd.StartInfo = psi;
            wnd.Start();
            NewProcess(wnd);

            ContextMenu cm = new ContextMenu();
            ContextMenuStrip cms = new ContextMenuStrip();

            cm.MenuItems.Add("PLACEHOLDER");
            cms.Items.Add("PLACEHOLDER");

            TabPageContextMenu = cm;
            TabPageContextMenuStrip = cms;

            DockHandler.TabPageContextMenu = cm;
            DockHandler.TabPageContextMenuStrip = cms;
        }
        public ProcessContainer(ProcessStartInfo psi)
        {
            Process wnd = new Process();
            wnd.EnableRaisingEvents = true;
            psi.WindowStyle = ProcessWindowStyle.Minimized;
            wnd.StartInfo = psi;
            wnd.Start();
            NewProcess(wnd);
        }
        public ProcessContainer(Process p)
        {
            InitializeComponent();
            process = p;

            Load += new EventHandler(Init);
        }

        private void Init(object Sender, EventArgs e)
        {
            menuStrip1.Visible = Program.dev;

            Resize += new EventHandler(ResizeEmbedded);
            //ResizeEnd += new EventHandler(ResizeEmbedded);
            Process wnd = process;
            wnd.WaitForInputIdle(3000);
            long val = 0;
            //val += 0x11000000L;
            IntPtr ptr = new IntPtr(val);
            IntPtr Wnd = wnd.MainWindowHandle;
            //MessageBox.Show(Native.GetParent(Wnd).ToString());
            Native.SetParent(Wnd, panel1.Handle);
            Native.ShowWindow(Wnd, WindowShowStyle.ShowNormal);
            Native.SetWindowLongPtr(Wnd, (int)WindowLongFlags.GWL_STYLE, 0);
            Native.SetWindowPos(Wnd, Native.HWND_TOPMOST, 0, 0, panel1.Width, panel1.Height, (SWP.FRAMECHANGED + SWP.SHOWWINDOW));
            this.DataBindings.Add("Text", wnd, "MainWindowTitle");
            File.AppendAllText("log.txt", string.Format("INIT: {0} {1}x{2} {3}x{4}\r\n", wnd.MainWindowTitle, 0, 0, Width, Height));
            Native.SetWindowPos(process.MainWindowHandle, IntPtr.Zero, 0, 0, panel1.Width, panel1.Height, (SWP.FRAMECHANGED + SWP.NOZORDER + SWP.NOACTIVATE));
        }

        public void SetFocus()
        {
            Native.SetFocus(process.MainWindowHandle);
            Native.SetWindowPos(process.MainWindowHandle, IntPtr.Zero, 0, 0, 0, 0, (SWP.NOZORDER + SWP.NOSIZE + SWP.NOMOVE + SWP.SHOWWINDOW));
        }
        
        void NewProcess(Process wnd)
        {
            InitializeComponent();

            Native.ShowWindow(wnd.MainWindowHandle, WindowShowStyle.Hide);
            process = wnd;

            //Native.AttachThreadInput((uint)process.Threads[0].Id, (uint)System.Threading.Thread.CurrentThread.ManagedThreadId,true);

            Load += new EventHandler(Init);
            FormClosing += new FormClosingEventHandler(ProcessContainer_FormClosing);
            Disposed += new EventHandler(ProcessContainer_Disposed);
            //GotFocus += new EventHandler(ProcessContainer_GotFocus);
            process.Exited += new EventHandler(process_Exited);
        }

        void process_Exited(object sender, EventArgs e)
        {
            try
            {
                if (!IsDisposed)
                    Invoke((Action)(() =>
                    {
                        Close();
                    }));
            }
            catch (Exception ex) { }
            //this.Hide();
        }

        void ProcessContainer_GotFocus(object sender, EventArgs e)
        {
            SetFocus();
        }
        void ProcessContainer_Disposed(object sender, EventArgs e)
        {
            if (!process.HasExited)
            {
                try
                {
                    process.CloseMainWindow();
                    process.WaitForExit(10000);
                    if (!process.Responding)
                        process.Kill();
                }
                catch (Exception ex) { }
            }
        }
        void ProcessContainer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!process.HasExited)
            {
                try
                {
                    process.CloseMainWindow();
                    process.WaitForExit(10000);
                    if (!process.Responding)
                        process.Kill();
                }
                catch (Exception ex) { }
            }
        }

        private void ProcessContainer_Load(object sender, EventArgs e)
        {

        }
        private void ResizeEmbedded(object sender, EventArgs e)
        {
            //MessageBox.Show("You are in the Form.ResizeEnd event.");
            if (Width == 0 || Height == 0)
                return;
            Native.SetWindowPos(process.MainWindowHandle, IntPtr.Zero, 0, 0, panel1.Width, panel1.Height, (SWP.FRAMECHANGED + SWP.NOZORDER + SWP.NOACTIVATE));
            File.AppendAllText("log.txt", string.Format("RESIZE: {0} {1}x{2} {3}x{4}\r\n", process.MainWindowTitle, 0, 0, Width, Height));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            IntPtr mnu = Native.GetSystemMenu(process.MainWindowHandle, false);
            uint cmd = Native.TrackPopupMenuEx(mnu, 0x0100, MousePosition.X, MousePosition.Y, Handle, IntPtr.Zero);
            Native.PostMessage(Handle, 0x112, new IntPtr(cmd), IntPtr.Zero);
        }
        private void contextMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IntPtr mnu = Native.GetSystemMenu(process.MainWindowHandle, false);
            uint cmd = Native.TrackPopupMenuEx(mnu, 0x0100, MousePosition.X, MousePosition.Y, Handle, IntPtr.Zero);
            //MessageBox.Show("" + cmd);
            Native.PostMessage(process.MainWindowHandle, 0x112, new IntPtr(cmd), IntPtr.Zero);
        }
    }
}
