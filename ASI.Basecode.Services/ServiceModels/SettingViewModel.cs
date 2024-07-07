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
        public DateTime? BookingReminder { get; set; }
        public TimeSpan? BookingDuration { get; set; }
    }
}
