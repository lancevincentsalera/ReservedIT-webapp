using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Room
    {
        public Room()
        {
            Bookings = new HashSet<Booking>();
            ImageGalleries = new HashSet<ImageGallery>();
            RoomEquipments = new HashSet<RoomEquipment>();
        }

        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string Thumbnail { get; set; }
        public int? Capacity { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<ImageGallery> ImageGalleries { get; set; }
        public virtual ICollection<RoomEquipment> RoomEquipments { get; set; }
    }
}
