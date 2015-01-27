using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Instructables.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Instructables.Controls
{
    public sealed partial class ChannelSelector : UserControl
    {
        public ChannelSelector()
        {
            this.InitializeComponent();
            var vm = ViewModelLocator.Instance.ExploreVM;
            if (vm.SelectedSort.Display == "Featured")
            {
                this.Resources.Add("ListBoxItemPressedBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0xcc, 0xff, 0xae, 0x00)));
                this.Resources.Add("ListBoxItemSelectedBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0xcc, 0xff, 0xae, 0x00)));
                this.Resources.Add("ListBoxItemPointerOverBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0xcc, 0xff, 0xae, 0x00)));
                this.Resources.Add("ListBoxItemSelectedPointerOverBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0xcc, 0xff, 0xae, 0x00)));
            }
            else if (vm.SelectedSort.Display == "Recent")
            {
                this.Resources.Add("ListBoxItemPressedBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0xcc, 0xfc, 0x65, 0x00)));
                this.Resources.Add("ListBoxItemSelectedBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0xcc, 0xfc, 0x65, 0x00)));
                this.Resources.Add("ListBoxItemPointerOverBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0xcc, 0xfc, 0x65, 0x00)));
                this.Resources.Add("ListBoxItemSelectedPointerOverBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0xcc, 0xfc, 0x65, 0x00)));
            }
            else
            {
                this.Resources.Add("ListBoxItemPressedBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x88, 0xc2, 0x00, 0x89)));
                this.Resources.Add("ListBoxItemSelectedBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x88, 0xc2, 0x00, 0x89)));
                this.Resources.Add("ListBoxItemPointerOverBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x88, 0xc2, 0x00, 0x89)));
                this.Resources.Add("ListBoxItemSelectedPointerOverBackgroundThemeBrush1", new SolidColorBrush(Color.FromArgb(0x88, 0xc2, 0x00, 0x89)));
            }
        }
    }
}
