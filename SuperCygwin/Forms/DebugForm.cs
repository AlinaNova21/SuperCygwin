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
                        checkedListBox1.Items.Add(string.Format(
                            "{0} {1} {3} {2}",
                            p.ProcessName,
                            Native.GetAncestor(p.MainWindowHandle,GetAncestorFlags.GetRoot),
                            p.MainWindowHandle,
                            p.Id),
                            Native.GetParent(p.MainWindowHandle).ToString() != "0");
                }
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
    }
}
