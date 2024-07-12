using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Room
    {
        public Room()
        {
            ImageGalleries = new HashSet<ImageGallery>();
        }

        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string Thumbnail { get; set; }
        public int? Capacity { get; set; }
        public string Description { get; set; }
        public string Equipment { get; set; }
        public string Location { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDt { get; set; }
        public string Equipments { get; set; }

        public virtual ICollection<ImageGallery> ImageGalleries { get; set; }
    }
}
