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
    string identificationType,
    string IdentificationNumber,
    DateTime BirthDate,
    UserRole Role);    public record RegisterClientRequestDto(
       string Email,
       string Password,
       string FirstName,
       string LastName,
       string IdentificationNumber,
       string IdentificationType,
       DateTime BirthDate,
       UserRole Role = UserRole.Applicant
    );

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
    );    public record CreditApplicationResponseDto
    {
        public int Id { get; set; }
        public decimal RequestedAmount { get; set; }
        public int TermInMonths { get; set; }
        public decimal MonthlyIncome { get; set; }
        public int WorkExperienceYears { get; set; }
        public ApplicationStatus Status { get; set; }
        public ApplicationStatus? SuggestedStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
    };

    public record UpdateStatusDto(ApplicationStatus Status);
}
