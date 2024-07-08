using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class RoomEquipmentViewModel
    {
        public int RoomEquipmentId { get; set; }
        public int? RoomId { get; set; }
        public int? EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string Type { get; set; }
    }
}
