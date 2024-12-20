using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GeneticAlgorithmApp2
{
    public class IntToStr_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return $"{(int)value}"; // for some reason this doesn't work now
            return $"{value}";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return int.TryParse((string)value, out int result) ? result : 0;
        }
    }

    public class DoubleToStr_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return $"{(double)value}";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double.TryParse((string)value, out double result);
            return double.TryParse((string)value, out result) ? result : 0;
        }
    }
}
