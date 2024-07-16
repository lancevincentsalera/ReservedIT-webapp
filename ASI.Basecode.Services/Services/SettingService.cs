using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class SettingService : ISettingService
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public SettingService(ISettingRepository settingRepository, IUserRepository userRepository, IMapper mapper)
        {
            _settingRepository = settingRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public IEnumerable<SettingViewModel> GetSettings()
        {
            var settings = _settingRepository.GetSettings().Select(s => new SettingViewModel
            {
                SettingId = s.SettingId,
                UserId = s.UserId,
                BookingSuccess = s.BookingSuccess,
                BookingStatusChange = s.BookingStatusChange,
                BookingReminder = s.BookingReminder,
                BookingDuration = s.BookingDuration,
            });

            return settings;
        }

        public SettingViewModel GetSetting(int userId)
        {
            var existingData = _settingRepository.GetSettings().Where(s => s.UserId == userId).FirstOrDefault();

            var setting = new SettingViewModel
            {
                SettingId = existingData.SettingId,
                UserId = existingData.UserId,
                BookingSuccess = existingData.BookingSuccess,
                BookingStatusChange = existingData.BookingStatusChange,
                BookingReminder = existingData.BookingReminder,
                BookingDuration = existingData.BookingDuration,
            };

            return setting;
        }

        public Role GetRole(int roleId)
        {
            return _settingRepository.GetRoles().ToList().Where(r => r.RoleId == roleId).FirstOrDefault();
        }

        public void Add(SettingViewModel model)
        {
            var newModel = new Setting();
            _mapper.Map(model, newModel);
            _settingRepository.AddSetting(newModel);
        }

        public void Update(SettingViewModel model)
        {
            var existingData = _settingRepository.GetSettings().Where(s => s.SettingId == model.SettingId).FirstOrDefault();
            _mapper.Map(model, existingData);
            _settingRepository.UpdateSetting(existingData);
        }

        public void Delete(int userId)
        {
            var existingData = _settingRepository.GetSettings().Where(s => s.UserId == userId).FirstOrDefault();
            _settingRepository.DeleteSetting(existingData);
        }

        public bool SettingExists(int userId)
        {
            return _settingRepository.SettingExists(userId);
        }

    }
}
