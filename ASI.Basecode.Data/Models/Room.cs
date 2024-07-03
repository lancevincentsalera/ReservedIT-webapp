using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Room
    {
        public Room()
        {
            Bookings = new HashSet<Booking>();
            RoomGalleries = new HashSet<RoomGallery>();
        }

        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomDescription { get; set; }
        public string RoomLocation { get; set; }
        public int? RoomCapacity { get; set; }
        public string RoomFacility { get; set; }
        public string RoomImage { get; set; }
        public DateTime? RoomInsDt { get; set; }
        public string RoomInsBy { get; set; }
        public DateTime? RoomUpdDt { get; set; }
        public string RoomUpdBy { get; set; }
        public string RoomThumbnail { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<RoomGallery> RoomGalleries { get; set; }
    }
}
