using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymManagementDAL.Data.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _context;

        public SessionRepository(GymDbContext context) : base(context)
        {
            _context = context;
        }
        public IEnumerable<Session> GetAllSessionsWithTrainerAndCategory()
        {
            return _context.Sessions.Include(s=>s.Trainer).Include(s=>s.Category).ToList();
        }

        public int GetCountOfBookedSlots(int sessionId)
        {
            return _context.Bookings.Where(x=>x.SessionId == sessionId).Count();
        }

        public Session? GetSessionByIdWithTrainerAndCategory(int sessionId)
        {
            return _context.Sessions.Include(s => s.Trainer).Include(s => s.Category).FirstOrDefault(s => s.Id == sessionId);
        }
    }
}
