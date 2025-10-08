using Domain.Entities;
using Domain.Enums;

namespace Api.DTOs
{
    public class ChatPreviewDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
        
        public Message LastMessage { get; set; }

        public ChatPreviewDTO(Chat chat)
        {
            this.Id = chat.Id;
            this.Name = chat.Name;
            this.Type = chat.Type.ToString();
            this.LastMessage = chat.Messages.FirstOrDefault();
        }
    }
}
