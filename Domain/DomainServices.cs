using Domain.UseCases;

namespace Domain
{
    public static class DomainServices
    {
        public static void Register(Action<Type> registerScoped)
        {
            registerScoped(typeof(RegisterUseCase));
            registerScoped(typeof(LoginUseCase));
            registerScoped(typeof(LogoutUseCase));
        }
    }
}
