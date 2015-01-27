using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.DataModel;
using Instructables.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Instructables.Selectors
{
    class ExploreItemTemplateSelector : DataTemplateSelector
    {
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            var layoutItem = item as InstructableSummary;
            if (ViewModelLocator.Instance.ExploreVM.SelectedType.Network.StartsWith("guide"))
                return (DataTemplate)Application.Current.Resources["ExploreGuideItemTemplate"];
            else
                return (DataTemplate)Application.Current.Resources["ExploreItemTemplate"];
        }
    }
}
