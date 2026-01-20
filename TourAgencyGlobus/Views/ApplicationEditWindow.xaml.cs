using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TourAgencyGlobus.Models;
using TourAgencyGlobus.Services;

namespace TourAgencyGlobus.Views
{
    public partial class ApplicationEditWindow : Window
    {
        private readonly DataService _dataService;
        private readonly TourApplication _application;
        private bool _isNewApplication;

        public ApplicationEditWindow(DataService dataService, TourApplication application = null)
        {
            InitializeComponent();
            _dataService = dataService;
            _application = application ?? new TourApplication();
            _isNewApplication = application == null;

            LoadData();
            SetupEventHandlers();
        }

        private void LoadData()
        {
            try
            {
                // Загружаем клиентов
                var clients = _dataService.GetClients();
                cmbClient.ItemsSource = clients;

                // Загружаем туры
                var tours = _dataService.GetAllTours();
                cmbTour.ItemsSource = tours;

                if (!_isNewApplication)
                {
                    Title = $"Редактирование заявки #{_application.Id}";

                    // Заполняем данные
                    foreach (User client in cmbClient.Items)
                    {
                        if (client.Id == _application.ClientId)
                        {
                            cmbClient.SelectedItem = client;
                            break;
                        }
                    }

                    foreach (Tour tour in cmbTour.Items)
                    {
                        if (tour.Id == _application.TourId)
                        {
                            cmbTour.SelectedItem = tour;
                            UpdateTourInfo(tour);
                            break;
                        }
                    }

                    txtPersonsCount.Text = _application.NumberOfPeople.ToString();
                    txtTotalCost.Text = _application.TotalCost.ToString("F2");
                    txtComment.Text = _application.Comment;

                    // Выбираем статус
                    foreach (ComboBoxItem item in cmbStatus.Items)
                    {
                        if (item.Tag.ToString() == _application.StatusId.ToString())
                        {
                            cmbStatus.SelectedItem = item;
                            break;
                        }
                    }
                }
                else
                {
                    Title = "Создание новой заявки";

                    // Значения по умолчанию
                    if (clients.Any()) cmbClient.SelectedIndex = 0;
                    if (tours.Any()) cmbTour.SelectedIndex = 0;
                    cmbStatus.SelectedIndex = 0; // Новая
                    txtPersonsCount.Text = "1";

                    // Рассчитываем стоимость при выборе тура
                    if (cmbTour.SelectedItem is Tour selectedTour)
                    {
                        UpdateTourInfo(selectedTour);
                        CalculateTotalCost();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
        }

        private void SetupEventHandlers()
        {
            cmbTour.SelectionChanged += (s, e) =>
            {
                if (cmbTour.SelectedItem is Tour selectedTour)
                {
                    UpdateTourInfo(selectedTour);
                    CalculateTotalCost();
                }
            };

            txtPersonsCount.TextChanged += (s, e) =>
            {
                CalculateTotalCost();
            };
        }

        private void UpdateTourInfo(Tour tour)
        {
            txtTourInfo.Text = $"{tour.Name}\n" +
                              $"Страна: {tour.Country?.Name}\n" +
                              $"Дата начала: {tour.StartDate:dd.MM.yyyy}\n" +
                              $"Продолжительность: {tour.Duration} дней\n" +
                              $"Базовая стоимость: {tour.Price:F2} руб.";

            txtTourSeats.Text = $"Свободных мест: {tour.FreeSeats}";

            if (tour.FreeSeats == 0)
            {
                txtTourSeats.Foreground = System.Windows.Media.Brushes.Red;
            }
            else if (tour.FreeSeats < 10)
            {
                txtTourSeats.Foreground = System.Windows.Media.Brushes.Orange;
            }
            else
            {
                txtTourSeats.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void CalculateTotalCost()
        {
            try
            {
                if (cmbTour.SelectedItem is Tour selectedTour &&
                    int.TryParse(txtPersonsCount.Text, out int personsCount) &&
                    personsCount > 0)
                {
                    decimal totalCost = selectedTour.FinalPrice * personsCount;
                    txtTotalCost.Text = totalCost.ToString("F2");
                }
            }
            catch
            {
                txtTotalCost.Text = "0.00";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                // Заполняем данные заявки
                _application.ClientId = ((User)cmbClient.SelectedItem).Id;
                _application.TourId = ((Tour)cmbTour.SelectedItem).Id;
                _application.NumberOfPeople = int.Parse(txtPersonsCount.Text);
                _application.TotalCost = decimal.Parse(txtTotalCost.Text);
                _application.Comment = txtComment.Text;
                _application.StatusId = int.Parse(((ComboBoxItem)cmbStatus.SelectedItem).Tag.ToString());

                if (_isNewApplication)
                {
                    _application.ApplicationDate = DateTime.Now;

                    // Проверяем свободные места при подтверждении
                    if (_application.StatusId == 3)
                    {
                        var tour = (Tour)cmbTour.SelectedItem;
                        if (tour.FreeSeats < _application.NumberOfPeople)
                        {
                            MessageBox.Show($"Недостаточно свободных мест! Доступно: {tour.FreeSeats}, требуется: {_application.NumberOfPeople}", "Ошибка");
                            return;
                        }
                    }

                    if (_dataService.AddApplication(_application))
                    {
                        MessageBox.Show("Заявка успешно создана!", "Успех");
                        DialogResult = true;
                        Close();
                    }
                }
                else
                {
                    if (_dataService.UpdateApplication(_application))
                    {
                        MessageBox.Show("Заявка успешно обновлена!", "Успех");
                        DialogResult = true;
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка");
            }
        }

        private bool ValidateInput()
        {
            if (cmbClient.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента", "Ошибка");
                return false;
            }

            if (cmbTour.SelectedItem == null)
            {
                MessageBox.Show("Выберите тур", "Ошибка");
                return false;
            }

            if (!int.TryParse(txtPersonsCount.Text, out int personsCount) || personsCount <= 0)
            {
                MessageBox.Show("Введите корректное количество человек", "Ошибка");
                return false;
            }

            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус", "Ошибка");
                return false;
            }

            if (!decimal.TryParse(txtTotalCost.Text, out decimal totalCost) || totalCost <= 0)
            {
                MessageBox.Show("Введите корректную стоимость", "Ошибка");
                return false;
            }

            // Проверка на превышение максимальной вместимости
            var tour = (Tour)cmbTour.SelectedItem;
            if (personsCount > tour.TotalSeats)
            {
                MessageBox.Show($"Количество человек ({personsCount}) превышает вместимость автобуса ({tour.TotalSeats})", "Ошибка");
                return false;
            }

            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}