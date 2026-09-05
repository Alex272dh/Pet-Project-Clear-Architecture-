using Data.Mapping;
using DataBase.Scheme;
using Npgsql;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DataBase.Scheme
{
    public class RelationalRepositoryScheme : ISchemeRepository
    {
        private static readonly string connectionString2 = "Host=localhost;Port=5433;UserId=postgres;Password=DA13q30by20NAT156;Database=Finances;";
        private ConcurrentDictionary<ValidTable, bool> ValidTables = new();//В дальнейшем можно заменить на хэширование в отдельном приложении

        public async Task<List<ValidTable>> GetValidTablesAsync()
        {
            if (ValidTables.Any())
            {
                return ValidTables.Select(table => table.Key).ToList();
            }

            await UploadValidTablesAsync();

            return ValidTables.Select(table => table.Key).ToList();
        }

        public async Task<List<string>> GetValidColumnsAsync(ValidTable validTable)
        {
            if (validTable is null)
            {
                return null;
            }

            if (validTable.ValidColumns.Any())
            {
                return validTable.ValidColumns.Select(column => column.Key).ToList();
            }

            await UploadValidColumnsAsync(validTable);

            return validTable.ValidColumns.Select(column => column.Key).ToList();
        }

        private async Task UploadValidTablesAsync()
        {
            await using var connection = new NpgsqlConnection(connectionString2);
            string query = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'";
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(query, connection);
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (!ValidTables.Keys.Any(table => table.TableName == reader.GetString("table_name")))
                {
                    ValidTable validTable = new(reader.GetString("table_name"), new ConcurrentDictionary<string, bool> ());
                    ValidTables[validTable] = true;
                }
            }
        }

        private async Task UploadValidColumnsAsync(ValidTable validTable)
        {
            await using var connection = new NpgsqlConnection(connectionString2);
            string query = "SELECT column_name FROM information_schema.columns WHERE table_schema = 'public' AND table_name = @tableName;";
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@tableName", validTable.TableName);
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                validTable.ValidColumns[reader.GetString("column_name")] = true;
            }
        }
    }

    /*
    public class ValidTable
    {
        public string TableName { get; set; }
        public ConcurrentDictionary<string, bool> ValidColumns;
        public ValidTable()
        {
            TableName = "";
            ValidColumns = new ConcurrentDictionary<string, bool>();
        }
        public ValidTable(string tableName)
        {
            TableName = tableName;
            ValidColumns = new ConcurrentDictionary<string, bool>();
        }
    }
    */
}