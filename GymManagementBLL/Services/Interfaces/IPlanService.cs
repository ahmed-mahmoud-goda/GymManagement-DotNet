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
        bool UpdatePlan(int id, UpdatePlanViewModel input);
        UpdatePlanViewModel? GetPlanToUpdate(int id);
        IEnumerable<PlanViewModel> GetAllPlans();
        PlanViewModel? GetPlanById(int id);
        bool Activate(int planId);
    }
}
