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


using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Web.Http.Headers;
using Windows.Web.Http;

using Instructables.ViewModels;
using Instructables.DataModel;
using Instructables.DataServices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditProfilePage : Instructables.Common.LayoutAwarePage, IFileOpenPickerContinuable
    {
        public EditProfilePage()
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
            var vm = DataContext as UserProfileViewModel;

            if (vm.userProfile.about != null)
            {
                userAbout.Text = vm.userProfile.about;
            }

            if (vm.userProfile.interests != null)
            {
                userInterests.Text = vm.userProfile.interests;
            }

            if (vm.userProfile.location != null)
            {
                userLocation.Text = vm.userProfile.location;
            }

            if (vm.userProfile.gender != null)
            {
                foreach(ComboBoxItem item in userGender.Items)
                {
                    if (string.Compare(Convert.ToString(item.Content), vm.userProfile.gender) == 0)
                    {
                        userGender.SelectedItem = item;
                        break;
                    }
                }
            }

            if (vm.userProfile.square3Url != null)
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.UriSource = new Uri(vm.userProfile.square3Url);
                Image image = new Image();
                image.Source = bitmapImage;
                image.Stretch = Stretch.Fill;
                userPhoto.Content = image;
            }

            SavingPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void updatePhoto(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            // Launch file open picker and caller app is suspended and may be terminated if required
            openPicker.PickSingleFileAndContinue();
        }

        private byte[] imageBytes;
        /// <summary>
        /// Handle the returned files from file picker
        /// This method is triggered by ContinuationManager based on ActivationKind
        /// </summary>
        /// <param name="args">File open picker continuation activation argment. It cantains the list of files user selected with file open picker </param>
        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count > 0)
            {
                StorageFile photo = args.Files[0];
                Image picture = new Image();
                using (IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(stream);
                    picture.Source = bitmapImage;
                    picture.Stretch = Stretch.UniformToFill;

                    var dataReader = new DataReader(stream.GetInputStreamAt(0));
                    uint size = (uint)stream.Size;
                    await dataReader.LoadAsync(size);

                    imageBytes = new byte[size];
                    dataReader.ReadBytes(imageBytes);
                }

                userPhoto.Content = picture;
                userPhoto.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                userPhoto.VerticalContentAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
            }
        }

        private async void AppBarSaveButton_Click(object sender, RoutedEventArgs e)
        {
            SavingPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            var svc = InstructablesDataService.DataServiceSingleton;
            var loginUserProfile = ViewModelLocator.Instance.LandingVM.userProfile;

            if (imageBytes != null)
            {
                UploadResult uploadResult = await svc.UploadPhoto(imageBytes);

                if (uploadResult != null)
                {
                    loginUserProfile.avatarId = uploadResult.loaded[0].id;
                    loginUserProfile.square3Url = uploadResult.loaded[0].square3Url;
                }
            }

            loginUserProfile.about = this.userAbout.Text;
            loginUserProfile.interests = this.userInterests.Text;
            loginUserProfile.location = this.userLocation.Text;
            var selectedItem = this.userGender.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                loginUserProfile.gender = selectedItem.Content as string;
            }

            await svc.SaveUserProfile(loginUserProfile);

            this.GoBack(this, new RoutedEventArgs());
        }

    }
}
