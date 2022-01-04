using System;
using System.Globalization;
using UniversityDesktop.MVVM.Core.Converter;

namespace UniversityDesktop.Converters
{
    public sealed class AuthenticationConverter : MultiConverterBase<AuthenticationConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Tuple<string, string> tuple = new Tuple<string, string>(
                (string)values[0], (string)values[1]);
            return (object)tuple;
        }
    }
}