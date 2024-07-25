using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Services.ServiceModels
{
    public class SettingViewModel
    {
        public int SettingId { get; set; }
        public int? UserId { get; set; }
        public int? BookingSuccess { get; set; }
        public int? BookingStatusChange { get; set; }
        public int? BookingReminder { get; set; }
        public int? BookingDuration { get; set; }
        public virtual User User { get; set; }
        
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and include: Uppercase letter, Lowercase letter, Digit, Special character")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Confirmation Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
