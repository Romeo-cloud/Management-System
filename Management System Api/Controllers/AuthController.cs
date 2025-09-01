using Management_System_Api.Models.Domain;
using Management_System_Api.Models.DTOs;
using Management_System_Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Management_System_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        // ==================== REGISTER ====================
        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request,
            [FromQuery] string role = "Operator")
        {
            var resp = await _auth.RegisterAsync(request, role);
            return Ok(resp);
        }

        // ==================== LOGIN ====================
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var resp = await _auth.LoginAsync(request);

            // Set JWT in HttpOnly cookie for Swagger/browser
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // production
                SameSite = SameSiteMode.Strict,
                Expires = resp.ExpiresAtUtc
            };
            Response.Cookies.Append("jwt", resp.AccessToken, cookieOptions);

            // Return token in JSON for CLI/Postman
            return Ok(new
            {
                message = "Login successful",
                expiresAtUtc = resp.ExpiresAtUtc,
                roles = resp.Roles,
                accessToken = resp.AccessToken
            });
        }

        // ==================== ASSIGN ROLE ====================
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            await _auth.AssignRoleAsync(request);
            return NoContent();
        }
    }
}
