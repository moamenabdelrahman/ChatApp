using Domain.Entities;

namespace Domain.Requests
{
    public class ResetPasswordRequest
    {
        public string UserName { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
