using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.ViewModels;
using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagementDAL.Entities;

namespace GymManagementBLL.MappingProfiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Session, BookingViewModel>()
            .ForMember(d => d.CategoryName, s => s.MapFrom(s => s.Category.CategoryName))
            .ForMember(d => d.TrainerName, s => s.MapFrom(s => s.Trainer.Name));

            CreateMap<Booking, MemberBookingViewModel>()
                .ForMember(d => d.MemberName, s => s.MapFrom(s => s.Member.Name));

            CreateMap<Member, MemberSelectViewModel>();
            CreateMap<CreateBookingViewModel, Booking>();
        }
    }
}
