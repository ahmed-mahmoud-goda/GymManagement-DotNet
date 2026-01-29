using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Execution;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Specifications
{
    public class BookingWithFilterSpecification : Specification<Booking>
    {
        public BookingWithFilterSpecification(int memberId) : base(x => x.MemberId == memberId && x.Session.StartDate > DateTime.Now) { }
        public BookingWithFilterSpecification(bool upcomingSessionsOnly,int? memberId = null,int? sessionId = null): base(x =>
        (!memberId.HasValue || x.MemberId == memberId) && 
        (!sessionId.HasValue || x.SessionId == sessionId) &&
        (!upcomingSessionsOnly || x.Session.StartDate > DateTime.Now))
        {
            AddInclude(x => x.Member);
        }
    }
}
