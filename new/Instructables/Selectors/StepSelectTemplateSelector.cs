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
    class StepSelectorTemplateSelector : DataTemplateSelector
    {
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            var layoutItem = item as StepGroup;
            var coreItem = layoutItem.Steps[0];
            if (coreItem.stepIndex == 0)
                return (DataTemplate)Application.Current.Resources["InstroctionStepDataTemplate"];
            else
                return (DataTemplate)Application.Current.Resources["NormalStepDataTemplate"];
        }
    }
}
