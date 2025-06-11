using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppCreditosBackEnd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error registrando usuario: {ex.Message}" });
            }
        }

        /// <summary>
        /// Inicia sesión con un usuario existente
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error en el login: {ex.Message}" });
            }
        }

        /// <summary>
        /// Refresca un token existente
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request.Token, request.RefreshToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error refrescando el token: {ex.Message}" });
            }
        }        /// <summary>
        /// Cierra la sesión y invalida el token
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Logout()
        {
            try
            {
                // Obtener el token del encabezado de autorización
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { message = "Token no proporcionado" });
                }

                await _authService.LogoutAsync(token);
                return Ok(new { message = "Logout exitoso" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error en el logout: {ex.Message}" });
            }
        }
        
        /// <summary>
        /// Endpoint de prueba para verificar la autorización de Analistas
        /// </summary>
        [HttpGet("test-analyst")]
        [Authorize(Roles = "Analyst")]
        public ActionResult TestAnalystRole()
        {
            return Ok(new { message = "Tienes acceso como Analista", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Endpoint de prueba para verificar la autorización de Solicitantes
        /// </summary>
        [HttpGet("test-applicant")]
        [Authorize(Roles = "Applicant")]
        public ActionResult TestApplicantRole()
        {
            return Ok(new { message = "Tienes acceso como Solicitante", timestamp = DateTime.UtcNow });
        }
        
        /// <summary>
        /// Endpoint de prueba para verificar la autorización de cualquier usuario autenticado
        /// </summary>
        [HttpGet("user-info")]
        [Authorize]
        public ActionResult GetUserInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = User.FindFirst(ClaimTypes.Surname)?.Value;
            
            return Ok(new { 
                userId,
                email = userEmail,
                role = userRole,
                firstName,
                lastName,
                isAuthenticated = User.Identity?.IsAuthenticated ?? false
            });
        }
    }

    public class RefreshTokenRequest
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
