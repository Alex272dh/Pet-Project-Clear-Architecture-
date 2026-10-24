using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetProject.MenuItemBaseInterface;
using PetProject.MenuBaseInterface;

namespace PetProject.MenuActionItemInterface
{
    public interface IMenuActionItem : IMenuItemBase
    {
        public string Title { get; }
        public Func<IMenuBase> Action { get; }
        public bool Selected { get; }
    }
}

