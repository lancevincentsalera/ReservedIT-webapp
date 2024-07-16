using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ISettingService
    {
        IEnumerable<SettingViewModel> GetSettings();
        SettingViewModel GetSetting(int userId);
        Role GetRole(int roleId);
        void Add(SettingViewModel model);
        void Update(SettingViewModel model);
        void Delete(int userId);
        bool SettingExists(int userId);
    }
}
