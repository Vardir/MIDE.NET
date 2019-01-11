using System;
using MIDE.Helpers;
using MIDE.API.Measurements;
using System.ComponentModel;
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
        public ObservableCollection<GridCell> Children { get; }
        
        public GridLayout(string id) : base(id)
        {
            Rows = new ObservableCollection<GridRow>();
            Columns = new ObservableCollection<GridColumn>();
            Children = new ObservableCollection<GridCell>();
        }

        public void AddChild(LayoutComponent component, int row, int column)
        {
            if (IsSealed)
                throw new InvalidOperationException("Can not add any child elements into a sealed control");
            if (Rows.OutOfRange(row))
                throw new IndexOutOfRangeException("Row index is out of range");
            if (Columns.OutOfRange(column))
                throw new IndexOutOfRangeException("Column index is out of range");

            int index = Children.FirstIndexWith(cell => cell.Row == row && cell.Column == column);
            if (index == -1)
                Children.Add(new GridCell(component, row, column));
            else
                Children[index].Component = component;
        }
        public void RemoveChild(int row, int column)
        {
            if (IsSealed)
                throw new InvalidOperationException("Can not remove child elements from a sealed control");
            if (Rows.OutOfRange(row))
                throw new IndexOutOfRangeException("Row index is out of range");
            if (Columns.OutOfRange(column))
                throw new IndexOutOfRangeException("Column index is out of range");

            int index = Children.FirstIndexWith(cell => cell.Row == row && cell.Column == column);
            if (index == -1)
                return;
            Children.RemoveAt(index);
        }

        public override bool Contains(string id)
        {
            throw new System.NotImplementedException();
        }
        public override LayoutComponent Find(string id)
        {
            throw new System.NotImplementedException();
        }

        protected override void AddChild_Impl(LayoutComponent component)
        {
            AddChild(component, 0, 0);
        }
        protected override void RemoveChild_Impl(string id)
        {
            int index = Children.FirstIndexWith(cell => cell.Component.Id == id);
            if (index == -1)
                return;
            Children.RemoveAt(index);
        }
        protected override void RemoveChild_Impl(LayoutComponent component)
        {
            int index = Children.FirstIndexWith(cell => cell.Component == component);
            if (index == -1)
                return;
            Children.RemoveAt(index);
        }

    }

    public class GridCell : INotifyPropertyChanged
    {
        private int row;
        private int column;
        private LayoutComponent component;

        public int Row
        {
            get => row;
            set
            {
                if (value == row)
                    return;
                row = value;
                OnPropertyChanged(nameof(Row));
            }
        }
        public int Column
        {
            get => column;
            set
            {
                if (value == column)
                    return;
                column = value;
                OnPropertyChanged(nameof(Column));
            }
        }
        public LayoutComponent Component
        {
            get => component;
            set
            {
                if (value == component)
                    return;
                component = value;
                OnPropertyChanged(nameof(Component));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public GridCell(LayoutComponent component, int row, int column)
        {
            Row = row;
            Column = column;
            Component = component;
        }

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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

        public GridColumn()
        {
            Width = new GridLength("*");
            MinWidth = new GridLength(0);
            MaxWidth = new GridLength(double.MaxValue);
        }
        public GridColumn(GridLength width)
        {
            Width = width;
            MinWidth = new GridLength(0);
            MaxWidth = new GridLength(double.MaxValue);
        }

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public class GridRow : INotifyPropertyChanged
    {
        private GridLength height;
        private GridLength minHeight;
        private GridLength maxHeight;
        
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

        public GridRow()
        {
            Height = new GridLength("*");
            MinHeight = new GridLength(0);
            MaxHeight = new GridLength(double.MaxValue);
        }
        public GridRow(GridLength heigth)
        {
            Height = heigth;
            MinHeight = new GridLength(0);
            MaxHeight = new GridLength(double.MaxValue);
        }
        
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}