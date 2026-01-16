using System.Windows;
using TourAgencyGlobus.Services;

namespace TourAgencyGlobus.Views
{
    public partial class ApplicationsWindow : Window
    {
        private readonly DataService _dataService;

        public ApplicationsWindow(DataService service)
        {
            InitializeComponent();
            _dataService = service;
            LoadApplications();
        }

        private void LoadApplications()
        {
            try
            {
                var applications = _dataService.GetAllApplications();
                dgApplications.ItemsSource = applications;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка");
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadApplications();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}