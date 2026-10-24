using PetProject.Data.Mapping;
using OfficeOpenXml;

namespace PetProject.ExcelParser
{
    public class ExcelParser
    {
        public Table Parse(Table parsedTable, string filePath)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Александр Демидов");//Без указания лицензии не работает

            //Чтение файла
            using var package = new ExcelPackage(new FileInfo(filePath));
            //Здесь предполагается, что в Excel гарантированно только один лист
            var worksheets = package.Workbook.Worksheets[0];
            int maxRows = worksheets.Dimension?.End.Row ?? 0;
            int maxCols = worksheets.Dimension?.End.Column ?? 0;

            //Заполняем parsedData в соответствии с полями SourceHeader
            for (int col = 1; col <= maxCols; col++)
            {
                string columnHeader = "";
                int columnHeaderRow = 0;
                for (int row = 1; row <= maxRows; row++)
                {
                    if (!parsedTable.ContainsSourceColumn(columnHeader))
                    {
                        columnHeader = worksheets.Cells[row, col]?.Text.ToString() ?? "";
                        columnHeaderRow = row;
                    }
                    if (parsedTable.ContainsSourceColumn(columnHeader) && row > columnHeaderRow)
                    {
                        var targetColumn = parsedTable.Columns.Where(column => column.SourceColumn == columnHeader).FirstOrDefault();
                        targetColumn.Data.Add(worksheets.Cells[row, col]?.Text.ToString() ?? "");
                    }
                    if (!parsedTable.ContainsSourceColumn(columnHeader) && !string.IsNullOrEmpty(columnHeader))
                    {
                        //Добавлять в лог
                        string columnHeadersList = string.Join(", ", parsedTable.Columns.Select(column => $"\"{column.SourceColumn}\""));
                        Console.WriteLine($"Value \"{columnHeader}\" in the cell [{row}][{col}] is not in the list of column headers: {columnHeadersList}.");
                    }
                }
            }
            return parsedTable;
        }
    }
}
