using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

using CefSharp;
using CefSharp.WinForms;
using System.Diagnostics;
using System.Threading;

namespace AmivoiceWatcher
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class FormNotificationPanel : Form
    {
        #region Make no-framed Window movable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        #endregion

        //private bool bln_loadDefaultSetting = false;

        private State myState = State.otherpage;
        private enum State
        {
            browser_initialized,
            otherpage,
            loading_reloadPage,
            loading_reloadPage_finished,
            loading_panelPage,
            loading_panelPage_finished,
            notification_panel_loading_preference
        }

        //private bool isLoadDefalt

        private bool _blinkTaskbar = true;
        private double _opacity = 1.0;
        private string _fontsizeClass = "normal";
        private string _theme = "dark";

        private int _lastHeight;
        private int _minHeight = 50;

        //private FormNotificationPanelMinimized frmMinimized = new FormNotificationPanelMinimized();
        private FormTaskbarBlink frmTaskbarBlink = new FormTaskbarBlink();

        public void ResizeToLastHeight()
        {
            if (this.Height <= _minHeight)
            {
                this.Height = _lastHeight;

                SetCefSharpBrowserSize();

                browser.ExecuteScriptAsync("externalFx_afterResizeWinForm();");
            }

        }

        //public object GetJsonFromCefsharp(string url)
        //{
        //    var jsonOut = CefSharpEvaluateScript(cefsharpBrowser, String.Format("externalFx_getJson({0});", url));

        //    return jsonOut;
        //}

        private void FormLongNotification_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
                Globals.log.Debug("Start moving windows");

                timer1.Interval = 1000;
                timer1.Start();
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

        }
        delegate void MouseUpOnBrowserCallback();
        public void MouseUpOnBrowser()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    MouseUpOnBrowserCallback d = new MouseUpOnBrowserCallback(MouseUpOnBrowser);
                    this.Invoke(d, new object[] { });
                }
                else
                {
                    #region snap to window size
                    //var currentPoint = this.Location;

                    //currentPoint.Y = 0;

                    //if (currentPoint.X < 0)
                    //{
                    //    currentPoint.X = 0;
                    //}
                    //else if (currentPoint.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.75)
                    //{
                    //    currentPoint.X = (int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.75);
                    //}

                    //this.Location = currentPoint;
                    #endregion

                    #region avoid_behind_taskbar
                    var currentPoint = this.Location;

                    //currentPoint.Y = 0;
                    if (currentPoint.Y > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height)
                    {
                        currentPoint.Y = (int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - 30);
                    }

                    if (currentPoint.X < 0)
                    {
                        currentPoint.X = 0;
                    }

                    if (currentPoint.X + this.Width > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width)
                    {
                        currentPoint.X = (int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - this.Width);
                    }

                    this.Location = currentPoint;
                    #endregion
                }
            }
            catch (Exception e)
            {
                Globals.log.Debug(e.ToString());
            }
        }

        private void Document_MouseDown(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            if (e.MouseButtonsPressed == MouseButtons.Left)
            {
                if (e.MousePosition.Y < 40 && e.MousePosition.X < 287)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }

        private void Document_MouseUp(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            ////== Always move to top and snap to visible area
            //var currentPoint = this.Location;

            //currentPoint.Y = 0;

            //if (currentPoint.X < 0)
            //{
            //    currentPoint.X = 0;
            //} else if (currentPoint.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.75)
            //{
            //    currentPoint.X = (int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.75);
            //}

            //this.Location = currentPoint;

        }

        public void HideOrMinimize()
        {
            try
            {
                if (this.Height > _minHeight)
                {
                    _lastHeight = this.Height;
                    this.Height = _minHeight;
                }
                else
                {
                    ResizeToLastHeight();
                }

                //== Uncomment when enable MinimizedPanel
                //this.Hide();

                //frmMinimized.Show();
            }
            catch (Exception e)
            {
                Globals.log.Debug(e.ToString());
            }
        }

        delegate void ExternalHideCallback();
        public void ExternalHide()
        {
            if (this.InvokeRequired)
            {
                ExternalHideCallback d = new ExternalHideCallback(ExternalHide);
                this.Invoke(d, new object[] { });
            }
            else
            {
                HideOrMinimize();
            }
        }

        public void ExternalReload()
        {
            try
            {
                //browser.Stop();

                var address = Globals.Notifications.URL.Watchercli();

#if DEBUG
                address = Debug.url_watchercli_invalid;
#endif

                Globals.log.Debug("FormNotificationPanel:> Loading panel from URL:" + address);

                //bln_loadDefaultSetting = false;
                //webBrowser1.Navigate(new Uri(address));

                myState = State.loading_panelPage;

                browser.LoadingStateChanged += BrowserLoadingStateChanged;
                browser.Load(address);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            // Check to see if loading is complete - this event is called twice, one when loading starts
            // second time when it's finished
            // (rather than an iframe within the main frame).
            if (!e.IsLoading)
            {
                //browser.LoadingStateChanged -= BrowserLoadingStateChanged;

                // Remove the load event handler, because we only want one snapshot of the initial page.
                var script = @"document.getElementsByTagName ('html')[0].innerHTML";
                browser.EvaluateScriptAsync(script).ContinueWith(x =>
                {
                    if (x.IsFaulted)
                    {
                        Globals.log.Warn(String.Format("Page can NOT evaluate script:> " + script));
                    }
                    else
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                        {
                            FormWaiting.CloseDelegate();
                            AmivoiceWatcher.NotificationPanel.ShowPanel();

                            var html = response.Result.ToString();
                            //startDate is the value of a HTML element.

                            switch (myState)
                            {
                                case State.loading_panelPage:
                                case State.loading_reloadPage:
                                    {
                                        if (html == "")
                                        {

                                            myState = State.otherpage;

                                            //cefsharpBrowser.LoadingStateChanged += handler;

                                            //return;
                                        }

                                        //browser.LoadingStateChanged -= handler;

                                        var pagetype = GetPageType(html);

                                        Globals.log.Debug("FormCefbrowserDummy:> GetPageType(html) = " + pagetype);

                                        if (pagetype == "AmiVoice Watcher")
                                        {
                                            Globals.log.Debug("FormNotificationPanel:> BrowserLoadingStateChanged:> loaded Amivoice Watcher successfully.");
                                            myState = State.loading_panelPage_finished;

                                        }else if (pagetype == "AmiVoice Watcher (Reload)")
                                        {
                                            Globals.log.Debug("FormNotificationPanel:> loaded reload page successfully.");
                                            myState = State.loading_reloadPage_finished;
                                        }
                                        else
                                        {
                                            Thread.Sleep(500);
                                            LoadReloadPage();
                                        }

                                        //Load_ReloadPage();
                                    }

                                    break;
                            }
                        }
                    }
                });
            }
        }


        public string GetSetting()
        {

            Dictionary<string, string> object2save = new Dictionary<string, string>() {
                        {"theme", _theme},
                        {"opacity", _opacity.ToString()},
                        {"fontsizeClass", _fontsizeClass},
                        {"blinkTaskbar", _blinkTaskbar.ToString()}
                    };

            return Globals.functions.Json_toJsonFormatted(object2save);
        }

        delegate void SetOpacityCallback(double transparency);
        public void SetOpacity(double transparency)
        {
            if (frmTaskbarBlink.InvokeRequired)
            {
                SetOpacityCallback d = new SetOpacityCallback(SetOpacity);
                this.Invoke(d, transparency);
            }
            else
            {
                _opacity = 1.0 - transparency;

                this.Opacity = _opacity;

                //Uncomment when enable MinimizedPanel
                //frmMinimized.Opacity = _opacity;
            }
        }


        public void SetTaskbarBlinkPara(bool setBlink, int unread)
        {
            _blinkTaskbar = setBlink;
            if (setBlink && unread > 0)
            {
                frmTaskbarBlink.FlashForm();
            }
            else
            {
                frmTaskbarBlink.FlashFormStop();
            }

        }

        public void SaveSetting(string mainclass)
        {
            var theme = Regex.Match(mainclass, @"theme-(\S*)").Groups[1].Value;
            var fontsizeClass = Regex.Match(mainclass, @"wc-font-(\S*)").Groups[1].Value;

            Dictionary<string, string> object2save = new Dictionary<string, string>() {
                        {"theme",theme},
                        {"opacity", this.Opacity.ToString()},
                        {"fontsizeClass", fontsizeClass},
                        {"blinkTaskbar", _blinkTaskbar.ToString()}
                    };

            var jsonMsg = Globals.functions.Json_toJsonFormatted(object2save);

            var filepath = MyPath.notification_local_setting_file + ".new.json";
            File.WriteAllText(filepath, jsonMsg);
            Globals.log.Debug("Notification setting is saved successfully.");

        }

        public void SetThemeReload(string theme)
        {
            Globals.log.Debug(theme);

            Object[] objArray = new Object[1];
            objArray[0] = (Object)theme;

            //webBrowser1.Document.InvokeScript("externalFx_setTheme", objArray);
            browser.ExecuteScriptAsync(String.Format("externalFx_setTheme({0});", objArray));

        }

        public void SetThemeMinimized(string theme)
        {
            Globals.log.Debug(theme);


            //-- for webBrowser version
            //Object[] objArray = new Object[1];
            //objArray[0] = (Object)theme;
            //frmMinimized.webBrowser1.Document.InvokeScript("externalFx_setTheme", objArray);

            //-- for cefsharp Browser
            //Uncomment when enable MinimizedPanel
            //frmMinimized.cefsharpBrowser.ExecuteScriptAsync(String.Format("externalFx_setTheme('{0}');", theme));            
        }



        public FormNotificationPanel()
        {
            InitializeComponent();

            _minHeight = this.MinimumSize.Height;

        }
        //private const int cGrip = 16;      // Grip size
        //private const int cCaption = 32;   // Caption bar height;

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
        //    ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
        //    rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
        //    e.Graphics.FillRectangle(Brushes.DarkBlue, rc);
        //}

        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == 0x84)
        //    {  // Trap WM_NCHITTEST
        //        System.Drawing.Point pos = new System.Drawing.Point(m.LParam.ToInt32());
        //        pos = this.PointToClient(pos);
        //        if (pos.Y < cCaption)
        //        {
        //            m.Result = (IntPtr)2;  // HTCAPTION
        //            return;
        //        }
        //        if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
        //        {
        //            m.Result = (IntPtr)17; // HTBOTTOMRIGHT
        //            return;
        //        }
        //    }
        //    base.WndProc(ref m);
        //}

        //***********************************************************
        //This gives us the ability to resize the borderless from any borders instead of just the lower right corner
        protected override void WndProc(ref Message m)
        {
            const int wmNcHitTest = 0x84;
            const int htLeft = 10;
            const int htRight = 11;
            const int htTop = 12;
            const int htTopLeft = 13;
            const int htTopRight = 14;
            const int htBottom = 15;
            const int htBottomLeft = 16;
            const int htBottomRight = 17;

            if (m.Msg == wmNcHitTest)
            {
                int x = (int)(m.LParam.ToInt64() & 0xFFFF);
                int y = (int)((m.LParam.ToInt64() & 0xFFFF0000) >> 16);
                System.Drawing.Point pt = PointToClient(new System.Drawing.Point(x, y));
                System.Drawing.Size clientSize = ClientSize;
                ///allow resize on the lower right corner
                if (pt.X >= clientSize.Width - 16 && pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htBottomLeft : htBottomRight);
                    return;
                }
                ///allow resize on the lower left corner
                if (pt.X <= 16 && pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htBottomRight : htBottomLeft);
                    return;
                }
                ///allow resize on the upper right corner
                if (pt.X <= 16 && pt.Y <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htTopRight : htTopLeft);
                    return;
                }
                ///allow resize on the upper left corner
                if (pt.X >= clientSize.Width - 16 && pt.Y <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htTopLeft : htTopRight);
                    return;
                }
                ///allow resize on the top border
                //if (pt.Y <= 16 && clientSize.Height >= 16)
                //{
                //    m.Result = (IntPtr)(htTop);
                //    return;
                //}
                ///allow resize on the bottom border
                if (pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htBottom);
                    return;
                }
                ///allow resize on the left border
                if (pt.X <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htLeft);
                    return;
                }
                ///allow resize on the right border
                if (pt.X >= clientSize.Width - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htRight);
                    return;
                }
            }
            base.WndProc(ref m);
        }
        //***********************************************************

        private static CefSettings myCefSettings = new CefSettings();
        public ChromiumWebBrowser browser;
        //public static ChromiumWebBrowser cefsharpBrowser2;
        public static BoundJavascriptObject boundJavascript;
        public void InitBrowser()
        {
            boundJavascript = new BoundJavascriptObject(this);


            //if (!Cef.IsInitialized)
            //{
            //    //Cef.Initialize(new CefSettings());
            //    Cef.Initialize(myCefSettings, false, true);
            //}

            browser = new ChromiumWebBrowser("dummy:")
            {
                Width = 500,
                Height = 500,
                Dock = DockStyle.Fill

            };

            var waitBeforeRegisterJsObject = 10000;
            var waitBeforeRegisterJsObject_Here = waitBeforeRegisterJsObject;

#if DEBUG
            waitBeforeRegisterJsObject = 0;
#endif
            waitBeforeRegisterJsObject_Here = (int)(waitBeforeRegisterJsObject * 1.5);
            Globals.log.Debug("FormNotificationPanel:> Waiting before:> First, registerJsObject. Sleep= "+ waitBeforeRegisterJsObject_Here);
            Thread.Sleep(waitBeforeRegisterJsObject_Here);

            #region split wait
            //var waitBeforeRegisterJsObject_Split = 10;
            // waitBeforeRegisterJsObject = (int)(waitBeforeRegisterJsObject / waitBeforeRegisterJsObject_Split);
            //while (waitBeforeRegisterJsObject_Split > 0)
            //{
            //    Thread.Sleep(waitBeforeRegisterJsObject);
            //    waitBeforeRegisterJsObject_Split--;
            //}
            #endregion


            //browser.RegisterAsyncJsObject("bound", boundJavascript);
            browser.RegisterJsObject("bound", boundJavascript);

            waitBeforeRegisterJsObject_Here = (int)(waitBeforeRegisterJsObject * 1.5);
            Globals.log.Debug("FormNotificationPanel:> Waiting after:> First, registerJsObject. Sleep= " + waitBeforeRegisterJsObject_Here);
            Thread.Sleep(waitBeforeRegisterJsObject_Here);

            browser.FrameLoadEnd += boundJavascript.OnFrameLoadEnd;

            //var browserSettings = new BrowserSettings();
            //browserSettings.FileAccessFromFileUrls = CefState.Enabled;
            //browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
            //browser.BrowserSettings = browserSettings;

            //cefsharpBrowser2 = new ChromiumWebBrowser("about:blank");

            

            //cefsharpBrowser.RegisterJsObject("bound", new JSBinding());

#if !DEBUG
            DisableContextMenu();
#endif

            browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            browser.LoadingStateChanged += BrowserLoadingStateChanged;

            // browser = new ChromiumWebBrowser(@"http://192.168.1.51:3000/watchercli/sirisak/notification");
            this.Controls.Add(browser);



            try
            {
                waitBeforeRegisterJsObject_Here = (int)(waitBeforeRegisterJsObject * 1);
                Globals.log.Debug("FormNotificationPanel:> Waiting before:> Second, registerJsObject. Sleep= " + waitBeforeRegisterJsObject_Here);
                Thread.Sleep(waitBeforeRegisterJsObject_Here);

                Globals.log.Debug("Second, registerJsObject");
                browser.RegisterJsObject("bound", boundJavascript);

                Globals.log.Debug("FormNotificationPanel:> Waiting after:> Second, registerJsObject. Sleep= " + waitBeforeRegisterJsObject_Here);
                Thread.Sleep(waitBeforeRegisterJsObject_Here);
            }
            catch (Exception e)
            {
                Globals.log.Debug(e.ToString());
            }



            //this.Controls.Add(cefsharpBrowser2);
            //cefsharpBrowser.Dock = DockStyle.Fill;
            SetCefSharpBrowserSize();

            //browser.RequestHandler = new RequestHandler();
        }

        private void OnIsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs args)
        {
            try
            {
                if (args.IsBrowserInitialized)
                {                    
                    myState = State.browser_initialized;                    
                }
            }
            catch (Exception e)
            {
                Globals.log.Debug(e.ToString());
            }

        }

