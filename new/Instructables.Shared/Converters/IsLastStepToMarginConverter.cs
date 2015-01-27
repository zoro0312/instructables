using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Instructables.Converters
{
    public class IsLastStepToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //var margin = new Thickness(0, 0, 0, 0);
            if ((value is bool && (bool)value == true))
            {
                return new Thickness(0, 0, 0, 100);
            }
            else
            {
                return new Thickness(0, 0, 0, 0);
            }
            //return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
