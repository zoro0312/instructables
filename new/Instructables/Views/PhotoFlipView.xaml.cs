using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Instructables.DataModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Instructables.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Instructables.Views
{
    public sealed partial class PhotoFlipView : UserControl
    {
        private bool _webViewActive = false;
        private TextBlock tb = null;
        private const float SQUARE_RANGE_OFFSET = 0.2f;
        private const float LONG_RANGE_OFFSET = 0.4f;
        private const float PORTRIT_PICTURE_SCALE = 1.5f;
        private const float LONG_PORTRIT_PICTURE_SCALE = 1.5f;
        private const float LANDSCAPE_PICTURE_SCALE = 1.8f;
        private const float SQUARE_PICTURE_SCALE = 1.5f;

        public PhotoFlipView()
        {
            this.InitializeComponent();
        }

        private void Flipview_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*if (Window.Current.Bounds.Width < 600)
                flipview.Margin = new Thickness(10);
            else
                flipview.Margin = new Thickness(100, 50, 100, 50);*/
            if (InstructableDetailViewModel.CurrentInstructbaleDV != null)
            {
                InstructableDetailViewModel.CurrentInstructbaleDV.NoteText = "";
            }
            /* if (e.AddedItems != null && e.AddedItems.Count > 0)
             {
                 var addedItem = e.AddedItems[0] as MediaItem;
                 if (addedItem != null && addedItem.MediaType == MediaTypeOption.Video)
                 {
                     webView.Visibility = Visibility.Visible;
                     webView.Navigate(new Uri(WebUtility.UrlDecode(addedItem.VideoMedia.UriString)));
                     if (Window.Current.Bounds.Width-120 > addedItem.VideoMedia.Width + 250)
                     {
                         webView.Height = addedItem.VideoMedia.Height + 100;
                         webView.Width = addedItem.VideoMedia.Width + 100;
                     }
                     else
                     {
                         webView.Width = Window.Current.Bounds.Width - 220;
                         double ratio = ((double) addedItem.VideoMedia.Height)/((double) addedItem.VideoMedia.Width);
                         webView.Height = ratio*webView.Width;
                     }
                     _webViewActive = true;
                 }
                 else
                 {
                     if (_webViewActive)
                     {
                         webView.Navigate(new Uri("about:blank"));
                         webView.Visibility = Visibility.Collapsed;
                         _webViewActive = false;
                     }
                 }

             }*/
        }

        /*private void BackButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            if (_webViewActive)
            {
                webView.Navigate(new Uri("about:blank"));
                webView.Visibility = Visibility.Collapsed;
                _webViewActive = false;
            }
        }*/

        private void NavigationCompleted(object sender, WebViewNavigationCompletedEventArgs e)
        {
            Debug.WriteLine("Navigation Failed");
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void FileViewImage_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            /*FlipImageTranform.ScaleX *= e.Delta.Scale;
            FlipImageTranform.ScaleY *= e.Delta.Scale;
            FlipImageTranform.CenterX = e.Position.X;
            FlipImageTranform.CenterY = e.Position.Y;
            if (FlipImageTranform.ScaleX <= 1.0)
            {
                FlipImageTranform.ScaleX = 1.0;
                FlipImageTranform.ScaleY = 1.0;
                
                //IsCanManipulate = true;  
            }
            else
            {
                var newTranslateX = FlipImageTranform.TranslateX + e.Delta.Translation.X;
                var newTranslateY = FlipImageTranform.TranslateY + e.Delta.Translation.Y;
                //if (newTranslateX < (-1 * (imageGrid.ActualWidth / 2)) || newTranslateX > (imageGrid.ActualWidth / 2))
                //    return;
                //else
                //    FlipImageTranform.TranslateX = newTranslateX;
                //if (newTranslateY < (-1 * (imageGrid.ActualHeight / 2)) || newTranslateY > (imageGrid.ActualHeight / 2))
                //    return;
                //else
                    FlipImageTranform.TranslateY = newTranslateY;
            }*/
        }

        private void flipview_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {

        }

        private void PictureView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (InstructableDetailViewModel.CurrentInstructbaleDV != null)
            {
                InstructableDetailViewModel.CurrentInstructbaleDV.NoteText = "";
            }

            //flipview.IsEnabled = false;
            //flipview.ManipulationMode = ManipulationModes.None;
        }

        private void FileViewImage_ManipulationStart(object sender, ManipulationStartedRoutedEventArgs e)
        {

        }

        private void FlipViewSizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void FlipViewDragEnter(object sender, DragEventArgs e)
        {

        }

        private void OnPictureChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            //ScrollViewer viewer = sender as ScrollViewer;
            ////viewer.ChangeView(0, 0, 0.6f);
            //var media = args.NewValue as MediaItem;
            //var picture = media.FileMedia;
            //float ratio = (float)picture.width / (float)picture.height;
            ////float ratio2 = (float)picture.height / (float)picture.width;
            //if (ratio > 1 + SQUARE_RANGE_OFFSET)
            //{
            //    viewer.ZoomToFactor(LANDSCAPE_PICTURE_SCALE);
            //}
            //else if (ratio < 1 - SQUARE_RANGE_OFFSET && ratio > 1 - LONG_RANGE_OFFSET)
            //{
            //    viewer.ZoomToFactor(PORTRIT_PICTURE_SCALE);
            //}
            //else if (ratio < 1 - LONG_RANGE_OFFSET)
            //{
            //    viewer.ZoomToFactor(LONG_PORTRIT_PICTURE_SCALE);
            //}
            //else
            //{
            //    viewer.ZoomToFactor(SQUARE_PICTURE_SCALE);
            //}


        }

        private void BackButtonTapped(object sender, TappedRoutedEventArgs e)
        {

        } 
    }
}
