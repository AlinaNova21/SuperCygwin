using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Newtonsoft.Json;
using SuperCygwin.Forms.Presets;
using System.Xml.Linq;
using System.IO;
using Microsoft.Win32;

namespace SuperCygwin
{
    public partial class PresetsForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public static PresetsForm MainForm;
        EventManager em;
        public PresetsForm()
        {
            InitializeComponent();
            MainForm = this;
        }

        private void Presets_Load(object sender, EventArgs e)
        {
            tree.Nodes.Add("preset", "Presets");
            tree.Nodes.Add("recent", "Recent");
            //System.IO.File.WriteAllText("presets.json", JsonConvert.SerializeObject(Presets, Formatting.Indented));
            if (File.Exists("presets.json"))
            {
                string presetFile = System.IO.File.ReadAllText("presets.json");
                Object data = null;
                data = JsonConvert.DeserializeObject<List<Preset>>(presetFile);

                foreach (Preset p in (List<Preset>)data)
                    if (p.Args.StartsWith("/usr/bin/ssh"))
                        Presets.Add(new SSHPreset(p));
                    else
                        Presets.Add(p);

                foreach (Preset p in Presets)
                    AddProc(p);
            }
            em = EventManager.MainInstance;
            try
            {
                em.NewProcess += new EventManager.NewProcessEventHandler(em_NewProcess);
                tree.MouseClick += new MouseEventHandler(treeView1_MouseClick);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode n;
                n = tree.GetNodeAt(e.Location);
                if (n.Tag is Preset)
                {
                    nameToolStripMenuItem.Text = n.Text;
                    contextMenuStrip1.Tag = n.Tag;
                    editToolStripMenuItem.Text = n.Parent.Name == "recent" ? "Save" : "Edit";
                    contextMenuStrip1.Show(e.Location);
                    tree.SelectedNode = n;
                }
            }
        }
        List<Preset> Presets = new List<Preset>();

        public void AddPreset(string Name, ProcessStartInfo PSI)
        {
            Preset p = new Preset(Name, PSI.FileName, PSI.Arguments);
            AddPreset(p);
        }
        public void AddPreset(Preset p)
        {
            if (!Presets.Contains(p))
            {
                //if(p.Args.StartsWith("/usr/bin/ssh "))
                //    p
                Presets.Add(p);
                AddProc(p);
            }
            SavePresets();
        }
        void SavePresets()
        {
            System.IO.File.WriteAllText("presets.json", JsonConvert.SerializeObject(Presets, Formatting.Indented));
        }
        void em_NewProcess(object sender, EventManager.NewProcessEventArgs e)
        {
            TreeNode n = new TreeNode();
            Preset p = new Preset(e.Process.FileName + " " + e.Process.Arguments, e.Process.FileName, e.Process.Arguments);
            n.Name = p.Name;
            n.Text = p.Name;
            n.Tag = p;
            tree.Nodes["recent"].Nodes.Add(n);
        }
        void AddProc(Preset p)
        {
            TreeNode n = new TreeNode();
            n.Name = p.Name;
            n.Text = p.Name;
            n.Tag = p;
            tree.Nodes["preset"].Nodes.Add(n);
        }

        private void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void tree_DoubleClick(object sender, EventArgs e)
        {
            if (tree.SelectedNode != null && tree.SelectedNode.Tag is Preset)
            {
                em.RaiseNewProcess(((Preset)tree.SelectedNode.Tag).PSI);
            }
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            NewPreset n = new NewPreset();
            em.RaiseNewForm(n);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            em.RaiseNewProcess(((Preset)contextMenuStrip1.Tag).PSI);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewPreset n = new NewPreset((Preset)contextMenuStrip1.Tag);
            em.RaiseNewForm(n);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preset p=(Preset)contextMenuStrip1.Tag;
            if (MessageBox.Show("Are you sure you want to permanently delete " + p.Name + "?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                Presets.Remove(p);
                tree.Nodes["preset"].Nodes.RemoveByKey(p.Name);
                //MessageBox.Show(p.Name + "Has been deleted", "Preset Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SavePresets();
            }
        }

        private void cygwinSSHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportFromSuperPuTTY("Cygwin");
        }

