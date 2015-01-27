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
    class PhotoContentSelector : DataTemplateSelector
    {
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            Step step = item as Step;
            if (step != null)
            {
                if (step.VideoList != null && step.VideoList.Count > 0)
                {
                    if (step.VideoList.Count == 1)
                        return (DataTemplate)Application.Current.Resources["VideoTemplate1"];
                    else
                        return (DataTemplate)Application.Current.Resources["VideoTemplate2"];
                }
                else if (step.files != null && step.files.Count > 0)
                {
                    //return (DataTemplate)Application.Current.Resources["PhotoTemplate1"];
                    if (step.files.Count == 1)
                    {
                        return (DataTemplate)Application.Current.Resources["PhotoTemplate1WP"];
                    }
                    if (step.files.Count == 2)
                    {
                        return (DataTemplate)Application.Current.Resources["PhotoTemplate2WP"];
                    }
                    if (step.files.Count == 3)
                    {
                        return (DataTemplate)Application.Current.Resources["PhotoTemplate3WP"];
                    }
                    if (step.files.Count == 4)
                    {
                        return (DataTemplate)Application.Current.Resources["PhotoTemplate4WP"];
                    }
                    if (step.files.Count == 5)
                    {
                        return (DataTemplate)Application.Current.Resources["PhotoTemplate5WP"];
                    }
                    if (step.files.Count > 5)
                    {
                        return (DataTemplate)Application.Current.Resources["PhotoTemplate6plusWP"];
                    }
                }
                else
                {
                    return (DataTemplate)Application.Current.Resources["PhotoTemplate0"];
                }
            }
            return (DataTemplate)Application.Current.Resources["PhotoTemplate0"];
            //return base.SelectTemplateCore(item, container);
        }
    }
}
