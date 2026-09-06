using System.Collections.Concurrent;
using Domain.Models;

namespace Application.Repositories
{
    public interface IDatabaseSchemeRepository
    {
        Task<List<ValidTable>> GetValidTablesAsync();
        Task<List<string>> GetValidColumnsAsync(ValidTable table);
    }
}

