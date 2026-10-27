using Npgsql;
using PetProject.Data.Mapping;
using PetProject.DataBase.Scheme.Entities;
using System.Collections.Concurrent;
using System.Data;
using System.Text;

namespace PetProject.DataBase
{
    public class DataBase
    {
        //Добавить в среду, и забирать параметры подключения только из среды
        private readonly string _connectionString;

        private ConcurrentDictionary<ValidTable, bool> _validTables = new();

        public DataBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        /*
        public static async Task Select()
        {

        }
        */

        public async Task Insert(Table insertTable)
        {
            ConvertValues(insertTable);

            if (!insertTable.Columns.Any() || !insertTable.Columns.Any(column => column.Data.Count > 0))
            {
                return;
            }

            //Реализовать вставку в таблицу TargetTable, в столбцы TargetColumns
            string table = "\"" + insertTable.TargetTable + "\"";
            string columns = string.Join(", ", insertTable.Columns.Select(c => "\"" + c.TargetColumn + "\""));
            string query = $"INSERT INTO {table} ({columns}) VALUES (@InsertValues);";
            query = CreateParams(query, insertTable);

            //Подумай, где можно проставить try catch для обработки и логирования ошибок
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();
            await using var command = new NpgsqlCommand(query, connection, transaction);
            command.CommandText = AddValuesToParams(insertTable, command);
            //Выполнение асинхронной команды
            //Здесь вроде как не надо ничего приравнивать к int, ранее забыл удалить
            int rowsAffected = await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
        }

        public async Task InsertFile(string filePath)
        {

            string query = $"copy test (asset) from '{filePath}' with (format csv, HEADER true)";

            //Асинхронное подключение позволяет одновременно записывать информацию с разных устройств
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();
            await using var command = new NpgsqlCommand(query, connection, transaction);
            //command.CommandText = AddTableValuesToQuery(insertData, command);
            //Выполнение асинхронной команды
            int rowsAffected = await command.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }

        public Table ConvertValues(Table table)
        {
            Table convertedTable = new Table(table.SourceTable, table.TargetTable);
            foreach (var column in table.Columns)
            {
                Column convertedColumn = new Column(column.SourceColumn, column.TargetColumn, column.DataType);
                foreach (var value in column.Data)
                {
                    if (value is null)
                    {
                        //Добавлять в лог
                        convertedColumn.Data.Add(Activator.CreateInstance(column.DataType));
                    }
                    else if (value.GetType() == column.DataType)
                    {
                        convertedColumn.Data.Add(value);
                    }
                    else if (value.GetType() != column.DataType)
                    {
                        //try
                        //{
                        convertedColumn.Data.Add(Convert.ChangeType(value.ToString(), column.DataType));
                        //}
                        /*
                        catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
                        {
                            //Добавлять в лог
                            convertedColumn.Data.Add(Activator.CreateInstance(column.DataType));
                        }
                        */
                    }
                }
                convertedTable.Columns.Add(convertedColumn);
            }
            if (convertedTable.Columns.Count() != 0)
            {
                return convertedTable;
            }
            return table;
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

        private string CreateParams(string query, Table insertTable)
        {
            StringBuilder insertParams = new StringBuilder();
            int maxRows = insertTable.Columns.Select(column => column.Data.Count).Max();
            int maxCols = insertTable.Columns.Count;

            //Формирование списка параметров для SQL-запроса вида: '@p{maxCols * row + col}'.
            for (int row = 0; row < maxRows; row++)
            {
                List<string> rowParams = new List<string>();

                for (int col = 0; col < maxCols; col++)
                {
                    rowParams.Add($"@p{maxCols * row + col}");
                }

                string currentRow = string.Join(", ", rowParams);
                insertParams.Append($"({currentRow})");

                if (row != maxRows - 1)
                {
                    insertParams.Append(", ");
                }
            }

            //Замена в запросе @InsertValues на список параметров '@p{maxCols * row + col}'
            query = query.Replace("(@InsertValues)", insertParams.ToString());
            return query;
        }

        private string AddValuesToParams(Table insertTable, NpgsqlCommand command)
        {
            int maxRows = insertTable.Columns.Select(column => column.Data.Count).Max();
            int maxCols = insertTable.Columns.Count;

            //Добавление пользовательских данных в параметры запроса
            for (int row = 0; row < maxRows; row++)
            {
                for (int col = 0; col < maxCols; col++)
                {
                    string currentParam = $"@p{maxCols * row + col}";
                    var currentValue = insertTable[col][row];
                    command.Parameters.AddWithValue(currentParam, currentValue);
                }
            }

            return command.CommandText;
        }

        private async Task UploadValidTablesAsync()
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            string query = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'";
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(query, connection);
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (!_validTables.Keys.Any(table => table.Title == reader.GetString("table_name")))
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
            command.Parameters.AddWithValue("@tableName", validTable.Title);
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                validTable.ValidColumns[reader.GetString("column_name")] = true;
            }
        }
    }
}
