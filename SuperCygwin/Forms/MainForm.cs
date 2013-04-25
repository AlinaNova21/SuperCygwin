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
using System.Threading;
using System.Net;

namespace SuperCygwin
{
    public partial class MainForm : Form
    {
        DockPanel dp;
        FormWindowState oldWindowState=FormWindowState.Normal;
        EventManager em = new EventManager();
        public static MainForm Main;
        MARGINS margins=new MARGINS();
        static public Color Trans;


        public MainForm()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            Main = this;
            DoubleBuffered = true;
            AllowTransparency = true;
            Trans=Color.FromArgb(255, 123,213,231);
            BackColor = Trans;//Color.FromArgb(255, 1, 2, 3);
            TransparencyKey = Trans;
            if (Native.AeroEnabled())
            {
                margins.BottomHeight = -1;
                margins.TopHeight = -1;
                margins.LeftWidth = -1;
                margins.RightWidth = -1;
                Native.DwmExtendFrameIntoClientArea(Handle, ref margins);
                margins.BottomHeight = 9;
                margins.TopHeight = 30;
                margins.LeftWidth = 9;
                margins.RightWidth = 9;
            }
            

            dp = new DockPanel();
            dp.BorderStyle = BorderStyle.None;
            dp.DocumentStyle = DocumentStyle.DockingWindow;
            dp.Dock = DockStyle.Fill;
            //dp.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
            //dp.Location = new Point(10, 10);
            //dp.Size = new Size(Width - 20, Height - 40);
            dp.Show();
            dp.SkinStyle = WeifenLuo.WinFormsUI.Docking.Skins.Style.VisualStudio2005;
            dp.Skin = CreateVisualStudio2005();
            Controls.Add(dp);
            dp.Paint += new PaintEventHandler(dp_Paint);
            dp.ActiveDocumentChanged += new EventHandler(dp_ActiveDocumentChanged);

            statusStrip1.SendToBack();
            //toolStrip1.SendToBack();
            //toolStrip1.Visible = Program.dev;
            ni.DoubleClick += new EventHandler(ni_DoubleClick);

            FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            Resize += new EventHandler(Form1_Resize);
            //em.NewProcess += new EventManager.NewProcessEventHandler(em_NewProcess);
            ProcessContainer.RegisterNewProcessHandler(dp, em);

            PresetsForm p = new PresetsForm();
            if(Program.Config.AutoHidepresets)
                p.Show(dp, DockState.DockRightAutoHide);
            else
                p.Show(dp, DockState.DockRight);

            if (Program.dev)
            {
                DebugForm dbg = new DebugForm();
                dbg.Show(dp, DockState.Document);
            }

            em.NewForm += new EventManager.NewFormEventHandler(em_NewForm);

            version.Text = string.Format("Version: {0}", Application.ProductVersion);
            Thread UpdateCheck = new Thread(new ThreadStart(()=>{
                WebClient wc = new WebClient();
                try
                {
                    string type=Program.Config.DevBuilds ? "snapshot" : "latest";
                    string ver = wc.DownloadString(string.Format("http://ags131.co/supercygwin/{0}-version.txt", type));
                    if (new Version(ver).CompareTo(new Version(Application.ProductVersion)) > 0)
                        if (DialogResult.Yes == MessageBox.Show(string.Format("A new version is available: {0}Current: {1}{0}Latest: {2}{0}Do you want to update?", "\n", Application.ProductVersion, ver), "Update", MessageBoxButtons.YesNo))
                        {
                            Process.Start(string.Format("http://ags131.co/supercygwin/SuperCygwin-{0}.zip", ver));
                            Application.Exit();
                        }
                }
                catch (Exception ex){}
            }));
            UpdateCheck.Start();
            //Paint += new PaintEventHandler(MainForm_Paint);
            //Activated += new EventHandler(MainForm_Activated);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //e.Graphics.CompositingMode = CompositingMode.SourceOver;
            //e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            if (Native.AeroEnabled())
            {
                e.Graphics.Clear(BackColor);
            }
            else
            {
                e.Graphics.Clear(Color.Red);//Color.FromArgb(0xC2, 0xD9, 0xF7));
            }

            e.Graphics.FillRectangle(new SolidBrush(BackColor),
                    Rectangle.FromLTRB(
                        margins.LeftWidth - 0,
                        margins.TopHeight - 0,
                        Width - margins.RightWidth - 0,
                        Height - margins.BottomHeight - 0));
        }

