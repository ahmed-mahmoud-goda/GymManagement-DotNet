using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementBLL.Services.Interfaces;
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

        public AnalyticsViewModel GetAnalyticsData()
        {
            var sessionRepository = _unitOfWork.GetRepository<Session>();

            return new AnalyticsViewModel
            {
                ActiveMembers = _unitOfWork.GetRepository<Membership>().GetAll(x => x.Status == "Active").Count(),
                TotalMembers = _unitOfWork.GetRepository<Member>().GetAll().Count(),
                TotalTrainers = _unitOfWork.GetRepository<Trainer>().GetAll().Count(),
                UpcomingSessions = sessionRepository.GetAll(x => x.StartDate > DateTime.UtcNow).Count(),
                OngoingSessions = sessionRepository.GetAll(x => x.StartDate <= DateTime.UtcNow && x.EndDate >= DateTime.UtcNow).Count(),
                CompletedSessions = sessionRepository.GetAll(x => x.EndDate < DateTime.UtcNow).Count()
            };
        }
    }
}
