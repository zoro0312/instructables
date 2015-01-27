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
    class VideoDetailTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StepHeaderTemplate { get; set; }

        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            var layoutItem = item as InstructableDetailViewModel;
            if (layoutItem != null)
            {
                return (DataTemplate)Application.Current.Resources["VideoDataTemplate"];
            }
            else
            {
                
                return StepHeaderTemplate;
            }
        }
    }
}
