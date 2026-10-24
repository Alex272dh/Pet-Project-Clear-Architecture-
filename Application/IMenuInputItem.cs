using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetProject.MenuItemBaseInterface;

namespace PetProject.MenuInputItemInterface
{
    public interface IMenuInputItem : IMenuItemBase
    {
        public static Dictionary<char, Type> AllowedDataTypes { get; }

        public string Title { get; }

        public StringBuilder Value { get; }

        public Type? DataType { get; set; }
    }
}