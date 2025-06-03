using AutoMapper;
using Domain.Entities;
using Domain.Requests;
using Infrastructure.Data;
using Infrastructure.Identity;

namespace Infrastructure.MapperProfiles
{
    public class InfraMapperProfile : Profile
    {
        public InfraMapperProfile()
        {
            CreateMap<RegisterRequest, AppUser>();
            CreateMap<AppUser, User>();
            CreateMap<User, AppUser>();

            CreateMap<MessageEntity, Message>();
            CreateMap<Message, MessageEntity>()
                    .ForMember(me => me.SenderId, opt => opt.MapFrom(m => m.Sender.Id));

            CreateMap<ChatEntity, Chat>();
            CreateMap<Chat, ChatEntity>();
        }
    }
}
