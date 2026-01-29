using System.Threading.Tasks;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    [Authorize]
    public class PlanController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public PlanController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task<IActionResult> Index()
        {
            var plans = await _serviceManager.PlanService.GetAllPlansAsync();
            return View(plans);
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                TempData["errorMessage"] = "Id Of Plan Can Not Be 0 Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var plan = await _serviceManager.PlanService.GetPlanByIdAsync(id);
            if(plan == null)
            {
                TempData["errorMessage"] = "Plan Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);

        }
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["errorMessage"] = "Id Of Plan Can Not Be 0 Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var plan = await _serviceManager.PlanService.GetPlanToUpdateAsync(id);
            if (plan == null)
            {
                TempData["errorMessage"] = "Plan Can Not Be Updated";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, UpdatePlanViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }
            var result = await _serviceManager.PlanService.UpdatePlanAsync(id, input);
            if (result)
                TempData["successMessage"] = "Plan Updated Successfully";
            else
                TempData["errorMessage"] = "Failed To Update Plan";

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _serviceManager.PlanService.ActivateAsync(id);
            if (result)
                TempData["successMessage"] = "Plan Status Changed";
            else
                TempData["errorMessage"] = "Failed To Change Plan Status (Check if there is a member with this Plan)";

            return RedirectToAction(nameof(Index));
        }
    }
}
