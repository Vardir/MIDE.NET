using System;
using System.Linq;
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
        private GridRow newRowTemplate;
        private GridColumn newColumnTemplate;

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
        public GridRow NewRowTemplate
        {
            get => newRowTemplate?.Clone() ?? (newRowTemplate = new GridRow(new GridLength("auto")));
            set => newRowTemplate = value;
        }
        public GridColumn NewColumnTemplate
        {
            get => newColumnTemplate?.Clone() ?? (newColumnTemplate = new GridColumn(new GridLength("*")));
            set => newColumnTemplate = value;
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

        public void AddChild(LayoutComponent component, int row, int column, int rowSpan = 1, int colSpan = 1)
        {
            if (IsSealed)
                throw new InvalidOperationException("Can not add any child elements into a sealed control");
            if (Rows.OutOfRange(row))
                throw new IndexOutOfRangeException("Row index is out of range");
            if (Columns.OutOfRange(column))
                throw new IndexOutOfRangeException("Column index is out of range");
            if (rowSpan < 1)
                throw new ArgumentException("Row span can not be lower than 1");
            if (colSpan < 1)
                throw new ArgumentException("Column span can not be loser than 1");

            int index = Children.FirstIndexWith(cell => cell.Row == row && cell.Column == column);
            if (index == -1)
            {
                Children.Add(new GridCell(component, row, column)
                {
                    RowSpan = rowSpan,
                    ColumnSpan = colSpan
                });
            }
            else
                Children[index].Component = component;
        }
        public void AddRow(IEnumerable<LayoutComponent> components)
        {
            if (IsSealed)
                throw new InvalidOperationException("Can not add any child elements into a sealed control");
            if (Columns.Count != components.Count())
                throw new IndexOutOfRangeException("Not appropriate count of columns");

            Rows.Add(NewRowTemplate);
            int row = Rows.Count - 1;
            int column = 0;
            foreach (var component in components)
            {
                Children.Add(new GridCell(component, row, column++));
            }
        }
        public void DropRow(int index)
        {
            if (Rows.OutOfRange(index))
                throw new IndexOutOfRangeException("Row index was out of range");
            ClearRow(index);
            Rows.RemoveAt(index);
        }
        public void DropColumn(int index)
        {
            if (Columns.OutOfRange(index))
                throw new IndexOutOfRangeException("Column index was out of range");
            ClearColumn(index);
            Columns.RemoveAt(index);
        }
        public void ClearRow(int index)
        {
            if (Rows.OutOfRange(index))
                throw new IndexOutOfRangeException("Row index was out of range");

            GridCell[] cells = Children.Where(cell => cell.Row == index).ToArray();
            for (int i = 0; i < cells.Length; i++)
            {
                Children.Remove(cells[i]);
            }
        }
        public void ClearColumn(int index)
        {
            if (Columns.OutOfRange(index))
                throw new IndexOutOfRangeException("Column index was out of range");

            GridCell[] cells = Children.Where(cell => cell.Column == index).ToArray();
            for (int i = 0; i < cells.Length; i++)
            {
                Children.Remove(cells[i]);
            }
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
        public void EnsureRowCount(int count) => Rows.EnsureCount(() => NewRowTemplate, count);
        public void EnsureColumnCount(int count) => Columns.EnsureCount(() => NewColumnTemplate, count);

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
        private int rowSpan;
        private int columnSpan;
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
        public int RowSpan
        {
            get => rowSpan;
            set
            {
                if (value == rowSpan)
                    return;
                rowSpan = value > 0 ? value : 1;
                OnPropertyChanged(nameof(RowSpan));
            }
        }
        public int ColumnSpan
        {
            get => columnSpan;
            set
            {
                if (value == columnSpan)
                    return;
                columnSpan = value > 0 ? value : 1;
                OnPropertyChanged(nameof(ColumnSpan));
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

        public GridColumn Clone()
        {
            return new GridColumn(Width)
            {
                MinWidth = minWidth, MaxWidth = maxWidth
            };
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

        public GridRow Clone()
        {
            return new GridRow(Height)
            {
                MinHeight = minHeight, MaxHeight = maxHeight
            };
        }
        
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}