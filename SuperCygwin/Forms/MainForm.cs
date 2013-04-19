using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WeifenLuo.WinFormsUI.Docking;
using System.Drawing.Drawing2D;
using SuperCygwin.Forms;

namespace SuperCygwin
{
    public partial class MainForm : Form
    {
        DockPanel dp;
        FormWindowState oldWindowState=FormWindowState.Normal;
        EventManager em = new EventManager();

        public MainForm()
        {
            InitializeComponent();
            dp = new DockPanel();
            dp.DocumentStyle = DocumentStyle.DockingWindow;
            dp.Dock = DockStyle.Fill;
            dp.Show();
            dp.SkinStyle = WeifenLuo.WinFormsUI.Docking.Skins.Style.VisualStudio2005;
            dp.Skin = CreateVisualStudio2005();
            Controls.Add(dp);

            dp.ActiveDocumentChanged += new EventHandler(dp_ActiveDocumentChanged);


            toolStrip1.SendToBack();
            toolStrip1.Visible = Program.dev;
            ni.DoubleClick += new EventHandler(ni_DoubleClick);

            FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            Resize += new EventHandler(Form1_Resize);
            //em.NewProcess += new EventManager.NewProcessEventHandler(em_NewProcess);
            ProcessContainer.RegisterNewProcessHandler(dp, em);

            PresetsForm p = new PresetsForm();
            p.Show(dp, DockState.DockRight);

            if (Program.dev)
            {
                DebugForm dbg = new DebugForm();
                dbg.Show(dp, DockState.Document);
            }

            em.NewForm += new EventManager.NewFormEventHandler(em_NewForm);
        }

        void em_NewForm(object sender, EventManager.NewFormEventArgs e)
        {
            e.Form.Show(dp, e.DockState);
        }

        void dp_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (dp.ActiveDocument != null && dp.ActiveDocument.GetType() == typeof(ProcessContainer))
                ((ProcessContainer)dp.ActiveDocument).SetFocus();
        }

        void em_NewProcess(object sender, EventManager.NewProcessEventArgs e)
        {
            //ProcessContainer.Create(dp, @"C:\cygwin\bin\mintty.exe ", toolStripTextBox1.Text);
        }

        void ni_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                WindowState = oldWindowState;
            else
                WindowState = FormWindowState.Minimized;
        }

        void Form1_Resize(object sender, EventArgs e)
        {
            //ShowInTaskbar = WindowState != FormWindowState.Minimized;
            //if (ShowInTaskbar) oldWindowState = WindowState;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            IDockContent[] docs = dp.Documents.ToArray();
            foreach (IDockContent doc in docs)
                ((DockContent)doc).Close();
                //((ProcessContainer)doc).Close();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            /** /
            ProcessContainer.Create(dp, "notepad.exe");
            ProcessContainer.Create(dp, @"C:\cygwin\bin\mintty.exe");
            ProcessContainer.Create(dp, @"C:\cygwin\bin\mintty.exe");
            ProcessContainer.Create(dp, @"C:\cygwin\bin\mintty.exe");
            ProcessContainer.Create(dp, @"C:\cygwin\bin\mintty.exe");
            /**/
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //ProcessContainer.Create(dp, @"C:\cygwin\bin\mintty.exe ",toolStripTextBox1.Text);
            ProcessStartInfo psi = new ProcessStartInfo(@"C:\cygwin\bin\mintty.exe", toolStripTextBox1.Text);
            em.RaiseNewProcess(this,psi);
        }

        private DockPanelSkin CreateVisualStudio2005()
        {
            DockPanelSkin skin = new DockPanelSkin();

            skin.AutoHideStripSkin.DockStripGradient.StartColor = SystemColors.ControlLight;
            skin.AutoHideStripSkin.DockStripGradient.EndColor = SystemColors.ControlLight;
            skin.AutoHideStripSkin.TabGradient.TextColor = SystemColors.ControlDarkDark;

            skin.DockPaneStripSkin.DocumentGradient.DockStripGradient.StartColor = SystemColors.Control;
            skin.DockPaneStripSkin.DocumentGradient.DockStripGradient.EndColor = SystemColors.Control;
            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.StartColor = SystemColors.ControlLightLight;
            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.EndColor = SystemColors.ControlLight;
            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.LinearGradientMode = LinearGradientMode.Vertical;
            skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.StartColor = SystemColors.ControlLight;
            skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.EndColor = SystemColors.ControlLight;

            skin.DockPaneStripSkin.ToolWindowGradient.DockStripGradient.StartColor = SystemColors.ControlLight;
            skin.DockPaneStripSkin.ToolWindowGradient.DockStripGradient.EndColor = SystemColors.ControlLight;

            skin.DockPaneStripSkin.ToolWindowGradient.ActiveTabGradient.StartColor = SystemColors.Control;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveTabGradient.EndColor = SystemColors.Control;

            skin.DockPaneStripSkin.ToolWindowGradient.InactiveTabGradient.StartColor = Color.Transparent;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveTabGradient.EndColor = Color.Transparent;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveTabGradient.TextColor = SystemColors.ControlDarkDark;

            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.StartColor = SystemColors.GradientActiveCaption;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.EndColor = SystemColors.ActiveCaption;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.TextColor = SystemColors.ActiveCaptionText;

            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.StartColor = SystemColors.GradientInactiveCaption;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.EndColor = SystemColors.InactiveCaption;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.TextColor = SystemColors.InactiveCaptionText;

            return skin;
        }
    }
}
