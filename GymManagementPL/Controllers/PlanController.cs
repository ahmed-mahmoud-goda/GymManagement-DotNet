using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    public class PlanController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public PlanController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public IActionResult Index()
        {
            var plans =  _serviceManager.PlanService.GetAllPlans();
            return View(plans);
        }
        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["errorMessage"] = "Id Of Plan Can Not Be 0 Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var plan = _serviceManager.PlanService.GetPlanById(id);
            if(plan == null)
            {
                TempData["errorMessage"] = "Plan Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);

        }
        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                TempData["errorMessage"] = "Id Of Plan Can Not Be 0 Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var plan = _serviceManager.PlanService.GetPlanToUpdate(id);
            if (plan == null)
            {
                TempData["errorMessage"] = "Plan Can Not Be Updated";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
        [HttpPost]
        public IActionResult Edit([FromRoute] int id, UpdatePlanViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }
            var result = _serviceManager.PlanService.UpdatePlan(id, input);
            if (result)
                TempData["successMessage"] = "Plan Updated Successfully";
            else
                TempData["errorMessage"] = "Failed To Update Plan";

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult Activate(int id)
        {
            var result = _serviceManager.PlanService.Activate(id);
            if (result)
                TempData["successMessage"] = "Plan Status Changed";
            else
                TempData["errorMessage"] = "Failed To Change Plan Status";

            return RedirectToAction(nameof(Index));
        }
    }
}
