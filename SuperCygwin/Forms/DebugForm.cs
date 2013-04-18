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

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string prefer = "";
            if (radioCygwin.Checked)
                prefer = "Cygwin";
            else if (radioPutty.Checked)
                prefer = "Putty";
            else
            {
                MessageBox.Show("Please select a SSH Handler");
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.DefaultExt = "xml";
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;
            
            XElement x = XElement.Parse(File.ReadAllText(ofd.FileName));


            foreach (XElement el in x.Elements("SessionData"))
            {
                Preset p = new Preset();
                p.Name = el.Attribute("SessionName").Value + " - Putty Import";
                string proto = el.Attribute("Proto").Value;
                string host = el.Attribute("Host").Value;
                string port = el.Attribute("Port").Value;
                string user = el.Attribute("Username").Value;
                string tgt = (user != "" ? user + "@" : "") + host;
                if (proto == "SSH")
                {
                    if (prefer == "Putty")
                    {
                        p.Type = PresetType.Putty;
                        p.Path = "Putty.exe";
                        p.Args = string.Format("-ssh {0} -P {1}",tgt,port);
                    }
                    else
                    {
                        p.Type = PresetType.Mintty;
                        p.Path = @"C:\cygwin\bin\mintty.exe";
                        p.Args = string.Format("/usr/bin/ssh {0} -P {1}", tgt, port);
                    }
                }
                //NewPreset frm = new NewPreset(p);
                PresetsForm.MainForm.AddPreset(p);
            }
        }
    }
}
