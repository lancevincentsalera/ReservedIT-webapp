using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class ImageGallery
    {
        public int ImageId { get; set; }
        public int? RoomId { get; set; }
        public string ImageName { get; set; }
        public string Path { get; set; }

        public virtual Room Room { get; set; }
    }
}
