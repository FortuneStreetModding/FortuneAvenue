using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace Editor
{
    class OffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDouble(value) + System.Convert.ToDouble(parameter) - 32;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Int16 l = (Int16)(System.Convert.ToInt16(value) - System.Convert.ToInt16(parameter) + 32);
            if (MainWindow.Snap != 0)
                l = MainWindow.RoundBy(l, MainWindow.Snap);
            return l;
        }
    }
}
