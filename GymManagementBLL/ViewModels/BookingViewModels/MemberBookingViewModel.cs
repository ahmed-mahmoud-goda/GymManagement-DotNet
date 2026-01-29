using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.ViewModels
{
    public class MemberBookingViewModel
    {
        public int MemberId { get; set; }
        public int SessionId { get; set; }
        public string MemberName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsAttended { get; set; }
    }
}
