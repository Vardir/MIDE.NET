using MIDE.Helpers;
using System;
using System.Collections.Generic;

namespace MIDE.API.Components
{
    public class MenuGroup : ApplicationComponent
    {
        private int firstItemIndex;
        private int lastItemIndex;

        public short OrdinalIndex { get; }
        public int FirstItemIndex
        {
            get => firstItemIndex;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Index cat not be lesser than 0");
                if (value > lastItemIndex)
                    throw new ArgumentException("First index cannot be greater than last index");
                
                firstItemIndex = value;
            }
        }
        public int LastItemIndex
        {
            get => lastItemIndex;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Index cat not be lesser than 0");
                if (value < firstItemIndex)
                    throw new ArgumentException("Last index cannot be lesser than first index");
                lastItemIndex = value;
            }
        }
        public int ItemsCount => LastItemIndex - FirstItemIndex + 1;
        public MenuItem Container { get; }

        public MenuGroup(string id, short ordinal, MenuItem container) : base(id)
        {
            OrdinalIndex = ordinal;
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IEnumerable<MenuItem> GetItems()
        {
            if (ItemsCount == 0)
                yield break;
            for (int i = FirstItemIndex; i <= LastItemIndex; i++)
            {
                yield return Container.Children[i];
            }
        }
        public (int index, short ordinal)[] GetOrdinals()
        {
            return Container.Children.Select((index, item) => (index, item.OrdinalIndex));
        }
    }
}