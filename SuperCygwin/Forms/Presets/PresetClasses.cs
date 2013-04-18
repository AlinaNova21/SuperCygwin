using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Diagnostics;

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

}
