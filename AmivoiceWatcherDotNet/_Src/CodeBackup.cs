using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmivoiceWatcher._Src
{
    class CodeBackup
    {
        #region RabbitMQWrapper
        //private static void Setup__UNUSED()
        //{

        //    //RegisterNotification();

        //    if (_rabbitMQ == null)
        //    {
        //        //IsRegisterNotificationSuccess = false;
        //    }
        //    else if (_rabbitMQ.rabbitmq == null)
        //    {
        //        //IsRegisterNotificationSuccess = false;
        //    }
        //    else
        //    {
        //        currentState = State.registered;
        //        //IsRegisterNotificationSuccess = true;

        //        factory = new ConnectionFactory()
        //        {
        //            HostName = _rabbitMQ.rabbitmq.host,
        //            UserName = _rabbitMQ.rabbitmq.username,
        //            Password = _rabbitMQ.rabbitmq.password,
        //            VirtualHost = _rabbitMQ.rabbitmq.vhost,

        //            RequestedConnectionTimeout = (int)(1000 * _rabbitMQ.rabbitmq.connection_timeout),
        //        };

        //        currentState = State.connected;
        //    }
        //}

        #endregion

        #region RestSharp
        //        private static void GetConfigurationFileByRestsharp(string configurationURL, string path4config)
        //        {

        //            if (!String.IsNullOrEmpty(configurationURL))
        //            {

        //                try
        //                {

        //#if DEBUG
        //                    //configurationURL = "xxx" + configurationURL + "xxx";
        //#endif
        //                    //while (!AmivoiceWatcher.myFormCefsharpDummy.is_FormLoadFinish)
        //                    //{
        //                    //    Thread.Sleep(500);
        //                    //}

        //                    Globals.log.Debug("Start downloading config file from ConfigurationURL(registry) = " + configurationURL);

        //                    //HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(configurationURL);
        //                    //httpWebRequest.ContentType = "application/json; charset=utf-8";
        //                    ////httpWebRequest.Accept = "text/*, text/html, text/html;level=1, */*, application/xhtml+xml, */*";
        //                    //httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        //                    //httpWebRequest.UserAgent = @"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.89 Safari/537.36";
        //                    //httpWebRequest.Method = "GET";
        //                    ////httpWebRequest.Connection = "keep-alive";
        //                    ////httpWebRequest.Host = "172.22.101.40";
        //                    //httpWebRequest.KeepAlive = true;

        //                    ////Get the headers associated with the request.
        //                    //WebHeaderCollection myWebHeaderCollection = httpWebRequest.Headers;
        //                    //myWebHeaderCollection.Add("Accept-Language", "en-US,en;q=0.9,th;q=0.8");
        //                    //myWebHeaderCollection.Add("Accept-Encoding", "gzip, deflate");
        //                    //myWebHeaderCollection.Add("Cache-Control", "no-cache");
        //                    //myWebHeaderCollection.Add("Upgrade-Insecure-Requests", "1");
        //                    //myWebHeaderCollection.Add("Pragma", "no-cache");

        //                    //IWebProxy proxy = httpWebRequest.Proxy;
        //                    //if (proxy != null)
        //                    //{
        //                    //    string proxyuri = proxy.GetProxy(httpWebRequest.RequestUri).ToString();
        //                    //    httpWebRequest.UseDefaultCredentials = true;
        //                    //    httpWebRequest.Proxy = new WebProxy(proxyuri, false);
        //                    //    //httpWebRequest.UseDefaultCredentials = false;
        //                    //    //httpWebRequest.Proxy = new WebProxy(proxyuri, true);
        //                    //    //httpWebRequest.Proxy = new WebProxy("172.22.101.40", false); 
        //                    //    //httpWebRequest.Proxy = new WebProxy("192.168.1.149", false);
        //                    //    httpWebRequest.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
        //                    //    //httpWebRequest.UserAgent = "[any words that is more than 5 characters]";
        //                    //    //httpWebRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
        //                    //    httpWebRequest.UserAgent = @"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.89 Safari/537.36";
        //                    //}


        //                    var client = new RestClient(configurationURL);
        //                    var request = new RestRequest("#", Method.GET);
        //                    //request.AddParameter("some_param_name", "some_param_value", ParameterType.QueryString);

        //                    //IRestResponse<ResponseData> response = client.Execute<ResponseData>(request);

        //                    //var fullUrl = client.BuildUri(request);


        //                    var fileBytes = client.DownloadData(request);

        //                    if (fileBytes == null)
        //                    {
        //                        Globals.log.Warn("Can not load config file from server! The recorded one(if exists) will be used.");
        //                        return;
        //                    }

        //                    string configStr = Encoding.UTF8.GetString(fileBytes);
        //                    var containBln = configStr.ToLower().Contains("html")
        //                        || configStr.ToLower().Contains("error");
        //                    if (containBln)
        //                    {
        //                        Globals.log.Warn("Can not load config file from server! The recorded one(if exists) will be used.");
        //                        return;
        //                    }

        //                    File.WriteAllBytes(path4config, fileBytes);
        //                    Globals.log.Debug("Complete uploading file to " + path4config);


        //                    //HttpWebResponse webResp = (HttpWebResponse)httpWebRequest.GetResponse();

        //                    //if (webResp == null || webResp.StatusCode != HttpStatusCode.OK)
        //                    //{
        //                    //    //if error str_content = "", so move code to the end of this method
        //                    //}
        //                    //else
        //                    //{
        //                    //    using (Stream stream = webResp.GetResponseStream())
        //                    //    using (StreamReader reader = new StreamReader(stream))
        //                    //    {
        //                    //        str_content = reader.ReadToEnd();
        //                    //    }

        //                    //    // Append text to an existing file named "WriteLines.txt".

        //                    //    System.IO.File.WriteAllText(path4config, str_content);

        //                    //    Globals.log.Debug("Complete uploading file to " + path4config);
        //                    //}
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

        #endregion

        #region RabbitMQWrapper
        //private static void RegisterNotification()
        //{
        //    RegisterNotificationByCefbrowser();
        //}
        //        private static void RegisterNotificationByCefbrowser()
        //        {
        //            string path = "";

        //            try
        //            {
        //                var login_name = Globals.myComputerInfo.UserName;
        //#if DEBUG
        //                //login_name = "aohsadmin";
        //#endif

        //                path = Configuration.configuration["aohs.baseurl"] + @"/webapi/client_notify?do_act=register&login_name=" + login_name;


        //#if DEBUG
        //                //path = path + "xxx";
        //#endif

        //                Globals.log.Debug("Register Notification by Cefbrowser through: " + path);

        //                //FormNotificationPanel.LoadPageToRegisterRabbitMq(@"http://192.168.1.88:3002/webapi/client_notify?do_act=register&login_name=Sirisak");
        //                FormNotificationPanel.LoadPageToRegisterRabbitMq(path);

        //                //FormNotificationPanel.cefsharpBrowser2.

        //                //var jsonObject = AmivoiceWatcher.NotificationPanel.GetJsonFromCefsharp(path);

        //                //if (jsonObject.success == "true")
        //                //{
        //                //    Globals.log.Debug("Get rabbitMq para from server >>" + result);

        //                //    _rabbitMQ = jsonObject;
        //                //}
        //                //else
        //                //{
        //                //    Globals.log.Debug("Disable Notification function because UNABLE to set rabbitMq para from server:" + path);
        //                //    _rabbitMQ = null;
        //                //}


        //            }
        //            catch (Exception e)
        //            {
        //                Globals.log.Debug(e.ToString());
        //                Globals.log.Debug("Disable Notification function because UNABLE to set rabbitMq para from server:" + path);
        //                _rabbitMQ = null;
        //            }
        //        }

        #endregion

        #region in_FOrmNotificationPanel
        //public static void LoadPageToRegisterRabbitMq(string url)
        //{
        //    if (cefsharpBrowser2.IsLoading)
        //    {
        //        return;
        //    }

        //    cefsharpBrowser2.Load(url);
        //    while (cefsharpBrowser2.IsLoading)
        //    {
        //        Thread.Sleep(500);
        //    }

        //    var script = @"document.getElementsByTagName ('html')[0].innerHTML";
        //    cefsharpBrowser2.EvaluateScriptAsync(script).ContinueWith(x =>
        //    {
        //        var response = x.Result;

        //        var html = response.Result.ToString();

        //        Regex regexp = new Regex(@"\{.*\}");
        //        var match = regexp.Match(html);

        //        if (match.Success)
        //        {
        //            var jsonString = match.Value;

        //            RabbitMQWrapper.RegisterNotificationByJson(jsonString);
        //        }
        //    });
        //}

        #endregion

        #region setcolor
        //delegate void SetSameBackgroundCallback();
        //public void SetSameBackground()
        //{

        //    if (this.InvokeRequired)
        //    {
        //        SetSameBackgroundCallback d = new SetSameBackgroundCallback(SetSameBackground);
        //        this.Invoke(d, new object[] { });
        //    }
        //    else
        //    {
        //        try
        //        {
        //            this.Refresh();

        //            cefsharpBrowser.Refresh();
        //            cefsharpBrowser.Update();
        //            this.Refresh();
        //            this.Update();


        //            Thread.Sleep(2000);

        //            var x = this.Left + SharpBrowser.margin + 20;
        //            var y = this.Top + SharpBrowser.margin + 20;

        //            var color = Colors.GetColorAt(x, y);

        //            this.BackColor = Color.FromArgb(color.A, color.R, color.G, color.B);
        //        }
        //        catch (Exception e)
        //        {
        //            Globals.log.Debug(e.ToString());
        //        }


        //    }

        //}
        #endregion


        #region Deligate
        //delegate void XCallback();
        //public void X()
        //{

        //    if (this.InvokeRequired)
        //    {
        //        XCallback d = new XCallback(X);
        //        this.Invoke(d, new object[] { });
        //    }
        //    else
        //    {



        //    }

        //}

        #endregion

        #region FormLongNotification_Load when use webbrowser
        //        private void FormLongNotification_Load(object sender, EventArgs e)
        //        {
        //            // Allow webbrowser to run parent function
        //            //webBrowser1.AllowWebBrowserDrop = false;
        //            //webBrowser1.IsWebBrowserContextMenuEnabled = false;
        //            //webBrowser1.WebBrowserShortcutsEnabled = false;
        //            //webBrowser1.ObjectForScripting = this;

        //#if DEBUG
        //            //webBrowser1.ScriptErrorsSuppressed = false;
        //#endif

        //        }

        #endregion

        #region AmivoiceWatcher.cs webbrowser+popup
        //    public static void Popup(string jsonMsg, int duration, FormAnimator.AnimationMethod animationMethod, FormAnimator.AnimationDirection animationDirection)
        //    {

        //        // InvokeRequired required compares the thread ID of the
        //        // calling thread to the thread ID of the creating thread.
        //        // If these threads are different, it returns true.
        //        if (formDummy.InvokeRequired)
        //        {
        //            Notifications.PopupCallback d = new Notifications.PopupCallback(Popup);

        //            formDummy.Invoke(d, new object[] { jsonMsg, duration, animationMethod, animationDirection });
        //        }
        //        else
        //        {
        //            try
        //            {
        //                NotificationMessage dictMsg = new NotificationMessage(jsonMsg);

        //                if (dictMsg.level == "" || dictMsg.contentAsWhole == "true")
        //                {

        //                    if (duration == -1)
        //                    {
        //                        dictMsg.duration = duration;
        //                    }

        //                    var frmLong = (FormNotificationPanel)getFormByName("FormNotificationPanel");

        //                    if (!frmLong.Visible)
        //                    {
        //                        //NotificationPopup toastNotification = new NotificationPopup(title, body, duration2use, animationMethod, animationDirection, width, height);
        //                        //toastNotification.Show(formDummy);
        //                        NotificationPopup.Popup(dictMsg.title, dictMsg.body, dictMsg.duration, animationMethod, animationDirection);

        //                        //var htmlString = Globals.htmlStringTemplatePure;
        //                        //htmlString = Globals.functions.HtmlWithAbsolutePaths("./ReplaceWithAbsolutePath", htmlString);
        //                        //htmlString = htmlString.Replace("[[body]]", body);
        //                        //var webBrowser1 = toastNotification.Controls["webBrowser1"] as WebBrowser;
        //                        //webBrowser1.DocumentText = htmlString;
        //                    }

        //                    object[] param = new object[1];

        //                    var dictMsgFormatted = dictMsg.GetDictionaryFormatted();

        //                    param[0] = Globals.functions.Json_toJsonObj(dictMsgFormatted);

        //                    Notifications.updateLongNotification(param);

        //                }
        //                else
        //                {

        //                    var datetime = dictMsg.GetFormattedDateTime();

        //                    if (duration == -1)
        //                    {
        //                        dictMsg.duration = duration;
        //                    }

        //                    NotificationPopup.Popup(dictMsg.title, dictMsg.body, dictMsg.duration, animationMethod, animationDirection);

        //                    //NotificationPopup toastNotification = new NotificationPopup(title, body, duration2use, animationMethod, animationDirection);
        //                    //if (Globals.notification_dialog_isTransparent)
        //                    //{
        //                    //    toastNotification.Opacity = Globals.notification_dialog_opacity;
        //                    //}
        //                    //else
        //                    //{
        //                    //    toastNotification.Opacity = 1;
        //                    //}

        //                    //toastNotification.Show(formDummy);

        //                    //var htmlString = Globals.htmlStringTemplate;

        //                    //htmlString = Globals.functions.HtmlWithAbsolutePaths("./ReplaceWithAbsolutePath/", htmlString);

        //                    //htmlString = htmlString.Replace("[[messageJson]]", Globals.functions.Json_toJson(jsonMsg));

        //                    //htmlString = htmlString.Replace("[[title]]", title);
        //                    //htmlString = htmlString.Replace("[[body]]", body);
        //                    //htmlString = htmlString.Replace("[[datetime]]", datetime);

        //                    //var backgroundColor = "#e3f7fc";
        //                    //var borderColor = "#8ed9f6";

        //                    //switch (dictMsg["level"])
        //                    //{
        //                    //    case "notice":
        //                    //        backgroundColor = "#e3f7fc";
        //                    //        borderColor = "#8ed9f6";
        //                    //        //dictMsg["image"] = Globals.notification_image.notice;
        //                    //        break;
        //                    //    case "error":
        //                    //        backgroundColor = "#ffecec";
        //                    //        borderColor = "#f5aca6";
        //                    //        //dictMsg["image"] = Globals.notification_image.error;
        //                    //        break;
        //                    //    case "success":
        //                    //        backgroundColor = "#e9ffd9";
        //                    //        borderColor = "#a6ca8a";
        //                    //        //dictMsg["image"] = Globals.notification_image.success;
        //                    //        break;
        //                    //    case "warning":
        //                    //        backgroundColor = "#fff8c4";
        //                    //        borderColor = "#f2c779";
        //                    //        //dictMsg["image"] = Globals.notification_image.warning;
        //                    //        break;
        //                    //    case "question":
        //                    //        backgroundColor = "#fff8c4";
        //                    //        borderColor = "#f2c779";
        //                    //        //dictMsg["image"] = Globals.notification_image.question;
        //                    //        break;
        //                    //    case "custom":
        //                    //        backgroundColor = Globals.functions.Color2HexConverter(Globals.notification_dialog_backgroundColor);
        //                    //        //borderColor =  Globals.functions.Color2HexConverter(ControlPaint.Dark(Globals.notification_dialog_backgroundColor));
        //                    //        borderColor = Globals.functions.Color2HexConverter(Globals.functions.DarkerColor(Globals.notification_dialog_backgroundColor, 90f));
        //                    //        break;
        //                    //}

        //                    //htmlString = htmlString.Replace("[[background-color]]", backgroundColor);
        //                    //htmlString = htmlString.Replace("[[border-color]]", borderColor);

        //                    //string foregroundColor;

        //                    //if (dictMsg["level"] == "custom")
        //                    //{
        //                    //    foregroundColor = Globals.functions.Color2HexConverter(Globals.functions.CalculateForegroundColor(Globals.notification_dialog_backgroundColor));
        //                    //}
        //                    //else
        //                    //{
        //                    //    foregroundColor = "#000000";
        //                    //}

        //                    //htmlString = htmlString.Replace("[[foreground-color]]", foregroundColor);

        //                    //var strImage = "default";

        //                    //if (dictMsg.ContainsKey("image"))
        //                    //{
        //                    //    strImage = dictMsg["image"];
        //                    //}
        //                    //else if (dictMsg.ContainsKey("icon"))
        //                    //{
        //                    //    strImage = dictMsg["icon"];
        //                    //}

        //                    //if (Globals.notification_show_icon)
        //                    //{
        //                    //    if (strImage == "default")
        //                    //    {
        //                    //        switch (dictMsg["level"])
        //                    //        {
        //                    //            case "notice":
        //                    //                strImage = Globals.notification_image_file.notice;
        //                    //                break;
        //                    //            case "error":
        //                    //                strImage = Globals.notification_image_file.error;
        //                    //                break;
        //                    //            case "success":
        //                    //                strImage = Globals.notification_image_file.success;
        //                    //                break;
        //                    //            case "warning":
        //                    //                strImage = Globals.notification_image_file.warning;
        //                    //                break;
        //                    //            case "question":
        //                    //                strImage = Globals.notification_image_file.question;
        //                    //                break;
        //                    //        }
        //                    //        if (!String.IsNullOrEmpty(strImage))
        //                    //        {
        //                    //            htmlString = htmlString.Replace("[[image_src]]", strImage);
        //                    //        }
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        if (!strImage.StartsWith("data:"))
        //                    //        {
        //                    //            strImage = Path.Combine(Globals.PathLocalAppData, "Template", "image", strImage);
        //                    //        }
        //                    //        htmlString = htmlString.Replace("[[image_src]]", strImage);
        //                    //    }
        //                    //}

        //                    //if (dictMsg.ContainsKey("answers"))
        //                    //{
        //                    //    htmlString = htmlString.Replace("[[answers]]", dictMsg["answers"]);
        //                    //}
        //                    //else
        //                    //{
        //                    //    htmlString = htmlString.Replace("[[answers]]", "");
        //                    //}

        //                    //if (dictMsg.ContainsKey("link"))
        //                    //{
        //                    //    var splitTemp = dictMsg["link"].Split('>');

        //                    //    htmlString = htmlString.Replace("[[linkCaption]]", splitTemp[0]);
        //                    //    htmlString = htmlString.Replace("[[link]]", "javascript:window.external.openLinkInDefaultBrowser('" + splitTemp[1] + "')");
        //                    //}

        //                    //if (dictMsg.ContainsKey("links"))
        //                    //{
        //                    //    htmlString = htmlString.Replace("[[links]]", dictMsg["links"]);
        //                    //}
        //                    //var webBrowser1 = toastNotification.Controls["webBrowser1"] as WebBrowser;
        //                    //webBrowser1.DocumentText = htmlString;

        //                    object[] param = new object[1];
        //                    var dictMsgFormatted = dictMsg.GetDictionaryFormatted();

        //                    param[0] = Globals.functions.Json_toJsonObj(dictMsgFormatted);

        //                    Notifications.updateLongNotification(param);
        //                }

        //                Globals.notification_client_numberNow++;

        //            }
        //            catch (Exception e)
        //            {
        //                Globals.log.Debug(e.ToString());
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
