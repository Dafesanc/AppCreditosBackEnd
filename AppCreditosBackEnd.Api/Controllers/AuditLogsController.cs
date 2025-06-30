using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Application.DTOs.Output;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace AppCreditosBackEnd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Verifica si el usuario está autenticado y tiene el rol requerido
        /// </summary>
        /// <param name="requiredRole">Rol requerido</param>
        /// <returns>ObjectResult con error si no está autorizado, null si está autorizado</returns>
        private ObjectResult? CheckAuthorization(string? requiredRole = null)
        {
            // Verificar autenticación
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return StatusCode(401, new ApiResponse<object>
                {
                    ErrorCode = 401,
                    Message = "Usuario no autenticado. Por favor, inicie sesión para acceder a este recurso.",
                    Data = null
                });
            }

            // Verificar rol si se especifica
            if (!string.IsNullOrEmpty(requiredRole) && !User.IsInRole(requiredRole))
            {
                return StatusCode(403, new ApiResponse<object>
                {
                    ErrorCode = 403,
                    Message = $"Este usuario no está autorizado a realizar esta consulta. Se requiere el rol '{requiredRole}'.",
                    Data = null
                });
            }

            return null; // Autorizado
        }/// <summary>
        /// Obtiene todas las auditorías del sistema
        /// </summary>
        /// <returns>Lista de registros de auditoría o objeto con mensaje si no hay registros</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<AuditLogDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetAllLogs()
        {
            // Verificar autorización personalizada
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                var logs = await _auditLogService.GetAllLogsAsync();
                
                if (logs == null || !logs.Any())
                {
                    // Retornar respuesta con array vacío pero con mensaje informativo
                    return Ok(new ApiResponse<List<AuditLogDto>>
                    {
                        ErrorCode = 404,
                        Message = "No se encontraron registros de auditorías para mostrar",
                        Data = new List<AuditLogDto>() // Array vacío
                    });
                }

                // Retornar respuesta exitosa con los datos
                return Ok(new ApiResponse<List<AuditLogDto>>
                {
                    ErrorCode = 200,
                    Message = "Registros de auditoría recuperados correctamente",
                    Data = logs
                });
            }
            catch (Exception ex)
            {
                // Retornar error en caso de excepción
                return StatusCode(500, new ApiResponse<List<AuditLogDto>>
                {
                    ErrorCode = 500,
                    Message = $"Error al obtener registros de auditoría: {ex.Message}",
                    Data = null
                });
            }
        }        /// <summary>
        /// Obtiene las auditorías filtradas por diferentes criterios
        /// </summary>
        /// <param name="filter">Filtros a aplicar (fechas como string)</param>
        /// <returns>Lista de registros de auditoría filtrados</returns>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(ApiResponse<List<AuditLogDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetFilteredLogs([FromQuery] AuditLogFilterStringDto filter)
        {
            // Verificar autorización personalizada
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                // Convertir fechas string a DateTime
                var (startDate, endDate, dateError) = DateTimeHelper.ParseDateRange(filter.StartDate, filter.EndDate);
                
                if (!string.IsNullOrEmpty(dateError))
                {
                    return BadRequest(new ApiResponse<List<AuditLogDto>>
                    {
                        ErrorCode = 400,
                        Message = dateError,
                        Data = null
                    });
                }

                // Crear el filtro con DateTime
                var dateTimeFilter = new AuditLogFilterDto(
                    filter.CreditApplicationId,
                    filter.UserId,
                    filter.Action,
                    startDate,
                    endDate
                );

                var logs = await _auditLogService.GetFilteredLogsAsync(dateTimeFilter);
                
                if (logs == null || !logs.Any())
                {
                    // Retornar respuesta con array vacío pero con mensaje informativo
                    return Ok(new ApiResponse<List<AuditLogDto>>
                    {
                        ErrorCode = 404,
                        Message = "No se encontraron registros de auditorías que cumplan con los criterios de búsqueda",
                        Data = new List<AuditLogDto>() // Array vacío
                    });
                }

                // Retornar respuesta exitosa con los datos
                return Ok(new ApiResponse<List<AuditLogDto>>
                {
                    ErrorCode = 200,
                    Message = "Registros de auditoría filtrados recuperados correctamente",
                    Data = logs
                });
            }
            catch (Exception ex)
            {
                // Retornar error en caso de excepción
                return StatusCode(500, new ApiResponse<List<AuditLogDto>>
                {
                    ErrorCode = 500,
                    Message = $"Error al filtrar registros de auditoría: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Obtiene las auditorías relacionadas con una solicitud de crédito específica
        /// </summary>
        /// <param name="creditApplicationId">ID de la solicitud de crédito</param>
        /// <returns>Lista de registros de auditoría para una solicitud</returns>
        [HttpGet("application/{creditApplicationId}")]
        [ProducesResponseType(typeof(ApiResponse<List<AuditLogDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetByCreditApplicationId(int creditApplicationId)
        {
            try
            {
                var logs = await _auditLogService.GetByCreditApplicationIdAsync(creditApplicationId);
                
                if (logs == null || !logs.Any())
                {
                    return Ok(new ApiResponse<List<AuditLogDto>>
                    {
                        ErrorCode = 404,
                        Message = $"No se encontraron registros de auditoría para la solicitud ID {creditApplicationId}",
                        Data = new List<AuditLogDto>()
                    });
                }

                return Ok(new ApiResponse<List<AuditLogDto>>
                {
                    ErrorCode = 200,
                    Message = $"Registros de auditoría para solicitud ID {creditApplicationId} recuperados correctamente",
                    Data = logs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<AuditLogDto>>
                {
                    ErrorCode = 500,
                    Message = $"Error al obtener registros de auditoría por solicitud: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Obtiene las auditorías relacionadas con un usuario específico
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Lista de registros de auditoría para un usuario</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<List<AuditLogDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetByUserId(Guid userId)
        {
            try
            {
                var logs = await _auditLogService.GetByUserIdAsync(userId);
                
                if (logs == null || !logs.Any())
                {
                    return Ok(new ApiResponse<List<AuditLogDto>>
                    {
                        ErrorCode = 404,
                        Message = $"No se encontraron registros de auditoría para el usuario ID {userId}",
                        Data = new List<AuditLogDto>()
                    });
                }

                return Ok(new ApiResponse<List<AuditLogDto>>
                {
                    ErrorCode = 200,
                    Message = $"Registros de auditoría para usuario ID {userId} recuperados correctamente",
                    Data = logs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<AuditLogDto>>
                {
                    ErrorCode = 500,
                    Message = $"Error al obtener registros de auditoría por usuario: {ex.Message}",
                    Data = null
                });
            }
        }        /// <summary>
        /// Obtiene las auditorías paginadas, útil para manejar grandes cantidades de registros
        /// </summary>
        /// <param name="page">Número de página (comienza en 1)</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <param name="filter">Filtros opcionales a aplicar (fechas como string)</param>
        /// <returns>Lista paginada de registros de auditoría</returns>
        [HttpGet("paginated")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<AuditLogDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PaginatedResult<AuditLogDto>>>> GetPaginatedLogs(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] AuditLogFilterStringDto? filter = null)
        {
            // Verificar autorización personalizada
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                AuditLogFilterDto? dateTimeFilter = null;
                
                // Solo convertir fechas si hay filtros
                if (filter != null)
                {
                    var (startDate, endDate, dateError) = DateTimeHelper.ParseDateRange(filter.StartDate, filter.EndDate);
                    
                    if (!string.IsNullOrEmpty(dateError))
                    {
                        return BadRequest(new ApiResponse<PaginatedResult<AuditLogDto>>
                        {
                            ErrorCode = 400,
                            Message = dateError,
                            Data = null
                        });
                    }

                    dateTimeFilter = new AuditLogFilterDto(
                        filter.CreditApplicationId,
                        filter.UserId,
                        filter.Action,
                        startDate,
                        endDate
                    );
                }
                
                var paginatedLogs = await _auditLogService.GetPaginatedLogsAsync(page, pageSize, dateTimeFilter);
                
                return Ok(new ApiResponse<PaginatedResult<AuditLogDto>>
                {
                    ErrorCode = 200,
                    Message = "Registros de auditoría paginados recuperados correctamente",
                    Data = paginatedLogs
                });
            }
            catch (Exception ex)
            {
                // Retornar error en caso de excepción
                return StatusCode(500, new ApiResponse<PaginatedResult<AuditLogDto>>
                {
                    ErrorCode = 500,
                    Message = $"Error al obtener registros de auditoría paginados: {ex.Message}",
                    Data = null
                });
            }
        }        /// <summary>
        /// Obtiene estadísticas de auditoría para dashboard
        /// </summary>
        /// <param name="startDate">Fecha de inicio en formato string (ej: 2024-01-15)</param>
        /// <param name="endDate">Fecha de fin en formato string (ej: 2024-01-31)</param>
        /// <returns>Estadísticas de los registros de auditoría</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(ApiResponse<AuditLogStatisticsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<AuditLogStatisticsDto>>> GetStatistics(
            [FromQuery] string? startDate = null, 
            [FromQuery] string? endDate = null)
        {
            // Verificar autorización personalizada
            var authCheck = CheckAuthorization("Analyst");
            if (authCheck != null) return authCheck;

            try
            {
                // Convertir fechas string a DateTime
                var (parsedStartDate, parsedEndDate, dateError) = DateTimeHelper.ParseDateRange(startDate, endDate);
                
                if (!string.IsNullOrEmpty(dateError))
                {
                    return BadRequest(new ApiResponse<AuditLogStatisticsDto>
                    {
                        ErrorCode = 400,
                        Message = dateError,
                        Data = null
                    });
                }

                var statistics = await _auditLogService.GetAuditLogStatisticsAsync(parsedStartDate, parsedEndDate);
                
                return Ok(new ApiResponse<AuditLogStatisticsDto>
                {
                    ErrorCode = 200,
                    Message = "Estadísticas de auditoría recuperadas correctamente",
                    Data = statistics
                });
            }
            catch (Exception ex)
            {
                // Retornar error en caso de excepción
                return StatusCode(500, new ApiResponse<AuditLogStatisticsDto>
                {
                    ErrorCode = 500,
                    Message = $"Error al generar estadísticas de auditoría: {ex.Message}",
                    Data = null
                });
            }
        }
    }

    
}
