using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SuperCygwin
{
    public class EventManager
    {
        public static EventManager MainInstance;

        public EventManager()
        {
            MainInstance = this;
        }

        /* BEGIN New Process Event */
        public class NewProcessEventArgs : EventArgs
        {
            public ProcessStartInfo Process { get; set; }
            public NewProcessEventArgs(ProcessStartInfo info)
            {
                Process = info;
            }
        }
        public delegate void NewProcessEventHandler(object sender, NewProcessEventArgs e);
        public event NewProcessEventHandler NewProcess;
        public void RaiseNewProcess(ProcessStartInfo psi)
        {
            NewProcess(this, new NewProcessEventArgs(psi));
        }
        public void RaiseNewProcess(Object sender, ProcessStartInfo psi)
        {
            NewProcess(sender, new NewProcessEventArgs(psi));
        }
        /* END New Process Event */

        /* BEGIN New Form Event */
        public class NewFormEventArgs : EventArgs
        {
            public DockContent Form { get; set; }
            public DockState DockState { get; set; }
            public NewFormEventArgs(DockContent form)
            {
                Form = form;
                DockState = DockState.Document;
            }
            public NewFormEventArgs(DockContent form, DockState dockState)
            {
                Form = form;
                DockState = dockState;
            }
        }
        public delegate void NewFormEventHandler(object sender, NewFormEventArgs e);
        public event NewFormEventHandler NewForm;
        public void RaiseNewForm(DockContent form)
        {
            NewForm(this, new NewFormEventArgs(form));
        }
        public void RaiseNewForm(DockContent form, DockState dockState)
        {
            NewForm(this, new NewFormEventArgs(form, dockState));
        }
        /* END New Process Event */
    }
}
