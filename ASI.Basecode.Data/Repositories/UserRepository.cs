using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ASI.Basecode.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public void AddUser(User user)
        {
            this.GetDbSet<User>().Add(user);
            UnitOfWork.SaveChanges();
        }

        public IEnumerable<Role> GetRoles()
        {
            return this.GetDbSet<Role>().ToList();
        }

        public IQueryable<User> GetUsers()
        {
            return this.GetDbSet<User>().Include(r => r.Role);
        }

        public bool UserExists(string email)
        {
            return this.GetDbSet<User>().Any(x => x.Email == email);
        }

        public void ActivateOrRestrictUser(User user) {
            this.GetDbSet<User>().Update(user);
            UnitOfWork.SaveChanges();
        }
    }
}
