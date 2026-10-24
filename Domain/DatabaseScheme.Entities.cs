using System.Collections.Concurrent;

namespace PetProject.DataBase.Scheme.Entities
{
    public class ValidTable
    {
        public string TableName { get; private set; }
        public ConcurrentDictionary<string, bool> ValidColumns { get; private set; }= new ConcurrentDictionary<string, bool>();

        public ValidTable(string tableName)
        {
            TableName = tableName;
        }
    };
}