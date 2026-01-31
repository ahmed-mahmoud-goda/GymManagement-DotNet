using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Specifications
{
    public class SessionWithFilterSpecification : Specification<Session>
    {
        public SessionWithFilterSpecification(string status): base(status switch
        {
            "Upcoming" => s => s.StartDate > DateTime.UtcNow,
            "Ongoing" => s => s.StartDate <= DateTime.UtcNow && s.EndDate >= DateTime.UtcNow,
            "Completed" => s => s.EndDate < DateTime.UtcNow,
            _ => throw new ArgumentException("Invalid session status. Use Upcoming, Ongoing or Completed")
        })
        {
            
        }

        public SessionWithFilterSpecification(int trainerId): base(x => x.TrainerId == trainerId && x.StartDate > DateTime.Now) { }

        public SessionWithFilterSpecification(bool isChanged = false): base(
            s => !isChanged
                || (s.StartDate <= DateTime.Now && s.StartDate > DateTime.Now.AddMinutes(-1))
                || (s.EndDate <= DateTime.Now && s.EndDate > DateTime.Now.AddMinutes(-1))
            )
        {
            AddInclude(s => s.Category);
            AddInclude(s => s.Trainer);
            if (!isChanged)
            {
                AddInclude(s => s.SessionMembers);
                setOrderByDescending(s => s.StartDate);
            }
        }

        public SessionWithFilterSpecification(int sessionId, bool addInclude = true) : base(s => s.Id == sessionId)
        {
            if (addInclude)
            {
                AddInclude(s => s.Trainer);
                AddInclude(s => s.Category);
            }
        }
    }
}
