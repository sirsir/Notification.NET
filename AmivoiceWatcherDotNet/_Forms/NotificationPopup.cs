using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CefSharp.WinForms;
using CefSharp;
using System.Threading;
using System.Diagnostics;

namespace AmivoiceWatcher
{
    public partial class NotificationPopup : Form
    {
        public NotificationPopup(string title, string body, int duration, FormAnimator.AnimationMethod animation, FormAnimator.AnimationDirection direction, int width = 0, int height = 0)
        {

            InitializeComponent();

            if (duration <= 0)
                duration = int.MaxValue;
            else
                duration = duration * 1000;


            timer1.Interval = duration;

            if (width > 0)
            {
                this.Width = width;
            }

            if (width > 0)
            {
                this.Height = height;
            }

            InitBrowser(body);

            //_animator = new FormAnimator(this, animation, direction, 15000);
        }

        private const int maxPopup = 3;
        //private static int CurrentNotificationIdx = 0;
        private static List<string> NotificationBodies = new List<string>();
        private static readonly List<NotificationPopup> OpenNotifications = new List<NotificationPopup>();
        private bool _allowFocus;
        private FormAnimator _animator;
        private IntPtr _currentForegroundWindow;

        public static void PreparePopupWindows()
        {
            while (OpenNotifications.Count() < maxPopup) {
                int idx = OpenNotifications.Count();
                NotificationPopup toastNotification = new NotificationPopup("", "", -1, FormAnimator.AnimationMethod.Fade, FormAnimator.AnimationDirection.Up);
                
                //toastNotification.WindowState = FormWindowState.Minimized;
                //toastNotification.Visible = false;
                OpenNotifications.Add(toastNotification); Rectangle workingArea = Screen.GetWorkingArea(toastNotification);
                toastNotification.Location = new Point(workingArea.Right - toastNotification.Width,
                                          workingArea.Bottom + (-idx-1)*toastNotification.Height);

                toastNotification.Show();
            }
            

        }

        public ChromiumWebBrowser cefsharpBrowser;
        public BoundJavascriptObject4 boundJavascript;
        public void InitBrowser(string htmlBody)
        {
            boundJavascript = new BoundJavascriptObject4(this);

            if (!Cef.IsInitialized)
            {
                Cef.Initialize(new CefSettings());
                //Cef.Initialize(new CefSettings(), true, true);
            }

            cefsharpBrowser = new ChromiumWebBrowser("about:blank");

            cefsharpBrowser.Load("dummy:");
            //cefsharpBrowser.Load("about:blank");
            //cefsharpBrowser.LoadHtml(htmlBody,"");
            cefsharpBrowser.RegisterJsObject("bound", boundJavascript);

            // browser = new ChromiumWebBrowser(@"http://192.168.1.51:3000/watchercli/sirisak/notification");
            this.Controls.Add(cefsharpBrowser);

            cefsharpBrowser.Dock = DockStyle.Fill;
            //cefsharpBrowser.Dock = DockStyle.Top;

            cefsharpBrowser.FrameLoadEnd += OnBrowserFrameLoadEnd;
            //browser.RequestHandler = new RequestHandler();
        }

        private void OnBrowserFrameLoadEnd(object sender, FrameLoadEndEventArgs args)
        {
            if (args.Frame.IsMain)
            {
                args
                    .Browser
                    .MainFrame
                    .ExecuteJavaScriptAsync(
                    "document.body.style.overflow = 'hidden'");
            }
        }

        #region Methods
        public static void PopupOld(string title, string body, int duration, FormAnimator.AnimationMethod animationMethod, FormAnimator.AnimationDirection animationDirection)
        {
            NotificationPopup toastNotification = new NotificationPopup(title, body, duration, animationMethod, animationDirection);
            toastNotification.Show();
        }

