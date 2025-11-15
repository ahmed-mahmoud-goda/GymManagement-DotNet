using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Entities;

namespace GymManagementDAL.Data.Repositories.Interfaces
{
    public interface ITrainerRepository
    {
        Trainer? GetById(int id);
        IEnumerable<Trainer> GetAll();
        int Add(Trainer member);
        int Update(Trainer member);
        int Delete(int id);
    }
}
