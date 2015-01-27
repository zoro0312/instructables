using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Instructables.Converters
{
    public class InsCountFormatterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int count = (int)value;

            string param = parameter as string;
            if (param == "drafts")
            {
                return (String.Format("{0} saved drafts ", count));
            }
            else
            {
                return (String.Format("{0} published instructables ", count));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
