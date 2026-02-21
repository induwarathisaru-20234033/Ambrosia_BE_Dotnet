namespace AMB.Application.Dtos
{
    public class UpdatePasswordRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
