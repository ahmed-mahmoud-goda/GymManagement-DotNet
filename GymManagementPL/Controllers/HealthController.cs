using System.Threading.Tasks;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    public class HealthController : Controller
    {
        [HttpGet("/health")]
        public IActionResult Health()
        {
            return Ok("Healthy");
        }
    }
}
