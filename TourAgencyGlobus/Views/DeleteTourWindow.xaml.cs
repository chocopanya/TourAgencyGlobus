using System.Windows;
using TourAgencyGlobus.Services;

namespace TourAgencyGlobus.Views
{
    public partial class DeleteTourWindow : Window
    {
        private readonly DataService _dataService;

        public DeleteTourWindow(DataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
            LoadTours();
        }

        private void LoadTours()
        {
            var tours = _dataService.GetAllTours();
            dgTours.ItemsSource = tours;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedTour = dgTours.SelectedItem as Models.Tour;
            if (selectedTour != null)
            {
                var result = MessageBox.Show($"Удалить тур '{selectedTour.Name}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_dataService.DeleteTour(selectedTour.Id))
                    {
                        MessageBox.Show("Тур успешно удалён", "Успех");
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении тура", "Ошибка");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите тур для удаления", "Внимание");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}