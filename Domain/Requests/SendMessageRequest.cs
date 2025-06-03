namespace Domain.Requests
{
    public class SendMessageRequest
    {
        public string SenderUserName { get; set; }

        public int ChatId { get; set; }

        public string Content { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
