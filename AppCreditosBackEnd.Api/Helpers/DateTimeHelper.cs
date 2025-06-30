using System.Globalization;

namespace AppCreditosBackEnd.Api.Helpers
{
    /// <summary>
    /// Helper para conversión y manejo de fechas
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Convierte un string de fecha a DateTime?
        /// Acepta múltiples formatos comunes
        /// </summary>
        /// <param name="dateString">Fecha en formato string</param>
        /// <returns>DateTime? - null si no se puede convertir</returns>
        public static DateTime? ParseDateString(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            // Formatos aceptados
            string[] formats = {
                "yyyy-MM-dd",           // 2024-01-15
                "yyyy-MM-ddTHH:mm:ss",  // 2024-01-15T10:30:00
                "yyyy-MM-ddTHH:mm:ssZ", // 2024-01-15T10:30:00Z
                "yyyy-MM-dd HH:mm:ss",  // 2024-01-15 10:30:00
                "dd/MM/yyyy",           // 15/01/2024
                "MM/dd/yyyy",           // 01/15/2024
                "dd-MM-yyyy",           // 15-01-2024
                "MM-dd-yyyy"            // 01-15-2024
            };

            // Intentar parsear con diferentes formatos
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }

            // Intentar parsear de forma automática
            if (DateTime.TryParse(dateString, out DateTime autoResult))
            {
                return autoResult;
            }

            return null;
        }

        /// <summary>
        /// Convierte fechas string a DateTime con validación de rango
        /// </summary>
        /// <param name="startDateString">Fecha inicio como string</param>
        /// <param name="endDateString">Fecha fin como string</param>
        /// <returns>Tupla con las fechas convertidas</returns>
        public static (DateTime? startDate, DateTime? endDate, string? errorMessage) ParseDateRange(string? startDateString, string? endDateString)
        {
            var startDate = ParseDateString(startDateString);
            var endDate = ParseDateString(endDateString);

            // Validar que las fechas sean válidas si se proporcionaron
            if (!string.IsNullOrWhiteSpace(startDateString) && startDate == null)
            {
                return (null, null, $"Formato de fecha de inicio inválido: {startDateString}. Use formato: YYYY-MM-DD");
            }

            if (!string.IsNullOrWhiteSpace(endDateString) && endDate == null)
            {
                return (null, null, $"Formato de fecha de fin inválido: {endDateString}. Use formato: YYYY-MM-DD");
            }

            // Validar que la fecha de inicio no sea mayor que la fecha de fin
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return (null, null, "La fecha de inicio no puede ser mayor que la fecha de fin");
            }

            // Si solo se proporciona endDate, ajustar la hora al final del día
            if (endDate.HasValue && endDate.Value.TimeOfDay == TimeSpan.Zero)
            {
                endDate = endDate.Value.AddDays(1).AddSeconds(-1);
            }

            return (startDate, endDate, null);
        }
    }
}
