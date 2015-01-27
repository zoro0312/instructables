using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Instructables.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime thisdate = (DateTime)value;
            string formatstring = parameter as string;
            if (!String.IsNullOrEmpty(formatstring))
            {
                return thisdate.ToString(formatstring);
            }
            else
                return thisdate.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
