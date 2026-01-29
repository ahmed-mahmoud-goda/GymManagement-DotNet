using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementBLL.ViewModels;
using GymManagementBLL.ViewModels.BookingViewModels;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<MemberSessionViewModel> GetSessionsAsync();
        Task<IEnumerable<MemberBookingViewModel>> GetMembersBookingsaAsync(int sessionId);
        Task<IEnumerable<MemberSelectViewModel>> GetMemberDropDownAsync(int sessionId);
        Task<bool> CreateBookingAsync(CreateBookingViewModel input);
        Task<bool> CancelBookingAsync(int memberId,int sessionId);
        Task<bool> AttendSessionAsync(int memberId,int sessionId);
    }
}
