using GymManagementBLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public HomeController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task<IActionResult> Index()
        {
            var analytics = await _serviceManager.AnalyticsService.GetAnalyticsDataAsync();
            return View(analytics);
        }
    }
}
