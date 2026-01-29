using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementBLL.ViewModels;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<IEnumerable<MemberViewModel>> GetInactiveMembersAsync();
        Task<IEnumerable<MembershipViewModel>> GetActiveMembershipsAsync();
        Task<bool> CancelMembershipAsync(int memberId,int planId);
        Task<bool> CreateMembershipAsync(CreateMembershipViewModel input);
    }
}
