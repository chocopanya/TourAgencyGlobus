using System.Windows;
using TourAgencyGlobus.Views;

namespace TourAgencyGlobus
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Просто показываем окно входа
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}