using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IRoomRepository
    {
        IQueryable<Room> GetRooms();
        IQueryable<ImageGallery> GetRoomGalleries();
        IQueryable<Equipment> GetEquipments();
        IQueryable<RoomEquipment> GetRoomEquipments();
        bool RoomExists(int roomId);
        void AddRoom(Room room);
        void UpdateRoom(Room room);
        void UpdateGallery(ImageGallery imageGallery);
        void DeleteRoom(Room room);
        void DeleteEquipment(Equipment equipment);
        void DeleteRoomEquipment(RoomEquipment roomEquipment);
        void DeleteRoomImage(ImageGallery imageGallery);
    }
}
