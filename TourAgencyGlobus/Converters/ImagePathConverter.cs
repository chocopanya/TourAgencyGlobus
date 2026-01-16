using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TourAgencyGlobus
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is string fileName && !string.IsNullOrEmpty(fileName))
                {
                    // Пробуем несколько возможных путей
                    string[] possiblePaths = {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Debug", "Images", fileName),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Release", "Images", fileName),
                        Path.Combine(Environment.CurrentDirectory, "Images", fileName)
                    };

                    foreach (string path in possiblePaths)
                    {
                        if (File.Exists(path))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.UriSource = new Uri(path);
                            bitmap.EndInit();
                            return bitmap;
                        }
                    }
                }

                // Заглушка, если изображение не найдено
                string placeholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "placeholder.jpg");
                if (File.Exists(placeholderPath))
                {
                    var placeholder = new BitmapImage();
                    placeholder.BeginInit();
                    placeholder.CacheOption = BitmapCacheOption.OnLoad;
                    placeholder.UriSource = new Uri(placeholderPath);
                    placeholder.EndInit();
                    return placeholder;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}