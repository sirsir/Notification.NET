using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmivoiceWatcher
{
    public partial class FormTaskbarBlink : Form
    {
        #region hide from task list
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
        #endregion

        #region flashwindow
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        public const UInt32 FLASHW_ALL = 3;
        //Flash Taskbar
        public const UInt32 FLASHW_TRAY = 2;
        // Flash continuously until the window comes to the foreground. 
        public const UInt32 FLASHW_TIMERNOFG = 12;
        // Stop Flash 
        public const UInt32 FLASHW_STOP = 0;




        // Do the flashing - this does not involve a raincoat.
        public static bool FlashWindowEx(Form form)
        {
            IntPtr hWnd = form.Handle;
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            //fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
            fInfo.dwFlags = FLASHW_ALL;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }

        delegate bool FlashFormCallback();
        public bool FlashForm()
        {

            if (this.InvokeRequired)
            {
                FlashFormCallback d = new FlashFormCallback(FlashForm);
                this.Invoke(d, new object[] { });

                return true;
            }
            else
            {
                if (Win2000OrLater)
                {
                    Form form = this;
                    IntPtr hWnd = form.Handle;
                    FLASHWINFO fInfo = new FLASHWINFO();

                    fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
                    fInfo.hwnd = hWnd;
                    //fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
                    fInfo.dwFlags = FLASHW_TRAY;
                    fInfo.uCount = UInt32.MaxValue;
                    fInfo.dwTimeout = 0;

                    return FlashWindowEx(ref fInfo);
                }
                else
                {
                    //this.ShowInTaskbar = false;
                    this.Hide();

                    return false;
                }

                

            }

        }


        delegate bool FlashFormStopCallback();
        public bool FlashFormStop()
        {
            if (this.InvokeRequired)
            {
                FlashFormStopCallback d = new FlashFormStopCallback(FlashFormStop);
                this.Invoke(d, new object[] { });

                return true;
            }
            else
            {
                if (Win2000OrLater)
                {
                    Form form = this;
                    IntPtr hWnd = form.Handle;
                    FLASHWINFO fInfo = new FLASHWINFO();

                    fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
                    fInfo.hwnd = hWnd;
                    fInfo.dwFlags = FLASHW_STOP;
                    fInfo.uCount = UInt32.MaxValue;
                    fInfo.dwTimeout = 0;

                    return FlashWindowEx(ref fInfo);
                }else
                {
                    //this.ShowInTaskbar = false;
                    this.Hide();

                    return false;
                }
                
                
            }


        }

        /// <summary>
        /// A boolean value indicating whether the application is running on Windows 2000 or later.
        /// </summary>
        private static bool Win2000OrLater
        {
            get { return System.Environment.OSVersion.Version.Major >= 5; }
            //get { return System.Environment.OSVersion.Version.Major > 0.5; }
        }




        #endregion


        public FormTaskbarBlink()
        {
            InitializeComponent();

            this.Text = Globals.GUI.ApplicationName;
            this.WindowState = FormWindowState.Minimized;
            this.FlashForm();

        }

        private void FormTaskbarBlink_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void FormTaskbarBlink_SizeChanged(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            AmivoiceWatcher.TrayMenu.ShowFormLongNotification();
            //this.Parent.Show();
        }
    }
}
