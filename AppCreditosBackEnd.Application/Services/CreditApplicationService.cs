using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;
using AppCreditosBackEnd.Domain.Interfaces;
using AutoMapper;

namespace AppCreditosBackEnd.Application.Services
{
    public class CreditApplicationService : ICreditApplicationService
    {
        private readonly ICreditApplicationRepository _repository;
        private readonly IAuditLogService _auditLogService;
        private readonly ICreditEvaluationService _evaluationService;
        private readonly IMapper _mapper;        
        public CreditApplicationService(
            ICreditApplicationRepository repository,
            IAuditLogService auditLogService,
            ICreditEvaluationService evaluationService,
            IMapper mapper)
        {
            _repository = repository;
            _auditLogService = auditLogService;
            _evaluationService = evaluationService;
            _mapper = mapper;
        }
        public async Task<List<CreditApplicationResponseDto>> GetUserApplicationsAsync(Guid userId)
        {
            var applications = await _repository.GetByUserIdAsync(userId);
            return _mapper.Map<List<CreditApplicationResponseDto>>(applications);
        }
        public async Task<List<CreditApplicationResponseDto>> GetAllApplicationsAsync(ApplicationStatus? status)
        {
            var applications = await _repository.GetAllAsync(status);
            return _mapper.Map<List<CreditApplicationResponseDto>>(applications);
        }
        public async Task<CreditApplicationResponseDto> CreateApplicationAsync(Guid userId, CreateCreditApplicationDto dto)
        {
            var application = new CreditApplication
            {
                UserId = userId,
                RequestedAmount = dto.RequestedAmount,
                TermInMonths = dto.TermInMonths,
                MonthlyIncome = dto.MonthlyIncome,
                WorkExperienceYears = dto.WorkExperienceYears,
                Status = ApplicationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            // Evaluación automática
            application.SuggestedStatus = await _evaluationService.EvaluateApplicationAsync(application);            
            var created = await _repository.CreateAsync(application);

            // Registro de auditoría
            await _auditLogService.LogCreditApplicationAction(
                creditApplicationId: created.Id,
                userId: userId,
                action: "CREATE",
                details: $"Credit application created for ${created.RequestedAmount:C} with term of {created.TermInMonths} months",
                newStatus: ApplicationStatus.Pending);

            return _mapper.Map<CreditApplicationResponseDto>(created);
        }

        public async Task<CreditApplicationResponseDto> UpdateStatusAsync(int applicationId, UpdateStatusDto dto, Guid analystId)
        {
            var application = await _repository.GetByIdAsync(applicationId);
            if (application == null)
                //throw new NotFoundException("Credit application not found");
                throw new Exception("Credit application not found");

            var previousStatus = application.Status;
            application.Status = dto.Status;
            application.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(application);            // Registro de auditoría
            await _auditLogService.LogCreditApplicationAction(
                creditApplicationId: applicationId,
                userId: analystId,
                action: "STATUS_UPDATE",
                details: $"Status changed from {previousStatus} to {dto.Status} for application amount ${application.RequestedAmount:C}",
                previousStatus: previousStatus,
                newStatus: dto.Status);

            return _mapper.Map<CreditApplicationResponseDto>(application);
        }
    }
}
