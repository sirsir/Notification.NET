using System;
using System.Drawing;
using System.Windows.Forms;

//Registry CRUD
using Microsoft.Win32;
using System.Net;
using System.IO;

using System.Globalization;

using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using CefSharp;

namespace AmivoiceWatcher
{
    public class AmivoiceWatcher : Form
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public static FormDummy formDummy = new FormDummy();

        private static NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        private Thread threadComputerInfo;
        private Thread threadUserActivity;
        //private Thread threadVdoUploader;
        private static Thread threadRabbitMQ;

        [STAThread]
        public static void Main()
        {
            try
            {
                //For datetime format
                //.NET4.5.2
                //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

                //during init of application bind to this event  
                SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);

                // Allow only one instant of AmivoiceWatcher
                bool createdNew = true;
                using (Mutex mutex = new Mutex(true, "Amivoice Watcher", out createdNew))
                {
                    if (createdNew)
                    {
                        Application.Run(new AmivoiceWatcher());
                    }
                    else
                    {
                        Process current = Process.GetCurrentProcess();
                        foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                        {
                            if (process.Id != current.Id)
                            {
                                SetForegroundWindow(process.MainWindowHandle);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public AmivoiceWatcher()
        {
            try
            {
                Globals.log.Debug("/");
                Globals.log.Debug("/");
                Globals.log.Debug("/");
                Globals.log.Debug("/");
                Globals.log.Debug("Welcome to Amivoice Watcher v" + ComputerInfo.WatcherVersion);

#if DEBUG
                Console.WriteLine("Mode=Debug");
                //DebugHelper();
#endif
                FormWaiting.ShowSplashScreen();

                Configuration.Initialize();

                InitializeCefsharp();
                InitializeFormCefsharpDummy();

                //CefsharpOffscreen.MyInitialize();

                //MainForm mainForm = new MainForm(); //this takes ages
                //FormWaiting.CloseForm();
                //Application.Run(mainForm);

                Globals.CreateAllDirectoryAndFiles();

                InitializeTrayMenu();                

                if (Configuration.CheckEnableAgentActivity())
                {
                    threadUserActivity = new Thread(new ThreadStart(UserActivityThread.ThreadMain));
                    threadUserActivity.Start();
                }

                threadComputerInfo = new Thread(new ThreadStart(ComputerInfo.ThreadMain));
                threadComputerInfo.Start();

                // Will be start by FormNotificationPanel
                threadRabbitMQ = new Thread(new ThreadStart(RabbitMQWrapper.ThreadMain));
                threadRabbitMQ.Start();

                SendComputerLogStartup_by_myFormCefsharpDummy();

                NotificationPanel._init();

#if DEBUG
                //Globals.Notifications.PopupWelcomeMessage();
#endif

                //DONT DELETE
                //StartCaptureScreenRecord();                

            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
            }
        }

        public static void SendComputerLogStartup_by_myFormCefsharpDummy()
        {
            Thread thread1 = new Thread(new ThreadStart(SendComputerLogStartup_by_myFormCefsharpDummy_Thread));
            thread1.Start();           
        }

        private static void SendComputerLogStartup_by_myFormCefsharpDummy_Thread()
        {
            while ((Configuration.myState != Configuration.State.completed) || (myFormCefsharpDummy.myState < FormCefsharpDummy.State.loading_reloadPage_finish))
            {
                Thread.Sleep(500);
            }

            myFormCefsharpDummy.SendComputerLog_startup();
        }

        public static FormCefsharpDummy myFormCefsharpDummy;

        private void InitializeCefsharp()
        {
            try
            {
                Globals.log.Debug("Initializing Cefsharp ...");

                var myCefSettings = new CefSettings();
                myCefSettings.IgnoreCertificateErrors = true;
                //myCefSettings.WindowlessRenderingEnabled = true;
                //myCefSettings.SetOffScreenRenderingBestPerformanceArgs();
                myCefSettings.LogFile = Path.Combine(MyPath.PathLocalAppData, "cefsharp.log");
                myCefSettings.LogSeverity = LogSeverity.Warning;
                myCefSettings.CefCommandLineArgs.Add("log-file", myCefSettings.LogFile);

                //myCefSettings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");

                //~ to fix dependent not found BUT still got error
                //myCefSettings.BrowserSubprocessPath = MyPath.currentDirectroy;
                //myCefSettings.LocalesDirPath = Path.Combine(MyPath.currentDirectroy, "Locales");

                Globals.log.Debug("myCefSettings.BrowserSubprocessPath = " + myCefSettings.BrowserSubprocessPath);

                if (!Cef.IsInitialized)
                {
                    //Cef.Initialize(new CefSettings());
                    //Cef.Initialize(myCefSettings, false, true);
                    Cef.Initialize(myCefSettings, false, false);

                    while (!Cef.IsInitialized)
                    {
                        Thread.Sleep(500);
                    }

                    Globals.log.Debug("AmivoiceWatcher:> Cef is initialized");

                }
            }
            catch (Exception e)
            {
                Globals.log.Debug("Can NOT Initialize Cef");
                Globals.log.Debug(e.ToString());
                Globals.log.Debug("Exit the programs");
                Application.Exit();
                Environment.Exit(0);
            }
        }


        private void InitializeFormCefsharpDummy()
        {
            Globals.log.Debug("AmivoiceWatcher:> InitializeFormCefsharpDummy()");

            myFormCefsharpDummy = new FormCefsharpDummy();
            myFormCefsharpDummy.Show(formDummy);
        }


        private static void ExitCefSharp()
        {
            try
            {
                if (Cef.IsInitialized)
                {
                    Globals.log.Debug("Cef.Shutdown()");
                    Cef.Shutdown();
                }
            }
            catch (Exception e)
            {
                Globals.log.Debug("Can't Cef.Shutdown()");
                Globals.log.Debug(e.ToString());
            }
        }

        public static void StartRabbitMqClient()
        {
            try
            {
                threadRabbitMQ = new Thread(new ThreadStart(RabbitMQWrapper.ThreadMain));
                threadRabbitMQ.Start();
            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
            }
        }

        public class NotificationPanel
        {
            private static FormNotificationPanel fTemp;
            public static void _init()
            {
                Thread thread1 = new Thread(new ThreadStart(NotificationPanel.FormNotificationPanel_Start_Thread));
                thread1.Start();
            }

            private static void FormNotificationPanel_Start_Thread()
            {
                while (RabbitMQWrapper.myState <= RabbitMQWrapper.State.registering)
                {
                    Thread.Sleep(500);
                }

                if (RabbitMQWrapper.myState == RabbitMQWrapper.State.register_fail)
                {
                    Globals.log.Debug("AmivoiceWatcher:> Cannot Regisger rabbitMq.");

                    Globals.log.Debug("AmivoiceWatcher:> Not run:> LongNotification.StartNotificationPanel()");
                }
                else
                {
                    Globals.log.Debug("AmivoiceWatcher:> LongNotification.StartNotificationPanel()");
                    NotificationPanel.StartNotificationPanel();
                }
            }

            delegate void StartNotificationPanelCallback();
            public static void StartNotificationPanel()
            {
                try
                {
                    if (AmivoiceWatcher.formDummy.InvokeRequired)
                    {
                        StartNotificationPanelCallback d = new StartNotificationPanelCallback(StartNotificationPanel);
                        AmivoiceWatcher.formDummy.Invoke(d, new object[] { });
                    }
                    else
                    {
                        fTemp = new FormNotificationPanel();
                        fTemp.Show(formDummy);
                        fTemp.Visible = false;
                        //FormWaiting.CloseForm();
                        //FormWaiting.CloseDelegate();

                        //fTemp.Hide();
                    }
                }
                catch (Exception e)
                {
                    Globals.log.Debug(e.ToString());
                }
            }

            delegate void ShowPanelCallback();
            public static void ShowPanel()
            {
                if (formDummy.InvokeRequired)
                {
                    ShowPanelCallback d = new ShowPanelCallback(ShowPanel);
                    formDummy.Invoke(d, new object[] { });
                }
                else
                {
                    fTemp.Visible = true;
                    //this.Visible = true;
                    fTemp.TopMost = true;
                    //fTemp.Activate();
                }
            }

            private static Form getFormByName(string nameIn)
            {
                Form formOut = null;

                foreach (Form frm in formDummy.OwnedForms)
                {
                    if (frm.Name == nameIn)
                    {
                        formOut = frm;
                    }
                }

                return formOut;
            }


            public static void showConnectionError2LongNotification()
            {
                try
                {
                    var frmLong = (FormNotificationPanel)getFormByName("FormNotificationPanel");

                    if (frmLong != null)
                    {
                        frmLong.ShowConnectionError();
                    }
                }
                catch (Exception e)
                {
                    Globals.log.Debug(e.ToString());
                }
            }

            public static void showConnectionSuccess2LongNotification()
            {
                try
                {
                    var frmLong = (FormNotificationPanel)getFormByName("FormNotificationPanel");

                    frmLong.ShowConnectionSuccess();
                }
                catch (Exception e)
                {
                    Globals.log.Debug(e.ToString());
                }
            }

            public static object GetJsonFromCefsharp(string url)
            {
                try
                {
                    var frmLong = (FormNotificationPanel)getFormByName("FormNotificationPanel");

                    var jsonOut = frmLong.browser.EvaluateScriptAsync(String.Format("externalFx_getJson('{0}');", url));

                    while (!jsonOut.IsCompleted)
                    {
                        Thread.Sleep(500);
                    }

                    return jsonOut;
                }
                catch (Exception e)
                {
                    Globals.log.Debug(e.ToString());

                    return null;
                }
            }

            public static void updateLongNotification(object[] param)
            {
                try
                {
                    Globals.log.Debug("updateLongNotification()");

                    var frmLong = (FormNotificationPanel)getFormByName("FormNotificationPanel");

                    frmLong.FlashTaskbar();

                    var paramAsString = Globals.functions.Json_toJsonObj(param);

                    //frmLong.webBrowser1.Document.InvokeScript("externalFx_addNewNotification", param)
                    frmLong.browser.ExecuteScriptAsync(String.Format("externalFx_addNewNotification({0});", paramAsString));

                    Thread.Sleep(2000);

                    frmLong.ResizeToLastHeight();
                }
                catch (Exception e)
                {
                    Globals.log.Debug(e.ToString());
                }

            }

            public delegate void PopupCallback(string jsonMsg, int duration, FormAnimator.AnimationMethod animationMethod, FormAnimator.AnimationDirection animationDirection);
            public static void Popup(string jsonMsg, int duration, FormAnimator.AnimationMethod animationMethod, FormAnimator.AnimationDirection animationDirection)
            {
                if (formDummy.InvokeRequired)
                {
                    NotificationPanel.PopupCallback d = new NotificationPanel.PopupCallback(Popup);
                    formDummy.Invoke(d, new object[] { jsonMsg, duration, animationMethod, animationDirection });
                }
                else
                {
                    try
                    {
                        NotificationMessage dictMsg = new NotificationMessage(jsonMsg);

                        if (dictMsg.level == "" || dictMsg.contentAsWhole == "true")
                        {

                            if (duration == -1)
                            {
                                dictMsg.duration = duration;
                            }

                            var frmLong = (FormNotificationPanel)getFormByName("FormNotificationPanel");

                            if (!frmLong.Visible)
                            {
                                NotificationPopup.Popup(dictMsg.title, dictMsg.body, dictMsg.duration, animationMethod, animationDirection);
                            }

                            object[] param = new object[1];

                            var dictMsgFormatted = dictMsg.GetDictionaryFormatted();

                            param[0] = Globals.functions.Json_toJsonObj(dictMsgFormatted);

                            NotificationPanel.updateLongNotification(param);

                        }
                        else
                        {

                            var datetime = dictMsg.GetFormattedDateTime();

                            if (duration == -1)
                            {
                                dictMsg.duration = duration;
                            }

                            NotificationPopup.Popup(dictMsg.title, dictMsg.body, dictMsg.duration, animationMethod, animationDirection);

                            object[] param = new object[1];
                            var dictMsgFormatted = dictMsg.GetDictionaryFormatted();

                            param[0] = Globals.functions.Json_toJsonObj(dictMsgFormatted);

                            NotificationPanel.updateLongNotification(param);
                        }

                        Globals.notification_client_numberNow++;

                    }
                    catch (Exception e)
                    {
                        Globals.log.Debug(e.ToString());
                    }
                }
            }
        }

        public static void StartCaptureScreenRecord()
        {
            if (Globals.EnabledCaptureSceen)
            {
                try
                {
                    //~ DONT DELETE
                    //Thread threadCaptureScreen = new Thread(new ThreadStart(CaptureScreenThread.ThreadMain));
                    //threadCaptureScreen.Start();
                }
                catch (Exception e)
                {
                    Globals.log.Error(e.ToString());
                }
            }
        }

        public static void ChangeNotifyIcon(Icon icon)
        {
            trayIcon.Icon = icon;
        }

        private void InitializeTrayMenu()
        {
            try
            {
                Globals.log.Debug("AmivoiceWatcher:> Start InitializeTrayMenu()");
                //---- Setup menu on Tray Icon
                trayMenu = new ContextMenu();

                MenuItem mi_Help = new MenuItem("Help");
                MenuItem mi_Exit = new MenuItem("Exit (&X)", OnExit);

                MenuItem mi_Transparent = new MenuItem("Transparent", TrayMenu.Transparent);

                mi_Transparent.Checked = Globals.notification_dialog_isTransparent;

                mi_Help.MenuItems.Add(new MenuItem("Open log file (&L)", TrayMenu.OpenLogFile));
                mi_Help.MenuItems.Add(new MenuItem("Open configuration file (&C)", TrayMenu.OpenConfigFile));
                mi_Help.MenuItems.Add(new MenuItem("Open log configuration file (&O)", TrayMenu.OpenLogConfigFile));
                mi_Help.MenuItems.Add(new MenuItem("Version dialog (&V)", TrayMenu.OpenVersionDialog));

                // Uncomment to enable
                // some function inside is obsoleted
                //mi_Help.MenuItems.Add(new MenuItem("Config Notification", TrayMenu.OpenConfigNotificationDialog));

                trayMenu.MenuItems.AddRange(new MenuItem[] { mi_Transparent, mi_Help, mi_Exit });

                // Create a tray icon
                trayIcon = new NotifyIcon();
                trayIcon.Text = "AmivoiceWatcher";
                //trayIcon.Icon = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".ico", 40, 40);
                trayIcon.Icon = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".ico", 32, 32);

                // Add menu to tray icon and show it.
                trayIcon.ContextMenu = trayMenu;
                trayIcon.Visible = true;

                trayIcon.MouseUp += new MouseEventHandler(TrayMenu.trayIcon_MouseLeftClick);

                trayIcon.BalloonTipClosed += (sender, e) => {
                    var thisIcon = (NotifyIcon)sender;
                    thisIcon.Visible = false;
                    thisIcon.Dispose();
                };

                //LongNotification._init();

                Globals.log.Debug("AmivoiceWatcher:> Finish InitializeTrayMenu()");

            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
            }
        }

        protected override void OnLoad(EventArgs ev)
        {
            try
            {
                Visible = false; // Hide form window.
                ShowInTaskbar = false; // Remove from taskbar.

                Globals.log.Debug(String.Format("{0}({1}) is login by {2}(ip:{3} , mac_address:{4})", Globals.myComputerInfo.ComputerName, Globals.myComputerInfo.Os, Globals.myComputerInfo.UserName, Globals.myComputerInfo.Ip, Globals.myComputerInfo.MacAddress));

                base.OnLoad(ev);
            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
            }
        }

        private void Exit_UserActivityThread()
        {
            try
            {
                if (Configuration.CheckEnableAgentActivity())
                {
                    threadUserActivity.Abort();
                }
            }
            catch (Exception e)
            {
                Globals.log.Debug(e.ToString());
            }
        }

        private void Exit_CaptureSceenThread()
        {
            try
            {
                if (Globals.EnabledCaptureSceen)
                {
                    //~ Auto exit()
                    //No CaptureScreen.blnThreadAborted = true;
                    Globals.log.Debug("Try Exit capture screen");
                    //CaptureScreenThread.Exit();
                }
            }
            catch (Exception e)
            {
                Globals.log.Debug(e.ToString());
            }
        }

        public static void ForceExit()
        {
            try
            {
                Globals.log.Debug("Start ForceExit()");

                Globals.log.Debug("Set:> Globals.isProgramExit = true");
                Globals.isProgramExit = true;

                //ComputerInfo.SubmitComputerLog(ComputerInfo.SubmitComputerLogMode.Logoff);

                //if (Configuration.CheckEnableAgentActivity())
                //{
                //    Exit_UserActivityThread();
                //}

                //Exit_CaptureSceenThread();
                //CloseForms();

                ExitCefSharp();

                Globals.log.Debug("Process.GetCurrentProcess().Kill()");

                //Application.Exit();
                //Environment.Exit(0);
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
            }
        }

        private void OnExit(object sender, EventArgs ev)
        {
            try
            {
                Globals.log.Debug("Start OnExit()");

                myFormCefsharpDummy.SendComputerLog_logoff();

                var countdown_wait = 5;
                while (myFormCefsharpDummy.myState != FormCefsharpDummy.State.ready_to_exit)
                {

                    FormCefsharpDummy.State enumDisplayStatus = (FormCefsharpDummy.State)myFormCefsharpDummy.myState;
                    var stringValue = enumDisplayStatus.ToString();

                    Thread.Sleep(500);
                    Globals.log.Debug("Wait until:> myFormCefsharpDummy.myState == ready_to_exit");
                    Globals.log.Debug("But now:> myFormCefsharpDummy.myState = " + stringValue);
                    if (countdown_wait < 0)
                    {
                        break;
                    }
                    countdown_wait--;
                }

                Globals.log.Debug("Set:> Globals.isProgramExit = true");
                Globals.isProgramExit = true;

                //ComputerInfo.SubmitComputerLog(ComputerInfo.SubmitComputerLogMode.Logoff);

                if (Configuration.CheckEnableAgentActivity())
                {
                    Exit_UserActivityThread();
                }

                //Exit_CaptureSceenThread();
                CloseForms();

                ExitCefSharp();

                Globals.log.Debug("Finish OnExit()");

                Application.Exit();
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
            }
        }

        public static void CloseForms()
        {
            Globals.log.Debug("Closing multiple forms");
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                var formName = "";
                try
                {
                    formName = Application.OpenForms[i].Name;
                    if (formName != "Menu")
                    {
                        Globals.log.Debug("Closing forms: " + formName);
                        Application.OpenForms[i].Hide();
                        Application.OpenForms[i].Close();
                    }
                }
                catch (Exception e)
                {
                    Globals.log.Debug("Cant Close form: " + formName);
                    Globals.log.Error(e.ToString());
                }
            }

            formDummy.Close();

            Globals.log.Debug("Finishied :> Close multiple forms");
        }

        public class TrayMenu
        {
            delegate void ToggleFormLongNotificationCallback();
            public static void ToggleFormLongNotification()
            {
                if (formDummy.InvokeRequired)
                {
                    ToggleFormLongNotificationCallback d = new ToggleFormLongNotificationCallback(ToggleFormLongNotification);
                    formDummy.Invoke(d, new object[] { });
                }
                else
                {
                    foreach (Form frm in formDummy.OwnedForms)
                    {
                        if (frm.Name == "FormNotificationPanel")
                        {
                            if (frm.Visible)
                            {
                                frm.Hide();
                            }
                            else
                            {
                                frm.Show(formDummy);
                            }
                        }
                    }
                }
            }

            public static void ShowFormLongNotification()
            {
                foreach (Form frm in formDummy.OwnedForms)
                {
                    if (frm.Name == "FormNotificationPanel")
                    {
                        if (frm.Visible)
                        {
                            //frm.Hide();
                        }
                        else
                        {
                            frm.Show(formDummy);
                        }
                    }
                }
            }

            public static void trayIcon_MouseLeftClick(object sender, MouseEventArgs e)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    ToggleFormLongNotification();
                }
            }

            public static void Transparent(object sender, EventArgs ev)
            {
                try
                {
                    MenuItem mi = sender as MenuItem;

                    Globals.notification_dialog_isTransparent = !Globals.notification_dialog_isTransparent;

                    if (Globals.notification_dialog_isTransparent)
                    {
                        foreach (Form frm in formDummy.OwnedForms)
                        {
                            frm.Opacity = Globals.notification_dialog_opacity;
                        }
                    }
                    else
                    {
                        foreach (Form frm in formDummy.OwnedForms)
                        {
                            frm.Opacity = 1;
                        }
                    }

                    mi.Checked = Globals.notification_dialog_isTransparent;
                }
                catch (Exception e)
                {
                    Globals.log.Error(e.ToString());
                }
            }

            public static void OpenLogFile(object sender, EventArgs ev)
            {
                try
                {
                    var path2file = MyPath.logfile;

                    Process.Start("notepad.exe", path2file);
                }
                catch (Exception e)
                {
                    Globals.log.Error(e.ToString());
                }
            }

            public static void OpenConfigFile(object sender, EventArgs ev)
            {
                try
                {
                    string fileName = "";

                    string path4config = MyPath.configfile;

                    if (File.Exists(path4config))
                    {
                        fileName = path4config;
                    }
                    else
                    {
                        //To get the location the assembly normally resides on disk or the install directory
                        string path4configSameAsExe = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        path4configSameAsExe = Path.Combine(path4configSameAsExe, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".ini");

                        if (File.Exists(path4configSameAsExe))
                        {
                            fileName = path4configSameAsExe;
                        }
                    }

                    Process.Start("notepad.exe", fileName);
                }
                catch (Exception e)
                {
                    Globals.log.Error(e.ToString());
                }
            }

            public static void OpenLogConfigFile(object sender, EventArgs ev)
            {
                try
                {
                    var fileName = "AmivoiceWatcher.exe.config";
                    Process.Start("notepad.exe", fileName);
                }
                catch (Exception e)
                {
                    Globals.log.Error(e.ToString());
                }
            }

            public static void OpenConfigNotificationDialog(object sender, EventArgs ev)
            {
                try
                {
                    using (FormConfigNotification fTemp = new FormConfigNotification())
                    {
                        fTemp.ShowDialog(formDummy);
                    }
                }
                catch (Exception e)
                {
                    Globals.log.Error(e.ToString());
                }
            }

            public static void OpenVersionDialog(object sender, EventArgs ev)
            {
                try
                {
                    using (FormAbout fAbout = new FormAbout())
                    {
                        fAbout.ShowDialog(formDummy);
                    }
                }
                catch (Exception e)
                {
                    Globals.log.Error(e.ToString());
                }
            }
        }


        // For dealing with shutdown signal
        private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs ev)
        {
            try
            {
                Globals.log.Debug("Exit Program because Receive SystemEvents_SessionEnding=" + ev.Reason.ToString());
                if (Environment.HasShutdownStarted || (ev.Reason == SessionEndReasons.SystemShutdown))
                {
                    ev.Cancel = true;
                    //Tackle Shutdown
                    ComputerInfo.SubmitComputerLog(ComputerInfo.SubmitComputerLogMode.Logoff);
                    ev.Cancel = false;
                }
                else if (ev.Reason == SessionEndReasons.Logoff)
                {
                    //Tackle log off
                    ComputerInfo.SubmitComputerLog(ComputerInfo.SubmitComputerLogMode.Logoff);
                }
                else
                {
                    ComputerInfo.SubmitComputerLog(ComputerInfo.SubmitComputerLogMode.Logoff);
                }
            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}