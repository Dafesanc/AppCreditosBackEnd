using AppCreditosBackEnd.Api.Controllers.Base;
using AppCreditosBackEnd.Api.Helpers;
using AppCreditosBackEnd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppCreditosBackEnd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CardsController : BaseApiController
    {
        private readonly ICardService _cardService;

        public CardsController(ICardService cardService)
        {
            _cardService = cardService;
        }

        /// <summary>
        /// Obtiene todas las tarjetas - Solo para Analistas
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                var cards = await _cardService.GetAllCardsAsync();
                return ApiResponseHelper.CreateSuccessResponse(cards, "Tarjetas obtenidas exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener las tarjetas: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene una tarjeta por su número usando Stored Procedure - Solo para Analistas
        /// </summary>
        [HttpGet("by-number/{cardNumber}")]
        public async Task<IActionResult> GetCardByNumber(string cardNumber)
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                var card = await _cardService.GetCardByNumberWithSPAsync(cardNumber);
                if (card == null)
                {
                    return ApiResponseHelper.CreateNotFoundResponse("Tarjeta no encontrada o no está activa.");
                }

                return ApiResponseHelper.CreateSuccessResponse(card, "Tarjeta obtenida exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener la tarjeta: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene las tarjetas activas del usuario actual - Accesible para todos los usuarios autenticados
        /// </summary>
        [HttpGet("my-cards")]
        public async Task<IActionResult> GetMyCards()
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return ApiResponseHelper.CreateUnauthorizedResponse("Usuario no autenticado.");
            }

            try
            {
                var cards = await _cardService.GetMyCardsAsync(currentUserId.Value);
                return ApiResponseHelper.CreateSuccessResponse(cards, "Tus tarjetas obtenidas exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener tus tarjetas: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene las tarjetas activas de un usuario específico - Los analistas pueden ver cualquier usuario, los aplicantes solo las suyas
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCardsByUserId(Guid userId)
        {
            // Los analistas pueden ver tarjetas de cualquier usuario
            // Los aplicantes solo pueden ver sus propias tarjetas
            if (!HasRole("Analyst"))
            {
                var accessCheck = CheckUserAccess(userId);
                if (accessCheck != null) return accessCheck;
            }

            try
            {
                var cards = await _cardService.GetMyCardsAsync(userId);
                return ApiResponseHelper.CreateSuccessResponse(cards, "Tarjetas del usuario obtenidas exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener las tarjetas del usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina una tarjeta - Solo para Analistas
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(Guid id)
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                var deleted = await _cardService.DeleteCardAsync(id);
                if (!deleted)
                {
                    return ApiResponseHelper.CreateNotFoundResponse("Tarjeta no encontrada.");
                }

                return ApiResponseHelper.CreateSuccessResponse("Tarjeta eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al eliminar la tarjeta: {ex.Message}");
            }
        }
    }
}
