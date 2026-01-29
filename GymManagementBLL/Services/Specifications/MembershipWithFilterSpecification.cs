using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Specifications
{
    public class MembershipWithFilterSpecification : Specification<Membership>
    {
        public MembershipWithFilterSpecification(bool activeOnly,int? memberId = null, int? planId= null) : base(m =>
        (!memberId.HasValue || m.MemberId == memberId.Value) &&
        (!planId.HasValue || m.PlanId == planId.Value) &&
        (!activeOnly || m.EndDate >= DateTime.Now))
        {
            AddInclude(m => m.Member);
            AddInclude(m => m.Plan);
        }
    }
}
