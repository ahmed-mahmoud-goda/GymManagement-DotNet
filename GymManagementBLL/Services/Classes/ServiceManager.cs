using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace GymManagementBLL.Services.Classes
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IMemberService> memberService;
        private readonly Lazy<ITrainerService> trainerService;
        private readonly Lazy<IPlanService> planService;
        private readonly Lazy<ISessionService> sessionService;
        private readonly Lazy<IAnalyticsService> analyticsService;
        private readonly Lazy<IAccountService> accountService;

        public ServiceManager(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            memberService = new Lazy<IMemberService>(() => new MemberService(unitOfWork,mapper));
            trainerService = new Lazy<ITrainerService>(() => new TrainerService(unitOfWork,mapper));
            planService = new Lazy<IPlanService>(() => new PlanService(unitOfWork,mapper));
            sessionService = new Lazy<ISessionService>(() => new SessionService(unitOfWork,mapper));
            analyticsService = new Lazy<IAnalyticsService>(()=> new AnalyticsService(unitOfWork));
            accountService = new Lazy<IAccountService>(() => new AccountService(userManager));
        }

        public IMemberService MemberService => memberService.Value;
        public ITrainerService TrainerService => trainerService.Value;
        public IPlanService PlanService => planService.Value;
        public ISessionService SessionService => sessionService.Value;
        public IAnalyticsService AnalyticsService => analyticsService.Value;
        public IAccountService AccountService => accountService.Value;
    }
}
