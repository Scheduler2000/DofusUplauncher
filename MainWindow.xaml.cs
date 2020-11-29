using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Uplauncher.Enums;
using Uplauncher.Managers;
using Uplauncher.ViewModel;

namespace Uplauncher
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;
        private readonly ProcessManager _processManager = new ProcessManager();

        public MainWindow()
        {
            InitializeComponent();

            this._viewModel = new MainWindowViewModel(extractInfo, progressbackground, play, progressBar, state, background,
                new Image[] { pagination1, pagination2, pagination3, pagination4, pagination5 });

            base.DataContext = _viewModel;

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            try { this.DragMove(); }
            catch { };

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SoundManager.Initialize();
            }
            catch(SocketException ex)
            {
                if (ex.ErrorCode == 10048)
                {
                    MessageBox.Show("Une instance de l'uplauncher est déja ouverte.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    base.Close();
                }
                else 
                    throw ex;
            }

            _viewModel.CheckFiles();
        }

        private void OnOptionsEnter(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Souhaitez-vous réparer le jeu ?", "Réparer le jeu",
                                 MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)

                _viewModel.FixGame(false);
        }

        private void OnReduceEnter(object sender, MouseButtonEventArgs e)
        { base.WindowState = WindowState.Minimized; }

        private void OnCloseEnter(object sender, MouseButtonEventArgs e)
        { Application.Current.Shutdown(); }

        private void OnSiteEnter(object sender, MouseButtonEventArgs e)
        { _processManager.OpenSite(); }

        private void OnForumEnter(object sender, MouseButtonEventArgs e)
        { _processManager.OpenForum(); }

        private void OnVoteEnter(object sender, MouseButtonEventArgs e)
        { _processManager.OpenVote(); }

        private void OnNextSlideEnter(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ExtractFileToDirectory();
            _viewModel.ImageManager.UpdatePagination(MouvementTransition.Next);
        }

        private void OnPrevSlideEnter(object sender, MouseButtonEventArgs e)
        { _viewModel.ImageManager.UpdatePagination(MouvementTransition.Previous); }

        private void OnPlayEnter(object sender, MouseButtonEventArgs e)
        { _processManager.OpenGame(); }

        private void Window_Closed(object sender, System.EventArgs e)
        {

            var folder = Directory.GetCurrentDirectory();
            var zipFileName = $"{folder}/client.zip";
            _viewModel.Downloader.Dispose();

            if (File.Exists(zipFileName))
                File.Delete(zipFileName);
        }
    }
}
