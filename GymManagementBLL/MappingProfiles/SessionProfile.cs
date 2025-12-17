using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementDAL.Entities;

namespace GymManagementBLL.MappingProfiles
{
    public class SessionProfile : Profile
    {
        public SessionProfile()
        {
            CreateMap<Session, SessionViewModel>()
                .ForMember(dest => dest.CategoryName, option => option.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.TrainerName, option => option.MapFrom(src => src.Trainer.Name))
                .ForMember(dest => dest.AvailableSlots, option => option.Ignore());

            CreateMap<CreateSessionViewModel, Session>();

            CreateMap<UpdateSessionViewModel, Session>().ReverseMap();
        }
    }
}