        void dp_Paint(object sender, PaintEventArgs e)
        {   
            //Invalidate();
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
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        bool _marginOk = true;
        protected override void WndProc(ref Message m)
        {
            //base.WndProc(ref m); return;
            int WM_NCCALCSIZE = 0x83;
            int WM_NCHITTEST = 0x84;
            IntPtr result;
            
            if (m.Msg == (int)WM.LBUTTONDOWN)
            {
                uint ret = NativeMethods.SendMessage(Handle, WM_NCHITTEST, 0, (uint)m.LParam);
                //MessageBox.Show(this.GetType().Name + " NCHITTEST " + (ret).ToString());
            }

            int dwmHandled = Dwm.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, out result);

            if (dwmHandled == 1)
            {
                m.Result = result;
                return;
            }

            if (m.Msg == WM_NCCALCSIZE && (int)m.WParam == 1)
            {
                NCCALCSIZE_PARAMS nccsp = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(NCCALCSIZE_PARAMS));

                // Adjust (shrink) the client rectangle to accommodate the border:
                nccsp.rect0.Top += 0;
                nccsp.rect0.Bottom += 0;
                nccsp.rect0.Left += 0;
                nccsp.rect0.Right += 0;

                //_marginOk = true;
                if (!_marginOk)
                {
                    //Set what client area would be for passing to DwmExtendIntoClientArea
                    margins.TopHeight = nccsp.rect2.Top - nccsp.rect1.Top;
                    margins.LeftWidth = nccsp.rect2.Left - nccsp.rect1.Left;
                    margins.BottomHeight = nccsp.rect1.Bottom - nccsp.rect2.Bottom;
                    margins.RightWidth = nccsp.rect1.Right - nccsp.rect2.Right;
                    _marginOk = true;
                }

                Marshal.StructureToPtr(nccsp, m.LParam, false);

                m.Result = IntPtr.Zero;
            }
            else if (m.Msg == WM_NCHITTEST && (int)m.Result == 0)
            {
                m.Result = HitTestNCA(m.HWnd, m.WParam, m.LParam);
            }
            else
            {
                base.WndProc(ref m);
            }
        }
        /// <summary>
        /// Equivalent to the LoWord C Macro
        /// </summary>
        /// <param name="dwValue"></param>
        /// <returns></returns>
        public static int LoWord(int dwValue)
        {
            return dwValue & 0xFFFF;
        }

        /// <summary>
        /// Equivalent to the HiWord C Macro
        /// </summary>
        /// <param name="dwValue"></param>
        /// <returns></returns>
        public static int HiWord(int dwValue)
        {
            return (dwValue >> 16) & 0xFFFF;
        }

        private IntPtr HitTestNCA(IntPtr hwnd, IntPtr wparam, IntPtr lparam)
        {
            int HTNOWHERE = 0;
            int HTCLIENT = 1;
            int HTCAPTION = 2;
            int HTGROWBOX = 4;
            int HTSIZE = HTGROWBOX;
            int HTMINBUTTON = 8;
            int HTMAXBUTTON = 9;
            int HTLEFT = 10;
            int HTRIGHT = 11;
            int HTTOP = 12;
            int HTTOPLEFT = 13;
            int HTTOPRIGHT = 14;
            int HTBOTTOM = 15;
            int HTBOTTOMLEFT = 16;
            int HTBOTTOMRIGHT = 17;
            int HTREDUCE = HTMINBUTTON;
            int HTZOOM = HTMAXBUTTON;
            int HTSIZEFIRST = HTLEFT;
            int HTSIZELAST = HTBOTTOMRIGHT;

            Point p = new Point(LoWord((int)lparam), HiWord((int)lparam));

            Rectangle topleft = RectangleToScreen(new Rectangle(0, 0, margins.LeftWidth, margins.LeftWidth));

            if (topleft.Contains(p))
                return new IntPtr(HTTOPLEFT);

            Rectangle topright = RectangleToScreen(new Rectangle(Width - margins.RightWidth, 0, margins.RightWidth, margins.RightWidth));

            if (topright.Contains(p))
                return new IntPtr(HTTOPRIGHT);

            Rectangle botleft = RectangleToScreen(new Rectangle(0, Height - margins.BottomHeight, margins.LeftWidth, margins.BottomHeight));

            if (botleft.Contains(p))
                return new IntPtr(HTBOTTOMLEFT);

            Rectangle botright = RectangleToScreen(new Rectangle(Width - margins.RightWidth, Height - margins.BottomHeight, margins.RightWidth, margins.BottomHeight));

            if (botright.Contains(p))
                return new IntPtr(HTBOTTOMRIGHT);

            Rectangle top = RectangleToScreen(new Rectangle(0, 0, Width, margins.LeftWidth));

            if (top.Contains(p))
                return new IntPtr(HTTOP);

            Rectangle cap = RectangleToScreen(new Rectangle(0, margins.LeftWidth, Width, margins.TopHeight - margins.LeftWidth));

            if (cap.Contains(p))
                return new IntPtr(HTCAPTION);

            Rectangle left = RectangleToScreen(new Rectangle(0, 0, margins.LeftWidth, Height));

            if (left.Contains(p))
                return new IntPtr(HTLEFT);

            Rectangle right = RectangleToScreen(new Rectangle(Width - margins.RightWidth, 0, margins.RightWidth, Height));

            if (right.Contains(p))
                return new IntPtr(HTRIGHT);

            Rectangle bottom = RectangleToScreen(new Rectangle(0, Height - margins.BottomHeight, Width, margins.BottomHeight));

            if (bottom.Contains(p))
                return new IntPtr(HTBOTTOM);

            return new IntPtr(HTCLIENT);
        }

        private DockPanelSkin CreateVisualStudio2005()
        {
            DockPanelSkin skin = dp.Skin;// new DockPanelSkin();
            dp.ForeColor = Color.FromArgb(0, 1, 2, 3);
            Color c = Color.FromArgb(255, 0, 0, 0);
            Color t = Color.FromArgb(255, 1, 2, 3);

            skin.AutoHideStripSkin.DockStripGradient.StartColor = Color.FromArgb(0, 0, 0, 0);
            skin.AutoHideStripSkin.DockStripGradient.EndColor = Color.FromArgb(0, 0, 0, 0);
            skin.AutoHideStripSkin.DockStripGradient.LinearGradientMode = LinearGradientMode.Horizontal;

            skin.AutoHideStripSkin.TabGradient.TextColor = SystemColors.ControlDarkDark;
            skin.AutoHideStripSkin.TabGradient.StartColor = SystemColors.Control;
            skin.AutoHideStripSkin.TabGradient.EndColor = SystemColors.Control;

            skin.DockPaneStripSkin.DocumentGradient.DockStripGradient.StartColor = Color.FromArgb(0,0,0,0);
            skin.DockPaneStripSkin.DocumentGradient.DockStripGradient.EndColor = Color.FromArgb(0,0,0,0);

            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.StartColor = SystemColors.ControlLightLight;
            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.EndColor = SystemColors.ControlLight;
            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.TextColor = t;
            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.LinearGradientMode = LinearGradientMode.Vertical;
            
            skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.StartColor = SystemColors.ControlLight;
            skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.EndColor = SystemColors.ControlLight;
            skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.TextColor = t;

            skin.DockPaneStripSkin.ToolWindowGradient.DockStripGradient.StartColor = SystemColors.ControlLight;
            skin.DockPaneStripSkin.ToolWindowGradient.DockStripGradient.EndColor = SystemColors.ControlLight;

            skin.DockPaneStripSkin.ToolWindowGradient.ActiveTabGradient.StartColor = SystemColors.Control;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveTabGradient.EndColor = SystemColors.Control;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveTabGradient.TextColor = Color.FromArgb(1, 2, 3);

            skin.DockPaneStripSkin.ToolWindowGradient.InactiveTabGradient.StartColor = Color.Transparent;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveTabGradient.EndColor = Color.Transparent;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveTabGradient.TextColor = Color.FromArgb(1, 2, 3);

            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.StartColor = SystemColors.GradientActiveCaption;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.EndColor = SystemColors.ActiveCaption;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.TextColor = Color.FromArgb(1, 2, 3);

            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.StartColor = SystemColors.GradientInactiveCaption;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.EndColor = SystemColors.InactiveCaption;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.TextColor = Color.FromArgb(1, 2, 3);

            return skin;
        }
    }
}