        private void puTTYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportFromPuTTY();
        }

        void ImportFromPuTTY()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\SimonTatham\PuTTY\Sessions\");
            
            foreach (string session in registryKey.GetSubKeyNames())
            {
                Preset p = new Preset();
                RegistryKey entry = registryKey.OpenSubKey(session);
                p.Name = session + " - Putty Import";
                string proto = (string)entry.GetValue("Protocol");
                string host = (string)entry.GetValue("HostName");
                int port = (int)entry.GetValue("PortNumber");
                string user = (string)entry.GetValue("UserName");
                string forwards = (string)entry.GetValue("PortForwardings");
                string ps = session;
                string tgt = (user != "" ? user + "@" : "") + host;

                //Reformat forwards for cygwin SSH
                forwards=forwards.Replace('=', ':').Replace(',',' ').Replace("L","-L").Replace("R","-R");
                if (!Directory.Exists("keys"))
                    Directory.CreateDirectory("keys");

                if (proto == "ssh")
                {
                    string key = "";
                    key = (string)entry.GetValue("PublicKeyFile", "");
                    //Possibly give option to install cygwin ssh key? 
                    p.Type = PresetType.Mintty;
                    p.Path = @"C:\cygwin\bin\mintty.exe";
                    p.Args = string.Format("/usr/bin/ssh {0} -P{1} {2}", tgt, port, forwards);
                    PresetsForm.MainForm.AddPreset(p);
                }
                else if (proto == "telnet")
                {
                    string key = "";
                    key = (string)entry.GetValue("PublicKeyFile", "");
                    //Possibly give option to install cygwin ssh key? 
                    p.Type = PresetType.Mintty;
                    p.Path = @"C:\cygwin\bin\mintty.exe";
                    p.Args = string.Format("/usr/bin/telnet {0} {1}", host, port);
                    PresetsForm.MainForm.AddPreset(p);
                }
                else
                {
                    MessageBox.Show(string.Format("Sorry, but the session {0} uses protocol {1} which is currently not supported.",session,proto));
                }
                //NewPreset frm = new NewPreset(p);
            }
        }

        void ImportFromSuperPuTTY(string prefer)
        {

            if (prefer == "")
                return;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.DefaultExt = "xml";
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;
            
            XElement x = XElement.Parse(File.ReadAllText(ofd.FileName));
            
            foreach (XElement el in x.Elements("SessionData"))
            {
                Preset p = new Preset();
                p.Name = el.Attribute("SessionName").Value + " - SuperPutty Import";
                string proto = el.Attribute("Proto").Value;
                string host = el.Attribute("Host").Value;
                string port = el.Attribute("Port").Value;
                string user = el.Attribute("Username").Value;
                string ps = el.Attribute("PuttySession").Value;
                string tgt = (user != "" ? user + "@" : "") + host;
                if (proto == "SSH")
                {
                    if (prefer == "Putty")
                    {
                        string key="";
                        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\SimonTatham\PuTTY\Sessions\" + ps);

                        if (registryKey != null)
                            key = (string)registryKey.GetValue("PublicKeyFile", "");
                        p.Type = PresetType.Putty;
                        p.Path = "Putty.exe";
                        p.Args = string.Format("-ssh {0} -P {1} {2}", tgt, port, key != "" ? "-i \"" + key + "\"" : "");
                    }
                    else
                    {
                        p.Type = PresetType.Mintty;
                        p.Path = @"C:\cygwin\bin\mintty.exe";
                        p.Args = string.Format("/usr/bin/ssh {0} -P{1}", tgt, port);
                    }
                }
                //NewPreset frm = new NewPreset(p);
                PresetsForm.MainForm.AddPreset(p);
            }
        }

        private void installKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preset p = (Preset)contextMenuStrip1.Tag;
            if (p.Path.Contains("mintty"))
            {
                if (DialogResult.Yes == MessageBox.Show("Install Cygwin public key onto remote server?", "Install Key", MessageBoxButtons.YesNo))
                {
                    string HOME = Directory.GetDirectories(@"C:\cygwin\home\")[0];
                    string key = File.ReadAllText(Path.Combine(HOME, ".ssh/id_rsa.pub"));
                    ProcessStartInfo psi = p.PSI;
                    psi.Arguments += string.Format(" '( echo \"{0}\" >> ~/.ssh/authorized_keys; echo Key Installed; echo Press \\[Enter\\] key to exit...; read; )'", key);
                    em.RaiseNewProcess(psi);
                }
            }
            else
                MessageBox.Show("Sorry, only cygwin is supported for installing keys.", "Cannot Install", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
