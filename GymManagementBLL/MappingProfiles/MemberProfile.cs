using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.ViewModels.MemberViewModels;
using GymManagementDAL.Entities;

namespace GymManagementBLL.MappingProfiles
{
    public class MemberProfile : Profile
    {
        public MemberProfile()
        {
            CreateMap<CreateMemberViewModel, Member>()
                  .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                  {
                      BuildingNumber = src.BuildingNumber,
                      City = src.City,
                      Street = src.Street
                  })).ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.healthRecordViewModel));

            CreateMap<HealthRecordViewModel, HealthRecord>().ReverseMap();

            CreateMap<Member, MemberViewModel>()
           .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"));

            CreateMap<Member, MemberToUpdateViewModel>()
            .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street));

            CreateMap<MemberToUpdateViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
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
