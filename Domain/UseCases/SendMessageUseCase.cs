using Domain.Entities;
using Domain.IRepositories;
using Domain.Requests;
using Domain.RequestsValidators;
using Domain.Responses;

namespace Domain.UseCases
{
    public class SendMessageUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly SendMessageValidator _validator;

        public SendMessageUseCase(IUserRepository userRepository,
                                  IChatRepository chatRepository,
                                  SendMessageValidator validator)
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _validator = validator;
        }

        public async Task<Result<Message>> Handle(SendMessageRequest request)
        {
            var validation = _validator.Validate(request);

            if (!validation.IsValid)
                return Result<Message>.Fail(validation.Errors.Select(e => e.ErrorMessage).ToList());

            var user = await _userRepository.GetUserByUserName(request.SenderUserName);
            var chat = await _chatRepository.GetChatById(request.ChatId);

            if (user is null || chat is null)
                return Result<Message>.Fail("Some of the provided entities aren't found!");

            if (!(await _chatRepository.IsInChat(user, chat)))
                return Result<Message>.Fail("You aren't a member of this chat!");

            var message = new Message()
            {
                ChatId = chat.Id,
                Sender = user,
                Content = request.Content,
                TimeStamp = request.TimeStamp
            };

            var result = await _chatRepository.SendMessage(message);

            return result;
        }
    }
}
