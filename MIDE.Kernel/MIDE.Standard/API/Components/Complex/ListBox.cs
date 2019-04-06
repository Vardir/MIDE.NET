using System;
using MIDE.Helpers;
using MIDE.API.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MIDE.API.Components
{
    public class ListBox : LayoutComponent
    {
        private bool isMultiselect;

        public bool IsMultiselect
        {
            get => isMultiselect;
            set
            {
                if (isMultiselect == value)
                    return;
                isMultiselect = value;
                OnPropertyChanged(nameof(IsMultiselect));
            }
        }
        public List<ListBoxItem> SelectedItems { get; }
        public ObservableCollection<ListBoxItem> Items { get; }

        public ListBox(string id) : base(id)
        {
            SelectedItems = new List<ListBoxItem>();
            Items = new ObservableCollection<ListBoxItem>();
        }

        public void Add(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            ListBoxItem item = new ListBoxItem();
            item.SelectedChanged += ItemSelectedChanged;
            item.Value = value;
            Items.Add(item);
        }
        public void Remove(object value)
        {
            int index = Items.IndexOf(item => item.Value == value);
            Items[index].SelectedChanged -= ItemSelectedChanged;
            Items.RemoveAt(index);
        }
        
        protected override LayoutComponent CloneInternal(string id)
        {
            ListBox clone = Create(id);
            return clone;
        }
        protected virtual ListBox Create(string id) => new ListBox(id);

        private void ItemSelectedChanged(ListBoxItem item, bool selected)
        {
            if (selected)
                SelectedItems.Add(item);
            else
                SelectedItems.Remove(item);
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
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        public object Value
        {
            get => value;
            set
            {
                if (this.value == value)
                    return;
                this.value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public event Action<ListBoxItem, bool> SelectedChanged;

        public ListBoxItem()
        {

        }
    }
}