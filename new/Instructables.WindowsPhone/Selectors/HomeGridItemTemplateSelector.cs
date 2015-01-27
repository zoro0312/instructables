using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.DataModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Instructables.Selectors
{
    class HomeGridItemTemplateSelector : DataTemplateSelector
    {
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            var layoutItem = item as InstructableSummary;
            if (layoutItem != null && layoutItem.Group.Layout == DataGroup.LayoutType.MainFeature && layoutItem.GroupOrdinal == 0)
            {
                return (DataTemplate)Application.Current.Resources["HeroGroupedItemTemplate"];
            }
            else
                if (layoutItem.Group.GroupName.ToLower() == "ebooks")
                    return (DataTemplate)Application.Current.Resources["GroupedeBookItemTemplate"];
                else
                    return (DataTemplate)Application.Current.Resources["GroupedItemTemplate"];
        }
    }
}
