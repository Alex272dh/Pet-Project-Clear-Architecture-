using System.Collections.Concurrent;
using PetProject.DataBase.Scheme.Entities;

namespace PetProject.Repositories
{
    public interface IRepository
    {
        Task<List<ValidTable>> GetValidTablesAsync();
        Task<List<string>> GetValidColumnsAsync(ValidTable table);
    }
}
