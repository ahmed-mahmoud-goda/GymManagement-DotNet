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
        public async Task<IActionResult> Index()
        {
            var trainers = await _serviceManager.TrainerService.GetAllTrainersAsync();
            return View(trainers);
        }
        public async Task<IActionResult> TrainerDetails(int id)
        {
            var trainer = await _serviceManager.TrainerService.GetTrainerDetailsAsync(id);
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
        public async Task<IActionResult> CreateTainer(CreateTrainerViewModel input)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataMissed", "Check Missing Data");
                return View(nameof(Create), input);
            }
            bool result = await _serviceManager.TrainerService.CreateTrainerAsync(input);
            if (result)
                TempData["successMessage"] = "Trainer Created Successfully";
            else
                TempData["errorMessage"] = "Trainer Failed To Create, Phone Number Or Email Already Exist";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> TrainerEdit(int id)
        {
            var trainer = await _serviceManager.TrainerService.GetTrainerToUpdateAsync(id);
            if (trainer is null)
            {
                TempData["errorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(trainer);
        }
        [HttpPost]
        public async Task<IActionResult> TrainerEdit([FromRoute] int id, TrainerToUpdateViewModel input)
        {

            if (!ModelState.IsValid)
            {
                return View(input);
            }
            var result = await _serviceManager.TrainerService.UpdateTrainerDetailsAsync(id,input);
            if (result)
                TempData["successMessage"] = "Trainer Updated Successfully";
            else
                TempData["errorMessage"] = "Trainer Failed To Update";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var result = await _serviceManager.TrainerService.RemoveTrainerAsync(id);
            if (result)
                TempData["successMessage"] = "Trainer Deleted Successfully";
            else
                TempData["errorMessage"] = "Trainer Failed To Delete, Could be due to upcoming sessions";
            return RedirectToAction(nameof(Index));

        }
    }
}
