using CefSharp;
using System;
using System.IO;
using System.Threading;

namespace AmivoiceWatcher
{
    public class DownloadHandler : IDownloadHandler
    {
        private string destination = "";
        public void OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    destination = Path.Combine(MyPath.PathLocalAppData, downloadItem.SuggestedFileName);
                    callback.Continue(destination, showDialog: false);

                    Globals.log.Debug("DownloadHandler:> CefsharpDummy start download file to " + destination);
                    Configuration.myState = Configuration.State.downloading;

                }
            }
        }

        public void OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            if (downloadItem.IsComplete)
            {
                Globals.log.Debug("DownloadHandler:> CefsharpDummy Successfully download file to " + destination);
                Configuration.myState = Configuration.State.downloadedSuccess;

                //AmivoiceWatcher.myFormCefsharpDummy.myState = FormCefsharpDummy.State.idle;

                AmivoiceWatcher.myFormCefsharpDummy.Load_ReloadPage();
            }
        }



    }
}