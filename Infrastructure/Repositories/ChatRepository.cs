using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepositories;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public ChatRepository(AppDbContext appDbContext,
                              IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<Result> AddToGroup(User user, Chat chat)
        {
            var chatEntity = await _appDbContext.Chats.FirstAsync(c => c.Id == chat.Id);

            var appUser = await _appDbContext.Users.FirstAsync(u => u.Id == user.Id);

            chatEntity.Members.Add(appUser);

            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
                return Result.Ok();

            return Result.Fail();
        }

        public async Task<Result<Chat>> CreateChat(Chat chat)
        {
            var chatEntity = _mapper.Map<ChatEntity>(chat);

            chatEntity.Members = chat.Members.Select<User, AppUser>(u =>
                _appDbContext.Users.First(au => au.Id == u.Id)
            ).ToList();

            _appDbContext.Chats.Add(chatEntity);

            var result = await _appDbContext.SaveChangesAsync();

            if(result > 0)
            {
                chat.Id = chatEntity.Id;
                return Result<Chat>.Ok(chat);
            }

            return Result<Chat>.Fail();
        }

        public async Task<Chat> GetChatById(int chatId)
        {
            var chatEntity = await _appDbContext.Chats.FirstOrDefaultAsync(c => c.Id == chatId);

            if (chatEntity is null)
                return null;

            var chat = _mapper.Map<Chat>(chatEntity);
            return chat;
        }

        public async Task<List<User>> GetChatMembers(Chat chat)
        {
            var chatEntity = await _appDbContext.Chats
                                .Include(c => c.Members)
                                .FirstAsync(c => c.Id == chat.Id).ConfigureAwait(false);

            return _mapper.Map<List<User>>(chatEntity.Members);
        }

        public async Task<List<Message>> GetChatMessages(Chat chat)
        {
            var messageEntities = await _appDbContext.Messages
                                .Include(msg => msg.Sender)
                                .Where(msg => msg.ChatId == chat.Id)
                                .OrderBy(msg => msg.TimeStamp)
                                .ToListAsync().ConfigureAwait(false);

            return _mapper.Map<List<Message>>(messageEntities);
        }

        public async Task<Chat> GetChatPreview(int chatId)
        {
            var chat = await _appDbContext.Chats
                            .Include(c => c.Messages)
                            .ThenInclude(msg => msg.Sender)
                            .FirstOrDefaultAsync(c => c.Id == chatId)
                            .ConfigureAwait(false);

            if (chat is null)
                return null;

            chat = new ChatEntity
            {
                Id = chat.Id,
                Name = chat.Name,
                Messages = chat.Messages.OrderByDescending(msg => msg.TimeStamp).Take(1).ToList(),
                Type = chat.Type
            };

            return _mapper.Map<Chat>(chat);
        }

        public async Task<Chat> GetPrivateChat(User user1, User user2)
        {
            var chatEntity = await _appDbContext.Chats
                            .Where(c => c.Type == ChatType.Private)
                            .Where(c => c.Members.Any(m => m.Id == user1.Id))
                            .Where(c => c.Members.Any(m => m.Id == user2.Id))
                            .FirstOrDefaultAsync().ConfigureAwait(false);

            if (chatEntity is null)
                return null;

            return _mapper.Map<Chat>(chatEntity);
        }

        public async Task<List<Chat>> GetUserChatsPreview(User user)
        {
            var chats = await _appDbContext.Chats
                            .Include(c => c.Members)
                            .Include(c => c.Messages)
                            .ThenInclude(msg => msg.Sender)
                            .Where(c => c.Members.Any(m => m.Id == user.Id))
                            .ToListAsync().ConfigureAwait(false);

            chats = chats.Select(c => new ChatEntity
            {
                Id = c.Id,
                Name = c.Name,
                Messages = c.Messages.OrderByDescending(msg => msg.TimeStamp).Take(1).ToList(),
                Type = c.Type
            }).ToList();

            return _mapper.Map<List<Chat>>(chats);
        }

        public async Task<bool> IsInChat(User user, Chat chat)
        {
            var result = await _appDbContext.Chats
                            .Include(c => c.Members)
                            .Where(c => c.Id == chat.Id)
                            .Select<ChatEntity, bool>(c => c.Members.Any(m => m.Id == user.Id))
                            .FirstAsync().ConfigureAwait(false);

            return result;
        }

        public async Task<Result> RemoveFromGroup(User user, Chat chat)
        {
            var appUser = await _appDbContext.Users.FirstAsync(u => u.Id == user.Id);
            
            var chatEntity = await _appDbContext.Chats
                                    .Include(c => c.Members)
                                    .FirstAsync(c => c.Id == chat.Id).ConfigureAwait(false);

            chatEntity.Members.Remove(appUser);
            
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
                return Result.Ok();

            return Result.Fail();
        }

        public async Task<Result<Message>> SendMessage(Message message)
        {
            var messageEntity = _mapper.Map<MessageEntity>(message);

            messageEntity.Sender = await _appDbContext.Users.FirstAsync(u => u.Id == message.Sender.Id);
            
            _appDbContext.Messages.Add(messageEntity);

            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
                return Result<Message>.Ok(_mapper.Map<Message>(messageEntity));

            return Result<Message>.Fail();
        }
    }
}
