using OfficeOpenXml.Drawing.Style.Fill;
using PetProject.ActionChoiceMenu;
using PetProject.DataBase.Scheme;
using PetProject.MenuActionItemInterface;
using PetProject.MenuBaseInterface;
using PetProject.UIDataInputMenu;
using PetProject.UITableChoiceMenu;
using System.Threading.Tasks;
using PetProject.DataBase.Scheme.Entities;

public static class Program
{
    public static async Task Main()
    {
        ActionChoiceMenu actionChoiceMenu = new ActionChoiceMenu();
        actionChoiceMenu.Display();

        IMenuBase selectedMenu = ((IMenuActionItem)actionChoiceMenu.MenuItems.First(item => ((IMenuActionItem)item).Selected == true)).Action();
        
        RelationalRepository repository = new RelationalRepository("Host=localhost;Port=5433;UserId=postgres;Password=DA13q30by20NAT156;Database=Finances;"); // "Host=localhost;Port=5433;UserId=postgres;Password=DA13q30by20NAT156;Database=Finances;"
        List<ValidTable> validTables = new List<ValidTable>();

        while (selectedMenu is not null)
        {
            if (selectedMenu is TableChoiceMenu)
            {
                validTables = await repository.GetValidTablesAsync();

                foreach (var validTable in validTables)
                {
                    ((TableChoiceMenu)selectedMenu).CreateMenuItem(validTable.Title);
                }

                selectedMenu.Display();

                selectedMenu = ((IMenuActionItem)selectedMenu.MenuItems.First(item => ((IMenuActionItem)item).Selected == true)).Action();
                //selectedTable = selectedMenuMeu
            }
            else if (selectedMenu is DataInputMenu)
            {
                var validColumns = await repository.GetValidColumnsAsync(validTables);
            }
        }
    }
}