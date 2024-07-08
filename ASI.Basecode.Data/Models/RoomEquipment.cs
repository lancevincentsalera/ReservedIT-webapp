using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class RoomEquipment
    {
        public int RoomEquipmentId { get; set; }
        public int? RoomId { get; set; }
        public int? EquipmentId { get; set; }

        public virtual Equipment Equipment { get; set; }
        public virtual Room Room { get; set; }
    }
}
