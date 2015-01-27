﻿using System.Diagnostics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Instructables.Controls
{
    public class TileCanvas : Canvas
    {
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(TileCanvas), new PropertyMetadata(null, ImageSourceChanged));

        private Size _lastActualSize;

        public TileCanvas()
        {
            LayoutUpdated += (sender, o) =>
                {
                    var newSize = new Size(ActualWidth, ActualHeight);
                    if (_lastActualSize != newSize)
                    {
                        _lastActualSize = newSize;
                        Rebuild();
                    }
                };
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        private static void ImageSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            TileCanvas self = (TileCanvas)o;
            var src = self.ImageSource;
            if (src != null)
            {
                var image = new Image { Source = src };
                image.ImageOpened += self.ImageOnImageOpened;
                image.ImageFailed += self.ImageOnImageFailed;

                //add it to the visual tree to kick off ImageOpened
                self.Children.Add(image);
            }
        }

        private void ImageOnImageFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArgs)
        {
            var image = (Image)sender;
            image.ImageOpened -= ImageOnImageOpened;
            image.ImageFailed -= ImageOnImageFailed;
            Children.Add(new TextBlock { Text = exceptionRoutedEventArgs.ErrorMessage, Foreground = new SolidColorBrush(Colors.Red) });
        }

        private void ImageOnImageOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var image = (Image)sender;
            image.ImageOpened -= ImageOnImageOpened;
            image.ImageFailed -= ImageOnImageFailed;
            Rebuild();
        }

        private void Rebuild()
        {
            var bmp = ImageSource as BitmapSource;

            if (bmp == null) return;
            var width = bmp.PixelWidth;
            var height = bmp.PixelHeight;

            if (width == 0 || height == 0)
            {
                return;
            }

            Children.Clear();
            for (int x = 0; x < ActualWidth; x += width)
            {
                for (int y = 0; y < ActualHeight; y += height)
                {
                    var image = new Image { Source = ImageSource };
                    Canvas.SetLeft(image, x);
                    Canvas.SetTop(image, y);
                    Children.Add(image);
                }
            }
            Clip = new RectangleGeometry { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };
        }
    }
}
