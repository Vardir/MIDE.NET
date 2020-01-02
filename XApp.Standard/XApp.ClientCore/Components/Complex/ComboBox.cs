using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Vardirsoft.Shared.MVVM;
using Vardirsoft.Shared.Helpers;

namespace Vardirsoft.XApp.Components
{
    public class ComboBox : LayoutComponent
    {
        private int _selectedIndex;
        private ComboBoxItem _selectedItem;

        public int ItemsCount { [DebuggerStepThrough] get => Items.Count; }
        
        public int SelectedIndex
        {
            [DebuggerStepThrough]
            get => _selectedIndex;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _selectedIndex, value);
        }
        
        public ComboBoxItem SelectedItem
        {
            [DebuggerStepThrough]
            get => _selectedItem;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _selectedItem, value, true);
        }
        
        public ObservableCollection<ComboBoxItem> Items { get; }

        public ComboBox(string id) : base(id)
        {
            Items = new ObservableCollection<ComboBoxItem>();
        }

        public void Add(object obj, string caption)
        {
            Items.Add(new ComboBoxItem(obj, caption));
        }
        public void Remove(object obj)
        {
            var index = Items.IndexWith(cbi => cbi.Value == obj);

            if (index == -1)
                return;

            Items.RemoveAt(index);
        }
        public void Clear()
        {
            Items.Clear();
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            throw new NotImplementedException();
        }
    }

    public class ComboBoxItem : BaseViewModel
    {
        private object _value;
        private string _caption;

        public object Value
        {
            [DebuggerStepThrough]
            get => _value;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref this._value, value, true);
        }
        
        public string Caption
        {
            [DebuggerStepThrough]
            get => _caption;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _caption, value, true);
        }

        public ComboBoxItem(object value, string caption)
        {
            Caption = caption;
            Value = value;
        }
    }
}