using System.Windows;

namespace Uplauncher
{
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Une érreur est survenue, veuillez contacter [Zerberos] : {e.Exception.ToString()}", "Erreur",
                            MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
