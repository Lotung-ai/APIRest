using Dot.Net.WebApi.Domain;
using System.Security.Cryptography;

namespace Dot.Net.WebApi.Data
{
    public interface IBidRepository
    {
        Task<BidList> CreateBidAsync(BidList bid);
        Task<BidList> GetBidByIdAsync(int id);
        Task<IEnumerable<BidList>> GetAllBidsAsync();
        Task<BidList> UpdateBidAsync(BidList bid);
        Task<bool> DeleteBidAsync(int id);
    }
}
