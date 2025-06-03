using Domain.IRepositories;
using Infrastructure.Repositories;

namespace Infrastructure
{
    public static class InfrastructureServices
    {
        public static void Register(Action<Type, Type> registerScoped)
        {
            registerScoped(typeof(IUserRepository), typeof(UserRepository));
            registerScoped(typeof(IChatRepository), typeof(ChatRepository));
        }
    }
}
