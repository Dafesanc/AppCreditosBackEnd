using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Application.Interfaces
{
    public interface ICreditEvaluationService
    {
        Task<ApplicationStatus> EvaluateApplicationAsync(CreditApplication application);
    }
}
