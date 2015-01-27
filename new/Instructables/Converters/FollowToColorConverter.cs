using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.ViewModels;
using Windows.UI.Xaml.Data;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Instructables.Converters
{
    class FollowToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? isFollow = value as bool?;
            Color titleColor;
            if (isFollow != null)
            {
                if(isFollow == true)
                {
                    titleColor = Color.FromArgb(0xFF, 0x69, 0x69, 0x69);
                }
                else
                {
                    titleColor = Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00);
                }

                SolidColorBrush newBrush = new SolidColorBrush();
                newBrush.Color = titleColor;
                return newBrush;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
