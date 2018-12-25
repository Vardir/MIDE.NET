using MIDE.API.Components;
using System.Collections.ObjectModel;

namespace MIDE.Standard.Helpers
{
    internal static class MenuHelpers
    {
        internal static void Insert(this Collection<MenuItem> menuItems, MenuItem item, short minOrdinal, short maxOrdinal)
        {
            int ordinal = item.OrdinalIndex;

            if (menuItems.Count == 0)
            {
                menuItems.Add(item);
                return;
            }
            if (ordinal < 0)
            {
                if (minOrdinal.BothEquals(menuItems[0].OrdinalIndex, ordinal))
                {
                    if (menuItems.Count > 1)
                        menuItems.Insert(1, item);
                    else
                        menuItems.Add(item);
                }
                int lastLesser = menuItems.LastIndexWith(i => i.OrdinalIndex < ordinal);
                if (lastLesser == -1)
                {
                    int fGreater = menuItems.FirstIndexWith(i => i.OrdinalIndex > ordinal);
                    menuItems.Insert(fGreater, item);
                }
                else if (lastLesser == menuItems.Count - 1)
                    menuItems.Add(item);
                else
                    menuItems.Insert(lastLesser + 1, item);
                return;
            }

            if (maxOrdinal.BothEquals(menuItems[menuItems.Count - 1].OrdinalIndex, ordinal))
            {
                menuItems.Insert(menuItems.Count - 1, item);
                return;
            }
            int firstGreater = menuItems.FirstIndexWith(i => i.OrdinalIndex > ordinal);
            if (firstGreater == -1)
                menuItems.Add(item);
            else
                menuItems.Insert(firstGreater, item);
        }
    }
}