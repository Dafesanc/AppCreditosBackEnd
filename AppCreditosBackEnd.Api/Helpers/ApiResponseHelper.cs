using AppCreditosBackEnd.Application.DTOs.Output;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AppCreditosBackEnd.Api.Helpers
{
    /// <summary>
    /// Helper para crear respuestas de API consistentes
    /// </summary>
    public static class ApiResponseHelper
    {
        /// <summary>
        /// Crea una respuesta de error 401 (No autorizado) con formato consistente
        /// </summary>
        /// <param name="message">Mensaje personalizado de error</param>
        /// <returns>ObjectResult con el formato ApiResponse</returns>
        public static ObjectResult CreateUnauthorizedResponse(string message = "Este usuario no está autorizado a realizar esta consulta")
        {
            return new ObjectResult(new ApiResponse<object>
            {
                ErrorCode = 401,
                Message = message,
                Data = null
            })
            {
                StatusCode = 401
            };
        }

        /// <summary>
        /// Crea una respuesta de error 403 (Prohibido) con formato consistente
        /// </summary>
        /// <param name="message">Mensaje personalizado de error</param>
        /// <returns>ObjectResult con el formato ApiResponse</returns>
        public static ObjectResult CreateForbiddenResponse(string message = "No tienes los permisos necesarios para acceder a este recurso")
        {
            return new ObjectResult(new ApiResponse<object>
            {
                ErrorCode = 403,
                Message = message,
                Data = null
            })
            {
                StatusCode = 403
            };
        }

        /// <summary>
        /// Crea una respuesta exitosa con formato consistente
        /// </summary>
        /// <typeparam name="T">Tipo de datos de la respuesta</typeparam>
        /// <param name="data">Datos a incluir en la respuesta</param>
        /// <param name="message">Mensaje de éxito</param>
        /// <returns>OkObjectResult con el formato ApiResponse</returns>
        public static OkObjectResult CreateSuccessResponse<T>(T data, string message = "Operación realizada exitosamente")
        {
            return new OkObjectResult(new ApiResponse<T>
            {
                ErrorCode = 200,
                Message = message,
                Data = data
            });
        }

        /// <summary>
        /// Crea una respuesta exitosa con formato consistente y código de estado personalizado
        /// </summary>
        /// <typeparam name="T">Tipo de datos de la respuesta</typeparam>
        /// <param name="data">Datos a incluir en la respuesta</param>
        /// <param name="message">Mensaje de éxito</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        /// <returns>ObjectResult con el formato ApiResponse</returns>
        public static ObjectResult CreateSuccessResponse<T>(T data, string message, int statusCode)
        {
            return new ObjectResult(new ApiResponse<T>
            {
                ErrorCode = statusCode,
                Message = message,
                Data = data
            })
            {
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Crea una respuesta exitosa sin datos con formato consistente
        /// </summary>
        /// <param name="message">Mensaje de éxito</param>
        /// <returns>OkObjectResult con el formato ApiResponse</returns>
        public static OkObjectResult CreateSuccessResponse(string message = "Operación realizada exitosamente")
        {
            return new OkObjectResult(new ApiResponse<object>
            {
                ErrorCode = 200,
                Message = message,
                Data = null
            });
        }

        /// <summary>
        /// Crea una respuesta de error genérica con formato consistente
        /// </summary>
        /// <param name="statusCode">Código de estado HTTP</param>
        /// <param name="message">Mensaje de error</param>
        /// <returns>ObjectResult con el formato ApiResponse</returns>
        public static ObjectResult CreateErrorResponse(int statusCode, string message)
        {
            return new ObjectResult(new ApiResponse<object>
            {
                ErrorCode = statusCode,
                Message = message,
                Data = null
            })
            {
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Crea una respuesta de error genérica con código 500 por defecto
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <returns>ObjectResult con el formato ApiResponse</returns>
        public static ObjectResult CreateErrorResponse(string message)
        {
            return CreateErrorResponse(500, message);
        }

        /// <summary>
        /// Crea una respuesta de no encontrado (404) con formato consistente
        /// </summary>
        /// <param name="message">Mensaje personalizado</param>
        /// <returns>NotFoundObjectResult con el formato ApiResponse</returns>
        public static NotFoundObjectResult CreateNotFoundResponse(string message = "Recurso no encontrado")
        {
            return new NotFoundObjectResult(new ApiResponse<object>
            {
                ErrorCode = 404,
                Message = message,
                Data = null
            });
        }

        /// <summary>
        /// Crea una respuesta de error de validación (400) con formato consistente
        /// </summary>
        /// <param name="message">Mensaje de error de validación</param>
        /// <returns>BadRequestObjectResult con el formato ApiResponse</returns>
        public static BadRequestObjectResult CreateValidationErrorResponse(string message = "Datos de entrada inválidos")
        {
            return new BadRequestObjectResult(new ApiResponse<object>
            {
                ErrorCode = 400,
                Message = message,
                Data = null
            });
        }

        /// <summary>
        /// Crea una respuesta de error de validación (400) con ModelState
        /// </summary>
        /// <param name="modelState">ModelState con errores de validación</param>
        /// <returns>BadRequestObjectResult con el formato ApiResponse</returns>
        public static BadRequestObjectResult CreateValidationErrorResponse(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value?.Errors ?? new Microsoft.AspNetCore.Mvc.ModelBinding.ModelErrorCollection())
                .Select(x => x.ErrorMessage)
                .ToList();

            var message = errors.Any() ? string.Join("; ", errors) : "Datos de entrada inválidos";

            return new BadRequestObjectResult(new ApiResponse<object>
            {
                ErrorCode = 400,
                Message = message,
                Data = null
            });
        }
    }
}
