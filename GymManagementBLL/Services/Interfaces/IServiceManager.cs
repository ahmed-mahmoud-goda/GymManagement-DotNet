using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementDAL.Data.Repositories.Interfaces;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IServiceManager
    {
        public IMemberService MemberService { get; }
        public ITrainerService TrainerService { get; }
        public IPlanService PlanService { get; }
        public ISessionService SessionService { get; }
    }
}
