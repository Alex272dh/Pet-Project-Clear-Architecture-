using PetProject.Data.Mapping;
using PetProject.MenuActionItemInterface;
using PetProject.MenuItemBaseInterface;
using PetProject.UIMenuBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetProject.UITableChoiceMenu;

namespace PetProject.ActionChoiceMenu
{
    public sealed class ActionChoiceMenu : MenuBase
    {
        public override List<IMenuItemBase> MenuItems { get; } = new List<IMenuItemBase>
        {
            new MenuActionItem("> Добавить запись", () => new TableChoiceMenu())
        };

        public override void Display()
        {
            int current = 0;
            Console.CursorVisible = false;

            while (true)
            {
                UpdateMenu(current);
                var key = Console.ReadKey().Key;

                if (key == ConsoleKey.UpArrow)
                {
                    current = (current - 1 + MenuItems.Count) % MenuItems.Count;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    current = (current + 1) % MenuItems.Count;
                }
                else if (key == ConsoleKey.Enter)
                {
                    ((MenuActionItem)MenuItems[current]).Selected = true;
                    break;
                }
            }

            Console.CursorVisible = true;
        }

        private void UpdateMenu(int current)
        {
            int contentsHight = 0;

            Console.Clear();
            DisplayContentString("======================================= Работа с СУБД ======================================", ref contentsHight);
            DisplayContentString("============================================= Меню выбора действий с СУБД ==============================================", ref contentsHight);
            DisplayContentString("", ref contentsHight);
            DisplayContentString("↑/↓ — выбор, Enter — выполнить, Esc — выход", ref contentsHight);
            DisplayContentString("", ref contentsHight);

            for (int i = 0; i < MenuItems.Count; i++)
            {
                if (i == current)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{MenuItems[i].Title}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{MenuItems[i].Title}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("");
            Console.WriteLine("=====================================================================================================================");
        }

        private void DisplayContentString(string content, ref int contentsHight)
        {
            Console.WriteLine(content);
            contentsHight++;
        }
    }
}
