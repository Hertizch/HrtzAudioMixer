using System;
using System.Globalization;
using System.Windows.Data;

namespace HrtzAudioMixer.Converters
{
    public class HeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var value1 = (double) values[0];
            var value2 = (double) values[1];
            var value3 = (double) values[2];

            return value1 + value2 + value3;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
