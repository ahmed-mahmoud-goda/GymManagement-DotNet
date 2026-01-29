using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.ViewModels
{
    public class CreateMembershipViewModel
    {
        [Required(ErrorMessage = "Member Is Required")]
        public int MemberId { get; set; }
        [Required(ErrorMessage = "Plan Is Required")]
        public int PlanId { get; set; }
    }
}
