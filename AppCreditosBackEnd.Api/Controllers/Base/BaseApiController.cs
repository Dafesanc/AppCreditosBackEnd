using AppCreditosBackEnd.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppCreditosBackEnd.Api.Controllers.Base
{
    /// <summary>
    /// Controlador base que proporciona métodos de autorización comunes
    /// </summary>
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Verifica si el usuario actual tiene el rol especificado
        /// </summary>
        /// <param name="role">Rol requerido</param>
        /// <returns>True si el usuario tiene el rol, False en caso contrario</returns>
        protected bool HasRole(string role)
        {
            return User.IsInRole(role);
        }

        /// <summary>
        /// Obtiene el ID del usuario actual desde el token JWT
        /// </summary>
        /// <returns>ID del usuario o null si no está autenticado</returns>
        protected Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// Obtiene el email del usuario actual desde el token JWT
        /// </summary>
        /// <returns>Email del usuario o null si no está autenticado</returns>
        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Verifica si el usuario está autenticado y tiene el rol especificado
        /// Retorna una respuesta de error si no cumple los requisitos
        /// </summary>
        /// <param name="requiredRole">Rol requerido (opcional)</param>
        /// <returns>ObjectResult con error si no está autorizado, null si está autorizado</returns>
        protected ObjectResult? CheckAuthorization(string? requiredRole = null)
        {
            // Verificar autenticación
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return ApiResponseHelper.CreateUnauthorizedResponse(
                    "Usuario no autenticado. Por favor, inicie sesión para acceder a este recurso.");
            }

            // Verificar rol si se especifica
            if (!string.IsNullOrEmpty(requiredRole) && !HasRole(requiredRole))
            {
                return ApiResponseHelper.CreateForbiddenResponse(
                    $"Este usuario no está autorizado a realizar esta consulta. Se requiere el rol '{requiredRole}'.");
            }

            return null; // Autorizado
        }

        /// <summary>
        /// Verifica si el usuario puede acceder a recursos de otro usuario
        /// Los analistas pueden acceder a cualquier recurso, los aplicantes solo a los suyos
        /// </summary>
        /// <param name="targetUserId">ID del usuario objetivo</param>
        /// <returns>ObjectResult con error si no está autorizado, null si está autorizado</returns>
        protected ObjectResult? CheckUserAccess(Guid targetUserId)
        {
            var currentUserId = GetCurrentUserId();
            
            if (!currentUserId.HasValue)
            {
                return ApiResponseHelper.CreateUnauthorizedResponse(
                    "Usuario no autenticado. Por favor, inicie sesión para acceder a este recurso.");
            }

            // Los analistas pueden acceder a cualquier recurso
            if (HasRole("Analyst"))
            {
                return null;
            }

            // Los aplicantes solo pueden acceder a sus propios recursos
            if (HasRole("Applicant") && currentUserId.Value == targetUserId)
            {
                return null;
            }

            return ApiResponseHelper.CreateForbiddenResponse(
                "No tienes permisos para acceder a los recursos de otro usuario.");
        }
    }
}
