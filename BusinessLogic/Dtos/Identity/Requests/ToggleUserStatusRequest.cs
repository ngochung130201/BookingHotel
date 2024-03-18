namespace BusinessLogic.Dtos.Identity.Requests
{
    public class ToggleUserStatusRequest
    {
        public bool ActivateUser { get; set; }
        public string UserId { get; set; } = default!;
    }
}