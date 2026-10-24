using PetProject.MenuItemBaseInterface;

namespace PetProject.MenuBaseInterface
{
    public interface IMenuBase
    {
        public List<IMenuItemBase> MenuItems { get; }
        public void Display();
    }
}
