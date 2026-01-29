using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementBLL.ViewModels.BookingViewModels;

namespace GymManagementBLL.ViewModels
{
    public class MemberSessionViewModel
    {
        public List<BookingViewModel> UpcomingSessions { get; set; } = new();
        public List<BookingViewModel> OngoingSessions { get; set; } = new();
    }
}
