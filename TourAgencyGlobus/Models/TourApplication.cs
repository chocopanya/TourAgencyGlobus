using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourAgencyGlobus.Models
{
    [Table("Applications")]
    public class TourApplication
    {
        [Key]
        [Column("ApplicationID")]
        public int Id { get; set; }

        [Column("TourID")]
        public int TourId { get; set; }

        [ForeignKey("TourId")]
        public virtual Tour Tour { get; set; }

        [Column("ClientID")]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual User Client { get; set; }

        [Column("ApplicationDate")]
        public DateTime ApplicationDate { get; set; }

        [Column("StatusID")]
        public int StatusId { get; set; }

        [NotMapped]
        public string Status
        {
            get
            {
                if (StatusId == 1)
                    return "Новая";
                if (StatusId == 2)
                    return "В обработке";
                if (StatusId == 3)
                    return "Подтверждена";
                if (StatusId == 4)
                    return "Отменена";
                return "Неизвестно";
            }
        }

        [Column("PersonsCount")]
        public int NumberOfPeople { get; set; }

        [Column("TotalPrice")]
        public decimal TotalCost { get; set; }

        [Column("Comment")]
        public string Comment { get; set; }
    }
}