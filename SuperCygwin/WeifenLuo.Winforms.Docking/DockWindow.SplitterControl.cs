using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    public partial class DockWindow
    {
        private class SplitterControl : SplitterBase
        {
            protected override int SplitterSize
            {
                get { return Measures.SplitterSize; }
            }

            protected override void StartDrag()
            {
                DockWindow window = Parent as DockWindow;
                if (window == null)
                    return;

                window.DockPanel.BeginDrag(window, window.RectangleToScreen(Bounds));
            }
            protected override void WndProc(ref Message m)
            {

                /** /
                if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDOWN)
                {
                    uint ret = NativeMethods.SendMessage(Handle, (int)Win32.Msgs.WM_NCHITTEST, 0, (uint)m.LParam);
                    MessageBox.Show(this.GetType().Name + " NCHITTEST " + ((Win32.HitTest)ret).ToString());
                }/**/
                base.WndProc(ref m);
            }
        }
    }
}
