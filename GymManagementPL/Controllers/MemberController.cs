using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MemberController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public MemberController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task<IActionResult> Index()
        {
            var members = await _serviceManager.MemberService.GetAllMembersAsync();
            return View(members);
        }
        public async Task<IActionResult> MemberDetails(int id)
        {
            var member = await _serviceManager.MemberService.GetMemberDetailsAsync(id);
            if (member is null)
            {
                TempData["errorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        public async Task<IActionResult> HealthRecordDetails(int id)
        {
            var member = await _serviceManager.MemberService.GetMemberHealthRecordAsync(id);
            if (member is null)
            {
                TempData["errorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> CreateMember(CreateMemberViewModel input)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataMissed", "Check Missing Data");
                return View(nameof(Create),input);
            }
            bool result = await _serviceManager.MemberService.CreateMemberAsync(input);
            if (result)
                TempData["successMessage"] = "Member Created Successfully";
            else
                TempData["errorMessage"] = "Member Failed To Create, Phone Number Or Email Already Exist";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> MemberEdit(int id)
        {
            var member = await _serviceManager.MemberService.GetMemberToUpdateAsync(id);
            if(member is null)
            {
                TempData["errorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }
        [HttpPost]
        public async Task<IActionResult> MemberEdit(int id, MemberToUpdateViewModel input)
        {

            if (!ModelState.IsValid)
            {
                return View(input);
            }
            var result = await _serviceManager.MemberService.UpdateMemberDetailsAsync(id,input);
            if (result)
                TempData["successMessage"] = "Member Updated Successfully";
            else
                TempData["errorMessage"] = "Member Failed To Update";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var result = await _serviceManager.MemberService.RemoveMemberAsync(id);
            if (result)
                TempData["successMessage"] = "Member Deleted Successfully";
            else
                TempData["errorMessage"] = "Member Failed To Delete";
            return RedirectToAction(nameof(Index));

        }

    }
}
