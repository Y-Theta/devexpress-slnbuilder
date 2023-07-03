using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VsSolutionGenerator.DevSCR
{
    internal class TooltipConvert : IValueConverter
    {
        public static readonly TooltipConvert Singleton = new TooltipConvert();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool flag)
            {
                if (parameter is string str)
                {
                    var strs = str.Split('|');
                    if (flag)
                    {
                        return strs[0];
                    }
                    else if (strs.Length > 0)
                    {
                        return strs[1];
                    }
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return default;
        }
    }
}
