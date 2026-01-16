using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourAgencyGlobus.Models
{
    [Table("BusTypes")]
    public class BusType
    {
        [Key]
        [Column("BusTypeID")]
        public int Id { get; set; }

        [Column("TypeName")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("Capacity")]
        public int Capacity { get; set; }

        public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
    }
}