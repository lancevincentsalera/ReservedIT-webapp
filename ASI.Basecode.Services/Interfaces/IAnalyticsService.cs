using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IAnalyticsService
    {
        List<int> BookingsPerMonth();
        Dictionary<int, List<Booking>> DailyBookings();
        IEnumerable<UserBookingFrequency> GetUserBookingFrequency();
        IEnumerable<RoomUsageSummary> GetRoomUsageSummary();
    }
}
