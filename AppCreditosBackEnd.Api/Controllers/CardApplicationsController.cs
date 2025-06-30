using AppCreditosBackEnd.Api.Controllers.Base;
using AppCreditosBackEnd.Api.Helpers;
using AppCreditosBackEnd.Application.DTOs.Input;
using AppCreditosBackEnd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppCreditosBackEnd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CardApplicationsController : BaseApiController
    {
        private readonly IBankingCardApplicationService _cardApplicationService;

        public CardApplicationsController(IBankingCardApplicationService cardApplicationService)
        {
            _cardApplicationService = cardApplicationService;
        }

        /// <summary>
        /// Obtiene todas las aplicaciones de tarjetas - Solo para Analistas
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCardApplications()
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                var applications = await _cardApplicationService.GetAllCardApplicationsAsync();
                return ApiResponseHelper.CreateSuccessResponse(applications, "Aplicaciones de tarjetas obtenidas exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener las aplicaciones de tarjetas: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene una aplicación de tarjeta por ID - Los usuarios solo pueden ver sus propias aplicaciones
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCardApplicationById(Guid id)
        {
            try
            {
                var application = await _cardApplicationService.GetCardApplicationByIdAsync(id);
                if (application == null)
                {
                    return ApiResponseHelper.CreateNotFoundResponse("Aplicación de tarjeta no encontrada.");
                }

                // Verificar acceso: Los aplicantes solo pueden ver sus propias aplicaciones
                var accessCheck = CheckUserAccess(application.UserId);
                if (accessCheck != null) return accessCheck;

                return ApiResponseHelper.CreateSuccessResponse(application, "Aplicación de tarjeta obtenida exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener la aplicación de tarjeta: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene las aplicaciones de tarjetas de un usuario específico - Los analistas pueden ver cualquier usuario, los aplicantes solo las suyas
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCardApplicationsByUserId(Guid userId)
        {
            // Los analistas pueden ver aplicaciones de cualquier usuario
            // Los aplicantes solo pueden ver sus propias aplicaciones
            if (!HasRole("Analyst"))
            {
                var accessCheck = CheckUserAccess(userId);
                if (accessCheck != null) return accessCheck;
            }

            try
            {
                var applications = await _cardApplicationService.GetCardApplicationsByUserIdAsync(userId);
                return ApiResponseHelper.CreateSuccessResponse(applications, "Aplicaciones de tarjetas del usuario obtenidas exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener las aplicaciones de tarjetas del usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene las aplicaciones de tarjetas del usuario actual
        /// </summary>
        [HttpGet("my-applications")]
        public async Task<IActionResult> GetMyCardApplications()
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return ApiResponseHelper.CreateUnauthorizedResponse("Usuario no autenticado.");
            }

            try
            {
                var applications = await _cardApplicationService.GetCardApplicationsByUserIdAsync(currentUserId.Value);
                return ApiResponseHelper.CreateSuccessResponse(applications, "Tus aplicaciones de tarjetas obtenidas exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener tus aplicaciones de tarjetas: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea una nueva aplicación de tarjeta - Todos los usuarios autenticados pueden crear aplicaciones
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCardApplication([FromBody] CreateCardApplicationDTO createCardApplicationDto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponseHelper.CreateValidationErrorResponse(ModelState);
            }

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return ApiResponseHelper.CreateUnauthorizedResponse("Usuario no autenticado.");
            }

            try
            {
                var application = await _cardApplicationService.CreateCardApplicationAsync(createCardApplicationDto, currentUserId.Value);
                return ApiResponseHelper.CreateSuccessResponse(application, "Aplicación de tarjeta creada exitosamente.", 201);
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al crear la aplicación de tarjeta: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea una aplicación de tarjeta usando stored procedure - Todos los usuarios autenticados
        /// </summary>
        [HttpPost("apply-with-sp")]
        public async Task<IActionResult> ApplyForCardWithSP([FromBody] CreateCardApplicationDTO createCardApplicationDto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponseHelper.CreateValidationErrorResponse(ModelState);
            }

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return ApiResponseHelper.CreateUnauthorizedResponse("Usuario no autenticado.");
            }

            try
            {
                var result = await _cardApplicationService.ApplyForCardWithSPAsync(createCardApplicationDto, currentUserId.Value);
                
                if (result.Result == "SUCCESS")
                {
                    return ApiResponseHelper.CreateSuccessResponse(result, "Aplicación de tarjeta creada exitosamente usando stored procedure.", 201);
                }
                else
                {
                    return ApiResponseHelper.CreateErrorResponse($"Error al crear la aplicación de tarjeta: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al crear la aplicación de tarjeta: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza el estado de una aplicación de tarjeta - Solo para Analistas
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateCardApplicationStatus(Guid id, [FromBody] UpdateCardApplicationStatusDTO updateStatusDto)
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            if (!ModelState.IsValid)
            {
                return ApiResponseHelper.CreateValidationErrorResponse(ModelState);
            }

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return ApiResponseHelper.CreateUnauthorizedResponse("Usuario no autenticado.");
            }

            try
            {
                var application = await _cardApplicationService.UpdateCardApplicationStatusAsync(id, updateStatusDto, currentUserId.Value);
                return ApiResponseHelper.CreateSuccessResponse(application, "Estado de la aplicación de tarjeta actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al actualizar el estado de la aplicación de tarjeta: {ex.Message}");
            }
        }

        /// <summary>
        /// Aprueba una aplicación de tarjeta usando stored procedure - Solo para Analistas
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveCardApplicationWithSP(Guid id)
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return ApiResponseHelper.CreateUnauthorizedResponse("Usuario no autenticado.");
            }

            try
            {
                var result = await _cardApplicationService.ApproveCardApplicationWithSPAsync(id, currentUserId.Value);
                
                if (result.Result == "SUCCESS")
                {
                    return ApiResponseHelper.CreateSuccessResponse(result, "Aplicación de tarjeta aprobada exitosamente. La tarjeta ha sido generada automáticamente.");
                }
                else
                {
                    return ApiResponseHelper.CreateErrorResponse($"Error al aprobar la aplicación de tarjeta: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al aprobar la aplicación de tarjeta: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina una aplicación de tarjeta - Solo para Analistas
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCardApplication(Guid id)
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                var deleted = await _cardApplicationService.DeleteCardApplicationAsync(id);
                if (!deleted)
                {
                    return ApiResponseHelper.CreateNotFoundResponse("Aplicación de tarjeta no encontrada.");
                }

                return ApiResponseHelper.CreateSuccessResponse("Aplicación de tarjeta eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al eliminar la aplicación de tarjeta: {ex.Message}");
            }
        }
    }
}
