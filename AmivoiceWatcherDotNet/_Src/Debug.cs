using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmivoiceWatcher
{
    class Debug
    {

#if DEBUG

        //public static string ip = @"http://192.168.43.31:3000";
        public static string ip = @"http://192.168.1.149:3000";


        //public static string url_watchercli_invalid = ip + @"/watchercli/sirisak/notificationXXXXXX";
        public static string url_watchercli_invalid = ip + @"/watchercli/sirisak/notification";

        public static string url_watchercli_valid = ip + @"/watchercli/sirisak/notification";
        //url_watchercli_valid = Configuration.configuration["aohs.baseurl"]+@"/watchercli/sirisak/notification";

        public static string url_download_config = ip + @"/configurations.txt?user=AohsAdmin";

        //public static string url_download_config = @"http://dl.dropboxusercontent.com/content_link/6OsHSX7ieTAnQXHt9f5hDFt1UGiiKK0Y92kmGlJDU38S8v5UnEcRqMJB1hKYbb5q/file?dl=1";

        //public static string url_download_config = @"Finish getting ConfigurationURL and start dowloading";
#endif

    }
}
