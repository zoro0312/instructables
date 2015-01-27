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
    class CategoryItemTemplateSelector : DataTemplateSelector
    {
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            var layoutItem = item as Category;
            if (layoutItem.CategoryName == "All")
                return (DataTemplate)Application.Current.Resources["CategoryItemAllDataTemplate"];
            else if (layoutItem.CategoryName == "Technology")
                return (DataTemplate)Application.Current.Resources["CategoryItemTechDataTemplate"];
            else if (layoutItem.CategoryName == "Workshop")
                return (DataTemplate)Application.Current.Resources["CategoryItemWorkDataTemplate"];
            else if (layoutItem.CategoryName == "Living")
                return (DataTemplate)Application.Current.Resources["CategoryItemLiveDataTemplate"];
            else if (layoutItem.CategoryName == "Food")
                return (DataTemplate)Application.Current.Resources["CategoryItemFoodDataTemplate"];
            else if (layoutItem.CategoryName == "Outside")
                return (DataTemplate)Application.Current.Resources["CategoryItemOutDataTemplate"];
            else if (layoutItem.CategoryName == "Play")
                return (DataTemplate)Application.Current.Resources["CategoryItemPlayDataTemplate"];
            else
                return (DataTemplate)Application.Current.Resources["CategoryItemAllDataTemplate"];
        }
    }
}
