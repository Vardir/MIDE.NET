using System;
using MIDE.Helpers;
using MIDE.API.Measurements;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MIDE.API.Components
{
    public class GridLayout : LayoutContainer
    {
        private GridLength rowMargin;
        private GridLength columnMargin;

        /// <summary>
        /// A space between rows
        /// </summary>
        public GridLength RowMargin
        {
            get => rowMargin;
            set
            {
                if (value == rowMargin)
                    return;
                rowMargin = value;
                OnPropertyChanged(nameof(RowMargin));
            }
        }
        /// <summary>
        /// A space between columns
        /// </summary>
        public GridLength ColumnMargin
        {
            get => columnMargin;
            set
            {
                if (value == columnMargin)
                    return;
                columnMargin = value;
                OnPropertyChanged(nameof(ColumnMargin));
            }
        }
        public ObservableCollection<GridRow> Rows { get; }
        public ObservableCollection<GridColumn> Columns { get; }

        public GridLayout(string id) : base(id)
        {
            Rows = new ObservableCollection<GridRow>();
            Columns = new ObservableCollection<GridColumn>();
        }

        public override void AddChild(LayoutComponent component)
        {
            throw new System.NotImplementedException();
        }
        public void AddChild(LayoutComponent component, int row, int column)
        {
            if (Rows.OutOfRange(row))
                throw new IndexOutOfRangeException("Row index is out of range");
            if (Columns.OutOfRange(column))
                throw new IndexOutOfRangeException("Column index is out of range");
            Rows[row][column] = component;
        }
        public override void RemoveChild(string id)
        {
            throw new System.NotImplementedException();
        }
        public override void RemoveChild(LayoutComponent component)
        {
            throw new System.NotImplementedException();
        }

        public override bool Contains(string id)
        {
            throw new System.NotImplementedException();
        }
        public override LayoutComponent Find(string id)
        {
            throw new System.NotImplementedException();
        }
    }

    public class GridColumn : INotifyPropertyChanged
    {
        private GridLength width;
        private GridLength minWidth;
        private GridLength maxWidth;

        public GridLength Width
        {
            get => width;
            set
            {
                if (value == width || value < MinWidth || value > MaxWidth)
                    return;
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }
        public GridLength MinWidth
        {
            get => minWidth;
            set
            {
                if (value == minWidth || value > MaxWidth)
                    return;
                if (Width < value)
                    Width = value;
                minWidth = value;
                OnPropertyChanged(nameof(MinWidth));
            }
        }
        public GridLength MaxWidth
        {
            get => maxWidth;
            set
            {
                if (value == maxWidth || value < MinWidth)
                    return;
                if (Width > value)
                    Width = value;
                maxWidth = value;
                OnPropertyChanged(nameof(MaxWidth));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public class GridRow : INotifyPropertyChanged, INotifyCollectionChanged
    {
        private GridLength height;
        private GridLength minHeight;
        private GridLength maxHeight;

        private List<LayoutComponent> Cells { get; }
        
        public GridLength Height
        {
            get => height;
            set
            {
                if (value == height || value < MinHeight || value > MaxHeight)
                    return;
                height = value;
                OnPropertyChanged(nameof(Height));
            }
        }
        public GridLength MinHeight
        {
            get => minHeight;
            set
            {
                if (value == minHeight || value > MaxHeight)
                    return;
                if (Height < value)
                    Height = value;
                minHeight = value;
                OnPropertyChanged(nameof(MinHeight));
            }
        }
        public GridLength MaxHeight
        {
            get => maxHeight;
            set
            {
                if (value == maxHeight || value < MinHeight)
                    return;
                if (Height > value)
                    Height = value;
                maxHeight = value;
                OnPropertyChanged(nameof(MaxHeight));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public LayoutComponent this[int index]
        {
            get => Cells[index];
            set
            {
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, value, Cells[index], index);
                Cells[index] = value;
            }
        } 

        public GridRow()
        {
            Cells = new List<LayoutComponent>();
        }

        public int GetFirstEmptyCellIndex() => Cells.FindIndex(l => l == null);

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        protected void OnCollectionChanged(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            var args = new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index);
            CollectionChanged?.Invoke(this, args);
        }
    }
}