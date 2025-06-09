using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int CreditApplicationId { get; set; }
        public CreditApplication CreditApplication { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public ApplicationStatus? PreviousStatus { get; set; }
        public ApplicationStatus? NewStatus { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