#region disableContextMenu
        public class CustomMenuHandler : CefSharp.IContextMenuHandler
        {
            public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
            {
                model.Clear();
            }

            public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            {

                return false;
            }

            public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {

            }

            public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            {
                return false;
            }
        }

        private void DisableContextMenu()
        {
            browser.MenuHandler = new CustomMenuHandler();
        }
#endregion

        private class SharpBrowser
        {
            public static int margin = 10;
        }

        private void SetCefSharpBrowserSize()
        {
            browser.Dock = DockStyle.Fill;

            browser.Dock = DockStyle.None;

            browser.Width = this.Width - SharpBrowser.margin * 2;
            browser.Height = this.Height - SharpBrowser.margin * 2 - SharpBrowser.margin;

            browser.Left = SharpBrowser.margin;
            browser.Top = SharpBrowser.margin;
        }

        private void LoadReloadPage()
        {
            //browser.Stop();
            //if (browser.IsLoading)
            //{
            //    Thread.Sleep(500);
            //}

            if (myState >= State.loading_reloadPage)
            {
                return;
            }

            myState = State.loading_reloadPage;

            browser.Load(Globals.functions.GetTemplatePath("LongNotificationReload.html"));
        }


        public void ShowConnectionError()
        {
            if (browser.IsBrowserInitialized)
            {
                browser.ExecuteScriptAsync(String.Format("externalFx_ShowError(\"{0}\");", "connectionError"));
            }

        }

        public void ShowConnectionSuccess()
        {
            if (browser.IsBrowserInitialized)
            {
                browser.ExecuteScriptAsync(String.Format("externalFx_HideError(\"{0}\");", "connectionError"));
            }

        }

        //public void LoadPageOld(ChromiumWebBrowser browser, string address = null)
        //{
        //    //var tcs = new TaskCompletionSource<bool>();

        //    EventHandler<LoadingStateChangedEventArgs> handler = null;
        //    handler = (sender, args) =>
        //    {
        //        //Wait for while page to finish loading not just the first frame
        //        if (!args.IsLoading)
        //        {
        //            //.NET4.5.2
        //            //if (browser.CanExecuteJavascriptInMainFrame)
        //            if (true)
        //            {
        //                //browser.ExecuteScriptAsync("alert('test');");

        //                //browser.ExecuteScriptAsync("alert(document.getElementsByTagName ('html')[0].innerHTML)");
        //                browser.LoadingStateChanged -= handler;

        //                string whatPage = WhatPage();
        //                if (whatPage == "AmiVoice Watcher")
        //                {
        //                    loadDefaultSetting();

        //                    //NotificationPopup.PreparePopupWindows();
        //                    //AmivoiceWatcher.StartRabbitMqClient();


        //                }
        //                else if (whatPage == "AmiVoice Watcher (Reload)")
        //                {
        //                    browser.LoadingStateChanged += handler;
        //                    //Do Nothing
        //                }
        //                else if (whatPage == "<to reload>")
        //                {
        //                    LoadReloadPage();
        //                    //if (!args.IsLoading)
        //                    //{
        //                    //    browser.LoadingStateChanged += handler;
        //                    //}
        //                }
        //                //var doc = browser.EvaluateScriptAsync(@"document.getElementsByTagName ('html')[0].innerHTML").ToString();
        //                //browser.LoadingStateChanged -= handler;

        //                //LoadReloadPage();
        //                //tcs.TrySetResult(true);
        //                //var task = browser.EvaluateScriptAsync(@"document.getElementsByTagName ('html')[0].innerHTML");
        //                // JavascriptResponse x = await browser.EvaluateScriptAsync(@"document.getElementsByTagName ('html')[0].innerHTML");


        //                //browser.Find("<meta name=\"application-name\"");

        //                //var response = task.Result;
        //                //var result = response.Success ? (response.Result ?? "null") : response.Message;

        //            }
        //            else
        //            {
        //                LoadReloadPage();
        //            }
        //        }
        //    };

        //    //browser.LoadingStateChanged += handler;

        //    if (!string.IsNullOrEmpty(address))
        //    {
        //        browser.Load(address);
        //    }
        //    //return tcs.Task;
        //}

        //public void LoadPage(ChromiumWebBrowser browser, string address = null)
        //{
        //    //var tcs = new TaskCompletionSource<bool>();

        //    EventHandler<LoadingStateChangedEventArgs> handler = null;
        //    handler = (sender, args) =>
        //    {
        //        //Thread.Sleep(2000);
        //        //Wait for while page to finish loading not just the first frame
        //        if (!args.IsLoading)
        //        {
        //            //.NET4.5.2
        //            //if (browser.CanExecuteJavascriptInMainFrame)
        //            if (true)
        //            {

        //                browser.LoadingStateChanged -= handler;

        //                LoadPageByCondition(handler);

        //            }

        //        }
        //    };

        //    browser.LoadingStateChanged += handler;

        //    if (!string.IsNullOrEmpty(address))
        //    {
        //        browser.Load(address);
        //    }
        //    //return tcs.Task;
        //}

        delegate void SetSameBackgroundCallback(string colorStr);
        public void SetSameBackground(string colorStr)
        {

            if (this.InvokeRequired)
            {
                SetSameBackgroundCallback d = new SetSameBackgroundCallback(SetSameBackground);
                this.Invoke(d, colorStr);
            }
            else
            {
                try
                {
                    var colors = Colors.String2IntArray(colorStr);

                    //this.BackColor = Color.FromArgb(color.A, color.R, color.G, color.B);
                    this.BackColor = Color.FromArgb(colors[0], colors[1], colors[2]);
                }
                catch (Exception e)
                {
                    Globals.log.Debug(e.ToString());
                }
            }
        }

        private string WhatPage()
        {
            var html = "";
            //.NET4.5.2
            //Task taskA = Task.Run(() =>
            //.NET4.0
            Task task = Task.Factory.StartNew(() =>
            {
                object js = CefSharpEvaluateScript(browser, @"document.getElementsByTagName ('html')[0].innerHTML");

                html = js.ToString();
                if (html == "")
                {
                    //return "<nopage>";
                    return "<to reload>";
                }

                Match match = Regex.Match(html, @"<meta name=""application-name"" content=""(.*?)"">");
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                else
                {
                    return "<to reload>";
                }
            });
            //task.Start();

            task.Wait(3000);

            return "<to reload>";
        }

        private static string GetPageType(string html)
        {
            //var strOut = "";

            Match match = Regex.Match(html, "@(.*)@");

            if (!match.Success)
            {
                return "";
            }

            var whatPage = match.Groups[1].Value;

            return whatPage;
            //if (whatPage == "AmiVoice Watcher")
            //{
            //    return whatPage
            //}
        }

        //delegate void LoadPageByConditionCallback(EventHandler<LoadingStateChangedEventArgs> handler);
