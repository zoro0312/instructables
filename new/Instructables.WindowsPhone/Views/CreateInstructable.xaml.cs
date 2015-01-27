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

using Windows.UI.Popups;

using Instructables.Common;
using Instructables.DataModel;
using Instructables.ViewModels;
using Instructables.DataServices;
using Windows.Phone.UI.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateInstructable : Instructables.Common.LayoutAwarePage
    {
        public CreateInstructable()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LanguageList.SelectedIndex = 0;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                while (this.Frame.CanGoBack &&
                       this.Frame.SourcePageType != typeof(LandingPage) &&
                       this.Frame.SourcePageType != typeof(UserProfilePage))
                    this.Frame.GoBack();
            }
        }

        protected async override void OnHardwareBackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;

            var evm = ViewModelLocator.Instance.ExploreVM;
            // If we selected a specific channel back to 'all'
            evm.SelectedCollectionCategoryIndex = 0;
            evm.SelectedChannelIndex = 0;

            if (CategoryMenu.Visibility == Windows.UI.Xaml.Visibility.Visible ||
                ChannelMenu.Visibility == Windows.UI.Xaml.Visibility.Visible ||
                LanguageMenu.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                _isSelectingCategory = false;
                _isSelectingChannel = false;
                CategoryMenu.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ChannelMenu.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                LanguageMenu.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                if (this.title.Text == string.Empty)
                {
                    this.GoBack(this, new RoutedEventArgs());
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("", "");
                    dialog.Title = "Leave?";
                    dialog.Content = "Would you like to save the instructable before leaving?";
                    dialog.Commands.Add(new UICommand("Save", async (command) => 
                    {
                        var cvm = DataContext as CreateViewModel;
                        var lvm = ViewModelLocator.Instance.LandingVM;

                        Instructable instructable = new Instructable();
                        instructable.title = this.title.Text;
                        instructable.category = category;
                        instructable.channel = channel;
                        instructable.author = lvm.LoginAuthor;

                        cvm.Initialize(instructable);
                        await cvm.SaveCloudInstructable();

                        this.GoBack(this, new RoutedEventArgs());
                    }));
                    dialog.Commands.Add(new UICommand("Discard", (command) =>
                    {
                        this.GoBack(this, new RoutedEventArgs());
                    }));
                    await dialog.ShowAsync();
                }
            }
        }

        private async void AppBarCreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.title.Text == string.Empty)
            {
                MessageDialog dialog = new MessageDialog("Enter a title to start your Instructable.", "Title required");
                dialog.Commands.Add(new UICommand("OK", (command) =>{}));
                await dialog.ShowAsync();
            }
            else
            {
                var cvm = DataContext as CreateViewModel;
                var lvm = ViewModelLocator.Instance.LandingVM;

                Instructable instructable = new Instructable();
                instructable.title = this.title.Text;
                instructable.category = category;
                instructable.channel = channel;
                instructable.author = lvm.LoginAuthor;

                cvm.Initialize(instructable);

                this.Frame.Navigate(typeof(EditInstructable));
            }
        }

        private string category;
        private string channel;
        private bool _isSelectingCategory = false;
        private bool _isSelectingChannel = false;

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryMenuList.SelectedIndex = -1;
            _isSelectingCategory = true;
            categoryButton.Focus(FocusState.Pointer);
            CategoryMenu.Visibility = Windows.UI.Xaml.Visibility.Visible;
            BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void LanguageButton_Click(object sender, RoutedEventArgs e)
        {
            languageButton.Focus(FocusState.Pointer);
            BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            LanguageMenu.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            languageButton.Focus(FocusState.Pointer);
            LanguageMenu.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            var listbox = sender as ListBox;
            languageButton.Content = (listbox.SelectedValue as ListBoxItem).Content;
            BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            
        }

        private void Channel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isSelectingChannel == false)
                return;
            languageButton.Focus(FocusState.Pointer);
            ChannelMenu.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

            var evm = ViewModelLocator.Instance.ExploreVM;
            if (evm != null)
            {
                Category cate = evm.SelectedCollectionCategory;
                Channel ch = evm.SelectedChannel;
                category = cate.CategoryName;
                channel = ch.display;

                if (channel.ToLower() == "all")
                {
                    categoryButton.Content = category;
                }
                else
                {
                    categoryButton.Content = channel;
                }
            }
            _isSelectingChannel = false;
        }

        private void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isSelectingCategory == false)
                return;
            categoryButton.Focus(FocusState.Pointer);
            CategoryMenu.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ChannelMenu.Visibility = Windows.UI.Xaml.Visibility.Visible;
            _isSelectingCategory = false;
            _isSelectingChannel = true;
        }
    }
}
