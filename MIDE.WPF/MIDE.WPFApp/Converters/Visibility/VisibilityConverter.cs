using System;
using System.Globalization;
using WIN = System.Windows;
using APIC = MIDE.API.Components;

namespace MIDE.WPFApp.ValueConverters
{
    public class VisibilityConverter : BaseValueConverter<VisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is APIC.Visibility visibility)
            {
                switch (visibility)
                {
                    case APIC.Visibility.Collapsed: return WIN.Visibility.Collapsed;
                    case APIC.Visibility.Hidden: return WIN.Visibility.Hidden;
                    case APIC.Visibility.Visible: return WIN.Visibility.Visible;
                }
            }
            throw new ArgumentException("Not a visibility value given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}