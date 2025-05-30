using AutoMapper;
using Domain.Entities;
using Domain.Requests;
using Infrastructure.Identity;

namespace Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterRequest, AppUser>();
            CreateMap<AppUser, User>();
        }
    }
}
