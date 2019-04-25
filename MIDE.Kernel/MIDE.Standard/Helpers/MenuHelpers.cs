using MIDE.API.Components;
using System.Collections.ObjectModel;

namespace MIDE.Helpers
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
                    if (fGreater == -1)
                        menuItems.Add(item);
                    else
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
        internal static void Insert(this MenuItem parent, MenuItem item, short minOrdinal, short maxOrdinal)
        {
            int ordinal = item.OrdinalIndex;

            MenuGroup group = parent.GetMenuGroup(item.Group);
            if (group == null)
            {
                item.Group = MenuItem.DEFAULT_GROUP;
                group = parent.GetMenuGroup(MenuItem.DEFAULT_GROUP);
            }
            var pairs = group.GetOrdinals();

            if (pairs.Length == 0)
            {
                menuItems.Add(item);//Replace
                return;
            }
            if (ordinal < 0)
            {
                if (minOrdinal.BothEquals(pairs[0].ordinal, ordinal))
                {
                    if (pairs.Length > 1)
                        menuItems.Insert(1, item);//Replace
                    else
                        menuItems.Add(item);//Replace
                }
                int lastLesser = pairs.LastIndexWith(i => i.ordinal < ordinal);
                if (lastLesser == -1)
                {
                    int fGreater = pairs.FirstIndexWith(i => i.ordinal > ordinal);
                    if (fGreater == -1)
                        menuItems.Add(item);//Replace
                    else
                        menuItems.Insert(fGreater, item);//Replace
                }
                else if (lastLesser == pairs.Length - 1)
                    menuItems.Add(item);//Replace
                else
                    menuItems.Insert(lastLesser + 1, item);//Replace
                return;
            }

            if (maxOrdinal.BothEquals(pairs[pairs.Length - 1].ordinal, ordinal))
            {
                menuItems.Insert(pairs.Length - 1, item);//Replace
                return;
            }
            int firstGreater = pairs.FirstIndexWith(i => i.ordinal > ordinal);
            if (firstGreater == -1)
                menuItems.Add(item);//Replace
            else
                menuItems.Insert(firstGreater, item);//Replace
        }

    }
}