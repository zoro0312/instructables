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
using Windows.UI.Xaml.Media.Imaging;

using Windows.Media;
using Windows.Media.Devices;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Devices.Enumeration;

using Windows.Graphics.Display;
using Windows.Graphics.Imaging;

using Windows.Storage;
using Windows.Storage.Streams;

using Instructables.DataModel;
using Instructables.ViewModels;
using Instructables.DataServices;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Instructables.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PhotoSequenceCapture : Instructables.Common.LayoutAwarePage
    {
        private MediaCapture m_capture;

        private StorageFolder photosFolder = null;

        private uint m_captureNum = 0;

        private Step m_currentStep;

        private List<string> m_images = new List<string>();

        public PhotoSequenceCapture()
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
            base.OnNavigatedTo(e);

            m_currentStep = e.Parameter as Step;

            m_captureNum = 0;
            this.captureNum.Text = m_captureNum.ToString();

            var vm = ViewModelLocator.Instance.CreateVM;
            //photosFolder = await vm.getInstructableFolder(vm.Instructable);

            m_capture = new Windows.Media.Capture.MediaCapture();
            await m_capture.InitializeAsync();

            SetBestResolution();

            if (m_capture.VideoDeviceController.FlashControl.Supported)
            {
                m_capture.VideoDeviceController.FlashControl.Enabled = false;
                m_capture.VideoDeviceController.FlashControl.Auto = true;
            }

            m_capture.VideoDeviceController.PrimaryUse = CaptureUse.Photo;

            previewElement.Source = m_capture;
            VideoEncodingProperties previewProps = m_capture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;
            double sx = this.Frame.ActualWidth / previewProps.Height;
            double sy = this.Frame.ActualHeight / previewProps.Width;
            double tx = this.Frame.ActualWidth;

            previewElement.RenderTransform = new CompositeTransform { ScaleX = sx, ScaleY = sy, Rotation = 90, TranslateX = tx };
            //previewElement.RenderTransform = new CompositeTransform { ScaleX = sx, ScaleY = sy, Rotation = -90, TranslateY = this.Frame.ActualHeight };

            await m_capture.StartPreviewAsync();

            if (m_capture.VideoDeviceController.FlashControl.Supported)
            {
                m_capture.FocusChanged += MediaCaptureFocusChanged;
                m_capture.VideoDeviceController.FocusControl.Configure(new FocusSettings { Mode = FocusMode.Continuous, AutoFocusRange = AutoFocusRange.Normal });
                await m_capture.VideoDeviceController.FocusControl.FocusAsync();
            }
        }

        public async void SetBestResolution()
        {
            System.Collections.Generic.IReadOnlyList<IMediaEncodingProperties> res;
            res = m_capture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo);
            uint maxResolution = 0;
            int indexMaxResolution = 0;

            if (res.Count >= 1)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    ImageEncodingProperties vp = res[i] as ImageEncodingProperties;

                    if (vp != null && vp.Width > maxResolution)
                    {
                        indexMaxResolution = i;
                        maxResolution = vp.Width;
                    }
                }
                await m_capture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, res[indexMaxResolution]);
            }
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await m_capture.StopPreviewAsync();
            m_capture.FocusChanged -= MediaCaptureFocusChanged;
        }

        private async void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    string name = DateTimeOffset.Now.ToString("MM_dd_yy_hh_mm_ss") + ".jpg";
                    StorageFile file = await photosFolder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
                    ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();

                    await m_capture.CapturePhotoToStorageFileAsync(imageProperties, file);

                    using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite),
                                                memStream = new InMemoryRandomAccessStream())
                    {
                        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                        // Set the encoder's destination to the temporary, in-memory stream.
                        BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(memStream, decoder);

                        BitmapPropertySet properties = new BitmapPropertySet();
                        ushort netExifOrientation = 6; //

                        // BitmapProperties requires the application to explicitly declare the type
                        // of the property to be written - this is different from FileProperties which
                        // automatically coerces the value to the correct type. System.Photo.Orientation
                        // is defined as an unsigned 16 bit integer.
                        BitmapTypedValue orientationTypedValue = new BitmapTypedValue(
                            netExifOrientation,
                            Windows.Foundation.PropertyType.UInt16
                            );

                        properties.Add("System.Photo.Orientation", orientationTypedValue);
                        await encoder.BitmapProperties.SetPropertiesAsync(properties);

                        //encoder.IsThumbnailGenerated = true;

                        try
                        {
                            await encoder.FlushAsync();
                        }
                        catch (Exception)
                        {
                        }

                        // Now that the file has been written to the temporary stream, copy the data to the file.
                        memStream.Seek(0);
                        fileStream.Seek(0);
                        fileStream.Size = 0;
                        await RandomAccessStream.CopyAsync(memStream, fileStream);
                    }

                    m_images.Add(name);
                    m_captureNum++;
                    this.captureNum.Text = m_captureNum.ToString();
                }
                catch (Exception )
                {
                    
                }
            });
        }

        private async void AppBarAcceptButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = ViewModelLocator.Instance.CreateVM;
            List<StorageFile> files = new List<StorageFile>();
            foreach (string image in m_images)
            {
                StorageFile file = await photosFolder.GetFileAsync(image);

                BitmapImage bitmap = vm.GetBitmapImage(file);
                File tempFile = new File();
                tempFile.name = image;
                tempFile.width = bitmap.PixelWidth;
                tempFile.height = bitmap.PixelHeight;
                m_currentStep.ImageNames.Add(tempFile);

                //vm.StepImages.Add(file);
                files.Add(file);
            }

            await InstructablesDataService.DataServiceSingleton.UploadPhotos(m_currentStep, files);

            this.Frame.GoBack();
        }

        private void AppBarCancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private async void MediaCaptureFocusChanged(MediaCapture sender, MediaCaptureFocusChangedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                switch (e.FocusState)
                {
                    case MediaCaptureFocusState.Searching:
                        FlashAutoFocusBrackets();
                        break;
                    case MediaCaptureFocusState.Focused:
                        LockAutoFocusBrackets();
                        break;
                    case MediaCaptureFocusState.Failed:
                    case MediaCaptureFocusState.Lost:
                    case MediaCaptureFocusState.Uninitialized:
                        DismissAutoFocusBrackets();
                        break;
                }
            });
        }

        /// <summary>
        /// Shows the automatic focus brackets at the center of the viewfinder.
        /// </summary>
        public void FlashAutoFocusBrackets()
        {
            this.AutoFocusInProgress.Begin();
        }

        /// <summary>
        /// Stops the automatic focus brackets animation, keeping them visible.
        /// </summary>
        public void LockAutoFocusBrackets()
        {
            this.AutoFocusInProgress.Stop();
            this.AutoFocusLocked.Begin();
        }

        /// <summary>
        /// Hides the automatic focus brackets.
        /// </summary>
        private void DismissAutoFocusBrackets()
        {
            this.AutoFocusInProgress.Stop();
            this.AutoFocusLocked.Stop();

            this.AutoFocusBrackets.Visibility = Visibility.Collapsed;
        }
    }
}
