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
using Microsoft.Extensions.Caching.Distributed;

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
        private readonly Lazy<IMembershipService> membershipService;
        private readonly Lazy<IBookingService> bookingService;

        public ServiceManager(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, IDistributedCache  cache )
        {
            memberService = new Lazy<IMemberService>(() => new MemberService(unitOfWork,mapper,cache));
            trainerService = new Lazy<ITrainerService>(() => new TrainerService(unitOfWork,mapper,cache));
            planService = new Lazy<IPlanService>(() => new PlanService(unitOfWork,mapper,cache));
            sessionService = new Lazy<ISessionService>(() => new SessionService(unitOfWork,mapper,cache));
            analyticsService = new Lazy<IAnalyticsService>(()=> new AnalyticsService(unitOfWork));
            accountService = new Lazy<IAccountService>(() => new AccountService(userManager));
            membershipService = new Lazy<IMembershipService>(() => new MembershipService(unitOfWork,mapper));
            bookingService = new Lazy<IBookingService>(() => new BookingService(unitOfWork, mapper));
        }

        public IMemberService MemberService => memberService.Value;
        public ITrainerService TrainerService => trainerService.Value;
        public IPlanService PlanService => planService.Value;
        public ISessionService SessionService => sessionService.Value;
        public IAnalyticsService AnalyticsService => analyticsService.Value;
        public IAccountService AccountService => accountService.Value;
        public IMembershipService MembershipService => membershipService.Value;
        public IBookingService BookingService => bookingService.Value;
    }
}
