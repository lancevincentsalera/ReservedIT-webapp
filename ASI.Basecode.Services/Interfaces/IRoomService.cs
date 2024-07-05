using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRoomService
    {
        IEnumerable<RoomViewModel> RetrieveAll(string roomName = null);
        RoomViewModel RetrieveRoom(int roomId);
        void AddRoom(RoomViewModel model);
        void UpdateRoom(RoomViewModel model);
        void UpdateGallery(RoomGalleryViewModel model);
    }
}
