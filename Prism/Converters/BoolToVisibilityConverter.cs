using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Prism.Converters
{
    /// <summary>
    /// 布尔值转换为 Visibility 的转换器
    /// true => Visible
    /// false => Collapsed
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        // 正向转换：bool -> Visibility
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        // 反向转换：Visibility -> bool
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
            {
                return v == Visibility.Visible;
            }
            return false;
        }
    }
}
