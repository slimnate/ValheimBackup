using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ValheimBackup.BO;

namespace ValheimBackup.Converters
{
    [ValueConversion(typeof(WorldSelection), typeof(bool))]
    public class WorldSelectionBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null || (WorldSelection)value == WorldSelection.All)
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;
            if(val)
            {
                return WorldSelection.Specific;
            } else
            {
                return WorldSelection.All;
            }
        }
    }
}
