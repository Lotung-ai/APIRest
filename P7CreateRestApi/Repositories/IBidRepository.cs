using P7CreateRestApi.Domain;
using System.Security.Cryptography;

namespace P7CreateRestApi.Repositories
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
