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
    public class SettingRepository : BaseRepository, ISettingRepository
    {
        public SettingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public void AddSetting(Setting setting)
        {
            this.GetDbSet<Setting>().Add(setting);
            UnitOfWork.SaveChanges();
        }

        public IQueryable<Setting> GetSettings()
        {
            return this.GetDbSet<Setting>();
        }

        public void UpdateSetting(Setting setting)
        {
            this.GetDbSet<Setting>().Update(setting);
            UnitOfWork.SaveChanges();
        }

        public void DeleteSetting(Setting setting)
        {
            this.GetDbSet<Setting>().Remove(setting);
            UnitOfWork.SaveChanges();
        }

        public bool SettingExists(int userId)
        {
            return this.GetDbSet<Setting>().Any(x => x.UserId == userId);
        }

    }
}
