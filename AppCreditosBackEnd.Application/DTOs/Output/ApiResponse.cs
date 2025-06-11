using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AppCreditosBackEnd.Application.DTOs.Output
{
    /// <summary>
    /// Clase para manejar respuestas de API consistentes
    /// </summary>
    /// <typeparam name="T">Tipo de dato que contendrá la respuesta</typeparam>
    /// 

    // Clase genérica para manejar respuestas de API consistentes
    public class ApiResponse<T>
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}

