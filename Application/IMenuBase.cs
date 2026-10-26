using PetProject.MenuItemBaseInterface;

namespace PetProject.MenuBaseInterface
{
    public interface IMenuBase
    {
        List<IMenuItemBase> MenuItems { get; }

        public void Display();
    }
}
