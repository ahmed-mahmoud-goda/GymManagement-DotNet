using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public bool Activate(int planId)
        {
            var plan = _unitOfWork.GetRepository<Plan>().GetById(planId);

            if(plan is null || HasActiveMemberships(planId))
                return false;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            return _unitOfWork.SaveChanges() > 0;
        }

        public IEnumerable<PlanViewModel> GetAllPlans()
        {
            var plans = _unitOfWork.GetRepository<Plan>().GetAll();

            if (plans is null || !plans.Any())
                return [];

            var mappedPlans = _mapper.Map<IEnumerable<Plan>,IEnumerable<PlanViewModel>>(plans);

            return mappedPlans;
        }

        public PlanViewModel? GetPlanById(int id)
        {
            var plan = _unitOfWork.GetRepository<Plan>().GetById(id);

            if (plan is null)
                return null;

            var mappedPlan = _mapper.Map<PlanViewModel>(plan);

            return mappedPlan;
        }

        public UpdatePlanViewModel? GetPlanToUpdate(int id)
        {
            var plan = _unitOfWork.GetRepository<Plan>().GetById(id);

            if (plan is null || plan.IsActive == false)
                return null;

            var mappedPlan = _mapper.Map<UpdatePlanViewModel>(plan);

            return mappedPlan;
        }

        public bool UpdatePlan(int id, UpdatePlanViewModel input)
        {
            var plan = _unitOfWork.GetRepository<Plan>().GetById(id);
            
            if (plan is null || HasActiveMemberships(id))
            return false;

            _mapper.Map(input, plan);

            _unitOfWork.GetRepository<Plan>().Update(plan);

            return _unitOfWork.SaveChanges() > 0; 
        }

        #region HelperMethods

        private bool HasActiveMemberships(int planId)
        {
            return _unitOfWork.GetRepository<Membership>().GetAll(x => x.PlanId == planId && x.Status == "Active").Any();
        }

        #endregion
    }
}
