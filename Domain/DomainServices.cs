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
            
            registerScoped(typeof(GetUserChatsUseCase));
            
            registerScoped(typeof(CreateGroupUseCase));
            registerScoped(typeof(CreateOrGetChatUseCase));
            registerScoped(typeof(GetChatMessagesUseCase));
            registerScoped(typeof(JoinGroupUseCase));
            registerScoped(typeof(LeaveGroupUseCase));
            registerScoped(typeof(SendMessageUseCase));
            registerScoped(typeof(GetChatMembersUseCase));
            registerScoped(typeof(GetChatPreviewUseCase));
            registerScoped(typeof(GetUserUseCase));
            registerScoped(typeof(SearchUsersUseCase));
        }
    }
}
