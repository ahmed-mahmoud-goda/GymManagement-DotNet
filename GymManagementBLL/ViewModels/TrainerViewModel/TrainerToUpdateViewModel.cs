using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Entities.Enum;

namespace GymManagementBLL.ViewModels.TrainerViewModel
{
    public class TrainerToUpdateViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        [RegularExpression(@"^(010|011|012|015)\d{8}", ErrorMessage = "Phone Number must be a valid Egyption Number")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Building Number is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Building Number should be greater than 0")]
        public int BuildingNumber { get; set; }
        [Required(ErrorMessage = "City is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "City must contain only letters and spaces")]
        public string City { get; set; } = null!;
        [Required(ErrorMessage = "Street is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Street must be between 2 and 150 characters")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Street must contain only letters and spaces")]
        public string Street { get; set; } = null!;
        [Required(ErrorMessage = "Speciality is required")]
        public string Specialities { get; set; } = null!;
    }
}