        public static void Popup(string title, string body, int duration, FormAnimator.AnimationMethod animationMethod, FormAnimator.AnimationDirection animationDirection)
        {
            //Enable below if want popup
            /***************************************************
            if (OpenNotifications.Count == 0)
            {
                PreparePopupWindows();
            }

            if (CurrentNotificationIdx > OpenNotifications.Count-1)
            {
                CurrentNotificationIdx = 0;
            }

            for (int i = 0; i < maxPopup; i++)
            {
                //OpenNotifications[i].DoubleBuffered = true;
                if (i != CurrentNotificationIdx)
                {
                    //OpenNotifications[i].WindowState = FormWindowState.Minimized;
                    //OpenNotifications[i].Visible = false;

                }
                
            }

            //OpenNotifications[OpenNotificationsIdx].cefsharpBrowser.Load("dummy:");
            //OpenNotifications[OpenNotificationsIdx].cefsharpBrowser.LoadHtml(body,"about:blank");
            //OpenNotifications[OpenNotificationsIdx].cefsharpBrowser.LoadHtml("<html><body><h1>Hello world!</h1></body></html>", "http://example/");
            OpenNotifications[CurrentNotificationIdx].cefsharpBrowser.LoadHtml(body, "http://example/");
            NotificationBodies.Add(body);
            //OpenNotifications[OpenNotificationsIdx].Visible = true;
            //OpenNotifications[OpenNotificationsIdx].WindowState = FormWindowState.Normal;
            OpenNotifications[CurrentNotificationIdx].Activate();
            OpenNotifications[CurrentNotificationIdx].cefsharpBrowser.Refresh();
            OpenNotifications[CurrentNotificationIdx].Refresh();
            CurrentNotificationIdx++;

    *///////////////////////
            
        }

        public new void Show()
        {
            // Determine the current foreground window so it can be reactivated each time this form tries to get the focus
            _currentForegroundWindow = NativeMethods.GetForegroundWindow();

            base.Show();
        }

        public void Close(String message)
        {
            MessageBox.Show(message, "client code");
        }

        public void openLinkInDefaultBrowser(string url)
        {
            Globals.functions.openLinkInDefaultBrowser(url);

        }

        public void openSuperNotification(string url)
        {
            Globals.functions.openSuperNotification(url);

        }

        public bool HasHorizontalScrollbar
        {
            get
            {
                //var width1 = webBrowser1.Document.Body.ScrollRectangle.Width;
                //var width2 = webBrowser1.Document.Window.Size.Width;

                //return width1 > width2;
                return false;
            }
        }

        #endregion // Methods

        #region Event Handlers





        private void NotificationPopup_Load(object sender, EventArgs e)
        {

            // Display the form just above the system tray.
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Width,
                                      Screen.PrimaryScreen.WorkingArea.Height - Height);

            // Move each open form upwards to make room for this one
            foreach (NotificationPopup openForm in OpenNotifications)
            {
                openForm.Top -= Height;
            }

            OpenNotifications.Add(this);
            timer1.Start();


        }

        private void Notification_Activated(object sender, EventArgs e)
        {
            // Prevent the form taking focus when it is initially shown
            if (!_allowFocus)
            {
                // Activate the window that previously had focus
                NativeMethods.SetForegroundWindow(_currentForegroundWindow);
            }
            //Console.WriteLine(" [x] Done");
        }

        private void Notification_Shown(object sender, EventArgs e)
        {
            // Once the animation has completed the form can receive focus
            _allowFocus = true;

            // Close the form by sliding down.
            _animator.Duration = 0;
            _animator.Direction = FormAnimator.AnimationDirection.Down;

        }

        private void Notification_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Move down any open forms above this one
            foreach (NotificationPopup openForm in OpenNotifications)
            {
                if (openForm == this)
                {
                    // Remaining forms are below this one
                    break;
                }
                openForm.Top += Height;
            }

            OpenNotifications.Remove(this);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
#if DEBUG
            Globals.log.Debug("lifeTime for popup = " + timer1.Interval);
            Thread.Sleep(2000);
#endif
            Close();
        }

        private void Notification_Click(object sender, EventArgs e)
        {
            //Close();
        }

        private void labelTitle_Click(object sender, EventArgs e)
        {
            //Close();
        }

        private void labelRO_Click(object sender, EventArgs e)
        {
            //Close();
        }

        private void buttonX_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (HasHorizontalScrollbar)
            {
                //webBrowser1.Size = new Size(webBrowser1.Document.Body.ScrollRectangle.Width, webBrowser1.Document.Body.ScrollRectangle.Height);
                //Notification.ActiveForm.Size = new Size(webBrowser1.Document.Body.ScrollRectangle.Width, webBrowser1.Document.Body.ScrollRectangle.Height);
            }
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {

        }

        #endregion // Event Handlers

    }


    public class BoundJavascriptObject4
    {
        private NotificationPopup frm;
        public BoundJavascriptObject4(NotificationPopup _frm)
        {
            frm = _frm;
        }
        //We expect an exception here, so tell VS to ignore
        [DebuggerHidden]
        public void Error()
        {
            throw new Exception("This is an exception coming from C#");
        }

        //We expect an exception here, so tell VS to ignore
        public int Div(int divident, int divisor)
        {
            return divident / divisor;
        }

    }

}
