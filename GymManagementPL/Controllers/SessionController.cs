using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    [Authorize]
    public class SessionController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public SessionController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task<IActionResult> Index()
        {
            var sessions = await _serviceManager.SessionService.GetAllSessionsAsync();
            return View(sessions);
        }
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesDropDown();
            await LoadTrainersDropDown();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel input)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesDropDown();
                await LoadTrainersDropDown();
                return View();
            }
            var result = await _serviceManager.SessionService.CreateSessionAsync(input);
            if (result)
            {
                TempData["successMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Failed to create session. Please verify trainer and category exist");
                return View(input);
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                TempData["errorMessage"] = "Id Of Session Can Not Be 0 Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var session = await _serviceManager.SessionService.GetSessionByIdAsync(id);
            if(session is null)
            {
                TempData["errorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["errorMessage"] = "Id Of Session Can Not Be 0 Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var session = await _serviceManager.SessionService.GetSessionToUpdateAsync(id);
            if (session == null)
            {
                TempData["errorMessage"] = "Session Can Not Be Updated";
                return RedirectToAction(nameof(Index));
            }
            await LoadTrainersDropDown();
            return View(session);
        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, UpdateSessionViewModel input)
        {
            if (!ModelState.IsValid)
            {
                await LoadTrainersDropDown();
                return View(input);
            }
            var result = await _serviceManager.SessionService.UpdateSessionAsync(id, input);
            if (result)
                TempData["successMessage"] = "Session Updated Successfully";
            else
                TempData["errorMessage"] = "Failed To Update Session (You can't update ongoing, completed or booked sessions)";

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var result = await _serviceManager.SessionService.RemoveSessionAsync(id);
            if (result)
                TempData["successMessage"] = "Session Deleted Successfully";
            else
                TempData["errorMessage"] = "Session Failed To Delete (You can't delete ongoing sessions or booked sessions)";
            return RedirectToAction(nameof(Index));

        }

        #region Helper Methods
        public async Task LoadCategoriesDropDown()
        {
            var categories = await _serviceManager.SessionService.GetCategoriesDropDownAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
        }
        public async Task LoadTrainersDropDown()
        {
            var trainers = await _serviceManager.SessionService.GetTrainerDropDownAsync();
            ViewBag.Trainers = new SelectList(trainers, "Id", "Name");
        }
        #endregion
    }
}
