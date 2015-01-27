using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System;
using Windows.UI.Popups;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OfficialRulesFlyout : SettingsFlyout
    {
        public OfficialRulesFlyout()
        {
            this.InitializeComponent();
            
        }


        public void Load(string html)
        {
            string DisableZoomHtml = "<head><style>body{-ms-content-zooming:none;}</style></head>" + html;
            ContestOfficalRules.NavigateToString(DisableZoomHtml);
        }     
    }
    
}
