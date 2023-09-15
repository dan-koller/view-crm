using View.Shared;
using View.WebApi.Data;

namespace View.WebApi.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> CreateUserAsync(RegisterRequest registerRequest);
}

