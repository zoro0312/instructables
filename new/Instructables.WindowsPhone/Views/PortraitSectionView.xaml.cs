using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Instructables.DataModel;
using Instructables.Utils;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Instructables.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Instructables.Views
{
    public sealed partial class PortraitSectionView : UserControl
    {
        public PortraitSectionView()
        {
            this.InitializeComponent();
            Loaded += (sender, args) =>
                {
                    var dc = this.DataContext as Step;
                    //dc = null;
                    if (dc != null)
                    // Get the target RichTextBlock
                    {
                        // using the local image firstly
                        if (dc.ImageNames.Count != 0)
                            PictureSource.Source = dc.ImageNames;

                        ContentTextBlock.Blocks.Clear();
                        // Wrap the value of the Html property in a div and convert it to a new RichTextBlock
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("<?xml version=\"1.0\"?>");
                        sb.AppendLine("<RichTextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:utils=\"using:Instructables.Utils\" >");
                        sb.AppendLine(dc.XamlBody);
                        sb.AppendLine("</RichTextBlock>");

                        try
                        {
                            RichTextBlock newRichText = (RichTextBlock)XamlReader.Load(sb.ToString());
                            if (dc.XamlBody == null || dc.XamlBody.Length <= 0)
                            {
                                ContentTextBlock.Visibility = Visibility.Collapsed;
                                return;
                            }
                            // Move the blocks in the new RichTextBlock to the target RichTextBlock
                            for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
                            {
                                Block b = newRichText.Blocks[i];
                                newRichText.Blocks.RemoveAt(i);
                                ContentTextBlock.Blocks.Insert(0, b);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                };
        }

        private async void UriTapped(object sender, TappedRoutedEventArgs e)
        {
            var richTB = sender as RichTextBlock;
            TextPointer tp = null;
            if (richTB != null)
            {
                tp = richTB.GetPositionFromPoint(e.GetPosition(richTB));
            }
            if (tp == null)
                return;

            var element = tp.Parent as TextElement;
            if (element == null)
                return;
            var uriString = Utils.AttachedUri.GetUriSource(element);
            if (uriString != String.Empty)
                await Launcher.LaunchUriAsync(new Uri(uriString));
        }

        private void WebViewLoadComplete(object sender, NavigationEventArgs e)
        {
            WebView webView = sender as WebView;
            if (webView.Visibility == Visibility.Collapsed)
                return;
            //webView.ContainsFullScreenElement = true;
            //webView.ZoomToFactor(2.0f);
        }

        private void onPictureLoaded(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            
        }

        private void OnContentLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void onSizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void onLayoutUpdated(object sender, object e)
        {

        }

        private void WebView_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

        }
    }
}
