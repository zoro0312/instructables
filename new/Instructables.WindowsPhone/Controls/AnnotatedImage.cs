using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.DataModel;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Instructables.ViewModels;

namespace Instructables.Controls
{
    class AnnotatedImage : Grid
    {
        public static readonly DependencyProperty AnnotationCollectionProperty =
            DependencyProperty.Register("AnnotationCollection", typeof(List<ImageNote>), typeof(AnnotatedImage),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(AnnotatedImage), new PropertyMetadata(null, ImageSourceChanged));
        public DependencyProperty ShowAnnotationProperty = DependencyProperty.Register("ShowAnnotation", typeof(bool), typeof(AnnotatedImage), new PropertyMetadata(true));
        //public static readonly DependencyProperty LocalDataContextProperty = DependencyProperty.Register("LocalDataContext", typeof(InstructableDetailViewModel), typeof(AnnotatedImage), new PropertyMetadata(null,DataContextChanged));
        //public static readonly DependencyProperty AnnotationTextProperty = DependencyProperty.Register("AnnotationText", typeof(string), typeof(AnnotatedImage), new PropertyMetadata("Test", DataContextChanged));
        
        //private Size _lastActualSize;
       //private InstructableDetailViewModel _localDataContext = null;
        //private Image _annotationFlag = null;


        public AnnotatedImage()
        {
            //Uri noteFileSource = new Uri("ms-appx:///Assets/HeaderLogos/Note.png");
            /*BitmapImage imageSource = new BitmapImage(noteFileSource);
            _annotationFlag = new Image();
            _annotationFlag.Source = imageSource;
            _annotationFlag.Stretch = Stretch.None;
            _annotationFlag.VerticalAlignment = VerticalAlignment.Top;
            _annotationFlag.HorizontalAlignment = HorizontalAlignment.Right;*/

            //this.Children.Add(_annotationFlag);
            //_annotationFlag.Visibility = Visibility.Visible;
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public bool ShowAnnotation
        {
            get { return (bool)GetValue(ShowAnnotationProperty); }
            set { SetValue(ShowAnnotationProperty, value); }
        }
        /*public InstructableDetailViewModel LocalDataContext
        {
            get { return (InstructableDetailViewModel)GetValue(LocalDataContextProperty); }
            set { SetValue(LocalDataContextProperty, value); }
        }*/

        public List<ImageNote> AnnotationCollection
        {
            get { return (List<ImageNote>)GetValue(AnnotationCollectionProperty); }
            set { SetValue(AnnotationCollectionProperty, value); }
        }

        /*public string AnnotationText
        {
            get { return (string)GetValue(AnnotationTextProperty); }
            set { SetValue(AnnotationTextProperty, value); }
        }*/

        private static void ImageSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            AnnotatedImage self = (AnnotatedImage)o;
            var src = self.ImageSource;
            if (src != null)
            {
                var image = new Image { Source = src, Stretch = Stretch.Uniform };
                image.HorizontalAlignment = HorizontalAlignment.Stretch;
                image.VerticalAlignment = VerticalAlignment.Stretch;
                image.ImageOpened += self.ImageOnImageOpened;
                image.ImageFailed += self.ImageOnImageFailed;
                self.Children.Clear();
                ProgressRing progressRing = new ProgressRing();
                progressRing.HorizontalAlignment = HorizontalAlignment.Center;
                progressRing.VerticalAlignment = VerticalAlignment.Center;
                progressRing.Height = 50;
                progressRing.Width = 50;
                progressRing.IsActive = true;
                self.Children.Add(progressRing);
                //add it to the visual tree to kick off ImageOpened
                self.Children.Add(image);
            }
        }

        private void ImageOnImageFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArgs)
        {
            var image = (Image)sender;
            image.ImageOpened -= ImageOnImageOpened;
            image.ImageFailed -= ImageOnImageFailed;
            ProgressRing pRing = this.Children[0] as ProgressRing;
            if (pRing != null)
                pRing.IsActive = false;
            Children.Add(new TextBlock { Text = "Unable to load image", Foreground = new SolidColorBrush(Colors.White), Margin = new Thickness(5,5,0,0)});
        }

        private void ImageOnImageOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var image = (Image)sender;
            image.ImageOpened -= ImageOnImageOpened;
            image.ImageFailed -= ImageOnImageFailed;
            ProgressRing pRing = this.Children[0] as ProgressRing;
            if (pRing != null)
                pRing.IsActive = false;
            Rebuild(image);
        }

        private void Rebuild(Image image)
        {
            /*if (AnnotationCollection.Count > 0 && ShowAnnotation == false)
            {
                _annotationFlag.Visibility = Visibility.Visible;
                this.Children.Add(_annotationFlag);
            }*/
            if (AnnotationCollection == null || ShowAnnotation==false)
                return;


            InstructableDetailViewModel.CurrentInstructbaleDV.NoteText = "";
            foreach (var annotation in AnnotationCollection)
            {
                Border b = new Border();
                b.BorderThickness = new Thickness(1.0);
                b.BorderBrush = new SolidColorBrush(Colors.Black);

                Border b2 = new Border();
                b2.Background = new SolidColorBrush(Colors.Transparent);
                b2.BorderThickness = new Thickness(1.0);
                b2.BorderBrush = new SolidColorBrush(Colors.White);
                b.Child = b2;
                TextBlock tb = new TextBlock();
                tb.Text = annotation.text;
                tb.MaxWidth = 400;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontSize = 16;
                ToolTip newTip = new ToolTip();
                newTip.Content = tb;
                ToolTipService.SetToolTip(b2, newTip);
                b2.IsRightTapEnabled = true;
                b2.IsTapEnabled = true;
                b2.Tapped += (sender, args) =>
                {
                    if (args.PointerDeviceType != PointerDeviceType.Touch)
                        return;
                    //Debug.WriteLine("Grid was right tapped");
                    ToolTip tooltip = ToolTipService.GetToolTip(b2) as ToolTip;
                    if (tooltip != null)
                    {
                        tooltip.IsOpen = true;
                    }
                    if (InstructableDetailViewModel.CurrentInstructbaleDV != null)
                    {
                        InstructableDetailViewModel.CurrentInstructbaleDV.NoteText = annotation.text;
                    }
                    
                    args.Handled = true;
                };

                /*b2.RightTapped += (sender, args) =>
                {
                    if (args.PointerDeviceType != PointerDeviceType.Touch)
                        return;
                    //Debug.WriteLine("Grid was right tapped");
                    ToolTip tooltip = ToolTipService.GetToolTip(b2) as ToolTip;
                    if (tooltip != null)
                    {
                        tooltip.IsOpen = true;
                    }
                    args.Handled = true;
                };*/

                double top = annotation.top * image.ActualHeight;
                double left = annotation.left * image.ActualWidth;
                double width = annotation.width * image.ActualWidth;
                double height = annotation.height * image.ActualHeight;

                b.Width = width;
                b.Height = height;
                b.HorizontalAlignment = HorizontalAlignment.Left;
                b.VerticalAlignment = VerticalAlignment.Top;
                b.Margin = new Thickness(left, top, 0, 0);

                this.Children.Add(b);
            }
        }
    }
}
