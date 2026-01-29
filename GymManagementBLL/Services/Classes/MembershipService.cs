using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.Services.Specifications;
using GymManagementBLL.ViewModels;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Classes
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<bool> CancelMembershipAsync(int memberId, int planId)
        {
            var membershipSpecs = new MembershipWithFilterSpecification(false,memberId,planId);
            var membership = await _unitOfWork.GetRepository<Membership>().GetBySpecificationAsync(membershipSpecs);
            if(membership is null)
                return false;

            _unitOfWork.GetRepository<Membership>().Delete(membership);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateMembershipAsync(CreateMembershipViewModel input)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(input.PlanId);
            if(plan is null) return false;

            var membership = _mapper.Map<Membership>(input);
            membership.EndDate= DateTime.Now.AddDays(plan.DurationDays);

            await _unitOfWork.GetRepository<Membership>().AddAsync(membership);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MembershipViewModel>> GetActiveMembershipsAsync()
        {
            var membershipSpecs = new MembershipWithFilterSpecification(true);
            var memberships = await _unitOfWork.GetRepository<Membership>().GetAllAsync(membershipSpecs);
            if (memberships is null || !memberships.Any())
                return [];

            var membershipViewModel = _mapper.Map<IEnumerable<MembershipViewModel>>(memberships);

            return membershipViewModel;
        }

        public async Task<IEnumerable<MemberViewModel>> GetInactiveMembersAsync()
        {
            var memberSpecs = new MemberWithFilterSpecification(false);
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(memberSpecs);
            if (members is null || !members.Any())
                return [];

            var memberViewModels = _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);

            return memberViewModels;
        }
        
    }
}