//        public void LoadPageByCondition(EventHandler<LoadingStateChangedEventArgs> handler)
//        {

//            if (frmTaskbarBlink.InvokeRequired)
//            {
//                LoadPageByConditionCallback d = new LoadPageByConditionCallback(LoadPageByCondition);
//                frmTaskbarBlink.Invoke(d, handler);
//            }
//            else
//            {
//                var script = @"document.getElementsByTagName ('html')[0].innerHTML";
//                browser.EvaluateScriptAsync(script).ContinueWith(x =>
//                {
//                    if (x.IsFaulted)


//                    {
//                        Globals.log.Warn(String.Format("Page cannot evaluate script:> " + script));
//                    }
//                    else
//                    {
//                        var response = x.Result;

//                        if (response.Success && response.Result != null)

//                        {
//                            var html = response.Result.ToString();
//                            //startDate is the value of a HTML element.

//                            //if ((html == "") || (html=="<head></head><body></body>"))
//                            if (html == "")
//                            {
//                                //cefsharpBrowser.LoadingStateChanged += handler;

//                                return;
//                            }

//                            browser.LoadingStateChanged -= handler;

//                            //Match match = Regex.Match(html, @"\\$(AmiVoice Watcher.*)\\$");
//                            Match match = Regex.Match(html, "@(.*)@");

