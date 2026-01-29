using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementBLL.ViewModels;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel input);
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int id);
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync();
        Task<PlanViewModel?> GetPlanByIdAsync(int id);
        Task<bool> ActivateAsync(int planId);
    }
}
