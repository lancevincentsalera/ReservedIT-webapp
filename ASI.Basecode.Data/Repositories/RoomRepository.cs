using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class RoomRepository : BaseRepository, IRoomRepository
    {
        public RoomRepository(IUnitOfWork unitOfWork) : base(unitOfWork) 
        { 

        }
        public IQueryable<Room> GetRooms()
        {
            return this.GetDbSet<Room>();
        }
        public IQueryable<ImageGallery> GetRoomGalleries ()
        {
            return this.GetDbSet<ImageGallery>();
        }
        public bool RoomExists(int roomId)
        {
            return this.GetDbSet<Room>().Any(x => x.RoomId == roomId);
        }
        public void AddRoom(Room room)
        {
            this.GetDbSet<Room>().Add(room); 
            UnitOfWork.SaveChanges();
        }
        public void UpdateRoom(Room room)
        {
            this.GetDbSet<Room>().Update(room);
            UnitOfWork.SaveChanges();
        }
        public void UpdateGallery(ImageGallery imageGallery) 
        {
            this.GetDbSet<ImageGallery>().Update(imageGallery);
            UnitOfWork.SaveChanges();
        }

        public void DeleteRoom(Room room) 
        {
            this.GetDbSet<Room>().Remove(room);
            UnitOfWork.SaveChanges();
        }
        public void DeleteRoomImage(ImageGallery imageGallery)
        {
            this.GetDbSet<ImageGallery>().Remove(imageGallery);
            UnitOfWork.SaveChanges();
        }

        public IQueryable<DayOfTheWeek> GetDays()
        {
            return this.GetDbSet<DayOfTheWeek>();
        }
    }
}
