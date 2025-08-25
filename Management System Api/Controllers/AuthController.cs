using Management_System_Api.Models.DTOs;
using Management_System_Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Management_System_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;


        [HttpPost("register")]
        [Authorize(Roles = "Admin")] // only Admin can create users
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, [FromQuery] string role = "Operator")
        { var resp = await _auth.RegisterAsync(request, role); return Ok(resp); }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        { var resp = await _auth.LoginAsync(request); return Ok(resp); }


        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        { await _auth.AssignRoleAsync(request); return NoContent(); }
    }
}
