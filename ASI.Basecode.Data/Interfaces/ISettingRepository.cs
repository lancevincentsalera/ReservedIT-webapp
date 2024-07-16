using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ISettingRepository
    {
        void AddSetting(Setting setting);
        IQueryable<Setting> GetSettings();
        IEnumerable<Role> GetRoles();
        void UpdateSetting(Setting setting);
        void DeleteSetting(Setting setting);
        bool SettingExists(int userId);
    }
}
