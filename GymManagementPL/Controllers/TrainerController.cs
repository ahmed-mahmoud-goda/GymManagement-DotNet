using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class TrainerController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public TrainerController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public IActionResult Index()
        {
            var trainers = _serviceManager.TrainerService.GetAllTrainers();
            return View(trainers);
        }
        public IActionResult TrainerDetails(int id)
        {
            var trainer = _serviceManager.TrainerService.GetTrainerDetails(id);
            if (trainer is null)
            {
                TempData["errorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult CreateTainer(CreateTrainerViewModel input)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataMissed", "Check Missing Data");
                return View(nameof(Create), input);
            }
            bool result = _serviceManager.TrainerService.CreateTrainer(input);
            if (result)
                TempData["successMessage"] = "Trainer Created Successfully";
            else
                TempData["errorMessage"] = "Trainer Failed To Create, Phone Number Or Email Already Exist";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult TrainerEdit(int id)
        {
            var trainer = _serviceManager.TrainerService.GetTrainerToUpdate(id);
            if (trainer is null)
            {
                TempData["errorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(trainer);
        }
        [HttpPost]
        public IActionResult TrainerEdit([FromRoute] int id, TrainerToUpdateViewModel input)
        {

            if (!ModelState.IsValid)
            {
                return View(input);
            }
            var result = _serviceManager.TrainerService.UpdateTrainerDetails(id,input);
            if (result)
                TempData["successMessage"] = "Trainer Updated Successfully";
            else
                TempData["errorMessage"] = "Trainer Failed To Update";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult Delete([FromForm] int id)
        {
            var result = _serviceManager.TrainerService.RemoveTrainer(id);
            if (result)
                TempData["successMessage"] = "Trainer Deleted Successfully";
            else
                TempData["errorMessage"] = "Trainer Failed To Delete, Could be due to upcoming sessions";
            return RedirectToAction(nameof(Index));

        }
    }
}
