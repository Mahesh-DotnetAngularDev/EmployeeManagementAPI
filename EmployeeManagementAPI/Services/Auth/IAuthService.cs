using EmployeeManagementAPI.Models.Auth;

namespace EmployeeManagementAPI.Services.Auth;

public interface IAuthService
{
    Task RegisterAsync(string username, string password);

    Task<LoginResponse> LoginAsync(string username, string password);
}