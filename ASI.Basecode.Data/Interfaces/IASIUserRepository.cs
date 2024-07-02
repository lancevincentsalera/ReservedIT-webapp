﻿using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IASIUserRepository
    {
        IQueryable<ASIUser> GetUsers();
        bool UserExists(string userId);
        void AddUser(ASIUser user);
    }
}
