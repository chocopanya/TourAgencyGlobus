using System.Windows;
using TourAgencyGlobus.Services;

namespace TourAgencyGlobus.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            rbGuest.Checked += (s, e) => ManagerPanel.Visibility = Visibility.Collapsed;
            rbManager.Checked += (s, e) => ManagerPanel.Visibility = Visibility.Visible;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (rbGuest.IsChecked == true)
            {
                // Гостевой режим
                var mainWindow = new MainWindow(null);
                mainWindow.Show();
                this.Close();
            }
            else if (rbManager.IsChecked == true)
            {
                // Проверка менеджера
                string login = txtLogin.Text.Trim();
                string password = txtPassword.Password;

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Введите логин и пароль", "Ошибка");
                    return;
                }

                using (var service = new DataService())
                {
                    var user = service.GetUserByLogin(login);

                    if (user != null && user.Password == password)
                    {
                        // Проверка роли (1-админ, 2-менеджер)
                        if (user.RoleId == 1 || user.RoleId == 2)
                        {
                            var mainWindow = new MainWindow(user);
                            mainWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Недостаточно прав", "Ошибка");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль", "Ошибка");
                    }
                }
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}