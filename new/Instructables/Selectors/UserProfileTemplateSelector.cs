using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.DataModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Instructables.ViewModels;

namespace Instructables.Selectors
{
    class UserProfileTemplateSelector : DataTemplateSelector
    {
        //protected const uint INDEX_MORE = 11;
        public static uint MoreIndex
        {
            get
            {
                return (uint)UserProfileViewModel.CurrentScreenMetrics.DualFeatureCount - 1;
            }
        }


        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            var layoutItem = item as Instructable;

            if (layoutItem.GroupOrdinal == MoreIndex)
            {
                if (layoutItem.Group.GroupName == "Draft")
                {
                    return (DataTemplate)Application.Current.Resources["GroupedDraftMoreItemTemplate"];
                }
                else if (layoutItem.Group.GroupName == "Published")
                {
                    return (DataTemplate)Application.Current.Resources["GroupedPublishedMoreItemTemplate"];
                }                
                else
                {
                    return (DataTemplate)Application.Current.Resources["GroupedUserProfileItemTemplate"];
                }
            }
            else
            {
                return (DataTemplate)Application.Current.Resources["GroupedUserProfileItemTemplate"];
            }
        }
    }
}
