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
    class GroupNameToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string groupName = value as string;
            if (groupName != null)
            {
                Color titleColor;
                SolidColorBrush newBrush = new SolidColorBrush();
                if (groupName == "Featured")
                {
                    titleColor = Color.FromArgb(0xFF, 0xFF, 0xAE, 0x00);
                }
                else if (groupName == "Recent")
                {
                    titleColor = Color.FromArgb(0xFF, 0xFC, 0x65, 0x00);
                }
                else if (groupName == "Popular")
                {
                    titleColor = Color.FromArgb(0xFF, 0xC2, 0x00, 0x89);
                }
                else
                {
                    return null;
                }
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
