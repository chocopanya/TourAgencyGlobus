using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TourAgencyGlobus.Models;
using TourAgencyGlobus.Services;
using System.Windows.Controls;

namespace TourAgencyGlobus.Views
{
    public partial class MainWindow : Window
    {
        private DataService _dataService;
        private User _currentUser;
        private ObservableCollection<Tour> _tours;
        private DateTime _lastClickTime = DateTime.MinValue;
        private Tour _lastClickedTour = null;

        public MainWindow(User user)
        {
            try
            {
                InitializeComponent();
                _dataService = new DataService();
                _currentUser = user;

                // Настройка интерфейса
                if (_currentUser != null && _currentUser.IsManager)
                {
                    Title = $"Турагентство 'Глобус' - Менеджер: {_currentUser.FullName}";
                    txtUserInfo.Text = $"Менеджер: {_currentUser.FullName}";
                    ManagerPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    Title = "Турагентство 'Глобус' - Гостевой режим";
                    txtUserInfo.Text = "Гость";
                    ManagerPanel.Visibility = Visibility.Collapsed;
                }

                // Загрузка данных
                LoadTours();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания главного окна: {ex.Message}", "Ошибка");
                throw;
            }
        }

        private void LoadTours()
        {
            try
            {
                var tours = _dataService.GetAllTours();
                _tours = new ObservableCollection<Tour>(tours);
                icTours.ItemsSource = _tours;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки туров: {ex.Message}", "Ошибка");
            }
        }

        private void TourGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (grid?.DataContext is Tour tour)
            {
                // Проверяем, был ли двойной клик (в течение 300 мс)
                var clickTime = DateTime.Now;
                if (_lastClickedTour == tour && (clickTime - _lastClickTime).TotalMilliseconds < 300)
                {
                    // Это двойной клик
                    if (_currentUser != null && _currentUser.IsManager)
                    {
                        var result = MessageBox.Show($"Создать заявку на тур '{tour.Name}'?", "Создание заявки",
                            MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            var appWindow = new ApplicationEditWindow(_dataService);
                            if (appWindow.ShowDialog() == true)
                            {
                                LoadTours(); // Обновляем список туров
                            }
                        }
                    }

                    // Сбрасываем таймер
                    _lastClickTime = DateTime.MinValue;
                    _lastClickedTour = null;
                }
                else
                {
                    // Простой клик - запоминаем время и тур
                    _lastClickTime = clickTime;
                    _lastClickedTour = tour;
                }
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void BtnAddTour_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new TourEditWindow(_dataService);
            if (editWindow.ShowDialog() == true)
            {
                LoadTours();
            }
        }

        private void BtnEditTour_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно редактирования тура
            var editWindow = new TourEditWindow(_dataService);
            if (editWindow.ShowDialog() == true)
            {
                LoadTours();
            }
        }

        private void BtnDeleteTour_Click(object sender, RoutedEventArgs e)
        {
            // Показываем окно с выбором тура для удаления
            var deleteWindow = new DeleteTourWindow(_dataService);
            if (deleteWindow.ShowDialog() == true)
            {
                LoadTours();
            }
        }

        private void BtnApplications_Click(object sender, RoutedEventArgs e)
        {
            var applicationsWindow = new ApplicationsWindow(_dataService);
            applicationsWindow.Owner = this;
            applicationsWindow.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _dataService?.Dispose();
        }
    }
}