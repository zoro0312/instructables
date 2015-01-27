using System;
using Windows.UI.Xaml;

namespace Instructables.Utils
{
    public class AttachedUri : DependencyObject
    {
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.RegisterAttached(
            "UriSource", typeof(String), typeof(AttachedUri), new PropertyMetadata(String.Empty));

        public static void SetUriSource(DependencyObject obj, string value)
        {
            obj.SetValue(UriSourceProperty, value);
        }

        public static string GetUriSource(DependencyObject obj)
        {
            return (string)obj.GetValue(UriSourceProperty);
        }
    }
}
