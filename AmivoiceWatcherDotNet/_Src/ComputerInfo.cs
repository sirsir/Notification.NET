using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

using System.Net;
using System.Net.NetworkInformation;

using System.Deployment.Application;
using System.Reflection;

using Microsoft.Win32;
using System.Management;
using System.Collections.Specialized;

namespace AmivoiceWatcher
{
    class ComputerInfo
    {
        private static int TimerToResubmit=60;
        private static bool blnThreadAborted = false;

        private string userName;
        private string os;
        private string computerName;
        private string ip;
        private string macAddress;
        public static string watcherVersion;

        private string domainName;

        public string DomainName
        {
            get { return domainName; }
            set { domainName = value; }
        }



        public static string WatcherVersion
        {
            get {
                if (watcherVersion == null)
                {
                    watcherVersion = ApplicationDeployment.IsNetworkDeployed
                       ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                       : Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }

                return watcherVersion;

            }
        }

        public string MacAddress
        {
            get { return macAddress; }
        }

        private string GetOSVersion()
        {
            string strOS = "";
            string strOSVersion = Environment.OSVersion.ToString();

            switch (Environment.OSVersion.Platform)
            {
                case System.PlatformID.Win32S:
                    strOS = "3.1";
                    break;
                case System.PlatformID.Win32Windows:
                    switch (Environment.OSVersion.Version.Major)
                    {
                        case 0:
                            strOS = "95";
                            break;
                        case 10:
                            strOS = "98";
                            break;
                        case 90:
                            strOS = "Me";
                            break;
                        default:
                            strOS = "95/98/Me";
                            break;
                    }
                    break;
                case System.PlatformID.Win32NT:
                    switch (Environment.OSVersion.Version.Major)
                    {
                        case 4:
                            strOS = "NT";
                            break;
                        case 5:
                            switch (Environment.OSVersion.Version.Minor)
                            {
                                case 0:
                                    strOS = "2000";
                                    break;

                                case 1:
                                    strOS = "XP";
                                    break;

                                case 2:
                                    strOS = "Server2003";
                                    break;
                            }
                            break;
                        case 6:
                            switch (Environment.OSVersion.Version.Minor)
                            {
                                case 0:
                                    strOS = "Vista";
                                    break;
                                case 1:
                                    strOS = "Win7";
                                    break;
                                case 2:
                                    var productNameFromReg = GetProductNameFromRegistry();
                                    if (productNameFromReg.StartsWith("Windows 10"))
                                    {
                                        strOS = "Win10";
                                        
                                    }
                                    else if(productNameFromReg.StartsWith("Windows 8.1"))
                                    {
                                        strOS = "Win8.1";
                                    }
                                    else
                                    {
                                        strOS = "Win8";
                                    }
                                    break;

                                case 3:
                                    strOS = "Win8.1";
                                    break;
                            }
                            break;
                        case 10:
                            switch (Environment.OSVersion.Version.Minor)
                            {
                                case 0:
                                    strOS = "Win10";
                                    break;
                            }
                            break;
                        default:
                            strOS = "NT/2000/XP";
                            break;
                    }
                    if ( String.IsNullOrEmpty(Environment.OSVersion.ServicePack))
                    {
                        strOS += " ";
                        strOS += Environment.OSVersion.ServicePack;
                    }
                    
                    break;
                default:
                    strOS = "OS Unknown";
                    break;
            }
            return strOS;
        }

        // Get mac address
        // return empty string if no network adapter enable
        static private string GetMACAddress()
        {
            string   macAddress = (
                    from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()
                      ).FirstOrDefault();
            

            // with separator 
            //macAddress = Regex.Replace(macAddress, @"(.{2})", "$1-").TrimEnd('-');

            return macAddress;
        }

        //-- Get MACaddress with different method (can get mac_address when all network adapter is disable
        // not use now
        static private string GetMACAddress2()
        {
            string macAddress = String.Empty;
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher("Select * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMOS.Get();
            foreach (ManagementObject objMO in objMOC)
            {
                object tempMacAddrObj = objMO["MacAddress"];

                if (tempMacAddrObj == null) //Skip objects without a MACAddress
                {
                    continue;
                }
                if (macAddress == String.Empty) // only return MAC Address from first card that has a MAC Address
                {
                    macAddress = tempMacAddrObj.ToString();
                }
                objMO.Dispose();
            }
            macAddress = macAddress.Replace(":", "");

            if (String.IsNullOrEmpty(macAddress))
            {
                macAddress = (
                    from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()
                      ).FirstOrDefault();
            }

            // with separator 
            //macAddress = Regex.Replace(macAddress, @"(.{2})", "$1-").TrimEnd('-');

            return macAddress;
        }

        static string GetProductNameFromRegistry()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            string productName = (string)reg.GetValue("ProductName");

            return productName;
        }

        public static string IpForShow(string strIp)
        {
            if (string.IsNullOrEmpty(strIp))
            {
                return "";
            }

            //.NET4.5.2
            //IPAddress ip = IPAddress.Parse(strIp);
            //Console.WriteLine(ip.MapToIPv4());
            //return ip.MapToIPv4().ToString();

            //.NET4.0
            //IPAddress ip = IPAddress.(strIp);
            //Console.WriteLine(ip.ToString());
            return strIp;

        }

