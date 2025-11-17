using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberViewModels;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<MemberViewModel> GetAllMembers()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll().ToList() ?? [];

            if (members is null || !members.Any())
                return [];

            var memberViewModels = members.Select(x => new MemberViewModel
            {
                Id = x.Id,
                Photo = x.Photo,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone,
                DateOfBirth = x.DateOfBirth.ToShortDateString(),
                Gender = x.Gender.ToString(),
            });

            return memberViewModels;
        }

        public bool CreateMember(CreateMemberViewModel model)
        {
            try
            {
                if(IsEmailExists(model.Email)||IsPhoneExists(model.Phone))
                    return false;

                var member = new Member
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Address = new Address
                    {
                        BuildingNumber = model.BuildingNumber,
                        City = model.City,
                        Street = model.Street,
                    },
                    HealthRecord = new HealthRecord
                    {
                        Height = model.healthRecordViewModel.Height,
                        Weight = model.healthRecordViewModel.Weight,
                        BloodType = model.healthRecordViewModel.BloodType,
                        Note = model.healthRecordViewModel.Note,
                    }
                };

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

            var memberViewModel = new MemberViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Photo = member.Photo,
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Gender = member.Gender.ToString(),
                Address = FormatAddress(member.Address),

            };

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

            return new HealthRecordViewModel
            {
                Height = memberHealthRecord.Height,
                Weight = memberHealthRecord.Weight,
                BloodType = memberHealthRecord.BloodType,
                Note = memberHealthRecord.Note,
            };

            
        }

        public MemberToUpdateViewModel? GetMemberToUpdate(int memberId)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            
            if(member is null) return null;

            var memberToUpdate = new MemberToUpdateViewModel
            {
                Photo = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                BuildingNumber = member.Address.BuildingNumber,
                City = member.Address.City,
                Street = member.Address.Street,
            };

            return memberToUpdate;
        }

        public bool UpdateMemberDetails(int memberId,MemberToUpdateViewModel model)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);

            if(member is null) return false;

            if (IsEmailExists(model.Email) || IsPhoneExists(model.Phone))
                return false;

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.UpdatedAt = DateTime.Now;

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

        private string FormatAddress(Address address)
        {
            if (address is null)
                return "N/A";
            else
                return $"{address.BuildingNumber}, {address.Street}, {address.City}";
        }

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