//                            if (match.Success)
//                            {
//                                var whatPage = match.Groups[1].Value;
//                                if (whatPage == "AmiVoice Watcher")
//                                {
//                                    if (myState < State.notification_panel)
//                                    {
//                                        myState = State.notification_panel;

//                                        //cefsharpBrowser.LoadingStateChanged -= handler;

//                                        Globals.log.Debug("FormNotificationPanel loaded Amivoice Watcher successfully.");

//                                        //loadDefaultSetting();
//                                        //SetSameBackground();

//#if DEBUG
//                                        //cefsharpBrowser.ExecuteScriptAsync("DEBUG=true;");
//#endif

//                                        //NotificationPopup.PreparePopupWindows();
//                                        //AmivoiceWatcher.StartRabbitMqClient();
//                                        FormWaiting.CloseDelegate();

//                                        AmivoiceWatcher.LongNotification.ShowPanel();

//                                        //cefsharpBrowser.Stop();


//                                        //myState = State.notification_panel;
//                                    }


//                                }
//                                else if (whatPage == "AmiVoice Watcher (Reload)")
//                                {





//                                    FormWaiting.CloseDelegate();
//                                    AmivoiceWatcher.LongNotification.ShowPanel();

//                                    if (myState < State.reloadpage)
//                                    {
//                                        myState = State.reloadpage;

