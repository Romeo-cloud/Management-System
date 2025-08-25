using Management_System_Api.Services.Interfaces;
using Management_System_Api.Models.DTOs;
using Management_System_Api.Models.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Management_System_Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        { _userManager = userManager; _signInManager = signInManager; _roleManager = roleManager; _config = config; }


        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                throw new Exceptions.BadRequestException($"Role '{role}' does not exist");


            var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FullName = request.FullName };
            var create = await _userManager.CreateAsync(user, request.Password);
            if (!create.Succeeded)
                throw new Exceptions.BadRequestException(string.Join("; ", create.Errors.Select(e => e.Description)));


            await _userManager.AddToRoleAsync(user, role);
            return await BuildAuthResponseAsync(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new Exceptions.BadRequestException("Invalid credentials");
            var check = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!check.Succeeded) throw new Exceptions.BadRequestException("Invalid credentials");
            user.LastLoginAtUtc = DateTime.UtcNow; await _userManager.UpdateAsync(user);
            return await BuildAuthResponseAsync(user);
        }

        public async Task AssignRoleAsync(AssignRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId) ?? throw new Exceptions.NotFoundException("User not found");
            if (!await _roleManager.RoleExistsAsync(request.RoleName))
                throw new Exceptions.BadRequestException($"Role '{request.RoleName}' does not exist");
            var currentRoles = await _userManager.GetRolesAsync(user);
            var remove = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!remove.Succeeded) throw new Exceptions.BadRequestException("Failed removing existing roles");
            var add = await _userManager.AddToRoleAsync(user, request.RoleName);
            if (!add.Succeeded) throw new Exceptions.BadRequestException("Failed assigning role");
        }
        private async Task<AuthResponse> BuildAuthResponseAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
{
new(JwtRegisteredClaimNames.Sub, user.Id),
new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
new(ClaimTypes.NameIdentifier, user.Id),
new(ClaimTypes.Name, user.Email ?? user.Id)
};
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "60"));


            var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);


            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return new AuthResponse(accessToken, expires, user.Id, roles.ToArray());
        }


    }
}
