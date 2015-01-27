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
    class InstructablesDetailTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StepHeaderTemplate { get; set; }

        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            var layoutItem = item as Step;
            if (layoutItem != null)
            {
                if(layoutItem.VideoList != null)
                {
                    return (DataTemplate)Application.Current.Resources["StepVideoDataTemplate"];
                }
                else
                {
                    return (DataTemplate)Application.Current.Resources["StepDataTemplate"];
                }
            }
            else
            {
                return StepHeaderTemplate;
            }
            
        }
    }
}
