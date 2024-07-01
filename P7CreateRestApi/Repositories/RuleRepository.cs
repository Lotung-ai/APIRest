using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace P7CreateRestApi.Repositories
{
    public class RuleRepository : IRuleRepository
    {
        private readonly LocalDbContext _context;

        public RuleRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<RuleName> CreateRuleNameAsync(RuleName ruleName)
        {
            _context.RuleNames.Add(ruleName);
            await _context.SaveChangesAsync();
            return ruleName;
        }

        public async Task<RuleName> GetRuleNameByIdAsync(int id)
        {
            return await _context.RuleNames.FindAsync(id);
        }

        public async Task<IEnumerable<RuleName>> GetAllRuleNamesAsync()
        {
            return await _context.RuleNames.ToListAsync();
        }

        public async Task<RuleName> UpdateRuleNameAsync(RuleName ruleName)
        {
            _context.RuleNames.Update(ruleName);
            await _context.SaveChangesAsync();
            return ruleName;
        }

        public async Task<bool> DeleteRuleNameAsync(int id)
        {
            var ruleName = await _context.RuleNames.FindAsync(id);
            if (ruleName == null)
            {
                return false;
            }

            _context.RuleNames.Remove(ruleName);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
