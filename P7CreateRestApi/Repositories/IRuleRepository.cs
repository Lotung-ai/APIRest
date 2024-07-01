using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public interface IRuleRepository
    {
        Task<RuleName> CreateRuleNameAsync(RuleName ruleName);
        Task<RuleName> GetRuleNameByIdAsync(int id);
        Task<IEnumerable<RuleName>> GetAllRuleNamesAsync();
        Task<RuleName> UpdateRuleNameAsync(RuleName ruleName);
        Task<bool> DeleteRuleNameAsync(int id);
    }
}