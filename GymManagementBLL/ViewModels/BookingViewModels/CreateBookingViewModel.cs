using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.ViewModels
{
    public class CreateBookingViewModel
    {
        public int MemberId { get; set; }
        public int SessionId { get; set; }
        public bool IsAttended { get; set; } = false;
    }
}
