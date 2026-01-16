using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;
using System.IO;

namespace TourAgencyGlobus.Models
{
    [Table("Tours")]
    public class Tour
    {
        [Key]
        [Column("TourID")]
        public int Id { get; set; }

        [Column("Title")]
        public string Name { get; set; }

        [Column("CountryID")]
        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }

        [Column("DurationDays")]
        public int Duration { get; set; }

        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        [Column("Price")]
        public decimal Price { get; set; }

        [Column("Discount")]
        public decimal Discount { get; set; } = 0;

        [Column("BusTypeID")]
        public int BusTypeId { get; set; }

        [ForeignKey("BusTypeId")]
        public virtual BusType BusType { get; set; }

        [Column("Capacity")]
        public int TotalSeats { get; set; }

        [Column("AvailableSeats")]
        public int FreeSeats { get; set; }

        [Column("PhotoFileName")]
        public string PhotoFileName { get; set; }

        // Вычисляемые свойства
        [NotMapped]
        public decimal FinalPrice => Price * (1 - Discount / 100);

        [NotMapped]
        public bool IsSpecialOffer => Discount > 15;

        [NotMapped]
        public bool IsFewSeats => TotalSeats > 0 && (double)FreeSeats / TotalSeats < 0.1;

        [NotMapped]
        public bool IsStartingSoon => (StartDate - DateTime.Now).TotalDays < 7;

        [NotMapped]
        public double OccupancyPercent => TotalSeats > 0 ?
            ((double)(TotalSeats - FreeSeats) / TotalSeats) * 100 : 0;

        [NotMapped]
        public bool HasAvailableSeats => FreeSeats > 0;

        [NotMapped]
        public string PhotoPath
        {
            get
            {
                if (!string.IsNullOrEmpty(PhotoFileName))
                {
                    string[] possiblePaths = {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", PhotoFileName),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Debug", "Images", PhotoFileName),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Release", "Images", PhotoFileName),
                        Path.Combine(Environment.CurrentDirectory, "Images", PhotoFileName)
                    };

                    foreach (string path in possiblePaths)
                    {
                        if (File.Exists(path))
                        {
                            return path;
                        }
                    }
                }

                // Путь к заглушке
                string placeholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "placeholder.jpg");
                if (File.Exists(placeholderPath))
                {
                    return placeholderPath;
                }

                return null;
            }
        }

        [NotMapped]
        public Brush OccupancyColor
        {
            get
            {
                if (OccupancyPercent < 50)
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80));
                if (OccupancyPercent < 80)
                    return new SolidColorBrush(Color.FromRgb(255, 193, 7));
                return new SolidColorBrush(Color.FromRgb(244, 67, 54));
            }
        }

        public virtual ICollection<TourApplication> Applications { get; set; } = new List<TourApplication>();
    }
}