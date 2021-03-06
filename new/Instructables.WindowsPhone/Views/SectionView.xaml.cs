﻿using System;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Instructables.Views
{
    public sealed partial class SectionView : UserControl
    {
        public SectionView()
        {
            this.InitializeComponent();
            Loaded += (sender, args) =>
                {
                    var dc = this.DataContext as Step;
                    //dc = null;
                    if (dc != null)
                    // Get the target RichTextBlock
                    {
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

                            // Move the blocks in the new RichTextBlock to the target RichTextBlock
                            for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
                            {
                                Block b = newRichText.Blocks[i];
                                newRichText.Blocks.RemoveAt(i);
                                ContentTextBlock.Blocks.Insert(0, b);
                            }
                            TextColumns.ResetLayout();

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
            else
            {
                var richTBO = sender as RichTextBlockOverflow;
                if (richTBO == null)
                    return;
                tp = richTBO.GetPositionFromPoint(e.GetPosition(richTBO));
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
    }
}
