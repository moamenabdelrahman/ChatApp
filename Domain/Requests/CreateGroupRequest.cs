namespace Domain.Requests
{
    public class CreateGroupRequest
    {
        public string Name { get; set; }

        public List<string> MemberUserNames { get; set; }
    }
}
