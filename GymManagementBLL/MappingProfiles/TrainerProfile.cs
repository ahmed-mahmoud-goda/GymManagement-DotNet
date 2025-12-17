using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.ViewModels.TrainerViewModel;
using GymManagementDAL.Entities;

namespace GymManagementBLL.MappingProfiles
{
    public class TrainerProfile : Profile
    {
        public TrainerProfile()
        {
            CreateMap<CreateTrainerViewModel, Trainer>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                {
                    BuildingNumber = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City
                }));
            CreateMap<Trainer, TrainerViewModel>();
            CreateMap<Trainer, TrainerToUpdateViewModel>()
                .ForMember(dist => dist.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dist => dist.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dist => dist.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber));

            CreateMap<TrainerToUpdateViewModel, Trainer>()
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.Address.BuildingNumber = src.BuildingNumber;
                dest.Address.City = src.City;
                dest.Address.Street = src.Street;
                dest.UpdatedAt = DateTime.Now;
            });
        }
    }
}
