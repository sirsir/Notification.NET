using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AmivoiceWatcher
{
    class MyPath
    {
        //public static string PathLocalAppData = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), @"AmivoiceWatcher");
        public static string PathLocalAppData = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), @"AmivoiceWatcher");

        //public static string currentDirectroy = Directory.GetCurrentDirectory();

        public static string currentDirectroy = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public static string notification_local_setting_file = Path.Combine(PathLocalAppData, "notification_setting.json");

        public static string unsent_activities = Path.Combine(MyPath.PathLocalAppData, @"AmivoiceWatcher\unsent_activities.txt");


        //public static string htmlStringTemplate = File.ReadAllText(Path.Combine(MyPath.PathLocalAppData, "Template", "NotificationTemplate.html"));
        //public static string htmlStringTemplatePure = File.ReadAllText(Path.Combine(MyPath.PathLocalAppData, "Template", "NotificationTemplatePure.html"));
        //public static string htmlStringTemplateSuper = File.ReadAllText(Path.Combine(MyPath.PathLocalAppData, "Template", "SuperNotificationTemplate.html"));


        public static string[] CleanAllDirectoryPath = {
            //Globals.PathLocalAppData,
            //Path.Combine(Globals.PathLocalAppData, "Screenshots"),
            Path.Combine(MyPath.PathLocalAppData, "ScreenshotsRAW"),
            //Path.Combine(Globals.PathLocalAppData, "ScreenshotsZIP"),
            //Path.Combine(Globals.PathLocalAppData, "ScreenshotsZipped"),
        };

        public static string[] CreateAllDirectoryAndFilesPath = {
                MyPath.PathLocalAppData,
                Path.Combine(MyPath.PathLocalAppData, "Screenshots"),
                Path.Combine(MyPath.PathLocalAppData, "ScreenshotsRAW"),
                Path.Combine(MyPath.PathLocalAppData, "ScreenshotsZIP"),
                Path.Combine(MyPath.PathLocalAppData, "ScreenshotsZipped")
                //Path.Combine(Globals.PathLocalAppData, "Template")
            };

        public static string CreateAllDirectoryAndFilesPathCopyFrom = Path.Combine(System.Environment.CurrentDirectory, "LocalAppData");
        public static string CreateAllDirectoryAndFilesPathCopyTo = MyPath.PathLocalAppData;

        public static string logfile = Path.Combine(MyPath.PathLocalAppData, "AmivoiceWatcher.log");

        public static string configfile = Path.Combine(MyPath.PathLocalAppData, "configuration.txt");


    }
}
