using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourAgencyGlobus.Models
{
    [Table("Countries")]
    public class Country
    {
        [Key]
        [Column("CountryID")]
        public int Id { get; set; }

        [Column("CountryName")]
        public string Name { get; set; }

        public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
    }
}