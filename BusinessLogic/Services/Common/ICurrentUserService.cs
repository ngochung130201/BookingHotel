namespace BusinessLogic.Services.Common
{
	public interface ICurrentUserService
	{
		string UserId { get; }
		string UserName { get; }
		string RoleName { get; }
		string Origin { get; }
	}
}