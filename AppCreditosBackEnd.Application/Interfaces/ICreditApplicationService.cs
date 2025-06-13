using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Application.Interfaces
{
    public interface ICreditApplicationService
    {
        Task<CreditApplicationResponseDto> UpdateStatusAsync(int applicationId, UpdateStatusDto dto, Guid analystId);
        Task<CreditApplicationResponseDto> CreateApplicationAsync(Guid userId, CreateCreditApplicationDto dto);
        Task<List<CreditApplicationResponseDto>> GetUserApplicationsAsync(Guid userId);
        //Task<List<CreditApplicationResponseDto>> GetAllApplicationsAsync(ApplicationStatus? status = null);
        Task<List<CreditApplicationResponseDto>> GetAllApplicationsAsync();

    }
}
