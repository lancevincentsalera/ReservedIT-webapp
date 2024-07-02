using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class ASIUserRepository : BaseRepository, IASIUserRepository
    {
        public ASIUserRepository(IUnitOfWork unitOfWork) : base(unitOfWork) 
        {

        }

        public IQueryable<ASIUser> GetUsers()
        {
            return this.GetDbSet<ASIUser>();
        }

        public bool UserExists(string userId)
        {
            return this.GetDbSet<ASIUser>().Any(x => x.UserId == userId);
        }

        public void AddUser(ASIUser user)
        {
            this.GetDbSet<ASIUser>().Add(user);
            UnitOfWork.SaveChanges();
        }

    }
}
