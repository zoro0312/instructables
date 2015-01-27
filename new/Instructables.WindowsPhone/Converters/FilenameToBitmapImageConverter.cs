using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

using Instructables.ViewModels;
using Instructables.DataModel;

namespace Instructables.Converters
{
    class FilenameToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            var thumbImages = ViewModelLocator.Instance.CreateVM.ThumbImages;
            File imagefile = value as File;
            if (imagefile != null)
            {
                if (imagefile.mediumUrl != null && imagefile.mediumUrl != string.Empty)
                    return imagefile.mediumUrl;
                else if (imagefile.name != null)
                {
                    
                    if (thumbImages.ContainsKey(imagefile.name))
                    {
                        return thumbImages[imagefile.name];
                    }

                    if (imagefile.name.StartsWith("ms-appx") || imagefile.name.StartsWith("http"))
                        return imagefile.name;
                }
            }

            string fileName = value as string;
            if (thumbImages.ContainsKey(fileName))
            {
                return thumbImages[fileName];
            }

            //var stepImages = ViewModelLocator.Instance.CreateVM.StepImages;

            //if (fileName == null || stepImages == null)
            //    return null;

            //foreach (StorageFile file in stepImages)
            //{
            //    if (file.Name == fileName)
            //    {
            //        var bitmapImage = new BitmapImage();
            //        var stream = file.GetThumbnailAsync(ThumbnailMode.PicturesView, 120, ThumbnailOptions.UseCurrentScale).AsTask();
            //        stream.Wait();
            //        bitmapImage.SetSource(stream.Result);
            //        return bitmapImage;
            //    }
            //}

            if (fileName.StartsWith("ms-appx") || fileName.StartsWith("http"))
                return fileName;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
