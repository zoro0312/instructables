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
    class GuideDetailTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StepHeaderTemplate { get; set; }
        public DataTemplate CollectionDataTemplate { get; set; }
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            var layoutItem = item as Introduction;
            if (layoutItem != null)
            {
                return (DataTemplate)Application.Current.Resources["IntroductionDataTemplate"];
            }
            else
            {
                var layoutItem2 = item as List<InstructableGuideItem>;
                if(layoutItem2 != null)
                {
                    return CollectionDataTemplate;
                }
                else
                {
                    return StepHeaderTemplate;
                }
            }
        }
    }
}
