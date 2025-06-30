namespace AppCreditosBackEnd.Domain.Enums
{
    public enum AccountType
    {
        Savings = 1,    // Ahorro
        Current = 2     // Corriente
    }

    public enum AccountStatus
    {
        NotApproved = 0,    // No aprobado
        Approved = 1,       // Aprobado
        Suspended = 2,      // Suspendida
        Deleted = 5         // Eliminada
    }

    public enum CardType
    {
        Debit = 1,      // Débito
        Credit = 2      // Crédito
    }

    public enum CardApplicationStatus
    {
        Pending = 1,    // Pendiente
        Approved = 2,   // Aprobada
        Rejected = 3    // Rechazada
    }

    public enum CardStatus
    {
        Active = 1,     // Activa
        Blocked = 2,    // Bloqueada
        Cancelled = 3   // Cancelada
    }

    public enum TransactionType
    {
        Transfer = 1,       // Transferencia
        CardPayment = 2,    // Pago con tarjeta
        Withdrawal = 3,     // Retiro
        Deposit = 4         // Depósito
    }

    public enum TransactionStatus
    {
        Success = 1,    // Exitosa
        Pending = 2,    // Pendiente
        Failed = 3      // Fallida
    }
}
