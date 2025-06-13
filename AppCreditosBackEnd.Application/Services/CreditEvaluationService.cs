using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Application.Services
{
    public class CreditEvaluationService : ICreditEvaluationService
    {
        public async Task<ApplicationStatus> EvaluateApplicationAsync(CreditApplication application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application), "Credit application cannot be null");
            }
            else if(application.RequestedAmount <= 10.00m)
            {
                throw new Exception("Requested amount must be greater than 10.00");

            }

            if (application.MonthlyIncome >= 1500.00m && application.WorkExperienceYears >= 2)
            {
                return ApplicationStatus.Approved;
            }
            else if (application.MonthlyIncome >= 1000.00m && application.WorkExperienceYears >= 1)
            {
                return ApplicationStatus.Pending; // Suggest pending for further review if conditions are met but not fully approved
            }
            else
            {
                return ApplicationStatus.Rejected;
            }
        }
    }
}
