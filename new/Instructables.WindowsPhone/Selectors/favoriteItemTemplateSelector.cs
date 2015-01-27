using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Instructables.ViewModels;

namespace Instructables.Selectors
{
    class FavoriteItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate myFavoriteItemTemplate { get; set; }
        public DataTemplate userFavoriteItemTemplate { get; set; }

        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            UserProfileViewModel vm = ViewModelLocator.Instance.UserProfileVM;
            if (vm != null && vm.isLoginUser)
                return myFavoriteItemTemplate;
            else
                return userFavoriteItemTemplate;
        }
    }
}
