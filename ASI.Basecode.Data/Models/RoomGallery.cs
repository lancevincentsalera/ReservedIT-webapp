using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class RoomGallery
    {
        public int GalleryId { get; set; }
        public int RoomId { get; set; }
        public string GalleryName { get; set; }
        public string GalleryPath { get; set; }

        public virtual Room Room { get; set; }
    }
}
