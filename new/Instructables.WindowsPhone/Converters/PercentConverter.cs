using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Instructables.Converters
{
    class PercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                double val = Double.Parse(value.ToString());
                double factor = Double.Parse(parameter.ToString());
                return System.Convert.ToDouble(val * factor);
            } catch(Exception)
            {

            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
