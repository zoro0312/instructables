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
    class LandingPageTemplateSelector : DataTemplateSelector
    {
        public static uint MoreIndex
        {
            get
            {                
                return (uint)LandingPageViewModel.CurrentScreenMetrics.DualFeatureCount - 1;                
            }
        }


        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            var layoutItem = item as InstructableSummary;

            if (layoutItem.GroupOrdinal == MoreIndex)
            {
                if (layoutItem.Group.GroupName=="Featured")
                {
                    return (DataTemplate)Application.Current.Resources["GroupedFeaturedMoreItemTemplate"];
                }
                else if (layoutItem.Group.GroupName == "Recent")
                {
                    return (DataTemplate)Application.Current.Resources["GroupedRecentMoreItemTemplate"];
                }
                else if (layoutItem.Group.GroupName == "Popular")
                {
                    return (DataTemplate)Application.Current.Resources["GroupedPopularMoreItemTemplate"];
                }
                else
                {
                    return (DataTemplate)Application.Current.Resources["GroupedItemTemplate"];
                }
            }
            else
            {
                return (DataTemplate)Application.Current.Resources["GroupedItemTemplate"];
            }
        }
    }
}
