using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Application.DTOs;

namespace AppCreditosBackEnd.Application.Interfaces
{
    public interface ICreditApplicationService
    {
        Task<CreditApplicationResponseDto> UpdateStatusAsync(int applicationId, UpdateStatusDto dto, int analystId);
        Task<CreditApplicationResponseDto> CreateApplicationAsync(int userId, CreateCreditApplicationDto dto);
    }
}
