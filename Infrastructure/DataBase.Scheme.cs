using PetProject.Data.Mapping;
using PetProject.DataBase.Scheme;
using Npgsql;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using PetProject.Repositories;
using PetProject.DataBase.Scheme.Entities;

namespace PetProject.DataBase.Scheme
{
    public class RelationalRepositoryScheme : ISchemeRepository
    {
        private readonly string _connectionString;

        private ConcurrentDictionary<ValidTable, bool> _validTables = new();

        public RelationalRepositoryScheme(string connection)
        {
            _connectionString = connection;
        }

        public async Task<List<ValidTable>> GetValidTablesAsync()
        {
            if (_validTables.Any())
            {
                return _validTables.Select(table => table.Key).ToList();
            }

            await UploadValidTablesAsync();

            return _validTables.Select(table => table.Key).ToList();
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
            await using var connection = new NpgsqlConnection(connectionString);
            string query = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'";
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(query, connection);
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (!_validTables.Keys.Any(table => table.TableName == reader.GetString("table_name")))
                {
                    ValidTable validTable = new ValidTable(reader.GetString("table_name"));
                    _validTables[validTable] = true;
                }
            }
        }

        private async Task UploadValidColumnsAsync(ValidTable validTable)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
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
}