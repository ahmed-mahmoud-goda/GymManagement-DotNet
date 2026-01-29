using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.ViewModels.BookingViewModels
{
    public class BookingViewModel
    {
        public int Id { get; set; }

        public string CategoryName { get; set; } = null!;
        public string Description { get; set; } = null!;

        public string TrainerName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Capacity { get; set; }
        public int AvailableSlots { get; set; }

        public string Date => $"{StartDate:MMM dd , yyyy}";
        public TimeSpan Duration => (EndDate - StartDate);
        public string TimeRangeDisplay => $"{StartDate:hh:mm tt} - {EndDate:hh:mm tt}";
        public string CapacityText => $"{AvailableSlots}/{Capacity} slots";
    }
}
