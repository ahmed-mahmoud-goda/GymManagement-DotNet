using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Entities;

namespace GymManagementDAL.Data.Repositories.Interfaces
{
    public interface IMemberRepository : IGenericRepository<Member>
    {
        IEnumerable<Session> GetAllSessions(int memberId);
    }
}
