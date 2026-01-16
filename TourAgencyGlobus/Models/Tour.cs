using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Column("Discount", TypeName = "decimal(5,2)")]
        public decimal Discount { get; set; } = 0;

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

        public virtual ICollection<TourApplication> Applications { get; set; } = new List<TourApplication>();
    }
}