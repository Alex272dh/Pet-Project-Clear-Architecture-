using System.Collections.Concurrent;
using PetProject.DataBase.Scheme.Entities;

namespace PetProject.Repositories
{
    public interface ISchemeRepository
    {
        Task<List<ValidTable>> GetValidTablesAsync();
        Task<List<string>> GetValidColumnsAsync(ValidTable table);
    }
}
