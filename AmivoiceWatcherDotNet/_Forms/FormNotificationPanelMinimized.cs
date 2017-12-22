using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmivoiceWatcher
{
    


    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class FormNotificationPanelMinimized : Form
    {

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
            fInfo.dwFlags = FLASHW_ALL ;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }

        public bool FlashForm()
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

        public bool FlashFormStop()
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
        }



        #endregion

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();


        public FormNotificationPanelMinimized()
        {
            InitializeComponent();

            //this.ClientSize = new System.Drawing.Size((int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.25), (int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height));
            //this.DesktopLocation = new System.Drawing.Point((int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.75), 0);
            //this.Location = new System.Drawing.Point((int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.75), 0);

            InitBrowser();

        }

        public ChromiumWebBrowser cefsharpBrowser;
        public BoundJavascriptObject2 boundJavascript;
        public void InitBrowser()
        {
            boundJavascript = new BoundJavascriptObject2(this);
            if (!Cef.IsInitialized)
            {
                Cef.Initialize(new CefSettings());
            }
            cefsharpBrowser = new ChromiumWebBrowser(Globals.functions.GetTemplatePath("LongNotificationMinimizedTemplate.html"));
            cefsharpBrowser.RegisterJsObject("bound", boundJavascript);

            this.Controls.Add(cefsharpBrowser);
            cefsharpBrowser.Dock = DockStyle.Fill;

            //browser.RequestHandler = new RequestHandler();
        }

        private void FormLongNotificationMinimized_Load(object sender, EventArgs e)
        {
            this.Location = new System.Drawing.Point((int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.75), 0);

            //webBrowser1.ObjectForScripting = this;

#if DEBUG
            //webBrowser1.Url = new Uri( @"www.google.com");

            //webBrowser1.Navigate(new System.Uri(@"http://www.google.com"));

            //webBrowser1.ScriptErrorsSuppressed = true;
#endif

            //webBrowser1.Navigate(new System.Uri(Globals.functions.GetTemplatePath("LongNotificationMinimizedTemplate.html")));

            //cefsharpBrowser.Load(Globals.functions.GetTemplatePath("LongNotificationMinimizedTemplate.html"));

            //cefsharpBrowser.Load(@"local://"+ Globals.functions.GetTemplatePath("LongNotificationMinimizedTemplate.html"));

            //cefsharpBrowser.Load(@"http://192.168.1.51:3000/watchercli/sirisak/notification");
            //cefsharpBrowser.Load(Globals.functions.GetTemplatePath("LongNotificationReload.html"));

        }

        private void FormLongNotificationMinimized_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        delegate void MouseDownOnBrowserCallback();
        public void MouseDownOnBrowser()
        {
            if (this.InvokeRequired)
            {
                MouseDownOnBrowserCallback d = new MouseDownOnBrowserCallback(MouseDownOnBrowser);
                this.Invoke(d, new object[] { });
            }
            else
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

        }


        private void Document_MouseDown(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            if (e.MouseButtonsPressed == MouseButtons.Left)
            {
                if (e.MousePosition.Y < 100 && e.MousePosition.X < this.Width*0.8)
                {

                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }

        }

        public void ExternalHide()
        {
            //this.Hide();
            AmivoiceWatcher.TrayMenu.ToggleFormLongNotification();
            //frmMinimized.Show();

        }

        //private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    if (webBrowser1.Document != null)
        //    {
        //        var htmlDoc = webBrowser1.Document;
        //        //htmlDoc.Click += Document_MouseDown;
        //        //htmlDoc.MouseDown += new System.Windows.Forms.HtmlElementEventHandler(this.button1_Click);

        //        htmlDoc.MouseDown += Document_MouseDown;
        //        //htmlDoc.MouseUp += Document_MouseUp;
        //        //htmlDoc.MouseLeave += Document_MouseUp;

        //        //htmlDoc.MouseDown += htmlDoc_MouseDown;
        //        //htmlDoc.MouseMove += htmlDoc_MouseMove;
        //        //htmlDoc.ContextMenuShowing += htmlDoc_ContextMenuShowing;
        //    }
        //}

    }

    public class BoundJavascriptObject2
    {
        private FormNotificationPanelMinimized frm;
        public BoundJavascriptObject2(FormNotificationPanelMinimized _frm)
        {
            frm = _frm;
        }

        public void Error()
        {
            throw new Exception("This is an exception coming from C#");
        }

        public int Div(int divident, int divisor)
        {
            return divident / divisor;
        }

        public void ExternalHide()
        {
            frm.ExternalHide();
        }

        public void Mousedown()
        {
            frm.MouseDownOnBrowser();
        }

        //public void ExternalReload()
        //{
        //    frm.ExternalReload();
        //}

        //public string GetSetting()
        //{
        //    return frm.GetSetting();
        //}

        //public void SaveSetting(string mainclass)
        //{
        //    frm.SaveSetting(mainclass);
        //}

        //public void UnFlashTaskbar()
        //{
        //    frm.UnFlashTaskbar();
        //}

        //public void FlashTaskbar()
        //{
        //    frm.FlashTaskbar();
        //}

        //public void SetOpacity(double transparency)
        //{
        //    frm.SetOpacity(transparency);
        //}

        //public void SetThemeMinimized(string theme)
        //{
        //    frm.SetThemeMinimized(theme);
        //}

        //public void SetTaskbarBlinkPara(bool setBlink, int unread)
        //{
        //    frm.SetTaskbarBlinkPara(setBlink, unread);
        //}


    }

}
