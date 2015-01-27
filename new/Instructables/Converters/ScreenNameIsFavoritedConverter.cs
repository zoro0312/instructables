using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

using Instructables.ViewModels;
using Instructables.DataModel;

namespace Instructables.Converters
{
    public class ScreenNameIsFavoritedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string screenName = value as string;
            var profile = ViewModelLocator.Instance.LandingVM.userProfile;

            if (profile != null)
            {
                ObservableCollection<Author> subscriptions = profile.subscriptions;
                if (subscriptions != null)
                {
                    foreach (var author in subscriptions)
                    {
                        if (author.screenName == screenName)
                            return true;
                    }
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TitleIsFavoritedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string title = value as string;
            var profile = ViewModelLocator.Instance.LandingVM.userProfile;

            if (profile != null)
            {
                ObservableCollection<Instructable> favorites = profile.favorites;
                if (favorites != null)
                {
                    foreach (var instructable in favorites)
                    {
                        if (instructable.title == title)
                            return true;
                    }
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
