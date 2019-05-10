using System;

namespace NodeGraphs.DataModels
{
    public class Axis
    {
        private int start = -200;
        private int end = 200;
        private int axisBasis = 0;

        public int Start
        {
            get => start;
            set
            {
                if (value >= End)
                    return;
                start = value;
                OnLengthChanged();
            }
        }
        public int End
        {
            get => end;
            set
            {
                if (value <= start)
                    return;
                end = value;
                OnLengthChanged();
            }
        }
        public int Basis => axisBasis;
        public int Length => end - start;

        public event Action<Axis> LengthChanged;

        public void Set(int start, int end)
        {
            this.start = Math.Min(start, end);
            this.end = Math.Max(start, end);
            OnLengthChanged();
        }

        private void OnLengthChanged()
        {
            EnsureBasis();
            LengthChanged?.Invoke(this);
        }
        private void EnsureBasis()
        {
            if (start > 0)
            {
                axisBasis = start;
            }
            else if (end < 0)
            {
                axisBasis = end;
            }
            else axisBasis = 0;
        }
    }
}
