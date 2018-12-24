using System;
using System.Globalization;
using MIDE.WPFApp.RelayCommands;
using MIDE.Standard.API.Commands;

namespace MIDE.WPFApp.ValueConverters
{
    public class RelayCommandConverter : BaseValueConverter<RelayCommandConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value is RelayCommand apiCommand)
                return apiCommand.Cast<WindowsRelayCommand>();
            if (value is RelayParameterizedCommand apiParamCommand)
                return apiParamCommand.Cast<WindowsRelayParameterizedCommand>();
            throw new ArgumentException("Not a relay command value-type given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}