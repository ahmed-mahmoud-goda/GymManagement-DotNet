using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.ViewModels;
using GymManagementDAL.Entities;

namespace GymManagementBLL.MappingProfiles
{
    public class MembershipProfile : Profile
    {
        public MembershipProfile()
        {
            CreateMap<Membership, MembershipViewModel>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Member.Name))
                    .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan.Name))
                    .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<CreateMembershipViewModel, Membership>();
        }
    }
}
