using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SuperCygwin.Forms.Presets
{
    public partial class NewPreset : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        Preset preset;
        
        public NewPreset()
        {
            InitializeComponent();
            preset = new Preset();
            propertyGrid1.SelectedObject = preset;
            //propertyGrid1.LostFocus += new EventHandler(AutoSaveHandler);
            propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(AutoSaveHandler);
            //propertyGrid1.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(AutoSaveHandler);
        }
        
        public NewPreset(Preset preset)
        {
            InitializeComponent();
            Text = "Edit Preset - " + preset.Name;
            this.preset = preset;
            propertyGrid1.SelectedObject = preset;
            propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(AutoSaveHandler);
        }

        void AutoSaveHandler(object s, EventArgs e)
        {
            PresetsForm.MainForm.AddPreset(preset);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            PresetsForm.MainForm.AddPreset(preset);
            //MessageBox.Show("Preset Saved");
        }

        private void NewPreset_Load(object sender, EventArgs e)
        {

        }

        private void sSHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            preset = new SSHPreset(preset);
            propertyGrid1.SelectedObject = preset;
        }

        private void genericToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preset p = new Preset();
            p.Args = preset.Args;
            p.Name = preset.Name;
            p.Path = preset.Path;
            p.Type = preset.Type;
            preset = p;
            propertyGrid1.SelectedObject = preset;
        }
    }
}
