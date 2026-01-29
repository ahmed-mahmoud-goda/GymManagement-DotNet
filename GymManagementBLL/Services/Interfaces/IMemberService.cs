using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementBLL.ViewModels;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<bool> CreateMemberAsync(CreateMemberViewModel model);
        Task<bool> UpdateMemberDetailsAsync(int memberId, MemberToUpdateViewModel model);
        Task<bool> RemoveMemberAsync(int memberId);
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync();
        Task<MemberViewModel?> GetMemberDetailsAsync(int memberId);
        Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId);
        Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId);
    }
}
