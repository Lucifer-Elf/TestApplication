namespace Servize.DTO.ADMIN
{
    public class ChangePasswordRequest
    {
        public string UserId { get; set; }

        public string OldPasword { get; set; }

        public string NewPassword { get; set; }

    }
}
