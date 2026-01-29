using System.Threading.Tasks;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    public class BookingController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public BookingController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task<IActionResult> Index()
        {
            var sessions = await _serviceManager.BookingService.GetSessionsAsync();
            return View(sessions);
        }
        public async Task<IActionResult> GetMembersForUpcomingSession(int id)
        {
            var bookings = await _serviceManager.BookingService.GetMembersBookingsaAsync(id);
            ViewBag.SessionId = id;
            return View(bookings);
        }
        public async Task<IActionResult> GetMembersForOngoingSession(int id)
        {
            var bookings = await _serviceManager.BookingService.GetMembersBookingsaAsync(id);
            ViewBag.SessionId = id;
            return View(bookings);
        }
        public async Task<IActionResult> Create(int id)
        {
            await LoadMembersDropDown(id);
            ViewBag.SessionId = id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel input)
        {
            if (!ModelState.IsValid)
            {
                await LoadMembersDropDown(input.SessionId);
                ViewBag.SessionId = input.SessionId;
                return View();
            }
            var result = await _serviceManager.BookingService.CreateBookingAsync(input);
            if (result)
            {
                TempData["successMessage"] = "New Booking Created Successfully";
                return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = input.SessionId });
            }
            else
            {
                TempData["errorMessage"] = "Failed To Create New Booking (Check If There Are Available Slots)";
                await LoadMembersDropDown(input.SessionId);
                ViewBag.SessionId = input.SessionId;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Cancel(int memberId, int sessionId)
        {
            var result = await _serviceManager.BookingService.CancelBookingAsync(memberId, sessionId);
            if (result)
            {
                TempData["successMessage"] = "Booking Deleted Successfully";
                return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = sessionId });
            }
            else
            {
                TempData["errorMessage"] = "Failed To Cancel Booking";
                return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = sessionId });
            }
        }
        public async Task<IActionResult> Attend(int memberId, int sessionId)
        {
            var result = await _serviceManager.BookingService.AttendSessionAsync(memberId, sessionId);
            if (result)
            {
                TempData["successMessage"] = "Session Attended Successfully";
                return RedirectToAction(nameof(GetMembersForOngoingSession), new { id = sessionId });
            }
            else
            {
                TempData["errorMessage"] = "Failed To Attend Session";
                return RedirectToAction(nameof(GetMembersForOngoingSession), new { id = sessionId });
            }
        }

        #region Helper Methods
        public async Task LoadMembersDropDown(int id)
        {
            var members = await _serviceManager.BookingService.GetMemberDropDownAsync(id);
            ViewBag.Members = new SelectList(members, "Id", "Name");
        }
        #endregion
    }
}