//                                        browser.LoadingStateChanged += handler;

//                                        //Thread.Sleep(2000);
//                                        //SetSameBackground();
//                                    }

//                                }
//                                else
//                                {
//                                    FormWaiting.CloseDelegate();
//                                    AmivoiceWatcher.LongNotification.ShowPanel();

//                                    if (myState < State.reloadpage)
//                                    {
//                                        myState = State.reloadpage;

//                                        browser.LoadingStateChanged += handler;

//                                        LoadReloadPage();
//                                        browser.LoadingStateChanged += handler;
//                                    }

//                                }
//                            }
//                            else
//                            {

//                                LoadReloadPage();
//                                browser.LoadingStateChanged += handler;
//                                //cefsharpBrowser.LoadingStateChanged += handler;
//                                //return "<to reload>";
//                            }
//                        }
//                    }


//                });

//            }

//        }



        static object CefSharpEvaluateScript(ChromiumWebBrowser b, string script)
        {
            var task = b.EvaluateScriptAsync(script);
            task.Wait();
            JavascriptResponse response = task.Result;
            return response.Success ? (response.Result ?? "") : response.Message;
        }

        private void setMoveEvent()
        {
            //if (webBrowser1.Document != null)
            //{
            //    var htmlDoc = webBrowser1.Document;

            //    htmlDoc.MouseDown += Document_MouseDown;
            //    htmlDoc.MouseUp += Document_MouseUp;
            //    htmlDoc.MouseLeave += Document_MouseUp;

            //}

        }


        delegate void loadDefaultSettingCallback();
        public void loadDefaultSetting()
        {
            try
            {

                if (this.InvokeRequired)
                {
                    loadDefaultSettingCallback d = new loadDefaultSettingCallback(loadDefaultSetting);
                    this.Invoke(d, new object[] { });
                }
                else
                {
                    if (myState == State.notification_panel_loading_preference)
                    {
                        return;
                    }
                    myState = State.notification_panel_loading_preference;

                    Globals.log.Debug("FormNotificationPanel is loading appearance from saved file.");

                    var filepath = MyPath.notification_local_setting_file;

                    if (File.Exists(filepath + ".new.json"))
                    {
                        filepath = filepath + ".new.json";
                    }
                    var savedSettingStr = File.ReadAllText(filepath);

                    Globals.log.Debug("Saved appearance= "+ savedSettingStr.Replace(Environment.NewLine, "" ));

                    var setting = Globals.functions.Json_toDictionary(savedSettingStr);
                    //if (File.Exists(filepath))

                    _opacity = 1.0;
                    _fontsizeClass = "normal";
                    _theme = "dark";

                    if (setting.ContainsKey("opacity"))
                    {
                        Double.TryParse(setting["opacity"], out _opacity);
                    }

                    if (setting.ContainsKey("blinkTaskbar"))
                    {
                        Boolean.TryParse(setting["blinkTaskbar"], out _blinkTaskbar);
                    }

                    this.Opacity = _opacity;

                    //Uncomment when enable MinimizedPanel
                    //frmMinimized.Opacity = _opacity;

                    if (setting.ContainsKey("theme"))
                    {
                        _theme = setting["theme"];
                    }

                    if (setting.ContainsKey("fontsizeClass"))
                    {
                        _fontsizeClass = setting["fontsizeClass"];
                    }

                    var para = _theme + " " + _fontsizeClass;

                    //Object[] objArray = new Object[1];
                    //objArray[0] = (Object)para;

                    //Object[] objArray2 = new Object[1];
                    //objArray2[0] = (Object)_theme;

                    //this.webBrowser1.Document.InvokeScript("externalFx_setTheme_Fontsize", objArray);
                    //this.webBrowser1.Document.InvokeScript("externalFx_setTheme", objArray2);

                    //--- TODO
                    //frmMinimized.webBrowser1.Document.InvokeScript("externalFx_setTheme_Fontsize", objArray);
                    //frmMinimized.webBrowser1.Document.InvokeScript("externalFx_setTheme", objArray2);

                    browser.ExecuteScriptAsync(String.Format("externalFx_setTheme_Fontsize(\"{0}\");", para));
                    browser.ExecuteScriptAsync(String.Format("externalFx_setTheme(\"{0}\");", _theme));

                    //Uncomment when enable MinimizedPanel
                    //frmMinimized.cefsharpBrowser.ExecuteScriptAsync(String.Format("externalFx_setTheme_Fontsize(\"{0}\");", para));
                    //frmMinimized.cefsharpBrowser.ExecuteScriptAsync(String.Format("externalFx_setTheme(\"{0}\");", _theme));

                }
            }
            catch (Exception e)
            {
                Globals.log.Debug(e.ToString());
            }
        }

        //private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    var todo = "";
        //    var appName = "";

        //    var appNameElements = webBrowser1.Document.All.GetElementsByName("application-name");

        //    if (appNameElements.Count > 0)
        //    {
        //        appName = appNameElements[0].GetAttribute("content");
        //        if (appName == "AmiVoice Watcher")
        //        {
        //            todo = "";
        //        } else if (appName == "AmiVoice Watcher (Reload)")
        //        {
        //            todo = "";
        //        } else
        //        {
        //            todo = "load reload page";
        //        }

        //    }
        //    else if (webBrowser1.Document.All.GetElementsByName("application-name").Count == 0)
        //    {
        //        todo = "load reload page";
        //    }

        //    if (todo == "")
        //    {
        //        setMoveEvent();

        //        if (!bln_loadDefaultSetting)
        //        {
        //            loadDefaultSetting();
        //            if (appName == "AmiVoice Watcher")
        //            {
        //                bln_loadDefaultSetting = true;
        //                AmivoiceWatcher.StartRabbitMqClient();
        //            }
        //        }

        //    } else if (todo == "load reload page") {
        //        webBrowser1.Navigate(new System.Uri(Globals.functions.GetTemplatePath("LongNotificationReload.html")));

        //    }

        //}

        private enum SetIconOption { unread, allRead }


        private void SetIcon(SetIconOption option)
        {
            try
            {
                if (option == SetIconOption.unread)
                {
                    setIconFromIcoFile_NewMessage();
                }
                else
                {
                    setIconFromIcoFile_AllMessageRead();
                }
            }
            catch (Exception e)
            {
                Globals.log.Debug(e.ToString());
            }
        }

        public void setIconFromPngFile_NewMessage()
        {

            Bitmap baseImage;
            Bitmap overlayImage;

            baseImage = (Bitmap)Image.FromFile(Path.Combine(MyPath.PathLocalAppData, "Template", "image", "logo.png"));
            //overlayImage = (Bitmap)Image.FromFile(Path.Combine(MyPath.PathLocalAppData, "Template", "image", "ok-icon.png"));
            overlayImage = (Bitmap)Image.FromFile(Path.Combine(MyPath.PathLocalAppData, "Template", "image", "n.png"));
            var finalImage = new Bitmap(baseImage.Width, baseImage.Height, PixelFormat.Format32bppArgb);

            var graphics = Graphics.FromImage(finalImage);
            graphics.CompositingMode = CompositingMode.SourceOver;

            graphics.DrawImage(baseImage, 0, 0, baseImage.Width, baseImage.Height);
            var overlayScale = 0.6;
            graphics.DrawImage(overlayImage, (int)(baseImage.Width * (1.0 - overlayScale)), 0, (int)(baseImage.Width * overlayScale), (int)(baseImage.Height * overlayScale));
            finalImage.Save(Path.Combine(MyPath.PathLocalAppData, "Template", "image", "final.jpg"), ImageFormat.Jpeg);

            IntPtr pIcon = finalImage.GetHicon();
            Icon i = Icon.FromHandle(pIcon);
            //this.Icon = i;

            frmTaskbarBlink.Icon = i;

            AmivoiceWatcher.ChangeNotifyIcon(i);

            i.Dispose();
            //this.Icon = Icon.FromHandle(img);

        }

        public void setIconFromIcoFile_NewMessage()
        {

            Icon i = new System.Drawing.Icon(Path.Combine(MyPath.PathLocalAppData, "Template", "image", "ami_watcher_NewMessage.ico"));

            frmTaskbarBlink.Hide();

            frmTaskbarBlink.Icon = i;
            AmivoiceWatcher.ChangeNotifyIcon(i);

            frmTaskbarBlink.Show();

            i.Dispose();
        }

        delegate void setIconFromIcoFile_AllMessageReadCallback();
        public void setIconFromIcoFile_AllMessageRead()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (frmTaskbarBlink.InvokeRequired)
            {
                setIconFromIcoFile_AllMessageReadCallback d = new setIconFromIcoFile_AllMessageReadCallback(setIconFromIcoFile_AllMessageRead);
                this.Invoke(d, new object[] { });
            }
            else
            {
                Icon i = new System.Drawing.Icon(Path.Combine(MyPath.PathLocalAppData, "Template", "image", "ami_watcher.ico"));

                frmTaskbarBlink.Hide();

                frmTaskbarBlink.Icon = i;
                AmivoiceWatcher.ChangeNotifyIcon(i);

                frmTaskbarBlink.Show();

                i.Dispose();
            }
        }

        public void setIconFromPngFile_AllMessageRead()
        {
            Bitmap baseImage = (Bitmap)Image.FromFile(Path.Combine(MyPath.PathLocalAppData, "Template", "image", "logo.png"));
            Icon i = Icon.FromHandle(baseImage.GetHicon());

            frmTaskbarBlink.Icon = i;

            AmivoiceWatcher.ChangeNotifyIcon(i);

            i.Dispose();
        }

        delegate void FlashTaskbarCallback();
        public void FlashTaskbar()
        {
            if (frmTaskbarBlink.InvokeRequired)
            {
                FlashTaskbarCallback d = new FlashTaskbarCallback(FlashTaskbar);
                this.Invoke(d, new object[] { });
            }
            else
            {
                try
                {
                    frmTaskbarBlink.Show();

                    // Uncomment to change Icon
                    //SetIcon(SetIconOption.unread);

                    if (_blinkTaskbar)
                    {
                        frmTaskbarBlink.FlashForm();
                    }
                    else
                    {
                        frmTaskbarBlink.FlashFormStop();
                    }

                    //== using minimized form
                    //if (!this.Visible)
                    //{
                    //    frmMinimized.ShowInTaskbar = true;
                    //    frmMinimized.FlashForm();
                    //}
                }
                catch (Exception e)
                {
                    Globals.log.Debug(e.ToString());
                }
            }
        }


        public void UnFlashTaskbar()
        {
            SetIcon(SetIconOption.allRead);

            frmTaskbarBlink.FlashFormStop();
            // Uncomment if you want to hide the taskbar button 
            // when all messages are read.
            //frmTaskbarBlink.Hide();

        }

        private void FormLongNotification_VisibleChanged(object sender, EventArgs e)
        {
            //Uncomment when enable MinimizedPanel
            //if (this.Visible)
            //{
            //    frmMinimized.Hide();
            //    frmMinimized.ShowInTaskbar = false;
            //    frmMinimized.FlashFormStop();
            //}
            //else
            //{                
            //    var currentPoint = this.Location;

            //    currentPoint.Y = 30;

            //    frmMinimized.Location = currentPoint;
            //    frmMinimized.Width = this.Width;
            //    frmMinimized.Opacity = this.Opacity;
            //    frmMinimized.Show(this);
            //}
        }

        private void FormLongNotification_KeyDown(object sender, KeyEventArgs e)
        {
            //if (!e.Alt) return;
            switch (e.KeyCode)
            {
                case Keys.Back:
                    //callButtonAClickProcedure();
                    break;
                case Keys.B:

                    break;
                    /* (etc.) */
            }
        }

        private void FormLongNotification_ResizeEnd(object sender, EventArgs e)
        {
            SetCefSharpBrowserSize();

            browser.ExecuteScriptAsync("externalFx_afterResizeWinForm();");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MouseUpOnBrowser();
        }

        private void FormNotificationPanel_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void FormNotificationPanel_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormNotificationPanel_Load(object sender, EventArgs e)
        {
            _lastHeight = this.Height;

            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this.ClientSize = new System.Drawing.Size((int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.25), (int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height * 0.3));
            //this.Location = new System.Drawing.Point((int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.75), 0);
            this.DesktopLocation = new System.Drawing.Point((int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.75), (int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.3));

            //this.Location = new System.Drawing.Point((int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.75), (int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.3));

            this.Location = new System.Drawing.Point((int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width), (int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height * 0.3));


            var address = Globals.Notifications.URL.Watchercli();

#if DEBUG
            address = Debug.url_watchercli_invalid;
#endif

            InitBrowser();

            //while (Configuration.myState != Configuration.State.completed)
            //{
            //    Thread.Sleep(500);
            //}

            //Globals.log.Debug("FormNotificationPanel:> Loading panel from URL:" + address);
            //myState = State.loading_panelPage;
            //webBrowser1.Navigate(new Uri(address));

            //browser.Load(address);

            //myState = State.loading_panelPage;
            //browser.Load(address);

            //LoadPage(browser, address);
            //FormWaiting.CloseDelegate();

            Thread thread1 = new Thread(new ThreadStart(PageLoader_Thread));
            thread1.Start();
        }

        delegate void MoveWindow2VisibleAreaCallback();
        private void MoveWindow2VisibleArea()
        {

            if (this.InvokeRequired)
            {
                MoveWindow2VisibleAreaCallback d = new MoveWindow2VisibleAreaCallback(MoveWindow2VisibleArea);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.Location = new System.Drawing.Point((int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.75), (int)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height * 0.3));
            }
        }

        private void PageLoader_Thread()
        {
            var autoload_coutdown = 10;
            while (!browser.IsBrowserInitialized)
            {
                Thread.Sleep(500);
            }

            while (Configuration.myState != Configuration.State.completed)
            {
                Thread.Sleep(500);
            }

            var loopcounter = 0;
            while (!Globals.isProgramExit)
            {
                try
                {
                    loopcounter++;

                    if (myState == State.browser_initialized) {
                        Globals.log.Debug("FormNotificationPanel:> Browser Initialized");

                        FormWaiting.CloseDelegate();
                        AmivoiceWatcher.NotificationPanel.ShowPanel();

                        ExternalReload();
                    }
                    else if (myState == State.loading_panelPage_finished)
                    {
                        loadDefaultSetting();

                        AmivoiceWatcher.NotificationPanel.showConnectionSuccess2LongNotification();
                        //SetSameBackground();

                        Globals.log.Debug("FormNotificationPanel:> Move window to visible area");
                        MoveWindow2VisibleArea();

                        Globals.log.Debug("FormNotificationPanel.Page_Loader :> exit()");
                        return;
                    }
                    else if (myState == State.loading_reloadPage_finished)
                    {


                        var address = Globals.Notifications.URL.Watchercli();

#if DEBUG
                        address = Debug.url_watchercli_valid;
#endif

                        if (autoload_coutdown > 0)
                        {
                            autoload_coutdown--;
                            Globals.log.Debug("FormNotificationPanel.Page_Loader :> loading panelpage");
                            //LoadPage(browser, address);
                            myState = State.loading_panelPage;
                            browser.Load(address);
                        }

                        if (!browser.IsLoading)
                        {
                            Thread.Sleep(52000 - 5000 * autoload_coutdown);
                        }

                        //Thread.Sleep(2000);
                    }
                    else if (myState == State.otherpage)
                    {
                        Globals.log.Debug("FormNotificationPanel.Page_Loader :> Load reload page");
                        //FormNotificationPanel:>
                        LoadReloadPage();
                        Thread.Sleep(2000);
                    }
                    else if (myState == State.loading_panelPage)
                    {
                        if (loopcounter % 20 == 0)
                        {
                            myState = State.otherpage;
                        }
                    }
                    else if (myState == State.loading_reloadPage)
                    {
                        if (loopcounter % 20 == 0)
                        {
                            myState = State.otherpage;
                        }
                    }

                    //if (!browser.IsLoading)
                    //{
                    //    Thread.Sleep(1000);
                    //}

                    Thread.Sleep(1000);

                }
                catch (Exception e)
                {
                    Globals.log.Error(e.ToString());
                }
            }
        }
    }

    public class BoundJavascriptObject
    {
        private static FormNotificationPanel frm;

        public void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Globals.log.Debug("FrameLoadEnd() is fired");
        }

        public BoundJavascriptObject(FormNotificationPanel _frm)
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

        public void afterSetTheme(string colorIn)
        {
            frm.SetSameBackground(colorIn);
        }

        public void ExternalHide()
        {
            frm.ExternalHide();
        }

        public void Mousedown()
        {
            frm.MouseDownOnBrowser();
        }

        public void Mouseup()
        {
            frm.MouseUpOnBrowser();
        }

        public void ExternalReload()
        {
            frm.ExternalReload();
        }

        public string GetSetting()
        {
            return frm.GetSetting();
        }

        public void SaveSetting(string mainclass)
        {
            frm.SaveSetting(mainclass);
        }

        public void UnFlashTaskbar()
        {
            frm.UnFlashTaskbar();
        }

        public void FlashTaskbar()
        {
            frm.FlashTaskbar();
        }

        public void SetOpacity(string transparency)
        {
            double doubleTransparent = 0;
            double.TryParse(transparency, out doubleTransparent);

            frm.SetOpacity(doubleTransparent);
        }

        public void SetThemeMinimized(string theme)
        {
            frm.SetThemeMinimized(theme);
        }

        public void SetTaskbarBlinkPara(bool setBlink, int unread)
        {
            frm.SetTaskbarBlinkPara(setBlink, unread);
        }

    }

}
