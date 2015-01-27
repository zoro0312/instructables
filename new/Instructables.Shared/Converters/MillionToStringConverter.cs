using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Instructables.Converters
{
    public class MillionToStringConverter : IValueConverter
    {
        const float million = 1000000;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int val = (int)value;
            if (val >= million ) {
                return (val / million).ToString("#.##M");
            } else{
                return val.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}