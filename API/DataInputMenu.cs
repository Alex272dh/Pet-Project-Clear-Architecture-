using PetProject.DataBase.Scheme.Entities;
using PetProject.MenuInputItemInterface;
using PetProject.MenuItemBaseInterface;
using PetProject.UIMenuBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetProject.DataBase.Scheme;

namespace PetProject.UIDataInputMenu
{
    public class DataInputMenu : MenuBase
    {
        public override List<IMenuItemBase> MenuItems { get; } = new List<IMenuItemBase>{};

        public override void Display()
        {
            int inputStart = 0;
            int cursorPosition = 0;
            int selected = 0;
            int contentsHeight = 0;

            Console.Clear();
            DisplayString("======================================= Работа с СУБД ======================================", ref contentsHeight);
            DisplayString("============================================ Меню ввода значений ==========================================", ref contentsHeight);
            DisplayString("", ref contentsHeight);
            DisplayString("↑/↓ — выбор, Enter — выполнить, Esc — выход", ref contentsHeight);
            DisplayString("", ref contentsHeight);
            DisplayString("Столбцы таблицы:", ref contentsHeight);

            foreach (var item in MenuItems)
            {
                Console.WriteLine("> " + item.Title + ":");
            }

            int commonHeight = contentsHeight + MenuItems.Count;
            DisplayString("", ref commonHeight);
            DisplayString("=====================================================================================================================", ref commonHeight);

            while (true)
            {
                inputStart = MenuItems[selected].Title.Length;
                Console.SetCursorPosition(inputStart + cursorPosition, selected + contentsHeight);

                var input = Console.ReadKey(intercept: true);
                var key = input.Key;
                var character = input.KeyChar;

                if (key == ConsoleKey.Enter)
                {
                    break;
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

                    cursorPosition = ((MenuInputItem)MenuItems[selected]).Value.Length;
                }
                else if (key == ConsoleKey.DownArrow || key == ConsoleKey.Tab)
                {
                    selected = (selected + 1) % MenuItems.Count;
                    cursorPosition = ((MenuInputItem)MenuItems[selected]).Value.Length;
                }
                else if (key == ConsoleKey.RightArrow)
                {
                    if (cursorPosition < ((MenuInputItem)MenuItems[selected]).Value.Length)
                    {
                        try
                        {
                            cursorPosition++;
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            Console.SetBufferSize(Console.BufferWidth * 2, Console.BufferHeight);
                        }
                    }
                }
                else if (key == ConsoleKey.LeftArrow)
                {
                    if (cursorPosition > 0)
                    {
                        cursorPosition--;
                    }
                }
                else if (key == ConsoleKey.Backspace)
                {
                    if (cursorPosition > 0)
                    {
                        ((MenuInputItem)MenuItems[selected]).Value.Remove(cursorPosition - 1, 1);
                        cursorPosition--;

                        RedrawCurrentLine(inputStart, cursorPosition, selected, contentsHeight, true);
                    }
                }
                else if (key == ConsoleKey.Delete)
                {
                    if (cursorPosition < ((MenuInputItem)MenuItems[selected]).Value.Length)
                    {
                        ((MenuInputItem)MenuItems[selected]).Value.Remove(cursorPosition, 1);

                        RedrawCurrentLine(inputStart, cursorPosition, selected, contentsHeight, true);
                    }
                }
                else if (!char.IsControl(character))
                {
                    ((MenuInputItem)MenuItems[selected]).Value.Insert(cursorPosition, character);
                    cursorPosition++;

                    RedrawCurrentLine(inputStart, cursorPosition, selected, contentsHeight, false);
                }
                else if (key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }
            }
        }

        private void RedrawCurrentLine(int inputStart, int cursorPosition, int selected, int contentsHeight, bool deleteLastCharacter)
        {
            try
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(inputStart, selected + contentsHeight);
                Console.Write(((MenuInputItem)MenuItems[selected]).Value.ToString());

                if (deleteLastCharacter)
                {
                    Console.SetCursorPosition(inputStart + ((MenuInputItem)MenuItems[selected]).Value.Length, selected + contentsHeight);
                    Console.Write(" ");
                }

                Console.CursorVisible = true;
                Console.SetCursorPosition(inputStart + cursorPosition, selected + contentsHeight);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.SetBufferSize(Console.BufferWidth * 2, Console.BufferHeight);
            }
        }

        private void DisplayString(string content, ref int contentsHeight)
        {
            Console.WriteLine(content);
            contentsHeight++;
        }
    }
}