        public ComputerInfo()
        {
            //Set variable
            if (!Int32.TryParse(Configuration.configuration["computer_info.senddata.timer.min"], out ComputerInfo.TimerToResubmit))
            {
                Globals.log.Error("Cant load ComputerInfo.TimerToResubmit. The program will use default value=" + ComputerInfo.TimerToResubmit);
            }

            if (ComputerInfo.TimerToResubmit < 10)
            {
                //ComputerInfo.TimerToResubmit = 0;
                blnThreadAborted = true;
            }
            //for debug
            //ComputerInfo.TimerToResubmit = 1;

            userName = Environment.UserName;
            os = GetOSVersion();
            computerName = Environment.MachineName;

            // Get domainName
            domainName = Environment.UserDomainName;

            // Get IP address
            string host = Dns.GetHostName();
            IPHostEntry objIp = Dns.GetHostEntry(host);
            //ip = objIp.AddressList[2].ToString();
            ip = objIp.AddressList.Last().ToString();
            if (ip == "127.0.0.1")
            {
                ip = "";
            }

            // Get Mac address
            macAddress = GetMACAddress();

            // Get watcherVersion
            watcherVersion= ApplicationDeployment.IsNetworkDeployed
                       ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                       : Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }



        public string UserName
        {
            get { return userName; }
        }

        public string Ip
        {
            get { return ip; }
        }

        public string Os
        {
            get { return os; }
        }

        public string ComputerName
        {
            get { return computerName; }
        }

        public static void ThreadMain()
        {          
            while (!blnThreadAborted && !Globals.isProgramExit)
            {

                Thread.Sleep(ComputerInfo.TimerToResubmit*1000*60);
                SubmitComputerLog(SubmitComputerLogMode.Logged);

            }

            Globals.log.Debug("ComputerInfo's thread exited.");
        }

        public Dictionary<string,string> ToDictionary()
        {
            Dictionary<string, string> dictTemp = new Dictionary<string, string>();

            dictTemp.Add("os_version", os);
            dictTemp.Add("watcher_version", watcherVersion);
            dictTemp.Add("computer_name", computerName);
            dictTemp.Add("login_name", userName);
            dictTemp.Add("mac_address", macAddress);
            dictTemp.Add("domainname", domainName);
            dictTemp.Add("ip", ip);
            //dict.Add("java_name", os);
            //dict.Add("ie_version", os);
            return dictTemp;
        }

        public enum SubmitComputerLogMode { Startup, Logoff, Logged };

        public static string GetComputerInfoJsonString(SubmitComputerLogMode mode)
        {

            var dictIn = GetComputerInfoDict(mode);
            var jsonString = Globals.functions.Json_toJsonObj(dictIn);

            return jsonString;
        }

         

        public static Dictionary<string,string> GetComputerInfoDict(SubmitComputerLogMode mode)
        {
            var dictTemp = Globals.myComputerInfo.ToDictionary();

            switch (mode)
            {
                case SubmitComputerLogMode.Startup:
                    dictTemp.Add("event", "startup");
                    break;
                case SubmitComputerLogMode.Logoff:
                    dictTemp.Add("event", "logoff");
                    break;
                case SubmitComputerLogMode.Logged:
                    dictTemp.Add("event", "logged");
                    break;
                default:
                    break;
            }

            //bool sent_completed = false;

            int retry_count_max = 5;
            int.TryParse(Configuration.configuration["update_computer.retry_count_max"], out retry_count_max);

            //int time_out = 5000;

            //int wait_time = 10;
            //int.TryParse(Configuration.configuration["update_computer.retry_wait"], out wait_time);

            return dictTemp;
        }

        public static void SubmitComputerLog(SubmitComputerLogMode mode)
        {
            try
            {

                var pathTemp = Configuration.configuration["update_computer.url"];
                var dictTemp = Globals.myComputerInfo.ToDictionary();

                switch (mode)
                {
                    case SubmitComputerLogMode.Startup:
                        dictTemp.Add("event", "startup");
                        break;
                    case SubmitComputerLogMode.Logoff:
                        dictTemp.Add("event", "logoff");
                        break;
                    case SubmitComputerLogMode.Logged:
                        dictTemp.Add("event", "logged");
                        break;
                    default:
                        break;
                }

                bool sent_completed = false;

                int retry_count_max = 5;
                int.TryParse(Configuration.configuration["update_computer.retry_count_max"], out retry_count_max);

                int time_out = 5000;

                int wait_time = 10;
                int.TryParse(Configuration.configuration["update_computer.retry_wait"], out wait_time);

                //.NET4.5.2
                //Task taskA = Task.Run(() =>
                //.NET4.0
                Task taskA = Task.Factory.StartNew(() =>
                {

                    int retry_count = 0;
                    while (!sent_completed && retry_count < retry_count_max)
                    {
                        Globals.log.Debug("\nTrying to submit computer info :: retry_count = " + retry_count);
                        var returnCode = Globals.functions.HttpPostRequestDictionary(pathTemp, dictTemp, time_out);
                        if (returnCode == Globals.functions.HttpPostRequestReturnCode.COMPLETED)
                        {
                            sent_completed = true;

                            break;
                        }

                        if (mode == ComputerInfo.SubmitComputerLogMode.Logoff || Globals.isProgramExit)
                        {
                            break;
                        }

                        Thread.Sleep(wait_time * 1000);
                        retry_count += 1;
                    }

                    if (sent_completed)
                    {
                        Globals.log.Debug("\nSuccess to submitting computer info :: retry_count = " + retry_count);
                    }
                    else
                    {
                        Globals.log.Warn(String.Format("\nUnable and giveup to submit computer info :: retry_count = {0}, time_out = {1}, wait_time = {2}", retry_count, time_out, wait_time));
                    }
                });
            }
            catch(Exception e)
            {
                Globals.log.Error("SubmitComputerLog(.)");
                Globals.log.Debug(e.ToString());
            }
            

        }
    }
}
