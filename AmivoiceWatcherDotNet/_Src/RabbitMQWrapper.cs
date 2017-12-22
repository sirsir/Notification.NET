using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace AmivoiceWatcher
{
    class RabbitMQWrapper
    {
        //Popup parameter
        private static int _duration = Globals.notification_duration;
        private static FormAnimator.AnimationMethod _animationMethod = FormAnimator.AnimationMethod.Fade;
        private static FormAnimator.AnimationDirection _animationDirection = FormAnimator.AnimationDirection.Up;

        //RabbitMQ parameter
        private static ConnectionFactory factory;
        private static ClientNotifyResponse _rabbitMQ;
        private static IConnection connection;
        private static IModel channel;

        // Thread
        public static State myState = State.waiting_for_configuration;
        public enum State
        {
            waiting_for_configuration,
            registering,
            registered,
            unconnected,
            connecting,
            connected,
            register_fail
        }

        //private static bool IsRegisterNotificationSuccess = false;


        public class ClientNotifyResponse
        {
            public string success { get; set; }
            public string login_name { get; set; }
            public string queue_name { get; set; }
            public string timestamp { get; set; }
            public RabbitMQ rabbitmq { get; set; }

        }

        public class RabbitMQ
        {
            public string host { get; set; }
            public string port { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string vhost { get; set; }
            public double connection_timeout { get; set; }
            public double wait_connection_timeout { get; set; }

        }




        public static void RegisterNotificationByJson(string jsonstring)
        {

            try
            {
                Globals.log.Debug("RabbitMQWrapper:> RegisterNotificationByJson()");

                myState = State.registering;

                var jsonObject = JsonConvert.DeserializeObject<ClientNotifyResponse>(jsonstring);

                if (jsonObject == null)
                {
                    Globals.log.Debug("Disable Notification function because UNABLE to set rabbitMq para from server");
                    _rabbitMQ = null;
                    //currentState = State.waiting_for_configuration;
                    //currentState = State.connected;
                    myState = State.register_fail;

                    return;
                }

                if (jsonObject.success == "true")
                {
                    Globals.log.Debug("Get rabbitMq para from server >>" + jsonstring);

                    _rabbitMQ = jsonObject;

                    //ConnectionInitialize();

                    myState = State.registered;

                }
                else
                {
                    Globals.log.Debug("Disable Notification function because UNABLE to set rabbitMq para from server");
                    _rabbitMQ = null;
                    //currentState = State.waiting_for_configuration;
                    //currentState = State.connected;
                    myState = State.register_fail;
                }

            }
            catch (Exception e)
            {
                Globals.log.Debug(e.ToString());
                Globals.log.Debug("Disable Notification function because UNABLE to set rabbitMq para from server");
                _rabbitMQ = null;
                myState = State.register_fail;
            }

            Globals.log.Debug("RabbitMQWrapper:> Finished RegisterNotificationByJson()");
        }


        //        private static void RegisterNotificationByWebrequest()
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

        //                Globals.log.Debug("Register Notification through: " + path);

        //                var httpWebRequest = (HttpWebRequest)WebRequest.Create(path);

        //                httpWebRequest.ContentType = "application/json; charset=utf-8";
        //                //httpWebRequest.Accept = "text/*, text/html, text/html;level=1, */*, application/xhtml+xml, */*";
        //                httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        //                httpWebRequest.UserAgent = @"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.89 Safari/537.36";
        //                httpWebRequest.Method = "GET";
        //                //httpWebRequest.Connection = "keep-alive";
        //                httpWebRequest.Host = "172.22.101.40";
        //                httpWebRequest.KeepAlive = true;

        //                // request.UseDefaultCredentials = true;
        //                // request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

        //                //Get the headers associated with the request.
        //                WebHeaderCollection myWebHeaderCollection = httpWebRequest.Headers;
        //                myWebHeaderCollection.Add("Accept-Language", "en-US,en;q=0.9,th;q=0.8");
        //                myWebHeaderCollection.Add("Accept-Encoding", "gzip, deflate");
        //                myWebHeaderCollection.Add("Cache-Control", "no-cache");
        //                myWebHeaderCollection.Add("Upgrade-Insecure-Requests", "1");
        //                myWebHeaderCollection.Add("Pragma", "no-cache");

        //                IWebProxy proxy = httpWebRequest.Proxy;
        //                if (proxy != null)
        //                {
        //                    string proxyuri = proxy.GetProxy(httpWebRequest.RequestUri).ToString();
        //                    //httpWebRequest.UseDefaultCredentials = true;
        //                    //httpWebRequest.Proxy = new WebProxy(proxyuri, false);
        //                    httpWebRequest.UseDefaultCredentials = false;
        //                    httpWebRequest.Proxy = new WebProxy(proxyuri, true);
        //                    //httpWebRequest.Proxy = new WebProxy("172.22.101.40", false); 
        //                    //httpWebRequest.Proxy = new WebProxy("192.168.1.149", false);
        //                    httpWebRequest.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
        //                    //httpWebRequest.UserAgent = "[any words that is more than 5 characters]";
        //                    //httpWebRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
        //                    httpWebRequest.UserAgent = @"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.89 Safari/537.36";
        //                }

        //                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

        //                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //                {
        //                    var result = streamReader.ReadToEnd();

        //                    var jsonObject = JsonConvert.DeserializeObject<ClientNotifyResponse>(result);

        //                    if (jsonObject.success == "true")
        //                    {
        //                        Globals.log.Debug("Get rabbitMq para from server >>" + result);

        //                        _rabbitMQ = jsonObject;
        //                    }
        //                    else
        //                    {
        //                        Globals.log.Debug("Disable Notification function because UNABLE to set rabbitMq para from server:" + path);
        //                        _rabbitMQ = null;
        //                    }

        //                }


        //            }
        //            catch (Exception e)
        //            {
        //                Globals.log.Debug(e.ToString());
        //                Globals.log.Debug("Disable Notification function because UNABLE to set rabbitMq para from server:" + path);
        //                _rabbitMQ = null;
        //            }
        //        }

        private static void ConnectionInitialize()
        {
            try
            {
                if (factory == null)
                {
                    factory = new ConnectionFactory()
                    {
                        HostName = _rabbitMQ.rabbitmq.host,
                        UserName = _rabbitMQ.rabbitmq.username,
                        Password = _rabbitMQ.rabbitmq.password,
                        VirtualHost = _rabbitMQ.rabbitmq.vhost,

                        RequestedConnectionTimeout = (int)(1000 * _rabbitMQ.rabbitmq.connection_timeout),
                    };
                }


                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: "direct_logs",
                                            type: "direct");

                channel.QueueDeclare(queue: _rabbitMQ.queue_name,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                                         );

                string[] routingKeys = new string[] { "all", Globals.myComputerInfo.UserName };

                foreach (var severity in routingKeys)
                {
                    channel.QueueBind(
                                      exchange: "direct_logs",
                                      queue: _rabbitMQ.queue_name,
                                      routingKey: severity);
                }
            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
            }


        }

        private static void ConnectToRabbitMqServer()
        {
            Globals.log.Debug("RabbitMQWrapper:> Try to connect to RabbitMq Server.");
            try
            {
                ConnectionInitialize();

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, e) =>
                {
                    if (e != null)
                    {

                        //Globals.log.Debug("Get message from queue:> ");

                        //string data = "{\"target_queue\":\"amiwatcher.sirisak\",\"timeout\":10,\"height\":100,\"width\":350,\"play_sound\":true,\"level\":\"warning\",\"title\":\"คำพูดไม่เหมาะสม\",\"subject\":\"พบคำว่า เก็บเงินเต็มจำนวน\",\"content\":\"\\u003cdiv style=\\\"width: 350px; height: 100px; background-color: #FAFAFA; border-color: #ff8c00 #cccccc #cccccc; border-style: solid; border-width: 2px 1px 1px;\\\"\\u003e  \\u003cdiv style=\\\"height: 100px; font-family: sans-serif; font-size: 10pt; vertical-align: top; width: 15%; border-right-width: 1px; border-right-color: #F5F5F5; border-right-style: solid; display: inline-block; background: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAMAAABEpIrGAAAAA3NCSVQICAjb4U/gAAAACXBIWXMAAADdAAAA3QFwU6IHAAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAATtQTFRF/////4CA/4BA/4Ag/45V/5Jb8YAr/45j/4xZ/4td9oQv/41h8IM2/5Fg+IYv84Yx/5Jb/5Be/49g/49c8YYu/5Fe8IUw/5Bd8ogr84Yu84cs/5Be9IUr/5Bd8Ycr/5Fe/5Bd8oYq/5Fe/49e8YUn/5Bf/5Be/5Bf8YQm/5Bf/5Be8IQl8YUk/5Be74Qj/5Be8IMg/5Bf7oIe7oMe/5Be7YId7oIb/5Be/5Be7YIa7YMY/5Be/5Be/5Be7IEV/5Be7IET/5Be64EQ/5Be64AN6oAN/5Be6oAL/5Be6YAJ6YAK/5Be6H4E6H4F6H4G6H4H6ogO7IsR7I0U7pMb75cf8Jsj8aAo+LY7+8JB/5Be/5Vh/5Zj/5hk/5tm/51o/6Bq/6Ns/7R5/8GC/8ct/8cv/8gz/9+Z/+Ob/+Sc9WklOgAAAEx0Uk5TAAIECAkOEhIUFhsdISUmKiouMDI3PEVHTVRXV1pjam9zen+LkJeanKCnra6ys7/Dys3V1tbZ4eHi5ejp7e/x8fX2+Pn7/Pz9/f7+/rM8r6EAAAERSURBVDjLldLFVsRAFIThy+AyOIO7uzsMLgMV3AqHIHn/J2DRJ+RGOdT2/073olvknyuuSMx5A8fWbCoBdJHAWHwv3SCBXFUsGCQJYD6u1x0aYLXFgEkagGx0b6UL0BfVC5Y8sFcUAXroAUyEe/kWSfL1BgBwWh8CwyTJx++PCwDAQrA3mAteHPvaXNIReITpINj0P0k7gwBDuheuhsFBmQK9DANMeT29EwVOMr9g3O18cuxLF2DF7Y309nYLb92mp+YUoOrYzhcRkU7d3++0GBURKVlX/dmxrxTIVYpIvz7g/uvzTB8xI1KzrwEfznWH1Swj9A/+LctaMjgyPzUeZKV6MQnsNolIulavRS8jf+8HYQyvutKZj6YAAAAASUVORK5CYII=') no-repeat center 20% / 75%; padding: 0.8em 0em 0em 0.8em;\\\"\\u003e\\u003c/div\\u003e  \\u003cdiv style=\\\"height: 100px; font-family: sans-serif; font-size: 10pt; vertical-align: top; width: 70%; display: inline-block; padding: 0.8em 0.8em 0em;\\\"\\u003e    \\u003cdiv style=\\\"font-family: sans-serif; font-size: 1.1em; vertical-align: top; width: 100%; font-weight: bold; color: #FF8C00;\\\"\\u003e\\u0026#3588;\\u0026#3635;\\u0026#3614;\\u0026#3641;\\u0026#3604;\\u0026#3652;\\u0026#3617;\\u0026#3656;\\u0026#3648;\\u0026#3627;\\u0026#3617;\\u0026#3634;\\u0026#3632;\\u0026#3626;\\u0026#3617;\\u003c/div\\u003e    \\u003cdiv style=\\\"font-family: sans-serif; font-size: 10pt; vertical-align: top; width: 100%; height: 50%; padding-top: 0.25em;\\\"\\u003e\\u0026#3614;\\u0026#3610;\\u0026#3588;\\u0026#3635;\\u0026#3623;\\u0026#3656;\\u0026#3634; \\u0026#3648;\\u0026#3585;\\u0026#3655;\\u0026#3610;\\u0026#3648;\\u0026#3591;\\u0026#3636;\\u0026#3609;\\u0026#3648;\\u0026#3605;\\u0026#3655;\\u0026#3617;\\u0026#3592;\\u0026#3635;\\u0026#3609;\\u0026#3623;\\u0026#3609;\\u003c/div\\u003e    \\u003cdiv style=\\\"font-family: sans-serif; font-size: 0.9em; vertical-align: top; font-style: italic;\\\" align=\\\"right\\\"\\u003e\\u003cspan style=\\\"font-family: sans-serif; font-size: 0.8em; padding-left: 1em; background: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAQAAAC1+jfqAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QAAKqNIzIAAAAJcEhZcwAADdcAAA3XAUIom3gAAAAHdElNRQfhBxMIAhTHv6GhAAAA60lEQVQoz3XRMUtCURwF8N97jU1pkzSFSOBm5dDUEM1tUUNTk5j4CcLvEtFmDVJBGuEokvQBTCNqCve2Bt97+kLPdO45/8u9538CMTYdK9tFT8+Nj6kcgsCFJ1uaysruFD2rCOK7gZZXmYhP5XVvWvFIPbGpqkYsa6AOBaPEpqGR8IyRQuhU18QiTHSdhLa1LUPbTqikkxKDOd5RCvE7J704V5yfX7HvyzA5j41de/QDjqyF+vZSTzRV3Ub8QJ+8bxv/PrcaxXyXh5qHBQmyBmqzVd/LpezUqqdlfbp0KCPnzJVhXNYs9ZK6/wDRrTchqG3bTwAAACV0RVh0ZGF0ZTpjcmVhdGUAMjAxNy0wNy0xOVQwODowMjoyMCswMjowMLmZ4/8AAAAldEVYdGRhdGU6bW9kaWZ5ADIwMTctMDctMTlUMDg6MDI6MjArMDI6MDDIxFtDAAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAABJRU5ErkJggg==') no-repeat left center / 0.8em 0.8em;\\\"\\u003e10:37:21\\u003c/span\\u003e\\u003c/div\\u003e  \\u003c/div\\u003e\\u003c/div\\u003e\",\"content_details\":\"\\u003cul\\u003e\\u003cli data-item-index=\\\"1\\\" data-item-type=\\\"btn\\\"\\u003ea\\u003c/li\\u003e\\u003cli data-item-index=\\\"2\\\" data-item-type=\\\"btn\\\"\\u003eb\\u003c/li\\u003e\\u003cli data-item-index=\\\"3\\\" data-item-type=\\\"btn\\\"\\u003ec\\u003c/li\\u003e\\u003c/ul\\u003e\\n\",\"timestamp\":\"2017-09-04 10:37:21\",\"messgae_uuid\":\"FRXv0_fFvDTHeg\"}";

                        string data = Encoding.UTF8.GetString(e.Body);

                        Globals.log.Debug("Get message from queue:> " + data);

                        AmivoiceWatcher.NotificationPanel.Popup(data, _duration, _animationMethod, _animationDirection);

                    }
                };

                //while ( (!channel.IsOpen) || channel.IsClosed || (!connection.IsOpen) )
                //{
                //    Thread.Sleep(200);
                //}

                channel.BasicConsume(queue: _rabbitMQ.queue_name,
                                     noAck: true,
                                     consumer: consumer);


                myState = State.connected;

                Globals.log.Debug("Successfully connected to RabbitMq Server. Ready to consume message.");

            }
            catch (Exception e)
            {
                Globals.log.Error(e.ToString());
            }
        }

        private static void RegisterRabbitMqByCefsharpDummy()
        {


            string path = "";

            try
            {
                var login_name = Globals.myComputerInfo.UserName;
#if DEBUG
                //login_name = "aohsadmin";
#endif

                path = Configuration.configuration["aohs.baseurl"] + @"/webapi/client_notify?do_act=register&login_name=" + login_name;

                var pathPure = Configuration.configuration["aohs.baseurl"] + @"/webapi/client_notify";
                var data = @"{'do_act':'register','login_name':'" + login_name + "'}";

#if DEBUG
                //path = path + "xxx";
#endif

                Globals.log.Debug("Register Notification by CefbrowserDummy through: " + path);

                //FormNotificationPanel.LoadPageToRegisterRabbitMq(@"http://192.168.1.88:3002/webapi/client_notify?do_act=register&login_name=Sirisak");
                //AmivoiceWatcher.myFormCefsharpDummy.RegisterRabbitMq(pathPure, data);
                //AmivoiceWatcher.myFormCefsharpDummy.RegisterRabbitMq(path);


            }
            catch (Exception e)
            {
                Globals.log.Debug(e.ToString());
                Globals.log.Debug("Disable Notification function because UNABLE to set rabbitMq para from server:" + path);
                _rabbitMQ = null;
            }

        }

        public static void ThreadMain()
        {

            //Thread thread1 = new Thread(new ThreadStart(RegisterRabbitMqByCefsharpDummy));
            //thread1.Start();

            //Setup();
            //if(!IsRegisterNotificationSuccess){
            //    Globals.log.Debug("Exit RabbitMQWrapper.ThreadMain().");
            //    return;

            //}

            int wait_connection_timeout = 5000;

            try
            {
                wait_connection_timeout = (int)(_rabbitMQ.rabbitmq.wait_connection_timeout * 1000);
            }
            catch
            {
                //Globals.log.Debug("Start RabbitMQWrapper.ThreadMain()");
                Globals.log.Debug("RabbitMQWrapper: Using default wait_connection_timeout = 5000");
            }

            int counter = 0;
            int counter_no_first_connect = 0;
            int counter_registering = 0;

            int tick_counter_for_connected_log = 10;

            bool hasSentConnectionSuccess = false;

            while (!Globals.isProgramExit)
            {
                try
                {
                    if (myState == State.waiting_for_configuration)
                    {
                        if (Configuration.myState == Configuration.State.completed)
                        {
                            //Setup();
                            myState = State.registering;
                            //RegisterRabbitMqByCefsharpDummy();
                        }

                    }
                    else if (myState == State.registering)
                    {
                        //currentState = State.registering_;

                        //Globals.log.Debug("Exit RabbitMQWrapper.ThreadMain().");
                        //return;
                        //AmivoiceWatcher.NotificationPanel.showConnectionError2LongNotification();


                        counter_registering++;

                        Globals.log.Debug("RabbitMQWrapper wait for registering result...");
                        Thread.Sleep(1000 + 2000 * counter_registering);


                        //Thread.Sleep(wait_connection_timeout);

                        //Setup();
                    }
                    else if (myState == State.registered)
                    {

                        hasSentConnectionSuccess = false;
                        AmivoiceWatcher.NotificationPanel.showConnectionError2LongNotification();

                        myState = State.connecting;
                        ConnectToRabbitMqServer();

                    }
                    else if (myState == State.unconnected)
                    {
                        tick_counter_for_connected_log = 10;

                        hasSentConnectionSuccess = false;
                        AmivoiceWatcher.NotificationPanel.showConnectionError2LongNotification();

                        myState = State.connecting;
                        ConnectToRabbitMqServer();
                        Thread.Sleep(wait_connection_timeout);

                    }
                    else if (myState == State.connecting)
                    {

                        if (!connection.IsOpen)
                        {
                            Globals.log.Debug("Fail to connnect to RabbitMq Server.");
                            AmivoiceWatcher.NotificationPanel.showConnectionError2LongNotification();
                            myState = State.unconnected;
                        }
                        else
                        {
                            myState = State.connected;
                        }



                    }
                    else if (myState == State.connected)
                    {
                        if (!connection.IsOpen)
                        {
                            Globals.log.Debug("Fail to connnect to RabbitMq Server.");
                            AmivoiceWatcher.NotificationPanel.showConnectionError2LongNotification();
                            myState = State.unconnected;
                        }
                        else
                        {

                             if (!hasSentConnectionSuccess || counter % 5 == 0)
                            {
                                hasSentConnectionSuccess = true;
                                AmivoiceWatcher.NotificationPanel.showConnectionSuccess2LongNotification();
                            }


                            counter++;
                            if (counter % tick_counter_for_connected_log == 0)
                            {
                                tick_counter_for_connected_log = (int)(tick_counter_for_connected_log*1.2);
                                counter = 0;
                                Globals.log.Debug("Connection to RabbitMq Server is OK.");
                            }
                            Thread.Sleep(wait_connection_timeout);
                        }
                    }
                    else if (myState == State.register_fail)
                    {
                        //if (!hasSentConnectionSuccess)
                        //{
                        //    hasSentConnectionSuccess = true;
                        //    AmivoiceWatcher.NotificationPanel.showConnectionSuccess2LongNotification();
                        //}

                        counter_no_first_connect++;
                        if (counter_no_first_connect % 10 == 0)
                        {
                            counter_no_first_connect = 0;
                            AmivoiceWatcher.NotificationPanel.showConnectionSuccess2LongNotification();
                            Thread.Sleep(10000);
                        }
                    }

                    Thread.Sleep(500);



                    //if (!IsRegisterNotificationSuccess)
                    //{
                    //    //Globals.log.Debug("Exit RabbitMQWrapper.ThreadMain().");
                    //    //return;
                    //    AmivoiceWatcher.NotificationPanel.showConnectionError2LongNotification();

                    //    Thread.Sleep(wait_connection_timeout);

                    //    //Setup();

                    //}
                    //else
                    //{
                    //    if (currentState == State.unconnected)
                    //    {
                    //        Thread.Sleep(wait_connection_timeout);

                    //        hasSentConnectionSuccess = false;
                    //        AmivoiceWatcher.NotificationPanel.showConnectionError2LongNotification();

                    //        ConnectToRabbitMqServer();

                    //    }
                    //    else
                    //    {
                    //        if (!connection.IsOpen)
                    //        {
                    //            Globals.log.Debug("Fail to connnect to RabbitMq Server.");
                    //            AmivoiceWatcher.NotificationPanel.showConnectionError2LongNotification();
                    //            currentState = State.unconnected;
                    //        }
                    //        else
                    //        {

                    //            if (!hasSentConnectionSuccess)
                    //            {
                    //                hasSentConnectionSuccess = true;
                    //                AmivoiceWatcher.NotificationPanel.showConnectionSuccess2LongNotification();
                    //            }


                    //            counter++;
                    //            if (counter % 10 == 0)
                    //            {
                    //                counter = 0;
                    //                Globals.log.Debug("Connection to RabbitMq Server is OK.");
                    //            }
                    //            Thread.Sleep(wait_connection_timeout);
                    //        }
                    //    }
                    //}


                }
                catch (Exception e)
                {
                    Globals.log.Debug(e.ToString());
                }

            }

            Globals.log.Debug("Exit RabbitMQWrapper.ThreadMain() Successfully");


        }
    }
}
