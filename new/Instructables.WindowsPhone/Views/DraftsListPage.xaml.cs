using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

using Instructables.ViewModels;
using Instructables.DataModel;
using Instructables.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DraftsListPage : Instructables.Common.LayoutAwarePage
    {
        public DraftsListPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            bottomBar.Visibility = Visibility.Collapsed;
            mainView.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;

            base.OnNavigatedTo(e);
            var vm = DataContext as UserProfileViewModel;
            if(vm != null)
            {
                while(vm.userName == string.Empty || vm.IsLoading)
                    await Task.Delay(200);
            }

            LoadingPanel.Visibility = Visibility.Collapsed;
            bottomBar.Visibility = Visibility.Visible;
            mainView.Visibility = Visibility.Visible;
        }

        private async void ListView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var listView = sender as ListView;
            var instructableSummary = listView.SelectedItem as Instructable;
            if (instructableSummary != null)
            {
                bottomBar.Visibility = Visibility.Collapsed;
                LoadingPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                var dataService = DataServices.InstructablesDataService.DataServiceSingleton;
                var instructable = await dataService.GetInstructable(instructableSummary.id);
                if (instructable != null)
                {
                    ViewModelLocator.Instance.CreateVM.Initialize(instructable);

                    this.Frame.Navigate(typeof(EditInstructable));
                }
                // Error promot message below

                LoadingPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;                
            }
        }

        private void Camera_Clicked(object sender, RoutedEventArgs e)
        {
            var svc = DataServices.InstructablesDataService.DataServiceSingleton;

            if (svc.isLogin())
            {
                if (!SessionManager.ShowedCreateTutorial())
                    this.Frame.Navigate(typeof(CreateTutorial));
                else
                    this.Frame.Navigate(typeof(CreateInstructable));
            }
            else
            {
                this.Frame.Navigate(typeof(LoginPage));
            }
        }
    }
}
