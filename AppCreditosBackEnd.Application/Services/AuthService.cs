using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Application.Helpers;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;

namespace AppCreditosBackEnd.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        
        // Diccionario para almacenar tokens inválidos (para implementar logout)
        private static readonly Dictionary<string, DateTime> _invalidatedTokens = new Dictionary<string, DateTime>();

        public AuthService(IUserRepository userRepository, IMapper mapper, IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            // Buscar usuario por email
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // Verificar contraseña
            if (user == null || !BC.Verify(request.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            // Generar token JWT
            var token = GenerateJwtToken(user);

            // Crear y devolver la respuesta de autenticación
            return new AuthResponseDto(
                Token: token,
                Email: user.Email,
                Role: user.Role,
                FullName: $"{user.FirstName} {user.LastName}"
            );
        }

        public async Task LogoutAsync(string token)
        {
            // Invalidar el token almacenándolo en la lista negra con su fecha de expiración
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);
            
            // Obtener la fecha de expiración del token
            var expiry = jwtToken.ValidTo;
            
            // Almacenar el token en la lista de tokens inválidos
            _invalidatedTokens[token] = expiry;
            
            // Limpieza de tokens expirados (como buena práctica)
            CleanupExpiredTokens();
            
            await Task.CompletedTask;
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken)
        {
            // Para simplificar, no implementamos refresh tokens, pero lo dejamos planteado
            // En una implementación real, verificaríamos el refresh token y generaríamos un nuevo token

            // Verificar si el token está en la lista negra
            if (_invalidatedTokens.ContainsKey(token))
            {
                throw new UnauthorizedAccessException("Token inválido");
            }

            // Validar el token JWT
            var principal = GetPrincipalFromToken(token);
            if (principal == null)
            {
                throw new UnauthorizedAccessException("Token inválido o expirado");
            }            // Obtener el email del usuario desde las claims
            var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                throw new UnauthorizedAccessException("Token inválido");
            }

            // Buscar el usuario
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuario no encontrado");
            }

            // Generar un nuevo token
            var newToken = GenerateJwtToken(user);

            return new AuthResponseDto(
                Token: newToken,
                Email: user.Email,
                Role: user.Role,
                FullName: $"{user.FirstName} {user.LastName}"
            );
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                // Verificar si el email ya existe
                try
                {
                    var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                    // Si llegamos aquí, significa que encontramos un usuario con ese email
                    throw new InvalidOperationException("El email ya está registrado");
                }
                catch (KeyNotFoundException)
                {
                    // Si el usuario no existe, continuamos con el registro (esto es lo que queremos)
                }

                // Crear el usuario
                var user = _mapper.Map<Users>(request);
                user.Id = Guid.NewGuid();
                user.CreatedAt = DateTime.UtcNow;
                
                // Hashear la contraseña
                user.Password = BC.HashPassword(request.Password);

                // Guardar el usuario en la base de datos
                var createdUser = await _userRepository.CreateAsync(user);

                // Generar token JWT
                var token = GenerateJwtToken(createdUser);

                // Devolver la respuesta
                return new AuthResponseDto(
                    Token: token,
                    Email: createdUser.Email,
                    Role: createdUser.Role,
                    FullName: $"{createdUser.FirstName} {createdUser.LastName}"
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar usuario: {ex.Message}", ex);
            }
        }

        // Método para generar un token JWT
        private string GenerateJwtToken(Users user)
        {
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Agregar nombres si están disponibles
            if (!string.IsNullOrEmpty(user.FirstName))
                claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
                
            if (!string.IsNullOrEmpty(user.LastName))
                claims.Add(new Claim(ClaimTypes.Surname, user.LastName));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Método para validar un token JWT
        private ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken || 
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        // Método para limpiar tokens expirados
        private void CleanupExpiredTokens()
        {
            var now = DateTime.UtcNow;
            var expiredTokens = _invalidatedTokens.Where(t => t.Value <= now).Select(t => t.Key).ToList();
            
            foreach (var token in expiredTokens)
            {
                _invalidatedTokens.Remove(token);
            }
        }
    }
}
