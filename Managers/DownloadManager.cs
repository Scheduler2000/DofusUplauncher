using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace Uplauncher.Managers
{
    public class DownloadManager
    {
        private readonly WebClient _client = new WebClient();

        public DownloadManager(DownloadProgressChangedEventHandler onDownloadProgress, AsyncCompletedEventHandler onDownloadCompleted)
        {
            this._client.DownloadProgressChanged += onDownloadProgress;
            this._client.DownloadFileCompleted += onDownloadCompleted;
        }

        public void DownloadClient()
        {
            var uri = new Uri(Constants.REMOTE_CLIENT_URI);
            var fileName = Path.GetFileName(uri.AbsoluteUri);

            _client.DownloadFileAsync(uri, fileName);
        }

        public void DownloadPatch()
        {
            var uri = new Uri(Constants.REMOTE_PATCH_URI);
            var fileName = Path.GetFileName(uri.AbsoluteUri);

            _client.DownloadFileAsync(uri, fileName);
        }

        public int GetVersion()
        {
            using Stream response = WebRequest.Create((Constants.REMOTE_VERSION))
                                              .GetResponse()
                                              .GetResponseStream();

            using var streamReader = new StreamReader(response);
            string currentVersion = streamReader.ReadToEnd();

            response.Close();
            streamReader.Close();

            return int.Parse(currentVersion);
        }


        public void Dispose()
        {  _client.Dispose();  }
    }
}
