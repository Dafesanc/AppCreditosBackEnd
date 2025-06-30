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
    public class EmployersController : BaseApiController
    {
        private readonly IEmployerService _employerService;

        public EmployersController(IEmployerService employerService)
        {
            _employerService = employerService;
        }

        /// <summary>
        /// Obtiene todos los empleadores - Solo para Analistas
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllEmployers()
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                var employers = await _employerService.GetAllEmployersAsync();
                return ApiResponseHelper.CreateSuccessResponse(employers, "Empleadores obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener los empleadores: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un empleador por ID - Solo para Analistas
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployerById(Guid id)
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                var employer = await _employerService.GetEmployerByIdAsync(id);
                if (employer == null)
                {
                    return ApiResponseHelper.CreateNotFoundResponse("Empleador no encontrado.");
                }

                return ApiResponseHelper.CreateSuccessResponse(employer, "Empleador obtenido exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener el empleador: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene los empleadores de un usuario específico - Solo para Analistas
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetEmployersByUserId(Guid userId)
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                var employers = await _employerService.GetEmployersByUserIdAsync(userId);
                return ApiResponseHelper.CreateSuccessResponse(employers, "Empleadores del usuario obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener los empleadores del usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo empleador - Solo para Analistas
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateEmployer([FromBody] CreateEmployerDTO createEmployerDto)
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
                var employer = await _employerService.CreateEmployerAsync(createEmployerDto, currentUserId.Value);
                return ApiResponseHelper.CreateSuccessResponse(employer, "Empleador creado exitosamente.", 201);
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al crear el empleador: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza un empleador - Solo para Analistas
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployer(Guid id, [FromBody] UpdateEmployerDTO updateEmployerDto)
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            if (!ModelState.IsValid)
            {
                return ApiResponseHelper.CreateValidationErrorResponse(ModelState);
            }

            try
            {
                var employer = await _employerService.UpdateEmployerAsync(id, updateEmployerDto);
                return ApiResponseHelper.CreateSuccessResponse(employer, "Empleador actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al actualizar el empleador: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina un empleador - Solo para Analistas
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployer(Guid id)
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                var deleted = await _employerService.DeleteEmployerAsync(id);
                if (!deleted)
                {
                    return ApiResponseHelper.CreateNotFoundResponse("Empleador no encontrado.");
                }

                return ApiResponseHelper.CreateSuccessResponse("Empleador eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al eliminar el empleador: {ex.Message}");
            }
        }
    }
}
