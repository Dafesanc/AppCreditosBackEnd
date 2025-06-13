using AppCreditosBackEnd.Application.DTOs.Output;
using Microsoft.AspNetCore.Mvc;

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
    }
}
