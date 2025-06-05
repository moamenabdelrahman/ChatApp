using Domain.Entities;
using Domain.IRepositories;
using Domain.Responses;

namespace Domain.UseCases
{
    public class GetChatPreviewUseCase
    {
        private readonly IChatRepository _chatRepository;

        public GetChatPreviewUseCase(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<Result<Chat>> Handle(int chatId)
        {
            var chat = await _chatRepository.GetChatPreview(chatId);
            if (chat is null)
                return Result<Chat>.Fail("Chat isn't found!");

            return Result<Chat>.Ok(chat);
        }
    }
}
