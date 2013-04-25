using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.Linq;
using System.IO;
using SuperCygwin.Forms.Presets;

namespace SuperCygwin.Forms
{
    public partial class DebugForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public DebugForm()
        {
            InitializeComponent();
        }

        private void DebugForm_Load(object sender, EventArgs e)
        {
            Timer t = new Timer();
            t.Interval = 1000;
            t.Enabled = true;
            t.Tick += new EventHandler((se, ev) =>
            {
                t.Enabled = false;
                t.Dispose();
                ProcessContainer pc = new ProcessContainer(Program.Config.MinTTYPath, "-");
                pc.TopLevel = false;
                pc.Parent = tabPage2;
                tabPage2.Controls.Add(pc);
                //pc.WindowState = FormWindowState.Maximized;
                pc.Dock = DockStyle.Fill;
                pc.FormBorderStyle = FormBorderStyle.None;
                pc.Show();
            });
            richTextBox1.Text = "Disabled.";

            //Process p = Process.Start(@"C:\cygwin\bin\mintty.exe", "-");

            //IntPtr localThread, remoteThread;
            //localThread = Native.GetWindowThreadProcessId(Handle, IntPtr.Zero);
            //remoteThread = Native.GetWindowThreadProcessId(p.MainWindowHandle, IntPtr.Zero);
            //Native.AttachThreadInput(remoteThread, localThread, true);
            //Native.BringWindowToTop(p.MainWindowHandle);
            //Native.SetFocus(p.MainWindowHandle);
            //startCmd();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            string[] names=new string[]{"bash","ssh","mintty"};
            foreach(string name in names)
                foreach (Process p in Process.GetProcessesByName(name))
                {
                    //if (Native.GetParent(p.MainWindowHandle).ToInt32() == 0)
                    List<IntPtr> h = (List<IntPtr>)Native.EnumerateProcessWindowHandles(p);
                    if (h.Count == 0) h.Add(IntPtr.Zero);
                    checkedListBox1.Items.Add(
                        string.Format(
                            "{0} {1} {2} {3} {4}",
                            p.ProcessName,
                            p.Id,
                            h.Count,
                            Native.GetAncestor(h[0],GetAncestorFlags.GetRootOwner),
                            h[0]),
                        Native.GetParent(h[0]).ToString() != "0");
                    if (Native.GetParent(h[0]).ToString() != "0")
                        Native.SetParent(h[0], IntPtr.Zero);
                }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (IntPtr i in Native.EnumerateProcessWindowHandles(
                    Process.GetProcessById(
                        int.Parse(
                            checkedListBox1.SelectedItem.ToString().Split(' ')[1]
                        )
                    )
                ).ToArray())
                listBox1.Items.Add(i.ToString()+"  "+title(i));

            listBox2.Items.Clear();
            foreach (ProcessThread t in Process.GetProcessById(
                        int.Parse(
                            checkedListBox1.SelectedItem.ToString().Split(' ')[1]
                        )
                    ).Threads)
                listBox2.Items.Add(t.Id+" "+t.StartAddress);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (string pr in checkedListBox1.CheckedItems)
            {
                int id = int.Parse(pr.ToString().Split(' ')[1]);
                try
                {
                    Process p = Process.GetProcessById(id);
                    foreach (ProcessThread pt in p.Threads)
                    {
                        Native.PostThreadMessage((uint)pt.Id, WM.QUIT, UIntPtr.Zero, IntPtr.Zero);
                    }
                }
                catch (Exception ex) { }
            }
        }

        string title(IntPtr hWnd)
        {
            StringBuilder message = new StringBuilder(1000);
            Native.SendMessage(hWnd, (uint)WM.GETTEXT, message.Capacity, message);
            return message.ToString();
        }

        private void richTextBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
        }

        Process cmd;

        void startCmd()
        {
            richTextBox1.Text = "";
            cmd = new Process();
            cmd.EnableRaisingEvents = true;
            ProcessStartInfo psi = new ProcessStartInfo(@"C:\cygwin\bin\ssh.exe", "root@ags131.co");
            psi.CreateNoWindow = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            cmd.StartInfo = psi;
            cmd.Start();
            //cmd.WaitForInputIdle();
            cmd.OutputDataReceived += new DataReceivedEventHandler(cmd_OutputDataReceived);
            cmd.BeginOutputReadLine();
        }

        void cmd_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Invoke((Action<string>)((d)=>{
                richTextBox1.AppendText(d+"\n");
            }),e.Data);
            
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (cmd == null) return;
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    esc = true;
                    break;
                case Keys.OemOpenBrackets:
                    if(esc)
                    {
                        esc = false;
                        csi = true;
                    }
                    break;
                case Keys.Return:
                    cmd.StandardInput.Close();
                    break;
                default:
                    if (csi)
                    {
                        buff += e.KeyCode.ToString();
                    }else
                        cmd.StandardInput.Write((char)e.KeyCode);
                    richTextBox1.AppendText(((char)e.KeyData).ToString());
                    break;
            }
        }

        public bool esc = false;
        public bool csi = false;
        public string buff = "";
        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == (int)WM.NCHITTEST)
            {
                m.Result = (IntPtr)2;
                return;
            }

            if (m.Msg == (int)WM.LBUTTONDOWN)
            {
                
                //int index = HitTest();
                //if (index == -1)
                {
                    //m.Result = (IntPtr)WeifenLuo.WinFormsUI.Docking.Win32.HitTest.HTCAPTION;
                   // return;
                }
            }

            //base.WndProc(ref m);
            return;
        }

    }
}
