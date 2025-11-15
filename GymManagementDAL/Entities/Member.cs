using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Entities.Enum;

namespace GymManagementDAL.Entities
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }
        public HealthRecord HealthRecord { get; set; } = null!;
        public ICollection<Membership> MemberPlans { get; set; } = null!;
        public ICollection<Booking> MemberSessions { get; set; } = null!;
    }
}
