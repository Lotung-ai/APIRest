using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Data
{
    public interface ITradeRepository
    {
        Task<Trade> CreateTradeAsync(Trade trade);
        Task<Trade> GetTradeByIdAsync(int id);
        Task<IEnumerable<Trade>> GetAllTradesAsync();
        Task<Trade> UpdateTradeAsync(Trade trade);
        Task<bool> DeleteTradeAsync(int id);
    }
}