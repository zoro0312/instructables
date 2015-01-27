using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Instructables.ViewModels;

namespace Instructables.Selectors
{
    class ProfileItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate myInstructableItemTemplate { get; set; }
        public DataTemplate userInstrucatableItemTemplate { get; set; }

        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            UserProfileViewModel vm = item as UserProfileViewModel;
            if (vm != null && vm.isLoginUser)
                return myInstructableItemTemplate;
            else
                return userInstrucatableItemTemplate;
        }
    }
}
