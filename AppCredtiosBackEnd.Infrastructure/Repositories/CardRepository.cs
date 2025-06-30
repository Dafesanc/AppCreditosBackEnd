using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Interfaces;
using AppCreditosBackEnd.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AppCreditosBackEnd.Infrastructure.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly CreditPlatformDbContext _context;

        public CardRepository(CreditPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Card>> GetAllAsync()
        {
            return await _context.Cards
                .Include(c => c.CardApplication)
                    .ThenInclude(ca => ca.User)
                .Include(c => c.CardApplication)
                    .ThenInclude(ca => ca.Account)
                .OrderByDescending(c => c.IssuedDate)
                .ToListAsync();
        }

        public async Task<Card?> GetByIdAsync(Guid id)
        {
            return await _context.Cards
                .Include(c => c.CardApplication)
                    .ThenInclude(ca => ca.User)
                .Include(c => c.CardApplication)
                    .ThenInclude(ca => ca.Account)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Card>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Cards
                .Include(c => c.CardApplication)
                    .ThenInclude(ca => ca.User)
                .Include(c => c.CardApplication)
                    .ThenInclude(ca => ca.Account)
                .Where(c => c.CardApplication.UserId == userId)
                .OrderByDescending(c => c.IssuedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Card>> GetActiveCardsByUserIdAsync(Guid userId)
        {
            return await _context.Cards
                .Include(c => c.CardApplication)
                    .ThenInclude(ca => ca.User)
                .Include(c => c.CardApplication)
                    .ThenInclude(ca => ca.Account)
                .Where(c => c.CardApplication.UserId == userId && c.Status == 1) // 1 = Active
                .OrderByDescending(c => c.IssuedDate)
                .ToListAsync();
        }

        public async Task<Card?> GetByCardNumberAsync(string cardNumber)
        {
            return await _context.Cards
                .Include(c => c.CardApplication)
                    .ThenInclude(ca => ca.User)
                .Include(c => c.CardApplication)
                    .ThenInclude(ca => ca.Account)
                .FirstOrDefaultAsync(c => c.CardNumber == cardNumber && c.Status == 1); // Solo tarjetas activas
        }

        public async Task<Card?> GetByCardNumberWithSPAsync(string cardNumber)
        {
            var connectionString = _context.Database.GetConnectionString();
            if (connectionString == null) 
                throw new InvalidOperationException("Database connection string is null");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("GetCardByNumber", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CardNumber", cardNumber);

                try
                {
                    await connection.OpenAsync();
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var card = new Card
                            {
                                Id = reader.GetGuid("Id"),
                                CardApplicationId = reader.GetGuid("CardApplicationId"),
                                CardNumber = reader.GetString("CardNumber"),
                                ExpiryDate = reader.GetDateTime("ExpiryDate"),
                                CVC = reader.GetString("CVC"),
                                IssuedDate = reader.GetDateTime("IssuedDate"),
                                Status = reader.GetInt32("Status")
                            };

                            return card;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing stored procedure GetCardByNumber: {ex.Message}", ex);
                }
            }

            return null;
        }

        public async Task<Card> CreateAsync(Card card)
        {
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();
            return card;
        }

        public async Task<Card> UpdateAsync(Card card)
        {
            _context.Cards.Update(card);
            await _context.SaveChangesAsync();
            return card;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null) return false;

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Cards.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> CardNumberExistsAsync(string cardNumber)
        {
            return await _context.Cards.AnyAsync(c => c.CardNumber == cardNumber);
        }
    }
}
