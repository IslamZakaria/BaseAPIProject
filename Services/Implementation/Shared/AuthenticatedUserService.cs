using Core.Interfaces.Shared.Services;
using Microsoft.AspNetCore.Http;

namespace Services.Implementation.Shared
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
        {
            var User_Id = httpContextAccessor.HttpContext?.User?.FindFirst("uid");
            if (User_Id is not null)
            {
                UserId = User_Id.Value.ToString();
            }

            var name = httpContextAccessor.HttpContext?.User?.FindFirst("name");
            if (name is not null)
            {
                Name = name.Value.ToString();
            }
        }

        public string? UserId { get; }
        public string? Name { get; }
    }
}
