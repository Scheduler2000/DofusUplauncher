using Ionic.Zip;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;
using Uplauncher.Enums;
using Uplauncher.Managers;

namespace Uplauncher.ViewModel
{

    public class MainWindowViewModel
    {
        private UplauncherState _uplauncherState;
        private readonly ProgressBar _progressBar;
        private readonly TextBlock _extractInfo;

        public DownloadManager Downloader { get; }


        public UplauncherState UplauncherState
        {
            get => this._uplauncherState;
            set
            {
                this._uplauncherState = value;
                if (value != UplauncherState.Update)
                    ImageManager.UpdatePlayButton(false);
                else ImageManager.UpdatePlayButton(true);

                ImageManager.UpdateUplauncherState(_uplauncherState);
            }
        }

        public ImageManager ImageManager { get; }


        public MainWindowViewModel(TextBlock extractInfo, Image pgBackground, Image play, ProgressBar bar, Image state, Image background, Image[] slides)
        {
            this._extractInfo = extractInfo;
            this._progressBar = bar;
            this.ImageManager = new ImageManager(pgBackground, play, state, background, slides);
            this.Downloader = new DownloadManager(OnDataDownloaded, OnDataCompleted);
        }

        public void CheckFiles()
        {

            var folder = Directory.GetCurrentDirectory();


            if (Directory.Exists($"{folder}/{Constants.LOCAL_CLIENT_URI}"))
            {
                int filesCount = Directory.EnumerateFiles($"{folder}/{Constants.LOCAL_CLIENT_URI}", "*.*", SearchOption.AllDirectories)
                                          .Count();

                var versionFile = $"{folder}/{Constants.LOCAL_CLIENT_URI}/version.txt";

                if (filesCount != Constants.FILES_CHECKSUM || !File.Exists(versionFile))
                    FixGame(true);
                else
                {
                    int remoteVersion = Downloader.GetVersion();
                    int localVersion = int.Parse(File.ReadAllText(versionFile));

                    if (localVersion < remoteVersion)
                        UpdateGame();
                    else
                        UplauncherState = UplauncherState.Update;
                }
            }
            else
                FixGame(false);
        }

        public void FixGame(bool clientExist)
        {
            if (clientExist)
            {
                var folder = Directory.GetCurrentDirectory();
                Directory.Delete($"{folder}/{Constants.LOCAL_CLIENT_URI}", true);
            }

            UplauncherState = UplauncherState.Dirty;
            ImageManager.UpdateProgressBar(true);
            Downloader.DownloadClient();
        }

        private void UpdateGame()
        {
            UplauncherState = UplauncherState.Updating;
            ImageManager.UpdateProgressBar(true);
            Downloader.DownloadPatch();
        }


        private void OnDataDownloaded(object sender, DownloadProgressChangedEventArgs e)
        {
            _progressBar.Maximum = (double)e.TotalBytesToReceive / 100;
            _progressBar.Value = (double)e.BytesReceived / 100;
        }

        private void OnDataCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ExtractFileToDirectory();
            UplauncherState = UplauncherState.Update;
            _progressBar.Value = 0;
            ImageManager.UpdateProgressBar(false);
        }

        public void ExtractFileToDirectory()
        {
            Task.Run(() =>
            {
                var folder = Directory.GetCurrentDirectory();
                var zipFileName = $"{folder}/client.zip";
                var outputDirectory = $"{folder}/{Constants.LOCAL_CLIENT_URI}";

                using ZipFile zip = ZipFile.Read(zipFileName);

                if (!Directory.Exists(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);

                int counter = 1;
                foreach (ZipEntry entry in zip)
                {
                    entry.Extract(outputDirectory, ExtractExistingFileAction.OverwriteSilently);
                    _progressBar.Dispatcher.Invoke(() =>
                    {
                        _progressBar.Maximum = zip.Count;
                        _progressBar.Value = counter;
                        _extractInfo.Text = $"{_progressBar.Value} / {_progressBar.Maximum}";
                    });
                    counter++;
                }

                zip.Dispose();
                File.Delete(zipFileName);
            });
        }
    }
}
