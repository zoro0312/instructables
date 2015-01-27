using Instructables.Common;
using Instructables.DataModel;
using Instructables.DataServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Instructables.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateTutorial : LayoutAwarePage
    {
        public CreateTutorial()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private bool showing;
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var param = e.Parameter as string;
            showing = param == "showing";

            var tutorialData = new TutorialDataSource();
            TutorialPages.ItemsSource = tutorialData.Items;
            ContextControl.ItemsSource = tutorialData.Items;

            SessionManager.SetShowedCreateTutorial(true);
            await SessionManager.WriteSession();
        }

        private void Exit_Tutorial_Click(object sender, RoutedEventArgs e)
        {
            if (showing)
            {
                GoBack(this, e);
            }
            else
            {
                this.Frame.Navigate(typeof(CreateInstructable));
            }
        }
    }
}
