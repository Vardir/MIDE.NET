using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Vardirsoft.Shared.MVVM;
using Vardirsoft.Shared.Helpers;

namespace Vardirsoft.XApp.Components
{
    [Obsolete]
    public class ListBox : LayoutComponent
    {
        private bool _isMultiselect;

        private readonly ObservableCollection<ListBoxItem> _items;

        public bool IsMultiselect
        {
            [DebuggerStepThrough]
            get => _isMultiselect;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _isMultiselect, value);
        }
        
        public List<ListBoxItem> SelectedItems { get; }
        
        public ReadOnlyObservableCollection<ListBoxItem> Items { get; }

        public ListBox(string id) : base(id)
        {
            SelectedItems = new List<ListBoxItem>();
            _items = new ObservableCollection<ListBoxItem>();
            Items = new ReadOnlyObservableCollection<ListBoxItem>(_items);
        }

        public void Add(object value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            var item = new ListBoxItem();

            item.SelectedChanged += ItemSelectedChanged;
            item.Value = value;

            _items.Add(item);
        }
        public void Remove(object value)
        {
            var index = Items.IndexWith(item => item.Value == value);

            Items[index].SelectedChanged -= ItemSelectedChanged;
            _items.RemoveAt(index);
        }
        public void Clear()
        {
            foreach (var item in Items)
            {
                item.SelectedChanged -= ItemSelectedChanged;
            }

            _items.Clear();
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

    [Obsolete]
    public class ListBoxItem : BaseViewModel
    {
        private bool _isSelected;
        private object _value;

        public bool IsSelected
        {
            [DebuggerStepThrough]
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;
                SelectedChanged?.Invoke(this, value);
                
                NotifyPropertyChanged();
            }
        }
        public object Value
        {
            [DebuggerStepThrough]
            get => _value;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref this._value, value, true);
        }

        public event Action<ListBoxItem, bool> SelectedChanged;
    }
}