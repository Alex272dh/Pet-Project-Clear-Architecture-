using System.Collections.Concurrent;
namespace Domain.Models
{
    public class ValidTable
    {
        public string TableName { get; }
        public ConcurrentDictionary<string, bool> ValidColumns { get; }

        public ValidTable(string tableName, ConcurrentDictionary<string, bool> validColumns)
        {
            TableName = tableName;
            ValidColumns = validColumns;
        }
    }
}
