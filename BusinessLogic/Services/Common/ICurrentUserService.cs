namespace BusinessLogic.Services.Common
{
	public interface ICurrentUserService
	{
		string UserId { get; }
		string UserName { get; }
		string RoleName { get; }
		string Origin { get; }
        string FullName { get; }
        string Email { get; }
        string PhoneNumber { get; }
    }
}