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
        private readonly IAuditLogRepository _auditRepository;
        private readonly ICreditEvaluationService _evaluationService;
        private readonly IMapper _mapper;        
        public CreditApplicationService(
            ICreditApplicationRepository repository,
            IAuditLogRepository auditRepository,
            ICreditEvaluationService evaluationService,
            IMapper mapper)
        {
            _repository = repository;
            _auditRepository = auditRepository;
            _evaluationService = evaluationService;
            _mapper = mapper;
        }
        public async Task<CreditApplicationResponseDto> CreateApplicationAsync(int userId, CreateCreditApplicationDto dto)
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
            await _auditRepository.CreateAsync(new AuditLog
            {
                CreditApplicationId = created.Id,
                UserId = userId,
                Action = "CREATE",
                Details = "Credit application created",
                NewStatus = ApplicationStatus.Pending,
                Timestamp = DateTime.UtcNow            });

            return _mapper.Map<CreditApplicationResponseDto>(created);
        }

        public async Task<CreditApplicationResponseDto> UpdateStatusAsync(int applicationId, UpdateStatusDto dto, int analystId)
        {
            var application = await _repository.GetByIdAsync(applicationId);
            if (application == null)
                //throw new NotFoundException("Credit application not found");
                throw new Exception("Credit application not found");

            var previousStatus = application.Status;
            application.Status = dto.Status;
            application.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(application);

            // Registro de auditoría
            await _auditRepository.CreateAsync(new AuditLog
            {
                CreditApplicationId = applicationId,
                UserId = analystId,
                Action = "STATUS_UPDATE",
                Details = $"Status changed from {previousStatus} to {dto.Status}",
                PreviousStatus = previousStatus,
                NewStatus = dto.Status,
                Timestamp = DateTime.UtcNow            });

            return _mapper.Map<CreditApplicationResponseDto>(application);
        }
    }
}
