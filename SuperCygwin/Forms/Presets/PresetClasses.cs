using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SuperCygwin.Forms.Presets
{
    public class Preset
    {

        public string Name
        {
            get;
            set;
        }
        [Browsable(false)]
        public PresetType Type;

        [EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Path
        {
            get;
            set;
        }
        public string Args
        {
            get;
            set;
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
            Path = @"C:\cygwin\bin\mintty.exe";
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
                return string.Format("/usr/bin/ssh {0} -P {1} {2}",
                    (Username != ""?Username+"@":"")+Hostname,
                    Port,
                    PrivateKey==""?"":"-i \""+PrivateKey+"\"",
                    Forwards);
            }
            set
            {
                Forwards = "";
                Hostname = "";
                Port = 22;
                value = value.Replace("/usr/bin/ssh ", "");
                string[] val=Regex.Replace(value,"(-[a-zA-Z]) ",@"\1").Split(' ');
                foreach (string v in val)
                {
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
                                if(v.Contains('@'))
                                {
                                    Username=v.Split('@')[0];
                                    Hostname=v.Split('@')[1];
                                }else
                                    Hostname=v;
                            }
                            break;
                    }
                }
            }
        }
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        [EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string PrivateKey { get; set; }
        public string Forwards { get; set; }
        public SSHPreset()
            : base()
        {
            Path = @"C:\cygwin\bin\mintty.exe";
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
