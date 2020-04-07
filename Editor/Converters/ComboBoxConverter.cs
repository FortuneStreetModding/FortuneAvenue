using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using FSEditor.FSData;
using System.Linq;

namespace Editor
{
    class ShopComboBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Byte val = System.Convert.ToByte(value);
            String str;

            if (MainWindow.ShopTypeList.Keys.Contains<Byte>(val))
                str = MainWindow.ShopTypeList[val];
            else
            {
                val = 0;
                str = MainWindow.ShopTypeList[0];
            }

            return new KeyValuePair<Byte, String>(val, str);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            KeyValuePair<Byte, String> kvp = (KeyValuePair<Byte, String>)value;
            return (Byte)kvp.Key;
        }
    }
    class SquareComboBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SquareType val = (SquareType)System.Convert.ToInt32(value);
            String str;

            if (MainWindow.SquareTypeList.Keys.Contains<SquareType>(val))
                str = MainWindow.SquareTypeList[val];
            else
            {
                val = 0;
                str = MainWindow.SquareTypeList[0];
            }

            return new KeyValuePair<SquareType, String>(val, str);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            KeyValuePair<SquareType, String> kvp = (KeyValuePair<SquareType, String>)value;
            return (SquareType)kvp.Key;
        }
    }
}
