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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace GymManagementBLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private const string cacheKey = "members";
        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync()
        {
            try
            {
                var cached = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cached))
                    return JsonSerializer.Deserialize<IEnumerable<MemberViewModel>>(cached)!;
            }
            catch
            {
                Console.WriteLine("Redis is Unavailable");
            }
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();

            var memberViewModels = _mapper.Map<IEnumerable<MemberViewModel>>(members);

            try
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(memberViewModels), cacheOptions);
            }
            catch { }

            return memberViewModels;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model)
        {
            try
            {
                if(await IsEmailOrPhoneExistsAsync(model.Email,model.Phone))
                    return false;

                var member = _mapper.Map<Member>(model);

                member.Photo = await GetPhoto(model.PhotoFile);

                await _unitOfWork.GetRepository<Member>().AddAsync(member);

                await _unitOfWork.SaveChangesAsync();
                try
                {
                    await _cache.RemoveAsync(cacheKey);
                }
                catch { }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId);

            if (member is null)
                return null;

            var memberViewModel = _mapper.Map<MemberViewModel>(member);

            var membershipSpecs = new MembershipWithFilterSpecification(true,member.Id);

            var activeMembership = await _unitOfWork.GetRepository<Membership>().GetBySpecificationAsync(membershipSpecs);
            
            if(activeMembership is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId);

                memberViewModel.PlanName = activePlan?.Name;
                memberViewModel.MembershipStartDate = activeMembership.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEndDate = activeMembership.EndDate.ToShortDateString();
            }
            return memberViewModel;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId)
        {
            var memberHealthRecord = await _unitOfWork.GetRepository<HealthRecord>().GetByIdAsync(memberId);

            if(memberHealthRecord is null) return null;

            return _mapper.Map<HealthRecordViewModel>(memberHealthRecord);
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId);
            
            if(member is null) return null;

            var memberToUpdate = _mapper.Map<MemberToUpdateViewModel>(member);

            return memberToUpdate;
        }

        public async Task<bool> UpdateMemberDetailsAsync(int memberId,MemberToUpdateViewModel model)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId);

            if(member is null) return false;

            if (await IsEmailOrPhoneExistsAsync(model.Email,model.Phone,memberId))
                return false;

            var newPhoto = await GetPhoto(model.PhotoFile);

            if (newPhoto != null)
            {
                DeletePhoto(member.Photo);
                member.Photo = newPhoto;
            }

            _mapper.Map(model, member);

            _unitOfWork.GetRepository<Member>().Update(member);
            await _unitOfWork.SaveChangesAsync();
            try
            {
                await _cache.RemoveAsync(cacheKey);
            }
            catch { }
            return true;
        }

        public async Task<bool> RemoveMemberAsync(int memberId)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId);

            if(member is null) return false;

            var bookingSpecs = new BookingWithFilterSpecification(memberId);
            var activeBooking = await _unitOfWork.GetRepository<Booking>().GetAllAsync(bookingSpecs);

            if (activeBooking.Any())
            {
                return false;
            }

            var membershipSpecs = new MembershipWithFilterSpecification(false,memberId);
            var memberships = await _unitOfWork.GetRepository<Membership>().GetAllAsync(membershipSpecs);

            try
            {
                if (memberships.Any())
                {
                    _unitOfWork.GetRepository<Membership>().DeleteRange(memberships);
                }
                DeletePhoto(member.Photo);
                _unitOfWork.GetRepository<Member>().Delete(member);
                await _unitOfWork.SaveChangesAsync();
                try
                {
                    await _cache.RemoveAsync(cacheKey);
                }
                catch { }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Helper Methods

        private async Task<bool> IsEmailOrPhoneExistsAsync(string email, string phone,int? id = null)
        {
            var memberSpecs = new MemberWithFilterSpecification(email,phone,id);
            var existingMember = await _unitOfWork.GetRepository<Member>().GetBySpecificationAsync(memberSpecs);
            return existingMember != null;
        }
        private async Task<string?> GetPhoto(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/members");
            Directory.CreateDirectory(folder);

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            string ext = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(ext))
                throw new Exception("Invalid file type");

            string fileName = Guid.NewGuid() + ext;
            string fullPath = Path.Combine(folder, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/images/members/" + fileName;
        }
        private void DeletePhoto(string? photo)
        {
            if (string.IsNullOrEmpty(photo))
                return;

            string fullPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                photo.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
            );

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        #endregion
    }
}
