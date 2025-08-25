using Management_System_Api.Models.DTOs;

namespace Management_System_Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, string role);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task AssignRoleAsync(AssignRoleRequest request);
    }
}
