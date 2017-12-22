using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmivoiceWatcher
{
    class NotificationMessage
    {
        public string title = "";
        public string content = "";
        public string timestamp = "";
        public string level = "";
        public string contentAsWhole = "";
        public string body = "";
        public string content_details = "";
        public string subject = "";
        public string content_type = "";
        

        public int duration = -1;
        public int width = 0;
        public int height = 0;


        public NotificationMessage(string jsonString)
        {
            var dictMsg = GetDictionaryFromJsonString(jsonString);

            Globals.functions.TrySetValueFromDictionary(ref title, dictMsg, "title");
            Globals.functions.TrySetValueFromDictionary(ref content, dictMsg, "content");
            Globals.functions.TrySetValueFromDictionary(ref timestamp, dictMsg, "timestamp");
            Globals.functions.TrySetValueFromDictionary(ref level, dictMsg, "level");
            Globals.functions.TrySetValueFromDictionary(ref contentAsWhole, dictMsg, "contentAsWhole");
            Globals.functions.TrySetValueFromDictionary(ref body, dictMsg, "body");
            Globals.functions.TrySetValueFromDictionary(ref content_details, dictMsg, "content_details");
            Globals.functions.TrySetValueFromDictionary(ref subject, dictMsg, "subject");

            Globals.functions.TrySetValueFromDictionary(ref content_type, dictMsg, "content_type");

            if (dictMsg.ContainsKey("timeout"))
            {
                Int32.TryParse(dictMsg["timeout"], out duration);
            }

            if (dictMsg.ContainsKey("width"))
            {
                Int32.TryParse(dictMsg["width"], out width);
            }

            if (dictMsg.ContainsKey("height"))
            {
                Int32.TryParse(dictMsg["height"], out height);
            }

            PutContentDetailsInDiv();

        }

        private void PutContentDetailsInDiv()
        {
            content_details = String.Format("<div data-content-type='{0}'>{1}</div>", content_type, content_details);
        }

        public string GetFormattedDateTime()
        {
            var datetime = "";
            if (timestamp != "")
            {
                datetime = timestamp;
                var datesplit = datetime.Split(' ');
                if (datesplit[0] == DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    var datetimeArray = datesplit[1].Split(':');
                    datetime = datetimeArray[0] + ":" + datetimeArray[1];
                }
            }
            else
            {
                datetime = "";
            }

            return datetime;
        }

        public static Dictionary<string, string> GetDictionaryFromJsonString(string jsonString)
        {
            var dictMsg = Globals.functions.Json_toDictionary(jsonString);

            if (!dictMsg.ContainsKey("title"))
            {
                dictMsg["title"] = "";
            }

            if (!dictMsg.ContainsKey("content"))
            {
                dictMsg["content"] = "";
            }

            if (!dictMsg.ContainsKey("timestamp"))
            {
                dictMsg["timestamp"] = "";
            }

            if (!dictMsg.ContainsKey("level"))
            {
                dictMsg["level"] = "";
            }

            if (!dictMsg.ContainsKey("contentAsWhole"))
            {
                dictMsg["contentAsWhole"] = "true";
            }

            dictMsg["body"] = dictMsg["content"];

            return dictMsg;
        }


        public Dictionary<string, string> GetDictionaryFormatted()
        {
            var dictMsg = new Dictionary<string, string>();

            dictMsg.Add("title", title);
            dictMsg.Add("timestamp", timestamp);
            dictMsg.Add("content_details", content_details);
            dictMsg.Add("subject", subject);
            dictMsg.Add("level", level);

            return dictMsg;
        }
    }
}