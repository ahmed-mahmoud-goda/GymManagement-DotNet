using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    public class SessionController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public SessionController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public IActionResult Index()
        {
            var sessions = _serviceManager.SessionService.GetAllSessions();
            return View(sessions);
        }
        public IActionResult Create()
        {
            LoadCategoriesDropDown();
            LoadTrainersDropDown();
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateSessionViewModel input)
        {
            if (!ModelState.IsValid)
            {
                LoadCategoriesDropDown();
                LoadTrainersDropDown();
                return View();
            }
            var result = _serviceManager.SessionService.CreateSession(input);
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
        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["errorMessage"] = "Id Of Session Can Not Be 0 Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var session = _serviceManager.SessionService.GetSessionById(id);
            if(session is null)
            {
                TempData["errorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }
        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                TempData["errorMessage"] = "Id Of Session Can Not Be 0 Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var session = _serviceManager.SessionService.GetSessionToUpdate(id);
            if (session == null)
            {
                TempData["errorMessage"] = "Session Can Not Be Updated";
                return RedirectToAction(nameof(Index));
            }
            LoadTrainersDropDown();
            return View(session);
        }
        [HttpPost]
        public IActionResult Edit([FromRoute] int id, UpdateSessionViewModel input)
        {
            if (!ModelState.IsValid)
            {
                LoadTrainersDropDown();
                return View(input);
            }
            var result = _serviceManager.SessionService.UpdateSession(id, input);
            if (result)
                TempData["successMessage"] = "Session Updated Successfully";
            else
                TempData["errorMessage"] = "Failed To Update Session (You can't update ongoing and completed sessions)";

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult Delete([FromForm] int id)
        {
            var result = _serviceManager.SessionService.RemoveSession(id);
            if (result)
                TempData["successMessage"] = "Session Deleted Successfully";
            else
                TempData["errorMessage"] = "Session Failed To Delete (You can't delete ongoing sessions)";
            return RedirectToAction(nameof(Index));

        }

        #region Helper Methods
        public void LoadCategoriesDropDown()
        {
            var categories = _serviceManager.SessionService.GetCategoriesDropDown();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
        }
        public void LoadTrainersDropDown()
        {
            var trainers = _serviceManager.SessionService.GetTrainerDropDown();
            ViewBag.Trainers = new SelectList(trainers, "Id", "Name");
        }
        #endregion
    }
}
