namespace Management_System_Api.Models.DTOs;

public record RegisterRequest(string Email, string Password, string FullName);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string AccessToken, DateTime ExpiresAtUtc, string UserId, string[] Roles);
public record AssignRoleRequest(string UserId, string RoleName);

