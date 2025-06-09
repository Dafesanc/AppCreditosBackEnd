using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Application.DTOs
{
    public record RegisterRequestDto(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserRole Role);

    public record LoginRequestDto(string Email, string Password);

    public record AuthResponseDto(
        string Token,
        string Email,
        UserRole Role,
        string FullName
    );

    // Application/DTOs/CreditApplicationDTOs.cs
    public record CreateCreditApplicationDto(
        decimal RequestedAmount,
        int TermInMonths,
        decimal MonthlyIncome,
        int WorkExperienceYears
    );

    public record CreditApplicationResponseDto(
        int Id,
        decimal RequestedAmount,
        int TermInMonths,
        decimal MonthlyIncome,
        int WorkExperienceYears,
        ApplicationStatus Status,
        ApplicationStatus? SuggestedStatus,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string ApplicantName
    );

    public record UpdateStatusDto(ApplicationStatus Status);
}
