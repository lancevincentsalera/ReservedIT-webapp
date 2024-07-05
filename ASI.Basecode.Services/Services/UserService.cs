using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public LoginResult AuthenticateUser(string email, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.Email == email &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public void AddUser(UserViewModel model)
        {
            var user = new User();
            if (!_repository.UserExists(model.Email))
            {
                _mapper.Map(model, user);
                user.AccountStatus = "PENDING";
                user.Password = PasswordManager.EncryptPassword(model.Password);
                user.CreatedDt = DateTime.Now;
                user.UpdatedDt = DateTime.Now;
                user.CreatedBy = System.Environment.UserName;
                user.UpdatedBy = System.Environment.UserName;

                _repository.AddUser(user);
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }
        }

        public List<Role> GetRoles()
        {
           return _repository.GetRoles().ToList();
        }

        public IEnumerable<UserViewModel> GetUsers()
        {
            var users = _repository.GetUsers().Select(u => new UserViewModel
            {
                UserId = u.UserId,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                RoleId = u.RoleId,
                RoleName = u.Role.RoleName,
                AccountStatus = u.AccountStatus,
            });

            return users;
        }

        public void UpdateUser(UserViewModel user)
        {
            var userToBeUpdated = _repository.GetUsers().Where(u => u.UserId ==  user.UserId).FirstOrDefault();
            if(userToBeUpdated != null)
            {
                _mapper.Map(user, userToBeUpdated);
                userToBeUpdated.Password = PasswordManager.EncryptPassword(user.Password);
                userToBeUpdated.UpdatedDt = DateTime.Now;
                userToBeUpdated.UpdatedBy = System.Environment.UserName;

                _repository.UpdateUser(userToBeUpdated);
            }
        }

        public void DeleteUser(UserViewModel user)
        {
            var userToBeDeleted = _repository.GetUsers().Where(u => u.UserId == user.UserId).FirstOrDefault();
            if (userToBeDeleted != null)
            {
                _repository.DeleteUser(userToBeDeleted);
            }

        }
    }
}
