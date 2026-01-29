using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.Services.Specifications;
using GymManagementBLL.ViewModels;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AnalyticsViewModel> GetAnalyticsDataAsync()
        {
            var activeMembersSpecs = new MembershipWithFilterSpecification(true);
            var upcomingSessionsSpecs = new SessionWithFilterSpecification("Upcoming");
            var ongoingSessionsSpecs = new SessionWithFilterSpecification("Ongoing");
            var completedSessionsSpecs = new SessionWithFilterSpecification("Completed");

            return new AnalyticsViewModel
            {
                ActiveMembers = await _unitOfWork.GetRepository<Membership>().CountAsync(activeMembersSpecs),
                TotalMembers = await _unitOfWork.GetRepository<Member>().CountAsync(),
                TotalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(),
                UpcomingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(upcomingSessionsSpecs),
                OngoingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(ongoingSessionsSpecs),
                CompletedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(completedSessionsSpecs)
            };
        }
    }
}
