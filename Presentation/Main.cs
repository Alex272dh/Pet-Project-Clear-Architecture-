using OfficeOpenXml.Drawing.Style.Fill;
using PetProject.ActionChoiceMenu;
using PetProject.DataBase.Scheme;
using PetProject.MenuActionItemInterface;
using PetProject.MenuBaseInterface;
using PetProject.UIDataInputMenu;
using PetProject.UITableChoiceMenu;
using System.Threading.Tasks;
using PetProject.DataBase.Scheme.Entities;
using PetProject.Data.Mapping;
using PetProject.MenuInputItemInterface;
using PetProject.UIDataTypeInputMenu;
using PetProject.DataBase;

public static class Program
{
    public static async Task Main()
    {
        ActionChoiceMenu actionChoiceMenu = new ActionChoiceMenu();
        actionChoiceMenu.Display();

        IMenuBase selectedMenu = ((IMenuActionItem)actionChoiceMenu.MenuItems.First(item => ((IMenuActionItem)item).Selected == true)).Action();
        
        DataBase dataBase = new DataBase("Host=localhost;Port=5433;UserId=postgres;Password=DA13q30by20NAT156;Database=Finances;"); // "Host=localhost;Port=5433;UserId=postgres;Password=DA13q30by20NAT156;Database=Finances;"

        if (selectedMenu is TableChoiceMenu)
        {
            List<ValidTable> validTables = await dataBase.GetValidTablesAsync();

            foreach (var validTable in validTables)
            {
                ((TableChoiceMenu)selectedMenu).CreateMenuItem(validTable.Title);
            }

            selectedMenu.Display();

            DataInputMenu tableChoiceMenu = (DataInputMenu)((IMenuActionItem)selectedMenu.MenuItems.First(item => ((IMenuActionItem)item).Selected == true)).Action();
            Table selectedTable = new Table(selectedMenu.MenuItems.First(item => ((IMenuActionItem)item).Selected == true).Title);

            List<string> validColumns = await dataBase.GetValidColumnsAsync(new ValidTable(selectedTable.TargetTable));

            foreach (var validColumn in validColumns)
            {
                ((DataInputMenu)selectedMenu).CreateMenuItem(validColumn);
            }

            tableChoiceMenu.Display();

            List<IMenuInputItem>  selectedColumns = selectedMenu.MenuItems.Select(item => (IMenuInputItem)item).ToList();

            if (selectedColumns.Count != 0)
            {
                ((DataTypeInputMenu)selectedMenu).MenuItems.AddRange(selectedColumns);
                selectedMenu.Display();

                foreach (var column in selectedColumns)
                {
                    selectedTable.Add(new Column(column.Title, column.DataType, new List<dynamic> { column.Value }));
                }
            }

            await dataBase.Insert(selectedTable);
        }
    }
}