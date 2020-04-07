using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace Editor
{
    class DistrictConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Byte sq = (Byte)value;
            switch (sq)
            {
                case 0: return "#FFFF0000";
                case 1: return "#FF00C0FF";
                case 2: return "#FFFFBB00";
                case 3: return "#FF00FF00";
                case 4: return "#FF0000FF";
                case 5: return "#FFFFA0C0";
                case 6: return "#FF8000A0";
                case 7: return "#FFFFFF60";
                case 8: return "#FF008000";
                case 9: return "#FFC080FF";
                case 10: return "#FFFF8040";
                case 11: return "#FFFF50A0";
            }
            return "#FF222222";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
