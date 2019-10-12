using System;
using System.Collections.ObjectModel;

using Vardirsoft.Shared.MVVM;
using Vardirsoft.Shared.Helpers;

namespace Vardirsoft.XApp.Components
{
    public class ComboBox : LayoutComponent
    {
        private int selectedIndex;
        private ComboBoxItem selectedItem;

        public int ItemsCount => Items.Count;
        public int SelectedIndex
        {
            get => selectedIndex;
            set => SetWithNotify(ref selectedIndex, value);
        }
        public ComboBoxItem SelectedItem
        {
            get => selectedItem;
            set => SetWithNotify(ref selectedItem, value, true);
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
        private object value;
        private string caption;

        public object Value
        {
            get => value;
            set => SetWithNotify(ref this.value, value, true);
        }
        public string Caption
        {
            get => caption;
            set => SetWithNotify(ref caption, value, true);
        }

        public ComboBoxItem(object value, string caption)
        {
            Caption = caption;
            Value = value;
        }
    }
}