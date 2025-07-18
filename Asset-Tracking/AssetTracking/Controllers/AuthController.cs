using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous] // ✅ No token required
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);
        return Ok(new { message = result });
    }

    [HttpPost("verify-otp")]
    [AllowAnonymous] // ✅ No token required
    public async Task<IActionResult> VerifyOtp([FromBody] OTPRequest request)
    {
        var token = await _authService.VerifyOtpAsync(request.Email, request.OTP);

        if (token == "Invalid or expired OTP")
        {
            return BadRequest(new { message = token });
        }

        return Ok(new
        {
            message = "OTP verified successfully",
            token = token
        });
    }

    [HttpPost("resend-otp")]
    [AllowAnonymous] // ✅ No token required
    public async Task<IActionResult> ResendOtp([FromBody] OTPRequest request)
    {
        var result = await _authService.ResendOtpAsync(request.Email);
        return Ok(new { message = result });
    }
}
