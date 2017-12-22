using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmivoiceWatcher
{
    class Configuration
    {
        public static Dictionary<string, string> configuration;

        public static string configUrl = "";

        public static State myState = State.zero;
        public enum State
        {
            zero,
            getting_configUrl,
            getting_configUrl_completed,
            downloading,
            downloadedSuccess,
            downloadedFail,
            setting_var,
            completed
        }

        //public static bool isDownloadFileSuccess = false;
        //public static bool isLoadingFile = true;

        //public static bool isSetConfigurationSuccess = false;

        private static string path4config = Path.Combine(MyPath.PathLocalAppData, @"configuration.txt");
        private static string path4configReal = "";

        public static string reg4ConfigurationURL;
        public static string regkey4ConfigurationURL;

        static Configuration()
        {
            configuration = new Dictionary<string, string>
                {
                    {@"agentactivity.foreground_check.enabled",@"0"},
                    {@"agentactivity.idle_check.enabled",@"0"},
                    {@"agentactivity.idle_check.min_time.sec",@"10"},
                    {@"agentactivity.ie_history_check.enabled",@"0"},
                    {@"agentactivity.ie_history_check.timer.sec",@"30"},
                    {@"agentactivity.localstorage.filename",@"%TEMP%\aa"},
                    {@"agentactivity.senddata.timer.sec",@"20"},
                    {@"agentactivity.upload_fail_recoveryN",@"24"},
                    {@"agentactivity.url",@"http://192.168.1.51/aohs/webapi/agent_activity"},
                    {@"computer_info",@"cti,advw,java,os,mediaplayer,ie"},
                    {@"computer_info.advw",@"C:Program FilesAmiVoiceAudio ViewerAmiAdvw.dll"},
                    {@"computer_info.cti",@"C:Program FilesEventCaptureCTI_TOOLBAR.exe"},
                    {@"computer_info.moss",@"127.0.0.1"},
                    {@"target.enable_watching_ie_control",@"1"},
                    {@"target.finding.executable_filename",@"CTI_TO"},
                    {@"target.finding.window_class.name",@"ThunderRT6FormDC"},
                    {@"target.finding.window_class.position",@"0"},
                    {@"target.finding.window_title.anti_pattern",@"Initiate Action,Log"},
                    {@"target.finding.window_title.anti_pattern_delim",@",@"},
                    {@"target.window_title.logout_pattern",@"logged out"},
                    {@"target.window_title.number_of_characters",@"48"},
                    {@"target.window_title.pattern", @"SoftPhone :: ACD"",""{a+} : Ext"",""{a+} : Ag ID"",""{a+}"},
                    {@"target.window_title.result_index.agentid",@"2"},
                    {@"target.window_title.result_index.extension",@"1"},
                    {@"target.window_title.result_index.extension2",@"0"},
                    {@"target.window_title.start_index",@"0"},
                    {@"update_computer.retry_count_max",@"5"},
                    {@"update_computer.retry_wait",@"10"},
                    {@"update_computer.sending_logoff",@"1"},
                    {@"update_computer.url",@"http://192.168.1.51/aohs/webapi/update_computer_log"},
                    {@"update_extension.url",@"http://192.168.1.51/aohs/webapi/update_extension"},
                    {@"watcher.log4cxx_properties.url",@""},
                    {@"watcher.startup_delay.sec",@"0"},
                    {@"watcher.timer.msec",@"1000.99"},
                    //NEW
                    {@"screencapture.record.vdo.enable",@"1"},
                    {@"screencapture.record.screenshot.enable",@"1"},
                    {@"screencapture.record.vdo.fps",@"4"},
                    {@"screencapture.record.vdo.resize_scale",@"0.5"},
                    {@"screencapture.record.timer.sec",@"2"},
                    {@"watcher.local_app.limit_size_Mb",@"100"},
                    {@"screencapture.record.vdo.upload.url",@"http://192.168.1.153/upload.php" },
                    {@"computer_info.senddata.timer.min",@"60"},
                    {@"screencapture.upload.timer.sec",@"10"},
                    //New not add to web
                    {@"notification.client.max",@"3"},
                    {@"notification.show.icon","true" },
                    {@"notification.duration","20" },
                    {@"aohs.baseurl",@"http://localhost:3000" }

              };

            reg4ConfigurationURL = @"HKEY_LOCAL_MACHINE\SOFTWARE\AmiVoice\AmiVoiceWatcher\1.0";
            regkey4ConfigurationURL = "ConfigurationURL";
        }

        private static bool String2bool(string strIn)
        {
            try
            {
                switch (strIn)
                {
                    case "true":
                    case "1":
                    case "True":
                    case "TRUE":
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
                return false;
            }            
        }


        public static bool CheckEnableAgentActivity()
        {
            try
            {
                return String2bool(configuration[@"agentactivity.foreground_check.enabled"]);
            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
                return false;
            }
        }

        public static void GetConfigurationUrl()
        {
            Globals.log.Debug("Configuration:> running GetConfigurationUrl()");

            configUrl = "";

            try
            {
                Globals.log.Debug("Trying to get ConfigurationURL in the registry " + Configuration.reg4ConfigurationURL + "/" + Configuration.regkey4ConfigurationURL);
                configUrl = (string)Registry.GetValue(Configuration.reg4ConfigurationURL, Configuration.regkey4ConfigurationURL, null);
            }
            catch (Exception e)
            {
                Globals.log.Warn("Can NOT get value from Registry");
                Globals.log.Warn(e.ToString());

            }

            if (String.IsNullOrEmpty(configUrl))
            {
                try
                {
                    Globals.log.Debug("ConfigurationURL is String.Empty . Trying to get the value by windows64bit method...");
                    configUrl = RegistryWOW6432.GetRegKey64(RegHive.HKEY_LOCAL_MACHINE, Configuration.reg4ConfigurationURL.Replace(@"HKEY_LOCAL_MACHINE\", ""), Configuration.regkey4ConfigurationURL);

                    if (String.IsNullOrEmpty(configUrl))
                    {
                        configUrl = RegistryWOW6432.GetRegKey32(RegHive.HKEY_LOCAL_MACHINE, Configuration.reg4ConfigurationURL.Replace(@"HKEY_LOCAL_MACHINE\", ""), Configuration.regkey4ConfigurationURL);
                    }
                }
                catch (Exception e)
                {
                    Globals.log.Debug("Error in getting the value by windows64bit method ");
                    Globals.log.Error(e.ToString());
                }

                if (String.IsNullOrEmpty(configUrl))
                {
                    Globals.log.Debug("ConfigurationURL is still String.Empty");
                }
            }            
        }

        private static void GetConfigurationFile(string configurationURL, string path4config)
        {
            string str_content = "";
            
            if (!String.IsNullOrEmpty(configurationURL))
            {
                try
                {

#if DEBUG
                        //ConfigurationURL = "xxx" + ConfigurationURL + "xxx";
#endif

                    Globals.log.Debug("Start downloading config file from ConfigurationURL(registry) = " + configurationURL);

                    HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(configurationURL);
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    //httpWebRequest.Accept = "text/*, text/html, text/html;level=1, */*, application/xhtml+xml, */*";
                    httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                    httpWebRequest.UserAgent = @"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.89 Safari/537.36";
                    httpWebRequest.Method = "GET";
                    //httpWebRequest.Connection = "keep-alive";
                    //httpWebRequest.Host = "172.22.101.40";
                    httpWebRequest.KeepAlive = true;

                    //Get the headers associated with the request.
                    WebHeaderCollection myWebHeaderCollection = httpWebRequest.Headers;
                    myWebHeaderCollection.Add("Accept-Language", "en-US,en;q=0.9,th;q=0.8");
                    myWebHeaderCollection.Add("Accept-Encoding", "gzip, deflate");
                    myWebHeaderCollection.Add("Cache-Control", "no-cache");
                    myWebHeaderCollection.Add("Upgrade-Insecure-Requests", "1");
                    myWebHeaderCollection.Add("Pragma", "no-cache");

                    IWebProxy proxy = httpWebRequest.Proxy;
                    if (proxy != null)
                    {
                        string proxyuri = proxy.GetProxy(httpWebRequest.RequestUri).ToString();
                        httpWebRequest.UseDefaultCredentials = true;
                        httpWebRequest.Proxy = new WebProxy(proxyuri, false);
                        //httpWebRequest.UseDefaultCredentials = false;
                        //httpWebRequest.Proxy = new WebProxy(proxyuri, true);
                        //httpWebRequest.Proxy = new WebProxy("172.22.101.40", false); 
                        //httpWebRequest.Proxy = new WebProxy("192.168.1.149", false);
                        httpWebRequest.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                        //httpWebRequest.UserAgent = "[any words that is more than 5 characters]";
                        //httpWebRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
                        httpWebRequest.UserAgent = @"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.89 Safari/537.36";
                    }

                    HttpWebResponse webResp = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (webResp == null || webResp.StatusCode != HttpStatusCode.OK)
                    {
                        //if error str_content = "", so move code to the end of this method
                    }
                    else
                    {
                        using (Stream stream = webResp.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            str_content = reader.ReadToEnd();
                        }

                        // Append text to an existing file named "WriteLines.txt".

                        System.IO.File.WriteAllText(path4config, str_content);

                        Globals.log.Debug("Complete uploading file to " + path4config);
                    }
                }
                catch (WebException e)
                {
                    Globals.log.Warn("Can not load config file from server! The recorded one(if exists) will be used.");
                    if (e.Response != null)
                    {
                        Globals.log.Warn(e.Response);
                    }

                    Globals.log.Warn(e.ToString());

                }
                catch (Exception e)
                {
                    Globals.log.Warn("Can not load config file from server! The recorded one(if exists) will be used.");
                    Globals.log.Warn(e.ToString());
                }
            }
        }

        private static void GetConfigurationFileByCefsharp(string configurationURL, string path4config)
        {
            string str_content = "";

            if (!String.IsNullOrEmpty(configurationURL))
            {
                try
                {

#if DEBUG
                        //ConfigurationURL = "xxx" + ConfigurationURL + "xxx";
#endif
                    Globals.log.Debug("Start downloading config file from ConfigurationURL(registry) = " + configurationURL);

                    HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(configurationURL);
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    //httpWebRequest.Accept = "text/*, text/html, text/html;level=1, */*, application/xhtml+xml, */*";
                    httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                    httpWebRequest.UserAgent = @"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.89 Safari/537.36";
                    httpWebRequest.Method = "GET";
                    //httpWebRequest.Connection = "keep-alive";
                    //httpWebRequest.Host = "172.22.101.40";
                    httpWebRequest.KeepAlive = true;

                    //Get the headers associated with the request.
                    WebHeaderCollection myWebHeaderCollection = httpWebRequest.Headers;
                    myWebHeaderCollection.Add("Accept-Language", "en-US,en;q=0.9,th;q=0.8");
                    myWebHeaderCollection.Add("Accept-Encoding", "gzip, deflate");
                    myWebHeaderCollection.Add("Cache-Control", "no-cache");
                    myWebHeaderCollection.Add("Upgrade-Insecure-Requests", "1");
                    myWebHeaderCollection.Add("Pragma", "no-cache");

                    IWebProxy proxy = httpWebRequest.Proxy;
                    if (proxy != null)
                    {
                        string proxyuri = proxy.GetProxy(httpWebRequest.RequestUri).ToString();
                        httpWebRequest.UseDefaultCredentials = true;
                        httpWebRequest.Proxy = new WebProxy(proxyuri, false);
                        //httpWebRequest.UseDefaultCredentials = false;
                        //httpWebRequest.Proxy = new WebProxy(proxyuri, true);
                        //httpWebRequest.Proxy = new WebProxy("172.22.101.40", false); 
                        //httpWebRequest.Proxy = new WebProxy("192.168.1.149", false);
                        httpWebRequest.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                        //httpWebRequest.UserAgent = "[any words that is more than 5 characters]";
                        //httpWebRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
                        httpWebRequest.UserAgent = @"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.89 Safari/537.36";
                    }

                    HttpWebResponse webResp = (HttpWebResponse)httpWebRequest.GetResponse();

                    if (webResp == null || webResp.StatusCode != HttpStatusCode.OK)
                    {
                        //if error str_content = "", so move code to the end of this method
                    }
                    else
                    {
                        using (Stream stream = webResp.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            str_content = reader.ReadToEnd();
                        }

                        // Append text to an existing file named "WriteLines.txt".

                        System.IO.File.WriteAllText(path4config, str_content);

                        Globals.log.Debug("Complete uploading file to " + path4config);
                    }
                }
                catch (WebException e)
                {
                    Globals.log.Warn("Can not load config file from server! The recorded one(if exists) will be used.");
                    if (e.Response != null)
                    {
                        Globals.log.Warn(e.Response);
                    }

                    Globals.log.Warn(e.ToString());
                }
                catch (Exception e)
                {
                    Globals.log.Warn("Can not load config file from server! The recorded one(if exists) will be used.");
                    Globals.log.Warn(e.ToString());
                }
            }
        }

       
        //private static void GetConfigurationFileByCefsharpDummyThread()
        //{
        //    //while (!AmivoiceWatcher.myFormCefsharpDummy.isFinishLoadDefault)
        //    //{
        //    //    Thread.Sleep(500);
        //    //}

        //    var i = 0;
            
        //    while ((myState != ConfigurationSetupState.downloadedSuccess) && (myState != ConfigurationSetupState.downloadedFail))
        //    {
        //        Thread.Sleep(1000);
        //        if (myState == ConfigurationSetupState.downloading)
        //        {
        //            i++;
        //            Thread.Sleep(1000);

        //            Globals.log.Debug(String.Format("Check if downloading configuration finish => {0} sec.", i));

        //            if (i > 20)
        //            {
        //                Globals.log.Warn("Can not load config file from server! The recorded one(if exists) will be used.");
        //                //isLoadingFile = false;
        //                myState = ConfigurationSetupState.downloadedFail;
        //                return;
        //            }
        //        }

        //    }

        //    myState = ConfigurationSetupState.setting_var;

        //    //isLoadingFile = false;

        //    //AmivoiceWatcher.myFormCefsharpDummy.DownloadFile(configurationURL);
        //    //AmivoiceWatcher.myFormCefsharpDummy.DownloadConfigFile();
        //}


        private static void SetConfiguration_Thread()
        {
            var waiting_counter = 0;
            while (!Globals.isProgramExit)
            {
                try
                {
                    switch (myState)
                    {
                        case State.zero:
                            {
                                myState = State.getting_configUrl;
                                break;

                            }
                        case State.getting_configUrl:
                            {
                                GetConfigurationUrl();

                                myState = State.getting_configUrl_completed;

                                break;
                            }
                        case State.getting_configUrl_completed:
                            {
                                if (configUrl == "")
                                {
                                    Globals.log.Debug("Configuration:> Finish getting ConfigurationURL=Sting.Empty");
                                    myState = State.setting_var;
                                }
                                else
                                {
                                    Globals.log.Debug("Configuration:> Finish getting ConfigurationURL and start waiting for dowloading url = " + configUrl);
                                    //GetConfigurationFileByCefsharpDummy();
                                    myState = State.downloading;
                                    Thread.Sleep(1000);
                                }

                                break;
                            }
                        case State.setting_var:
                            {
                                SetGlobalVariable();

                                //isSetConfigurationSuccess = true;
                                myState = myState = State.completed;
                                //AmivoiceWatcher.myFormCefsharpDummy.SendComputerLog_startup();
                                
                                break;
                            }
                        case State.downloading:
                            {                                
                                Thread.Sleep(1000);

                                Globals.log.Debug(String.Format("Configuration:> Check if downloading configuration finish => {0} sec.", waiting_counter));
                                waiting_counter++;

                                if (AmivoiceWatcher.myFormCefsharpDummy.myState < FormCefsharpDummy.State.browser_initialized)
                                {
                                    Globals.log.Warn("AmivoiceWatcher.myFormCefsharpDummy.myState < FormCefsharpDummy.State.browser_initialized");
                                    if (waiting_counter > 15)
                                    {
                                        Globals.log.Warn("Configuration:> Give up waiting. Try to exit the program now");
                                        //AmivoiceWatcher.ForceExit();
                                    }
                                }else if (AmivoiceWatcher.myFormCefsharpDummy.myState > FormCefsharpDummy.State.downloading_config)
                                {
                                    Globals.log.Warn("Configuration.State change to State.setting_var");
                                    //isLoadingFile = false;
                                    myState = State.setting_var;
                                }
                                else
                                {
                                    
                                    if (waiting_counter > 20)
                                    {
                                        Globals.log.Warn("Can not load config file from server! The recorded one(if exists) will be used.");
                                        //isLoadingFile = false;
                                        myState = State.downloadedFail;

                                        if (AmivoiceWatcher.myFormCefsharpDummy.myState == FormCefsharpDummy.State.downloading_config)
                                        {
                                            Globals.log.Warn("Configuration:> force myFormCefsharpDummy to stop downloading");
                                            FormCefsharpDummy.StopLoading();
                                            AmivoiceWatcher.myFormCefsharpDummy.Load_ReloadPage();
                                        }
                                    }
                                }
                              
                                break;
                            }
                        case State.downloadedFail:
                        case State.downloadedSuccess:
                            {
                                myState = State.setting_var;
                                break;
                            }
                        case State.completed:
                            {
                                Globals.log.Debug("Set configuration....OK");
                                Globals.log.Warn("Configuration:> fininish SetConfiguration_Thread()");
                                return;
                            }
                        default:
                            {
                                Thread.Sleep(500);
                                break;
                            }
                    }                    
                }
                catch(Exception e)
                {
                    Globals.log.Debug(e.ToString());
                }
            }

            Globals.log.Debug("Exit Configuration.SetConfiguration_Thread() Successfully");            
        }

        private static void SetGlobalVariable()
        {
            //---------- Set which config. file will be used.
            if (File.Exists(path4config))
            {
                path4configReal = path4config;
            }
            else
            {
                //To get the location the assembly normally resides on disk or the install directory
                //string path4configSameAsExe = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                string path4configSameAsExe = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                path4configSameAsExe = Path.Combine(path4configSameAsExe, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".ini");

                if (File.Exists(path4configSameAsExe))
                {
                    path4configReal = path4configSameAsExe;
                }
            }

            if (String.IsNullOrEmpty(path4configReal))
            {
                Globals.log.Warn("There is no configuration file! The default code-embed one will be used.");
            }
            else
            {
                Globals.log.Debug("Setup configuration from file: " + path4configReal);
                try
                {
                    Globals.iniData = Globals.iniParser.ReadFile(path4configReal);

                    foreach (IniParser.Model.KeyData key in Globals.iniData.Sections.GetSectionData("watcher").Keys)
                    {
                        Configuration.configuration[key.KeyName] = key.Value;
                    }
                }
                catch (Exception e)
                {
                    Globals.log.Warn(String.Format("Can not parse ini file: {0}", e.ToString()));
                }
            }

            foreach (string key in Configuration.configuration.Keys)
            {
                Globals.log.Debug(String.Format("Configuration.configuration[{0}]={1}", key, Configuration.configuration[key]));
            }

            //Post set configuration value 
            Globals.Configuration_SetValueFromSetFile();            
        }

//        private static void GetConfigurationFileByCefsharpDummy()
//        {
//            //while (myConfigurationSetupState != ConfigurationSetupState.zero)
//            //{
//            //    Thread.Sleep(500);
//            //}
//            if (!String.IsNullOrEmpty(configUrl))
//            {

//                try
//                {

//#if DEBUG
//                    //configurationURL = "xxx" + configurationURL + "xxx";
//#endif

//                    //Globals.log.Debug("Start downloading config file from ConfigurationURL(registry) = " + configurationURL);


//                    //AmivoiceWatcher.myFormCefsharpDummy.browser.Load(configurationURL);
//                    Thread thread1 = new Thread(new ThreadStart(GetConfigurationFileByCefsharpDummyThread));
//                    thread1.Start();


//                }
//                catch (WebException e)
//                {
//                    Globals.log.Warn("Can not load config file from server! The recorded one(if exists) will be used.");
//                    if (e.Response != null)
//                    {
//                        Globals.log.Warn(e.Response);
//                    }

//                    Globals.log.Warn(e.ToString());

//                }
//                catch (Exception e)
//                {
//                    Globals.log.Warn("Can not load config file from server! The recorded one(if exists) will be used.");
//                    Globals.log.Warn(e.ToString());
//                }
//            }
//        }

        public static void Initialize()
        {
            try
            {
                //Globals.log.Debug("Set configuration....");

                //path4config = Path.Combine(MyPath.PathLocalAppData, @"configuration.txt");
                //path4configReal = "";

                // prepare path for (eg C:\Users\UserName\AppData\Local\AmivoiceWatcher\configuration.txt)

                Globals.Initilize();
                // get url from registry
                // then load file content
                //Globals.log.Debug("Setup configuration value ...");
                //string configUrl = "";

                //GetConfigurationUrl();

                //GetConfigurationFile(configurationURL, path4config);

                //GetConfigurationFileByCefsharp(configurationURL, path4config);

                //GetConfigurationFileByRestsharp(configurationURL, path4config);

                //GetConfigurationFileByCefsharpDummy();

                Thread thread1 = new Thread(new ThreadStart(SetConfiguration_Thread));
                thread1.Start();

            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
            }
        }
    }

}
