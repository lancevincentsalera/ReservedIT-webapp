using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string email, string password, ref User user);
        void AddUser(UserViewModel model);
        List<Role> GetRoles();
        IEnumerable<UserViewModel> GetUsers();
        void UpdateUser(UserViewModel user);
        void DeleteUser(UserViewModel user);
    }
}
