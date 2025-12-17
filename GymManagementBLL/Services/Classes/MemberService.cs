using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberViewModels;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<MemberViewModel> GetAllMembers()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll().ToList() ?? [];

            if (members is null || !members.Any())
                return [];

            var memberViewModels = _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);

            return memberViewModels;
        }

        public bool CreateMember(CreateMemberViewModel model)
        {
            try
            {
                if(IsEmailExists(model.Email)||IsPhoneExists(model.Phone))
                    return false;

                var member = _mapper.Map<Member>(model);

                _unitOfWork.GetRepository<Member>().Add(member);

                _unitOfWork.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public MemberViewModel? GetMemberDetails(int memberId)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);

            if (member is null)
                return null;

            var memberViewModel = _mapper.Map<MemberViewModel>(member);

            var activeMembership = _unitOfWork.GetRepository<Membership>().GetAll(x => x.MemberId == memberId && x.Status == "Active").FirstOrDefault();
            
            if(activeMembership is not null)
            {
                var activePlan = _unitOfWork.GetRepository<Plan>().GetById(activeMembership.PlanId);

                memberViewModel.PlanName = activePlan?.Name;
                memberViewModel.MembershipStartDate = activeMembership.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEndDate = activeMembership.EndDate.ToShortDateString();
            }
            return memberViewModel;
        }

        public HealthRecordViewModel? GetMemberHealthRecord(int memberId)
        {
            var memberHealthRecord = _unitOfWork.GetRepository<HealthRecord>().GetById(memberId);

            if(memberHealthRecord is null) return null;

            return _mapper.Map<HealthRecordViewModel>(memberHealthRecord);
        }

        public MemberToUpdateViewModel? GetMemberToUpdate(int memberId)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            
            if(member is null) return null;

            var memberToUpdate = _mapper.Map<MemberToUpdateViewModel>(member);

            return memberToUpdate;
        }

        public bool UpdateMemberDetails(int memberId,MemberToUpdateViewModel model)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);

            if(member is null) return false;

            if (IsEmailExists(model.Email) || IsPhoneExists(model.Phone))
                return false;

            _mapper.Map(model, member);

            _unitOfWork.GetRepository<Member>().Update(member);
            _unitOfWork.SaveChanges();

            return true;
        }

        public bool RemoveMember(int memberId)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);

            if(member is null) return false;

            var activeBooking = _unitOfWork.GetRepository<Booking>().GetAll(x=>x.MemberId == memberId && x.Session.StartDate > DateTime.Now);

            if (activeBooking.Any())
            {
                return false;
            }

            var memberships = _unitOfWork.GetRepository<Membership>().GetAll(x => x.MemberId == memberId);

            try
            {
                if (memberships.Any())
                {
                    _unitOfWork.GetRepository<Membership>().DeleteRange(memberships);
                }
                _unitOfWork.GetRepository<Member>().Delete(member);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Helper Methods

        private bool IsEmailExists(string email)
        {
            var existingMember = _unitOfWork.GetRepository<Member>().GetAll(x => x.Email == email);
            return existingMember is not null && existingMember.Any();
        }
        private bool IsPhoneExists(string phone)
        {
            var existingMember = _unitOfWork.GetRepository<Member>().GetAll(x => x.Phone == phone);
            return existingMember is not null && existingMember.Any();
        }
        #endregion
    }
}
