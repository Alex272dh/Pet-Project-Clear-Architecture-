using PetProject.MenuInputItemInterface;
using PetProject.MenuItemBaseInterface;
using PetProject.UIDataInputMenu;
using PetProject.UIMenuBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetProject.UIDataTypeInputMenu
{
    public class DataTypeInputMenu : MenuBase
    {
        public override List<IMenuItemBase> MenuItems { get; } = new List<IMenuItemBase> { };

        public void CreateMenuItem(string title)
        {
            MenuItems.Add(new MenuInputItem("> " + title + "(тип значения): "));
        }

        public override void Display()
        {
            int inputStart = 0;
            int cursorPosition = 0;
            int selected = 0;
            int contentsHeight = 0;

            Console.Clear();
            DisplayString("======================================= Работа с СУБД ======================================", ref contentsHeight);
            DisplayString("============================================ Меню ввода типа значений ==========================================", ref contentsHeight);
            DisplayString("", ref contentsHeight);
            DisplayString("↑/↓ — выбор, Enter — выполнить, Esc — выход", ref contentsHeight);
            DisplayString("", ref contentsHeight);
            DisplayString("Список доступных типов значений:", ref contentsHeight);
            DisplayString("s - string", ref contentsHeight);
            DisplayString("i - int", ref contentsHeight);
            DisplayString("f - float", ref contentsHeight);
            DisplayString("d - DateTime", ref contentsHeight);
            DisplayString("", ref contentsHeight);
            DisplayString("Столбцы таблицы:", ref contentsHeight);

            foreach (var item in MenuItems)
            {
                DisplayString(item.Title, ref contentsHeight);
            }

            while (true)
            {
                inputStart = MenuItems[selected].Title.Length;
                Console.SetCursorPosition(inputStart + cursorPosition, selected + contentsHeight);

                var input = Console.ReadKey(intercept: true);
                var key = input.Key;
                var character = input.KeyChar;

                if (key == ConsoleKey.Enter)
                {
                    ValidateDataTypes();
                    break;
                }
                else if (key == ConsoleKey.Tab || key == ConsoleKey.DownArrow)
                {
                    selected = (selected + 1) % MenuItems.Count;
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    if (selected > 0)
                    {
                        selected = (selected - 1) % MenuItems.Count;
                    }
                    else
                    {
                        selected = MenuItems.Count - 1;
                    }
                }
                else if (key == ConsoleKey.Backspace)
                {
                    cursorPosition++;
                    Console.SetCursorPosition(inputStart + cursorPosition, selected + 7);
                    DeleteCharacters(1);
                    cursorPosition--;
                }
                else if (!char.IsControl(character))
                {
                    if (MenuInputItem.AllowedDataTypes.ContainsKey(character))
                    {
                        ((MenuInputItem)MenuItems[selected]).DataType = MenuInputItem.AllowedDataTypes[character];
                    }

                    Console.Write(character);
                }
            }
        }

        public void ValidateDataTypes()
        {
            foreach (var item in MenuItems)
            {
                if (!MenuInputItem.AllowedDataTypes.ContainsValue(((IMenuInputItem)item).DataType))
                {
                    throw new Exception($"Для поля {item.Title} указан недопустимый тип данных.");
                }
            }
        }

        private void DisplayString(string content, ref int contentsHeight)
        {
            Console.WriteLine(content);
            contentsHeight++;
        }

        private void DeleteCharacters(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                Console.Write("\b");
                Console.Write(" ");
                Console.Write("\b");
            }
        }
    }
}