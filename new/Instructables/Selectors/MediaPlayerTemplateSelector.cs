using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.DataModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Instructables.Selectors
{
    class MediaPlayerTemplateSelector : DataTemplateSelector
    {
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            MediaItem mediaItem = item as MediaItem;
            if (mediaItem != null)
            {
                if (mediaItem.MediaType == MediaTypeOption.Photo)
                    return (DataTemplate) Application.Current.Resources["PhotoViewerTemplate"];
                else if (mediaItem.MediaType == MediaTypeOption.Video)
                {
                    return (DataTemplate)Application.Current.Resources["VideoViewerTemplate"];
                }
            }
            return (DataTemplate)Application.Current.Resources["PhotoTemplate0"]; 
        }
    }
}
