using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace AmivoiceWatcher
{

    public partial class FormCefsharpDummy : Form
    {
        public static ChromiumWebBrowser browser;

        private static string configurationURL = Configuration.configUrl;

        public State myState = State.zero;
        public enum State
        {
            zero,
            browser_initialized,
            downloading_config,
            loading_reloadPage,
            loading_reloadPage_finish,
            sending_startup,
            registering_rabbitMq,
            registering_rabbitMq_finish,
            //busy,
            idle,
            sending_logoff,
            ready_to_exit
        }

        //public bool isFinishLoadDefault = false;
        public bool is_FormLoadFinish = false;

        public FormCefsharpDummy()
        {
            InitializeComponent();

            InitializeBrowser();

        }

        private void InitializGUI()
        {
            this.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width + 100;
            this.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height + 100;

            this.Width = 0;
            this.Height = 0;

            this.Visible = false;
            this.Hide();
        }

        private void InitializeBrowser()
        {
            //boundJavascript = new BoundJavascriptObject(this);
            //var myCefSettings = new CefSettings();
            //myCefSettings.IgnoreCertificateErrors = true;
            //myCefSettings.WindowlessRenderingEnabled = true;
            //myCefSettings.SetOffScreenRenderingBestPerformanceArgs();
            //myCefSettings.LogFile = Path.Combine(MyPath.PathLocalAppData, "cefsharp.log");
            //myCefSettings.LogSeverity = LogSeverity.Warning;
            //myCefSettings.CefCommandLineArgs.Add("log-file", myCefSettings.LogFile);


            //if (!Cef.IsInitialized)
            //{
            //    //Cef.Initialize(new CefSettings());
            //    Cef.Initialize(myCefSettings, false, true);
            //}

            //browser = new ChromiumWebBrowser("dummy:")
            //browser = new ChromiumWebBrowser(@"http://www.google.com")
            //browser = new ChromiumWebBrowser(Globals.functions.GetTemplatePath("LongNotificationReload.html"))            

            while ((Configuration.myState < Configuration.State.getting_configUrl_completed))
            {
                Globals.log.Debug("FormCefbrowserDummy:> waiting because Configuration.myState < Configuration.State.getting_configUrl_completed");
                Thread.Sleep(500);
            }

#if DEBUG
            configurationURL = Debug.url_download_config;
#endif

            //browser = new ChromiumWebBrowser(configurationURL)
            browser = new ChromiumWebBrowser("dummy:")
            {
                Width = 500,
                Height = 500,
                Dock = DockStyle.Fill

            };
            var browserSettings = new BrowserSettings();
            browserSettings.FileAccessFromFileUrls = CefState.Enabled;
            browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
            browser.BrowserSettings = browserSettings;

            //browser.LoadingStateChanged += BrowserLoadingStateChanged;

            //cefsharpBrowser2 = new ChromiumWebBrowser("dummy:");

            //cefsharpBrowser.RegisterJsObject("bound", boundJavascript);

            //browser.Dock = DockStyle.Fill;

            browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            browser.DownloadHandler = new DownloadHandler();
            //browser.LoadingStateChanged += BrowserLoadingStateChanged;

            this.Controls.Add(browser);

            //browser.LoadHtml("", "dummy");
            //browser.Load(configurationURL);

            //while (myState < State.browser_initialized)
            //{
            //    //browser.Load("dummy:");
            //    Thread.Sleep(500);
            //}

            //DownloadConfigFile();

            //Load_ReloadPage();

        }

        private void OnIsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs args)
        {
            if (args.IsBrowserInitialized)
            {
                Globals.log.Debug("FormCefbrowserDummy:> Browser is Initialized");
                myState = State.browser_initialized;
                DownloadConfigFile();
            }
        }


        delegate void DownloadConfigFileCallback();
        public void DownloadConfigFile()
        {
            if (this.InvokeRequired)
            {
                DownloadConfigFileCallback d = new DownloadConfigFileCallback(DownloadConfigFile);
                this.Invoke(d, new object[] { });
            }
            else
            {

                Globals.log.Debug("DownloadConfigFile()");

                //var configurationURL = Configuration.GetConfigurationUrl();

                //while ((Configuration.myConfigurationSetupState == Configuration.ConfigurationSetupState.zero) 
                //    || (Configuration.myConfigurationSetupState == Configuration.ConfigurationSetupState.getting_configUrl)
                //    || (Configuration.myConfigurationSetupState == Configuration.ConfigurationSetupState.getting_configUrl_completed))
                //while ((Configuration.myState < Configuration.State.getting_configUrl_completed))
                //{

                //    Thread.Sleep(500);
                //}

                // while (Configuration.myState != Configuration.ConfigurationSetupState.downloading)
                //{
                //    if (Configuration.myState == Configuration.ConfigurationSetupState.completed)
                //    {
                //        myState = State.idle;
                //        Load_ReloadPage();
                //        return;
                //    }
                //    Thread.Sleep(500);
                //}

                //this.Controls.Add(browser);

                configurationURL = Configuration.configUrl;

                Globals.log.Debug("configurationURL = "+ configurationURL);
                if (configurationURL == "")
                {
                    //Configuration.myState = Configuration.State.downloadedFail;
                    Configuration.myState = Configuration.State.setting_var;
                    //Configuration.myConfigurationSetupState = Configuration.ConfigurationSetupState.setting_var;
                }
                else
                {
                    myState = State.downloading_config;

                    browser.Load("dummy:");
                    browser.Load("");

                    //browser.Stop();

                    // An event that is fired when the first page is finished loading.
                    // This returns to us from another thread.
                    //browser.LoadingStateChanged += BrowserLoadingStateChanged;
                    //browser.DownloadHandler = new DownloadHandler();

                    Globals.log.Debug("FormCefbrowserDummy:> browser.load -> " + configurationURL);
                    browser.Load(configurationURL);
                }

                //myState = State.idle;

            }
        }

        public void Load_ReloadPage()
        {
            browser.Stop();
            Globals.log.Debug("FormCefsharpDummy => Load_ReloadPage()");

            myState = State.loading_reloadPage;

            browser.LoadingStateChanged += BrowserLoadingStateChanged;

            browser.Load((Globals.functions.GetTemplatePath("LongNotificationReload.html")));

            //myState = State.idle;
        }

        public void SendComputerLog_startup()
        {
            if (myState == State.sending_startup)
            {
                return;
            }
            myState = State.sending_startup;
            try
            {
                while (Configuration.myState != Configuration.State.completed)
                {
                    Thread.Sleep(500);
                }

                Globals.log.Debug("FormCefsharpDummy: Try to submitting computer info (startup)");

                browser.LoadingStateChanged += BrowserLoadingStateChanged;

                //var url = @"http://192.168.1.88:3002/webapi/update_computer_log";
                var url = Configuration.configuration["update_computer.url"];

                var postData = ComputerInfo.GetComputerInfoJsonString(ComputerInfo.SubmitComputerLogMode.Startup);

                //browser.Load("dummy:");
                //while (!isFinishLoadDefault)
                //{
                //    Thread.Sleep(1000);
                //}

                while (browser.IsLoading)
                {
                    Thread.Sleep(1000);
                }

                var script = String.Format("sendPostRequest('{0}','{1}')", url, postData);
                Globals.log.Debug("FormCefsharpDummy: runing script:> " + script);

                browser.EvaluateScriptAsync(script).ContinueWith(x =>
                {
                    var isOk = false;


                    if (x.IsFaulted)
                    {
                        Globals.log.Debug("FormCefsharpDummy: run script results x.IsFaulted ");
                    }
                    else
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                        {
                            var html = response.Result.ToString();

                            Globals.log.Debug("FormCefsharpDummy: run script return = " + html);

                            if (html == "")
                            {
                                //Globals.log.Warn(String.Format("FormCefsharpDummy: Unable to submit computer info to url" + url));
                            }
                            else
                            {
                                Globals.log.Debug("FormCefsharpDummy: run script return = " + html);
                                isOk = true;
                            }
                        }
                        else
                        {

                        }
                    }


                    if (isOk)
                    {
                        Globals.log.Debug("FormCefsharpDummy: Success to submitting computer info  ");
                    }
                    else
                    {
                        Globals.log.Warn(String.Format("FormCefsharpDummy: Unable to submit computer info to url" + url));
                    }

                    myState = State.registering_rabbitMq;

                    RegisterRabbitMq();

                });
            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
            }
            //myState = State.idle;
        }

        public void SendComputerLog_logoff()
        {
            myState = State.sending_logoff;
            try
            {
                Globals.log.Debug("Try to submitting computer info(logoff) ");

                browser.LoadingStateChanged += BrowserLoadingStateChanged;

                //var url = @"http://192.168.1.88:3002/webapi/update_computer_log";
                var url = Configuration.configuration["update_computer.url"];

                var postData = ComputerInfo.GetComputerInfoJsonString(ComputerInfo.SubmitComputerLogMode.Logoff);

                //browser.Load("dummy:");
                while (myState < State.loading_reloadPage_finish)
                {
                    Thread.Sleep(1000);
                }

                while (browser.IsLoading)
                {
                    Thread.Sleep(1000);
                }

                var script = String.Format("sendPostRequest('{0}','{1}')", url, postData);

                browser.EvaluateScriptAsync(script).ContinueWith(x =>
                {

                    if (x.IsFaulted)
                    {
                        Globals.log.Warn(String.Format("Unable to submit computer info to url" + url));
                    }
                    else
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                        {
                            var html = response.Result.ToString();

                            Globals.log.Debug("FormCefsharpDummy: run script return = " + html);

                            if (html == "")
                            {
                                Globals.log.Warn(String.Format("Unable to submit computer info to url" + url));
                            }
                            else
                            {
                                Globals.log.Debug("FormCefsharpDummy: run script return = " + html);
                                Globals.log.Debug("Success to submitting LOGOFF computer info  ");
                            }
                        }
                        else
                        {
                            Globals.log.Warn(String.Format("Unable to submit computer info to url" + url));
                        }
                    }

                    myState = State.ready_to_exit;
                });
            }
            catch (Exception e)
            {
                Globals.log.Debug("Cant submit computer info(logoff)");
                Globals.log.Error(e.ToString());
            }
            //myState = State.idle;
        }

        private bool hasRun_RegisterRabbitMq = false;
        public void RegisterRabbitMq()
        {
            try
            {
                if (hasRun_RegisterRabbitMq)
                {
                    return;
                }
                hasRun_RegisterRabbitMq = true;

                Globals.log.Debug("FormCefbrowserDummy:> Try to register RabbitMq ");


                while (Configuration.myState != Configuration.State.completed)
                {
                    Thread.Sleep(500);
                }

                var login_name = Globals.myComputerInfo.UserName;
#if DEBUG
                //login_name = "aohsadmin";
#endif

                var url = Configuration.configuration["aohs.baseurl"] + @"/webapi/client_notify?do_act=register&login_name=" + login_name;

                var pathPure = Configuration.configuration["aohs.baseurl"] + @"/webapi/client_notify";
                var data = @"{'do_act':'register','login_name':'" + login_name + "'}";

#if DEBUG
                //path = path + "xxx";
#endif

                Globals.log.Debug("FormCefsharpDummy :> Register Notification through: " + url);

                //var i = 0;
                //while (myState != State.idle)
                //{
                //    Globals.log.Debug("FormCefbrowserDummy:> Try to register RabbitMq BUT wait for State.idle");

                //    i++;
                //    if (i > 10)
                //    {
                //        break;
                //    }

                //    Thread.Sleep(500);
                //}

                //browser.LoadingStateChanged += BrowserLoadingStateChanged;

                //var url = Configuration.configuration["update_computer.url"];

                //var postData = ComputerInfo.GetComputerInfoJsonString(ComputerInfo.SubmitComputerLogMode.Logoff);
                //var postData = data;

                //browser.Load("dummy:");
                //if (!isFinishLoadDefault)
                //{
                //    if (browser.Address != Globals.functions.GetTemplatePath("LongNotificationReload.html"))
                //    {
                //        Load_ReloadPage();
                //    }
                //    Thread.Sleep(1000);

                //}

                //while (!isFinishLoadDefault)
                //{
                //    if (browser.Address != Globals.functions.GetTemplatePath("LongNotificationReload.html")){
                //        //Load_ReloadPage();
                //    }
                //    Thread.Sleep(1000);

                //}

                while (browser.IsLoading)
                {
                    Thread.Sleep(1000);
                }

                //var script = String.Format("sendGetRequest('{0}',\"{1}\")", url, postData);
                var script = String.Format("sendGetRequest('{0}')", url);

                Globals.log.Debug("FormCefsharpDummy :> run script:> " + script);

                browser.EvaluateScriptAsync(script).ContinueWith(x =>
                {
                    if (x.IsFaulted)
                    {
                        Globals.log.Debug("FormCefsharpDummy: run script results x.IsFaulted ");
                    }
                    else
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                        {

                            var html = response.Result.ToString();

                            Globals.log.Debug("FormCefsharpDummy: run script return = " + html);
                            Globals.log.Debug("FormCefsharpDummy: passing return value to RabbitMQWrapper.RegisterNotificationByJson(.)");

                            myState = State.registering_rabbitMq_finish;

                            RabbitMQWrapper.RegisterNotificationByJson(html);

                        }
                        else
                        {
                            myState = State.registering_rabbitMq_finish;

                            Globals.log.Warn(String.Format("Unable to submit computer info to url" + url));
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
            }
        }

        //public void SendPostRequest(string url, string data)
        //{
        //    try
        //    {
        //        browser.LoadingStateChanged += BrowserLoadingStateChanged;

        //        //var testUrl = @"http://192.168.1.88:3002/webapi/update_computer_log";
        //        var postData = ComputerInfo.GetComputerInfoJsonString(ComputerInfo.SubmitComputerLogMode.Startup);

        //        //browser.Load("dummy:");
        //        while (myState < State.loading_reloadPage_finish)
        //        {
        //            Thread.Sleep(1000);
        //        }

        //        while (browser.IsLoading)
        //        {
        //            Thread.Sleep(1000);
        //        }

        //        var script = String.Format("sendPostRequest('{0}','{1}')", url, postData);

        //        browser.EvaluateScriptAsync(script).ContinueWith(x =>
        //        {
        //            if (x.IsFaulted)
        //            {
        //                //Globals.log.Warn(String.Format("Unable to submit computer info to url" + url));
        //            }
        //            else
        //            {
        //                var response = x.Result;

        //                if (response.Success && response.Result != null)
        //                {
        //                    //var html = response.Result.ToString();

        //                    //Globals.log.Debug("Success to submitting computer info");

        //                }
        //                else
        //                {
        //                    //Globals.log.Warn(String.Format("Unable to submit computer info to url" + url));

        //                }
        //            }                    
        //        });               
        //    }
        //    catch (Exception e)
        //    {
        //        Globals.log.Error(e.ToString());
        //    }

        //}

        //public void DownloadFile(string url)
        //{
        //    try
        //    {
        //        browser.Load("dummy:");

        //        // An event that is fired when the first page is finished loading.
        //        // This returns to us from another thread.
        //        //browser.LoadingStateChanged += BrowserLoadingStateChanged;
        //        browser.DownloadHandler = new DownloadHandler();

        //        //var testUrl = @"http://192.168.1.88:3002/configurations.txt?user=AohsAdmin";
        //        browser.Load(url);

        //        //browser.Load(url);
        //    }
        //    catch (Exception e)
        //    {
        //        Globals.log.Error(e.ToString());
        //    }

        //}

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
                            var html = response.Result.ToString();
                            //startDate is the value of a HTML element.

                            switch (myState)
                            {
                                case State.downloading_config:
                                    {
                                        if (html == "")
                                        {
                                            //cefsharpBrowser.LoadingStateChanged += handler;

                                            return;
                                        }

                                        if (html == "<head></head><body></body>")
                                        {
                                            if (myState == State.loading_reloadPage)
                                            {
                                                return;
                                            }
                                            myState = State.loading_reloadPage;

                                            Globals.log.Debug("FormCefsharpDummy :> Cannot load config file <= html = " + html);

                                            //browser.LoadingStateChanged += BrowserLoadingStateChanged;
                                            Load_ReloadPage();
                                        }

                                        break;
                                    }
                                case State.loading_reloadPage:
                                    {
                                        if (html == "")
                                        {
                                            //cefsharpBrowser.LoadingStateChanged += handler;

                                            return;
                                        }

                                        //Match match = Regex.Match(html, @"\\$(AmiVoice Watcher.*)\\$");
                                        Match match = Regex.Match(html, "@(.*)@");

                                        if (match.Success)
                                        {
                                            var whatPage = match.Groups[1].Value;

                                            if (whatPage == "AmiVoice Watcher (Reload)")
                                            {
                                                if (myState != State.loading_reloadPage_finish)
                                                {                                                    
                                                    Globals.log.Debug("FormCefsharpDummy :> loaded Amivoice Watcher(Reload) successfully.");

                                                    myState = State.loading_reloadPage_finish;

                                                    //isFinishLoadDefault = true;
                                                }

                                                //SendComputerLog_startup();
                                                //AmivoiceWatcher.LongNotification.ShowPanel();
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                });
            }
        }

        internal static void StopLoading()
        {
            browser.Stop();
        }

        private void FormCefsharpDummy_Load(object sender, EventArgs e)
        {
            InitializGUI();

            is_FormLoadFinish = true;
        }

        //private void FormCefsharpDummy_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    //SendComputerLog_logoff();
        //    while(myState != State.idle)
        //    {
        //        Thread.Sleep(500);
        //    }
        //}


        private void FormCefsharpDummy_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void FormCefsharpDummy_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
