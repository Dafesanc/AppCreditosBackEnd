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
    public class AccountsController : BaseApiController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Obtiene todas las cuentas - Solo para Analistas
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                var accounts = await _accountService.GetAllAccountsAsync();
                return ApiResponseHelper.CreateSuccessResponse(accounts, "Cuentas obtenidas exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener las cuentas: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene una cuenta por ID - Los usuarios solo pueden ver sus propias cuentas
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(Guid id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return ApiResponseHelper.CreateNotFoundResponse("Cuenta no encontrada.");
                }

                // Verificar acceso: Los aplicantes solo pueden ver sus propias cuentas
                var accessCheck = CheckUserAccess(account.UserId);
                if (accessCheck != null) return accessCheck;

                return ApiResponseHelper.CreateSuccessResponse(account, "Cuenta obtenida exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener la cuenta: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene las cuentas de un usuario específico - Los usuarios solo pueden ver sus propias cuentas
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAccountsByUserId(Guid userId)
        {
            // Verificar acceso: Los aplicantes solo pueden ver sus propias cuentas
            var accessCheck = CheckUserAccess(userId);
            if (accessCheck != null) return accessCheck;

            try
            {
                var accounts = await _accountService.GetAccountsByUserIdAsync(userId);
                return ApiResponseHelper.CreateSuccessResponse(accounts, "Cuentas del usuario obtenidas exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener las cuentas del usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene las cuentas del usuario actual
        /// </summary>
        [HttpGet("my-accounts")]
        public async Task<IActionResult> GetMyAccounts()
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return ApiResponseHelper.CreateUnauthorizedResponse("Usuario no autenticado.");
            }

            try
            {
                var accounts = await _accountService.GetAccountsByUserIdAsync(currentUserId.Value);
                return ApiResponseHelper.CreateSuccessResponse(accounts, "Tus cuentas obtenidas exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al obtener tus cuentas: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea una nueva cuenta - Todos los usuarios autenticados pueden crear cuentas
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDTO createAccountDto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponseHelper.CreateValidationErrorResponse(ModelState);
            }

            try
            {
                var account = await _accountService.CreateAccountAsync(createAccountDto);
                return ApiResponseHelper.CreateSuccessResponse(account, "Cuenta creada exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al crear la cuenta: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea una cuenta usando stored procedure - Todos los usuarios autenticados
        /// </summary>
        [HttpPost("create-with-sp")]
        public async Task<IActionResult> CreateAccountWithSP([FromBody] CreateAccountDTO createAccountDto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponseHelper.CreateValidationErrorResponse(ModelState);
            }

            try
            {
                var result = await _accountService.CreateAccountWithSPAsync(createAccountDto);
                
                if (result.Result == "SUCCESS")
                {
                    return ApiResponseHelper.CreateSuccessResponse(result, "Cuenta creada exitosamente usando stored procedure.", 201);
                }
                else
                {
                    return ApiResponseHelper.CreateErrorResponse($"Error al crear la cuenta: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al crear la cuenta: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza una cuenta - Solo para Analistas
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountDTO updateAccountDto)
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            if (!ModelState.IsValid)
            {
                return ApiResponseHelper.CreateValidationErrorResponse(ModelState);
            }

            try
            {
                var account = await _accountService.UpdateAccountAsync(id, updateAccountDto);
                return ApiResponseHelper.CreateSuccessResponse(account, "Cuenta actualizada exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al actualizar la cuenta: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina una cuenta - Solo para Analistas
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                var deleted = await _accountService.DeleteAccountAsync(id);
                if (!deleted)
                {
                    return ApiResponseHelper.CreateNotFoundResponse("Cuenta no encontrada.");
                }

                return ApiResponseHelper.CreateSuccessResponse("Cuenta eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.CreateErrorResponse($"Error al eliminar la cuenta: {ex.Message}");
            }
        }
    }
}
