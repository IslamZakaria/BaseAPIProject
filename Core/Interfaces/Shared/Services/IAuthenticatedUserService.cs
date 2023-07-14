namespace Core.Interfaces.Shared.Services
{
    public interface IAuthenticatedUserService
    {
        string? UserId { get; }
        string? Name { get; }
    }
}
