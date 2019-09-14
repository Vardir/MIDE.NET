using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using XApp.API;
using XApp.Helpers;

namespace XApp.Components
{
    public class ListBox : LayoutComponent
    {
        private bool isMultiselect;
       
        protected ObservableCollection<ListBoxItem> mItems;

        public bool IsMultiselect
        {
            get => isMultiselect;
            set => SetAndNotify(value, ref isMultiselect);
        }
        public List<ListBoxItem> SelectedItems { get; }
        public ReadOnlyObservableCollection<ListBoxItem> Items { get; }

        public ListBox(string id) : base(id)
        {
            SelectedItems = new List<ListBoxItem>();
            mItems = new ObservableCollection<ListBoxItem>();
            Items = new ReadOnlyObservableCollection<ListBoxItem>(mItems);
        }

        public void Add(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var item = new ListBoxItem();

            item.SelectedChanged += ItemSelectedChanged;
            item.Value = value;

            mItems.Add(item);
        }
        public void Remove(object value)
        {
            var index = Items.IndexOf(item => item.Value == value);

            Items[index].SelectedChanged -= ItemSelectedChanged;
            mItems.RemoveAt(index);
        }
        public void Clear()
        {
            foreach (var item in Items)
            {
                item.SelectedChanged -= ItemSelectedChanged;
            }

            mItems.Clear();
        }
        
        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = Create(id);

            return clone;
        }
        protected virtual ListBox Create(string id) => new ListBox(id);

        private void ItemSelectedChanged(ListBoxItem item, bool selected)
        {
            if (selected)
            {
                SelectedItems.Add(item);
            }
            else
            {
                SelectedItems.Remove(item);
            }
        }
    }

    public class ListBoxItem : BaseViewModel
    {
        private bool isSelected;
        private object value;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected == value)
                    return;

                isSelected = value;
                SelectedChanged?.Invoke(this, value);
                
                OnPropertyChanged();
            }
        }
        public object Value
        {
            get => value;
            set => SetAndNotify(value, ref this.value);
        }

        public event Action<ListBoxItem, bool> SelectedChanged;

        public ListBoxItem()
        {

        }
    }
}