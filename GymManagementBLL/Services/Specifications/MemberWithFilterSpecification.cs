using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Specifications
{
    public class MemberWithFilterSpecification : Specification<Member>
    {
        public MemberWithFilterSpecification(string email,string phone, int? id) : base(x => (x.Email == email || x.Phone == phone) && x.Id != id) { }
        public MemberWithFilterSpecification(bool activeMember) : base(x => activeMember
          ? x.MemberPlans.Any(p => p.EndDate >= DateTime.Now)  : !x.MemberPlans.Any(p => p.EndDate >= DateTime.Now))
        {
            AddInclude(x => x.MemberPlans);
        }
        public MemberWithFilterSpecification(int sessionId) : base(m => m.MemberPlans.Any(p => p.EndDate >= DateTime.Now) && !m.MemberSessions.Any(x => x.SessionId == sessionId)) { }
    }
}
