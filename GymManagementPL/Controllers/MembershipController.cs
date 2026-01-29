using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    public class MembershipController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public MembershipController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task<IActionResult> Index()
        {
            var memberships =  await _serviceManager.MembershipService.GetActiveMembershipsAsync();
            return View(memberships);
        }
        public async Task<IActionResult> Create()
        {
            await LoadMembersDropDown();
            await LoadPlansDropDown();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateMembershipViewModel input)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataMissed", "Check Missing Data");
                await LoadMembersDropDown();
                await LoadPlansDropDown();
                return View(input);
            }
            var result = await _serviceManager.MembershipService.CreateMembershipAsync(input);
            if (result)
            {
                TempData["successMessage"] = "Membership Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Failed to create Membership");
                return View(input);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Cancel(int memberId,int planId)
        {
            var result = await _serviceManager.MembershipService.CancelMembershipAsync(memberId,planId);
            if (result)
                TempData["successMessage"] = "Membership Deleted Successfully";
            else
                TempData["errorMessage"] = "Membership Failed To Delete";
            return RedirectToAction(nameof(Index));

        }

        #region Helper Method
        public async Task LoadMembersDropDown()
        {
            var members = await _serviceManager.MembershipService.GetInactiveMembersAsync();
            ViewBag.Members = new SelectList(members, "Id", "Name");
        }
        public async Task LoadPlansDropDown()
        {
            var plans = await _serviceManager.PlanService.GetAllPlansAsync();
            ViewBag.Plans = new SelectList(plans, "Id", "Name");
        }
        #endregion
    }
}
