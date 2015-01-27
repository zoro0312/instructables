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
    class LandingPageContestTemplateSelector : DataTemplateSelector
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
            var layoutItem = item as Contest;

            if (layoutItem.GroupOrdinal == MoreIndex)
            {
                return (DataTemplate)Application.Current.Resources["GroupedContestMoreItemTemplate"];
            }
            else
            {
                return (DataTemplate)Application.Current.Resources["GroupedContestItemTemplate"];
            }
        }
    }
}
