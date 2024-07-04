using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class User
    {
        public User()
        {
            Bookings = new HashSet<Booking>();
            Settings = new HashSet<Setting>();
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
        public string AccountStatus { get; set; }
        public int? RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Setting> Settings { get; set; }
    }
}
