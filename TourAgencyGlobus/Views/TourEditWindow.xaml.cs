using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TourAgencyGlobus.Models;
using TourAgencyGlobus.Services;

namespace TourAgencyGlobus.Views
{
    public partial class TourEditWindow : Window
    {
        private readonly DataService _dataService;
        private readonly Tour _tour;
        private string _selectedImageFileName;

        public TourEditWindow(DataService dataService, Tour tour = null)
        {
            InitializeComponent();
            _dataService = dataService;
            _tour = tour ?? new Tour();
            _selectedImageFileName = tour?.PhotoFileName ?? string.Empty;

            LoadData();
            CalculateFinalPrice();

            // Подписка на события изменения цены и скидки
            txtPrice.TextChanged += OnPriceChanged;
            txtDiscount.TextChanged += OnPriceChanged;
        }

        private void LoadData()
        {
            try
            {
                // Загружаем списки
                var countries = _dataService.GetCountries();
                cmbCountry.ItemsSource = countries;
                cmbCountry.DisplayMemberPath = "Name";

                var busTypes = _dataService.GetBusTypes();
                cmbBusType.ItemsSource = busTypes;
                cmbBusType.DisplayMemberPath = "Name";

                if (_tour.Id > 0) // Редактирование
                {
                    Title = $"Редактирование тура: {_tour.Name}";

                    txtName.Text = _tour.Name;
                    txtDuration.Text = _tour.Duration.ToString();
                    dpStartDate.SelectedDate = _tour.StartDate;
                    txtPrice.Text = _tour.Price.ToString("F2");
                    txtDiscount.Text = _tour.Discount.ToString("F2");
                    txtTotalSeats.Text = _tour.TotalSeats.ToString();
                    txtFreeSeats.Text = _tour.FreeSeats.ToString();

                    // Выбираем страну
                    foreach (Country country in cmbCountry.Items)
                    {
                        if (country.Id == _tour.CountryId)
                        {
                            cmbCountry.SelectedItem = country;
                            break;
                        }
                    }

                    // Выбираем тип автобуса
                    foreach (BusType busType in cmbBusType.Items)
                    {
                        if (busType.Id == _tour.BusTypeId)
                        {
                            cmbBusType.SelectedItem = busType;
                            break;
                        }
                    }

                    // Загружаем изображение, если есть
                    if (!string.IsNullOrEmpty(_tour.PhotoFileName))
                    {
                        LoadImageFromFile(_tour.PhotoFileName);
                    }
                }
                else // Добавление
                {
                    Title = "Добавление нового тура";

                    dpStartDate.SelectedDate = DateTime.Today.AddDays(14);

                    // Значения по умолчанию
                    if (countries.Any()) cmbCountry.SelectedIndex = 0;
                    if (busTypes.Any()) cmbBusType.SelectedIndex = 0;
                    txtTotalSeats.Text = "45";
                    txtFreeSeats.Text = "45";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
        }

        private void LoadImageFromFile(string fileName)
        {
            try
            {
                // Пробуем несколько путей
                string[] possiblePaths = {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Debug", "Images", fileName),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Release", "Images", fileName),
                    Path.Combine(Environment.CurrentDirectory, "Images", fileName)
                };

                string foundPath = null;
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        foundPath = path;
                        break;
                    }
                }

                if (foundPath != null)
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(foundPath);
                    bitmap.EndInit();

                    imgTour.Source = bitmap;
                    txtNoPhoto.Visibility = Visibility.Collapsed;
                    _selectedImageFileName = fileName;
                }
                else
                {
                    // Если изображение не найдено, показываем заглушку
                    string placeholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "placeholder.jpg");
                    if (File.Exists(placeholderPath))
                    {
                        var placeholder = new BitmapImage();
                        placeholder.BeginInit();
                        placeholder.CacheOption = BitmapCacheOption.OnLoad;
                        placeholder.UriSource = new Uri(placeholderPath);
                        placeholder.EndInit();

                        imgTour.Source = placeholder;
                    }
                    txtNoPhoto.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка");
            }
        }

        private void BtnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения PNG (*.png)|*.png|Изображения JPG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Все файлы (*.*)|*.*",
                Title = "Выберите изображение для тура",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string sourcePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(sourcePath);

                    // Копируем файл в папку Images приложения
                    string destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

                    // Если файл уже существует, добавляем timestamp
                    if (File.Exists(destPath))
                    {
                        string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                        string extension = Path.GetExtension(fileName);
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        fileName = $"{nameWithoutExt}_{timestamp}{extension}";
                        destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
                    }

                    File.Copy(sourcePath, destPath, true);

                    // Загружаем изображение
                    LoadImageFromFile(fileName);

                    MessageBox.Show($"Изображение '{fileName}' успешно загружено!", "Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка");
                }
            }
        }

        private void BtnClearImage_Click(object sender, RoutedEventArgs e)
        {
            imgTour.Source = null;
            txtNoPhoto.Visibility = Visibility.Visible;
            _selectedImageFileName = string.Empty;
        }

        private void OnPriceChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalculateFinalPrice();
        }

        private void CalculateFinalPrice()
        {
            try
            {
                if (decimal.TryParse(txtPrice.Text, out decimal price) &&
                    decimal.TryParse(txtDiscount.Text, out decimal discount))
                {
                    decimal finalPrice = price * (1 - discount / 100);
                    txtFinalPrice.Text = $"{finalPrice:F2} руб.";

                    // Подсветка спецпредложения
                    if (discount > 15)
                    {
                        txtFinalPrice.Foreground = System.Windows.Media.Brushes.Red;
                        txtFinalPrice.Text += " (Спецпредложение!)";
                    }
                    else
                    {
                        txtFinalPrice.Foreground = System.Windows.Media.Brushes.Green;
                    }
                }
            }
            catch
            {
                txtFinalPrice.Text = "0 руб.";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                // Заполняем данные тура
                _tour.Name = txtName.Text.Trim();
                _tour.CountryId = ((Country)cmbCountry.SelectedItem).Id;
                _tour.Duration = int.Parse(txtDuration.Text);
                _tour.StartDate = dpStartDate.SelectedDate ?? DateTime.Today;
                _tour.Price = decimal.Parse(txtPrice.Text);
                _tour.Discount = decimal.Parse(txtDiscount.Text);
                _tour.BusTypeId = ((BusType)cmbBusType.SelectedItem).Id;
                _tour.TotalSeats = int.Parse(txtTotalSeats.Text);
                _tour.FreeSeats = int.Parse(txtFreeSeats.Text);

                // Сохраняем имя файла изображения
                _tour.PhotoFileName = _selectedImageFileName;

                bool success;
                if (_tour.Id > 0)
                {
                    success = _dataService.UpdateTour(_tour);
                }
                else
                {
                    success = _dataService.AddTour(_tour);
                }

                if (success)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка сохранения тура", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private bool ValidateInput()
        {
            string errorMessage = "";

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errorMessage += "• Введите название тура\n";
            }

            if (cmbCountry.SelectedItem == null)
            {
                errorMessage += "• Выберите страну\n";
            }

            if (!int.TryParse(txtDuration.Text, out int duration) || duration <= 0)
            {
                errorMessage += "• Введите корректную продолжительность (больше 0 дней)\n";
            }

            if (dpStartDate.SelectedDate == null)
            {
                errorMessage += "• Выберите дату начала\n";
            }
            else if (dpStartDate.SelectedDate < DateTime.Today)
            {
                errorMessage += "• Дата начала не может быть в прошлом\n";
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                errorMessage += "• Введите корректную стоимость (больше 0)\n";
            }

            if (!decimal.TryParse(txtDiscount.Text, out decimal discount) || discount < 0 || discount > 100)
            {
                errorMessage += "• Скидка должна быть от 0 до 100%\n";
            }

            if (cmbBusType.SelectedItem == null)
            {
                errorMessage += "• Выберите тип автобуса\n";
            }

            if (!int.TryParse(txtTotalSeats.Text, out int totalSeats) || totalSeats <= 0)
            {
                errorMessage += "• Введите корректную вместимость (больше 0)\n";
            }

            if (!int.TryParse(txtFreeSeats.Text, out int freeSeats) || freeSeats < 0 || freeSeats > totalSeats)
            {
                errorMessage += $"• Количество свободных мест должно быть от 0 до {totalSeats}\n";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show("Исправьте следующие ошибки:\n\n" + errorMessage, "Ошибка валидации");
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