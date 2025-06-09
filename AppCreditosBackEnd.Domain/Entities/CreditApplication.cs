using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Domain.Entities
{
    public class CreditApplication
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }
        public decimal RequestedAmount { get; set; }
        public int TermInMonths { get; set; }
        public decimal MonthlyIncome { get; set; }
        public int WorkExperienceYears { get; set; }
        public ApplicationStatus Status { get; set; }
        public ApplicationStatus? SuggestedStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<AuditLog> AuditLogs { get; set; }
    }
}
