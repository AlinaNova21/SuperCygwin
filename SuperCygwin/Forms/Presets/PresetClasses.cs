using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SuperCygwin.Forms.Presets
{
    public class Preset
    {
        protected string _args;
        protected string _path;
        protected string _name;
        [Browsable(false)]
        public PresetType Type;

        [Category("Process")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [Category("Process")]
        [EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        [Category("Process")]
        public string Args
        {
            get { return _args; }
            set { _args = value; }
        }

        [JsonIgnore]
        [Browsable(false)]
        public ProcessStartInfo PSI
        {
            get
            {
                return new ProcessStartInfo(Path, Args);
            }
            set { }
        }
        
        public Preset()
        {
            Name = "Change Me";
            Type = PresetType.Normal;
            Path = Program.Config.MinTTYPath;
            Args = "-";
        }
        public Preset(string name, string path, string args)
        {
            Name = name;
            Type = PresetType.Normal;
            Args = args;
            Path = path;
        }
        public Preset(string name, PresetType type, string path, string args)
        {
            Name = name;
            Type = type;
            Args = args;
            Path = path;
        }
    }
    public enum PresetType
    {
        Normal = 0,
        Mintty = 1,
        Putty = 2
    }
    public class SSHPreset : Preset
    {
        Preset orig;
        //[ReadOnly(true)]
        public new string Args
        {
            get
            {
                _args = string.Format("{0} {1} -P{2} {3}",
                    Program.Config.SSHPath,
                    (Username != "" ? Username + "@" : "root@") + Hostname,
                    Port,
                    "",//PrivateKey==""?"":"-i \""+PrivateKey+"\"",
                    Forwards);
                return _args;
            }
            set
            {
                _args = value;
                Forwards = "";
                Hostname = "";
                Port = 22;
                value = value.Replace(Program.Config.SSHPath, "");
                string[] val=Regex.Replace(value,"(-[a-zA-Z]) ",@"\1").Split(new string[]{" "},StringSplitOptions.RemoveEmptyEntries);
                foreach (string v in val)
                {
                    if (v.Length < 2) continue;
                    switch (v[1])
                    {
                        case 'P':
                            Port = int.Parse(v.Substring(2));
                            if (Port == 0) Port = 22;
                            break;
                        case 'i':
                            PrivateKey = v.Substring(2);
                            break;
                        case 'L':
                        case 'R':
                            Forwards += v + ' ';
                            break;
                        default:
                            if(Hostname=="")
                            {
                                if (v.Contains('@'))
                                {
                                    Username = v.Split('@')[0];
                                    Hostname = v.Split('@')[1];
                                }
                                else
                                {
                                    Username = "root";
                                    Hostname = v;
                                }
                            }
                            break;
                    }
                }
            }
        }
        [Category("SSH Connection")]
        public string Hostname { get; set; }
        [Category("SSH Connection")]
        public int Port { get; set; }
        [Category("SSH Authentication")]
        public string Username { get; set; }
        [Category("SSH Authentication")]
        [EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string PrivateKey { get; set; }
        [Category("SSH Tunnels")]
        public string Forwards { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        public new ProcessStartInfo PSI
        {
            get
            {
                return new ProcessStartInfo(Path, Args);
            }
            set { }
        }

        public SSHPreset()
            : base()
        {
            Path = Program.Config.MinTTYPath;
        }
        public SSHPreset(Preset p)
            : base()
        {
            orig = p;
            get();
        }
        void get()
        {
            Path = orig.Path;
            Args = orig.Args;
            Path = orig.Path;
            Name = orig.Name;
        }
        void set()
        {
            if (orig == null) return;
            orig.Path = Path;
            orig.Args = Args;
            orig.Path = Path;
            orig.Name = Name;
        }
    }
}
