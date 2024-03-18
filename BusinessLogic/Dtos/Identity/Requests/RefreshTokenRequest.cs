namespace BusinessLogic.Dtos.Identity.Requests
{
    public abstract class RefreshTokenRequest
    {
        public string Token { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}