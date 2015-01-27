using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;


namespace Instructables.Utils
{
    public class RichTextBlockHelper : DependencyObject
    {
        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  
        //This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string),
            typeof(RichTextBlockHelper),
            new PropertyMetadata(String.Empty, OnTextChanged));

        private static void OnTextChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            var control = sender as RichTextBlock;
            if (control != null)
            {
                control.Blocks.Clear();

                string value = e.NewValue.ToString();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\"?>");
                sb.AppendLine("<RichTextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:utils=\"using:Instructables.Utils\" >");
                sb.AppendLine(value);
                sb.AppendLine("</RichTextBlock>");

                try
                {
                    RichTextBlock newRichText = (RichTextBlock)XamlReader.Load(sb.ToString());
                    // Move the blocks in the new RichTextBlock to the target RichTextBlock
                    for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
                    {
                        Block b = newRichText.Blocks[i];
                        newRichText.Blocks.RemoveAt(i);
                        control.Blocks.Insert(0, b);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Exception formatting HTML content: {0}", ex.Message));
                }
            }
        }
    }
}
