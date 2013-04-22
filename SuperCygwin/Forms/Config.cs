using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SuperCygwin.Forms
{
    public partial class ConfigFrm : DockContent
    {
        public ConfigFrm()
        {
            InitializeComponent();
        }

        private void Config_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = Program.Config;
            propertyGrid1.PropertyValueChanged += AutoSaveHandler;
        }
        
        void AutoSaveHandler(object s, EventArgs e)
        {
            Program.Config.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to reset settings?", "Reset To defaults", MessageBoxButtons.YesNo))
            {
                Program.Config = new Config();
                Program.Config.Save();
                propertyGrid1.SelectedObject = Program.Config;
            }
        }
    }
}
