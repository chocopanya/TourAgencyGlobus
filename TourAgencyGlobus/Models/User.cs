using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourAgencyGlobus.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("UserID")]
        public int Id { get; set; }

        [Column("RoleID")]
        public int RoleId { get; set; }

        [Column("FullName")]
        public string FullName { get; set; }

        [Column("Login")]
        public string Login { get; set; }

        [Column("PasswordHash")]
        public string Password { get; set; }

        [NotMapped]
        public bool IsManager
        {
            get { return RoleId == 1 || RoleId == 2; } // Администратор или менеджер
        }

        public virtual ICollection<TourApplication> Applications { get; set; } = new List<TourApplication>();
    }
}