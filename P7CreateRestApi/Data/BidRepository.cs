using Dot.Net.WebApi.Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Dot.Net.WebApi.Data
{
    public class BidRepository : IBidRepository
    {
        private readonly LocalDbContext _context;

        public BidRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<BidList> CreateBidAsync(BidList bid)
        {
            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
            return bid;
        }

        public async Task<BidList> GetBidByIdAsync(int id)
        {
            return await _context.Bids.FindAsync(id);
        }

        public async Task<IEnumerable<BidList>> GetAllBidsAsync()
        {
            return await _context.Bids.ToListAsync();
        }

        public async Task<BidList> UpdateBidAsync(BidList bid)
        {
            _context.Entry(bid).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return bid;
        }

        public async Task<bool> DeleteBidAsync(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid == null) return false;

            _context.Bids.Remove(bid);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
