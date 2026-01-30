using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.Services.Specifications;
using GymManagementBLL.ViewModels;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace GymManagementBLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private const string cacheKey = "plans";

        public PlanService(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<bool> ActivateAsync(int planId)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId);

            if(plan is null || await HasActiveMembershipsAsync(planId))
                return false;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            await _cache.RemoveAsync(cacheKey);
            return (await _unitOfWork.SaveChangesAsync()) > 0;
        }

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync()
        {
            var cached = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<IEnumerable<PlanViewModel>>(cached)!;

            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync();

            if (plans is null || !plans.Any())
                return [];

            var mappedPlans = _mapper.Map<IEnumerable<PlanViewModel>>(plans);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(mappedPlans), cacheOptions);


            return mappedPlans;
        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int id)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id);

            if (plan is null)
                return null;

            var mappedPlan = _mapper.Map<PlanViewModel>(plan);

            return mappedPlan;
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int id)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id);

            if (plan is null || plan.IsActive == false)
                return null;

            var mappedPlan = _mapper.Map<UpdatePlanViewModel>(plan);

            return mappedPlan;
        }

        public async Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel input)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id);
            
            if (plan is null || await HasActiveMembershipsAsync(id))
            return false;

            _mapper.Map(input, plan);

            _unitOfWork.GetRepository<Plan>().Update(plan);

            await _cache.RemoveAsync(cacheKey);

            return (await _unitOfWork.SaveChangesAsync()) > 0; 
        }

        #region HelperMethods

        private async Task<bool> HasActiveMembershipsAsync(int planId)
        {
            var membershipSpecs = new MembershipWithFilterSpecification(true,planId: planId);
            var membership = await _unitOfWork.GetRepository<Membership>().GetAllAsync(membershipSpecs);
            return membership.Any();
        }

        #endregion
    }
}
