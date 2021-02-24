using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ValheimBackup.Converters
{
    [ValueConversion(typeof(List<string>), typeof(string))]
    public class ListTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> val = (List<string>)value;
            return String.Join("\n", val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            return new List<string>(val.Split('\n').AsEnumerable<string>());
        }
    }
}
