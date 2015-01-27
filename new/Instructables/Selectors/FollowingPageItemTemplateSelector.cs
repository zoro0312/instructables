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
    class FollowingPageItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AuthyItemTemplate { get; set; }
        public DataTemplate LoginItemTemplate { get; set; }

        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            var svc = DataServices.InstructablesDataService.DataServiceSingleton;
            bool isLogin = svc.isLogin();
            if (isLogin)
                return AuthyItemTemplate;
            else
                return LoginItemTemplate;
        }
    }
}
