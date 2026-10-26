using PetProject.MenuActionItemInterface;
using PetProject.MenuBaseInterface;
using PetProject.MenuInputItemInterface;
using PetProject.MenuItemBaseInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetProject.UIMenuBase
{
    public abstract class MenuBase : IMenuBase
    {
        public abstract List<IMenuItemBase> MenuItems { get; }

        public abstract void Display();


        protected class MenuInputItem : IMenuInputItem
        {
            public static Dictionary<char, Type> AllowedDataTypes = new Dictionary<char, Type>
            {
                {'s', typeof(string)}, {'i', typeof(int)}, {'f', typeof(float)}, {'d', typeof(DateTime)}
            };

            public string Title { get; private set; }

            public StringBuilder Value { get; set; } = new StringBuilder();

            public Type? DataType { get; set; }

            public MenuInputItem(string title)
            {
                Title = title;
            }
        }
        
        protected class MenuActionItem : IMenuActionItem
        {
            public string Title { get; private set; }

            public bool Selected { get; set; }

            public Func<IMenuBase> Action { get; set; }
            
            public MenuActionItem(string title, Func<IMenuBase> action)
            {
                Title = title;
                Action = action;
                Selected = false;
            }
        }
    }
}
