using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    public class MemberController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public MemberController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public IActionResult Index()
        {
            var members = _serviceManager.MemberService.GetAllMembers();
            return View(members);
        }
        public IActionResult MemberDetails(int id)
        {
            var member = _serviceManager.MemberService.GetMemberDetails(id);
            if (member is null)
            {
                TempData["errorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        public IActionResult HealthRecordDetails(int id)
        {
            var member = _serviceManager.MemberService.GetMemberHealthRecord(id);
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
        public IActionResult CreateMember(CreateMemberViewModel input)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataMissed", "Check Missing Data");
                return View(nameof(Create),input);
            }
            bool result = _serviceManager.MemberService.CreateMember(input);
            if (result)
                TempData["successMessage"] = "Member Created Successfully";
            else
                TempData["errorMessage"] = "Member Failed To Create, Phone Number Or Email Already Exist";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult MemberEdit(int id)
        {
            var member = _serviceManager.MemberService.GetMemberToUpdate(id);
            if(member is null)
            {
                TempData["errorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }
        [HttpPost]
        public IActionResult MemberEdit([FromRoute] int id, MemberToUpdateViewModel input)
        {

            if (!ModelState.IsValid)
            {
                return View(input);
            }
            var result = _serviceManager.MemberService.UpdateMemberDetails(id,input);
            if (result)
                TempData["successMessage"] = "Member Updated Successfully";
            else
                TempData["errorMessage"] = "Member Failed To Update";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult Delete([FromForm] int id)
        {
            var result = _serviceManager.MemberService.RemoveMember(id);
            if (result)
                TempData["successMessage"] = "Member Deleted Successfully";
            else
                TempData["errorMessage"] = "Member Failed To Delete";
            return RedirectToAction(nameof(Index));

        }

    }
}
