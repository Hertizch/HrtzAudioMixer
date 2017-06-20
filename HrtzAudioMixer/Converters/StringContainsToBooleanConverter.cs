using System;
using System.Globalization;
using System.Windows.Data;

namespace HrtzAudioMixer.Converters
{
    public class StringContainsToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value.ToString();
            var param = parameter.ToString();

            var res = culture.CompareInfo.IndexOf(val, param, CompareOptions.IgnoreCase);

            var result = res.Equals(0);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
