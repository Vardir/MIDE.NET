using System;
using MIDE.Helpers;
using MIDE.API.ViewModels;
using System.Collections.ObjectModel;

namespace MIDE.API.Components
{
    public class ComboBox : LayoutComponent
    {
        private int selectedIndex;
        private ComboBoxItem selectedItem;

        public int ItemsCount => Items.Count;
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                if (value == selectedIndex)
                    return;
                selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        public ComboBoxItem SelectedItem
        {
            get => selectedItem;
            set
            {
                if (value == selectedItem)
                    return;
                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
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
            int index = Items.FirstIndexWith(cbi => cbi.Value == obj);
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
        private object value;
        private string caption;

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
        public string Caption
        {
            get => caption;
            set
            {
                if (value == caption)
                    return;
                caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public ComboBoxItem(object value, string caption)
        {
            Caption = caption;
            Value = value;
        }
    }
}