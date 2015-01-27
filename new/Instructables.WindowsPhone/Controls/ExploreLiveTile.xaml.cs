using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Instructables.Controls
{
    public sealed partial class ExploreLiveTile : UserControl
    {
        private static Random ra = new Random();
        private Storyboard animUp;
        private Storyboard animDown;
        private Storyboard animFlip;
        private Storyboard animFlipBack;

        public ExploreLiveTile()
        {
            this.InitializeComponent();

            animUp = (Storyboard)FindName("liveTileAnimUp");
            animDown = (Storyboard)FindName("liveTileAnimDown");
            animFlip = (Storyboard)FindName("liveTileAnimFlip");
            animFlipBack = (Storyboard)FindName("liveTileAnimFlipBack");

            animUp.Duration = new Duration(TimeSpan.FromSeconds(ra.Next(7, 18)));
            animDown.Duration = new Duration(TimeSpan.FromSeconds(ra.Next(7, 18)));
            animFlip.Duration = new Duration(TimeSpan.FromSeconds(ra.Next(7, 15)));
            animFlipBack.Duration = new Duration(TimeSpan.FromSeconds(ra.Next(7, 15)));

            liveTileAnimDown_Completed(null, null);
        }

        private async void liveTileAnimDown_Completed(object sender, object e)
        {
            await Task.Delay(ra.Next(25000));
            animUp.Begin();
        }

        private async void liveTileAnimUp_Completed(object sender, object e)
        {
            await Task.Delay(ra.Next(15000));
            animFlip.Begin();
        }

        private void liveTileAnimFlip_Completed(object sender, object e)
        {
            animFlipBack.Begin();
        }

        private  void liveTileAnimFlipBack_Completed(object sender, object e)
        {
            animDown.Begin();
        }
    }
}
