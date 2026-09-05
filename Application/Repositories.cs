using System.Collections.Concurrent;

public interface ISchemeRepository
{
    Task<List<ValidTable>> GetValidTablesAsync();
    Task<List<string>> GetValidColumnsAsync(ValidTable table);
}

public record ValidTable(string TableName, ConcurrentDictionary<string, bool> ValidColumns);
