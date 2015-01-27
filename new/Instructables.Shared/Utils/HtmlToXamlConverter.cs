using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Instructables.Utils
{
    public static class Html2XamlConverter
    {
        private static Dictionary<string, Dictionary<string, string>> attributes = new Dictionary<string, Dictionary<string, string>>();
        private static Dictionary<string, TagDefinition> tags = new Dictionary<string, TagDefinition>(){
            {"div", new TagDefinition("<Span{0}>", "</Span>", true)},
            {"p", new TagDefinition("<Paragraph  LineStackingStrategy=\"MaxHeight\"{0}>", "</Paragraph>", true, true)},
            {"ul", new TagDefinition(parseList){MustBeTop = true, NewParagraph = true}},
            {"b", new TagDefinition("<Bold{0}>")},
            {"strong", new TagDefinition("<Bold{0}>")},
            {"i", new TagDefinition("<Italic{0}>")},
            {"u", new TagDefinition("<Underline{0}>")},
            {"br", new TagDefinition("<LineBreak />", "")},
            {"iframe", new TagDefinition(parseIframe){ MustBeTop = false, NewParagraph = false}},
            {"table", new TagDefinition(parseTable){ MustBeTop = true, NewParagraph = true}},
            {"a", new TagDefinition(parseLink){ MustBeTop = false, NewParagraph = false}},
            {"blockquote", new TagDefinition("<Paragraph TextIndent=\"12\"{0}>", "</Paragraph>", true, true)}
        };

        private static bool _inParagraph = false;

        /// <summary>
        /// Converts Html to Xaml.
        /// </summary>
        /// <param name="HtmlString">The Html to convert</param>
        /// <returns>Xaml markup that can be used as content in a RichTextBlock</returns>
        public static string Convert2Xaml(string HtmlString)
        {
            if (HtmlString == null)
                return String.Empty;
            if(HtmlString.Contains("<pre>"))
                HtmlString = HtmlString.Replace("\n", "<br>");

            populateAttributes();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HtmlString);
            StringBuilder xamlString = new StringBuilder();

            foreach (var node in doc.DocumentNode.ChildNodes)
            {
                processTopNode(xamlString, node, true);
            }
            if (_inParagraph)
                writeEndTag(xamlString, tags["p"], "p");
            //xamlString.Replace("&nbsp;"," ");
            //xamlString.Replace("&rsquo;", "'");
            //return System.Net.WebUtility.HtmlDecode(xamlString.ToString());
            return xamlString.ToString();
        }

        private static void processTopNode(StringBuilder xamlString, HtmlNode node, bool isTop = false)
        {
            HtmlNode nextNode = null;
            if (!string.IsNullOrWhiteSpace(node.InnerText))
            {
                if (testTop(node.FirstChild))
                {
                    processTopNode(xamlString, node.FirstChild);
                    return;
                }
                if (node.Name.Equals("blockquote", StringComparison.CurrentCultureIgnoreCase) || node.Name.Equals("ul", StringComparison.CurrentCultureIgnoreCase) || node.Name.Equals("p", StringComparison.CurrentCultureIgnoreCase))
                {
                    nextNode = processNode(xamlString, node, true);
                }
                else
                {
                    if (!_inParagraph)
                        writeBeginningTag(xamlString, tags["p"], "p");
                    nextNode = processNode(xamlString, node, true);
                    //if (_inParagraph)
                    //    writeEndTag(xamlString, tags["p"],"p");
                }
            }

            if (nextNode != null)
                processTopNode(xamlString, nextNode);
            if (node.Name == "br")
                processNode(xamlString, node, true);
            if (node.Name == "iframe")
                parseIframe(xamlString, node);
            if (!isTop && node.NextSibling != null)
            {
                if (testTop(node.NextSibling))
                    processTopNode(xamlString, node.NextSibling);
                else
                {
                    writeBeginningTag(xamlString, tags["p"], "p");
                    nextNode = processNode(xamlString, node.NextSibling);
                    writeEndTag(xamlString, tags["p"], "p");
                    if (nextNode != null)
                        processTopNode(xamlString, nextNode);
                }
            }
        }

        private static HtmlNode getNextTopNode(HtmlNode node)
        {
            if (node.NextSibling != null)
                if (testTop(node.NextSibling))
                    return node.NextSibling;
            //else
            //	return getNextTopNode(node.NextSibling);

            if (node.ParentNode != node.OwnerDocument.DocumentNode && node.ParentNode.NextSibling != null)
                if (testTop(node.ParentNode.NextSibling))
                    return node.ParentNode.NextSibling;
            //else
            //	return getNextTopNode(node.ParentNode.NextSibling);
            return null;
        }

        private static bool testTop(HtmlNode node)
        {
            if (node == null)
                return false;
            return (tags.ContainsKey(node.Name) && tags[node.Name].MustBeTop);
        }

        private static HtmlNode processNode(StringBuilder xamlString, HtmlNode node, bool isTop = false)
        {
            string tagName = node.Name.ToLower();

            HtmlNode top = null;
            if (tags.ContainsKey(tagName))
            {
                if (tags[tagName].MustBeTop && !isTop)
                    return node;

                if (tags[tagName].IsCustom)
                {
                    tags[tagName].CustomAction(xamlString, node);
                    if (top == null && node.NextSibling != null && !isTop)
                        top = processNode(xamlString, node.NextSibling);
                    return null;
                }
                else
                {
                    writeBeginningTag(xamlString, tags[tagName], tagName);

                    if (node.HasChildNodes)
                        top = processNode(xamlString, node.FirstChild);

                    writeEndTag(xamlString, tags[tagName], tagName);
                }
            }
            else
            {
                if (node.NodeType == HtmlNodeType.Text)
                    xamlString.Append(FixURIContentString(node.InnerText));

                if (node.HasChildNodes)
                    top = processNode(xamlString, node.FirstChild);
            }

            if (top == null && node.NextSibling != null && !isTop)
                top = processNode(xamlString, node.NextSibling);

            return top;
        }

        private static string DecodeString(string htmlString)
        {
            return System.Net.WebUtility.HtmlDecode(htmlString).Replace("&", "&amp;");
            //return htmlString;
        }

        private static string FixURIContentString(string htmlString)
        {
            return System.Net.WebUtility.HtmlDecode(htmlString).Replace("&", "&amp;").Replace("\"", "'").Replace("<", "&lt;").Replace(">", "&gt;");
            //return htmlString;
        }

        private static void writeEndTag(StringBuilder xamlString, TagDefinition tag, string tagValue)
        {
            if (tagValue == "p")
                _inParagraph = false;
            xamlString.Append(tag.EndXamlTag);
        }

        private static void writeBeginningTag(StringBuilder xamlString, TagDefinition tag, string tagValue)
        {
            if (tag.NewParagraph && _inParagraph)
                writeEndTag(xamlString, tags["p"], "p");
            if (!_inParagraph && tagValue != "p" && tagValue != "blockquote")
            {
                writeBeginningTag(xamlString, tags["p"], "p");
            }
            if (tagValue == "p")
                _inParagraph = true;
            string attrs = string.Empty;
            if (tag.Attributes.Count > 0)
                attrs = " " + string.Join(" ", tag.Attributes.Select(a => string.Format("{0}=\"{1}\"", a.Key, a.Value)).ToArray());

            xamlString.Append(string.Format(tag.BeginXamlTag, attrs));
        }

        private static void populateAttributes()
        {
            foreach (var attribute in attributes)
            {
                if (tags.ContainsKey(attribute.Key))
                    foreach (var attr in attribute.Value)
                        if (!tags[attribute.Key].Attributes.ContainsKey(attr.Key))
                            tags[attribute.Key].Attributes.Add(attr.Key, attr.Value);
            }
        }
        /// <summary>
        /// Converts Html to Xaml including attributes that can be used to determine the formatting of individual elements.
        /// <example><code>
        /// string Xaml = Html2XamlConverter.Convert2Xaml(html, new Dictionary<string, Dictionary<string, string>> { 
        /// 					{ "p", new Dictionary<string, string> { { "Margin", "0,10,0,0" } } },
        /// 					{ "i", new Dictionary<string, string> { { "Foreground", "#FF663C00"}}}
        /// 					});
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="HtmlString">The Html to convert</param>
        /// <param name="TagAttributes">A dictionary that allows you to add attributes to the Xaml being emitted by this method. 
        /// The first key is the Html tag you want to add formatting to. The dictionary associated with that tag allows you to set
        /// multiple attributes and values associated with that Html tag.</param>
        /// <returns>Xaml markup that can be used as content in a RichTextBlock</returns>
        public static string Convert2Xaml(string HtmlString, Dictionary<string, Dictionary<string, string>> TagAttributes)
        {
            if (TagAttributes != null)
                attributes = TagAttributes;
            return Convert2Xaml(HtmlString);
        }

        private static void parseIframe(StringBuilder xamlString, HtmlNode iFrameInstance)
        {
            //Debug.WriteLine("IFrame found");
            //foreach (var attribute in iFrameInstance.Attributes)
            //{
            //    Debug.WriteLine(String.Format("{0} - {1}",attribute.Name, attribute.Value));
            //}
        }

        private static void parseList(StringBuilder xamlString, HtmlNode listNode)
        {
            //writeBeginningTag(xamlString, tags["br"], "br");
            if (_inParagraph)
                writeEndTag(xamlString, tags["p"], "p");
            // Yeah, this actually works out okay, though hard-coded margins and diamond symbol kinda suck.
            foreach (var li in listNode.Descendants("li"))
            {
                xamlString.Append("<Paragraph Margin=\"24,0,0,0\" TextIndent=\"-24\"><Run FontSize=\"18\" FontFamily=\"Segoe UI Symbol\">&#xE236;</Run><Span><Run Text=\" \"/>");
                _inParagraph = true;
                processNode(xamlString, li.FirstChild);
                xamlString.Append("</Span></Paragraph>");
                _inParagraph = false;
            }
        }
        private static void parseLink(StringBuilder xamlString, HtmlNode anchorNode)
        {
//            xamlString.Append("<InlineUIContainer><HyperlinkButton Style=\"{StaticResource NoMarginHyperlinkButtonStyle}\" Content=\"" + FixURIContentString(anchorNode.InnerText) + "\" NavigateUri=\"" + FixURIContentString(anchorNode.Attributes["href"].Value) + "\" /></InlineUIContainer>");
            xamlString.Append("<Underline Foreground=\"#FFF15322\" >");
            if (anchorNode.Attributes["href"] != null)
                xamlString.Append("<Hyperlink Foreground=\"#FFF15322\" NavigateUri=\"" + FixURIContentString(anchorNode.Attributes["href"].Value) + "\">" + FixURIContentString(anchorNode.InnerText) + "</Hyperlink>");
            else
                xamlString.Append("<Run Text=\"" + FixURIContentString(anchorNode.InnerText) + "\" utils:AttachedUri.UriSource=\"" + FixURIContentString(anchorNode.InnerText) + "\" />");
            xamlString.Append("</Underline>");
        }
        private static void parseTable(StringBuilder xamlString, HtmlNode tableNode)
        {
            // saddle up, this is going to be a bumpy ride! And yes, it IS a bit indirect to 
            // populate a grid programmatically and then use a custom parser. It turned out to be 
            // the easiest of a lot of really bad options.
            writeBeginningTag(xamlString, tags["p"], "p");
            xamlString.Append("<InlineUIContainer>");
            int currentRow = 0;
            int maxColumns = 0;
            CustomGrid tableGrid = new CustomGrid();
            TextBlock caption = null;
            var cap = tableNode.Descendants("caption").FirstOrDefault();
            if (cap != null)
            {
                caption = new TextBlock() { Text = DecodeString(cap.InnerText) };
                currentRow += 1;
                tableGrid.Children.Add(caption);
                tableGrid.RowDefinitions.Add(new RowDefinition());
            }

            foreach (var row in tableNode.Descendants("tr"))
            {
                int colMax;
                tableGrid.RowDefinitions.Add(new RowDefinition());
                int currentColumn = 0;
                foreach (var headerCell in row.Descendants("th"))
                {
                    TextBlock cell = new TextBlock();
                    cell.FontWeight = Windows.UI.Text.FontWeights.Bold;
                    cell.Foreground = new SolidColorBrush(Colors.Black);
                    colMax = setCellAttributes(currentRow, currentColumn, headerCell, cell);
                    if (colMax > maxColumns)
                        maxColumns = colMax;
                    tableGrid.Children.Add(cell);
                    currentColumn += 1;
                }

                foreach (var cell in row.Descendants("td"))
                {
                    TextBlock textCell = new TextBlock();
                    textCell.Foreground = new SolidColorBrush(Colors.Black);
                    colMax = setCellAttributes(currentRow, currentColumn, cell, textCell);
                    if (colMax > maxColumns)
                        maxColumns = colMax;
                    tableGrid.Children.Add(textCell);
                    currentColumn += 1;
                }
                currentRow += 1;
            }

            for (int xx = 0; xx <= maxColumns; xx++)
            {
                tableGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            if (maxColumns > 1 && caption != null)
                Grid.SetColumnSpan(caption, maxColumns);

            Dictionary<string, string> attr = new Dictionary<string, string>();
            if (attributes.ContainsKey("table"))
                attr = attributes["table"];
            string xaml = tableGrid.GetXaml(attr);
            xamlString.Append(xaml);
            xamlString.Append("</InlineUIContainer>");
            writeEndTag(xamlString, tags["p"], "p");
        }

        private static int setCellAttributes(int currentRow, int currentColumn, HtmlNode cellNode, TextBlock cell)
        {
            int rowSpan = cellNode.GetAttributeValue("rowspan", 0);
            int colSpan = cellNode.GetAttributeValue("colspan", 0);
            if (rowSpan > 0)
            {
                Grid.SetRowSpan(cell, rowSpan);
            }
            if (colSpan > 0)
            {
                Grid.SetColumnSpan(cell, colSpan);
            }
            if (currentRow > 0)
            {
                Grid.SetRow(cell, currentRow);
            }
            if (currentColumn > 0)
            {
                Grid.SetColumn(cell, currentColumn);
            }
            cell.Text = DecodeString(cellNode.InnerText);

            return colSpan + currentColumn;
        }
    }

    internal class TagDefinition
    {
        public TagDefinition()
        {
            this.Attributes = new Dictionary<string, string>();
        }

        public TagDefinition(string xamlTag)
            : this()
        {
            this.BeginXamlTag = xamlTag;
            this.EndXamlTag = string.Format(xamlTag, string.Empty).Replace("<", "</");
        }
        public TagDefinition(string xamlBeginTag, string xamlEndTag, bool mustBeTop = false, bool newParagraph = false)
            : this()
        {
            this.BeginXamlTag = xamlBeginTag;
            this.EndXamlTag = xamlEndTag;
            this.MustBeTop = mustBeTop;
            this.NewParagraph = newParagraph;
        }
        public TagDefinition(Action<StringBuilder, HtmlNode> customAction)
            : this()
        {
            this.CustomAction = customAction;
            this.IsCustom = true;
        }
        public string BeginXamlTag { get; set; }
        public string EndXamlTag { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public bool MustBeTop { get; set; }
        public bool IsCustom { get; set; }
        public Action<StringBuilder, HtmlNode> CustomAction { get; set; }
        public bool NewParagraph { get; set; }
    }

    public class CustomGrid : Grid
    {
        public string GetXaml(Dictionary<string, string> textBlockAttributes)
        {
            StringBuilder sb = new StringBuilder();
            // The viewbox is completely necessary as most tables tend to be rather big and letting the 
            // cells adapt on their own tends to lead to a lot of truncating.
            sb.Append("<Viewbox StretchDirection=\"DownOnly\"><Grid>");

            sb.Append("<Grid.ColumnDefinitions>");
            foreach (var column in this.ColumnDefinitions)
                sb.Append("<ColumnDefinition/>");
            sb.Append("</Grid.ColumnDefinitions>");

            sb.Append("<Grid.RowDefinitions>");
            foreach (var row in this.RowDefinitions)
                sb.Append("<RowDefinition/>");
            sb.Append("</Grid.RowDefinitions>");

            string tbAttr = string.Empty;
            if (textBlockAttributes.Count > 0)
                tbAttr = string.Join(string.Empty, textBlockAttributes.Select(tb => string.Format(" {0}=\"{1}\"", tb.Key, tb.Value)).ToArray());

            List<RowAdjustment> adjustments = new List<RowAdjustment>();
            foreach (var item in this.Children.OfType<TextBlock>())
            {
                int column = Grid.GetColumn(item);
                int row = Grid.GetRow(item);
                int colSpan = Grid.GetColumnSpan(item);
                int rowSpan = Grid.GetRowSpan(item);

                if (adjustments.Count > 0)
                    column += adjustments.Sum(a => a.Row == row && a.StartAt <= column ? a.AdjAmt : 0);

                bool isBold = item.FontWeight.Equals(Windows.UI.Text.FontWeights.Bold);
                sb.Append(string.Format("<TextBlock Foreground=\"Black\" TextTrimming=\"WordEllipsis\" TextWrapping=\"Wrap\"{0}", tbAttr));
                if (column > 0)
                    sb.Append(string.Format(" Grid.Column=\"{0}\" Margin=\"4,0,0,0\"", column));
                if (colSpan > 1)
                    sb.Append(string.Format(" Grid.ColumnSpan=\"{0}\" TextAlignment=\"Center\"", colSpan));
                if (row > 0)
                    sb.Append(string.Format(" Grid.Row=\"{0}\"", row));
                if (rowSpan > 1)
                {
                    for (int xx = 1; xx < rowSpan; xx++)
                    {
                        // To manually track pushing columns over in the Xaml.
                        adjustments.Add(new RowAdjustment() { Row = row + xx, StartAt = column, AdjAmt = colSpan });
                    }
                    sb.Append(string.Format(" VerticalAlignment=\"Center\" Grid.RowSpan=\"{0}\"", rowSpan));
                }
                if (isBold)
                    sb.Append(" FontWeight=\"Bold\"");
                sb.Append(string.Format(" Text=\"{0}\" />", item.Text));
            }
            sb.Append("</Grid></Viewbox>");
            return sb.ToString();
        }

        /// <summary>
        /// Because html can't overlap, it treats rowspans differently than Xaml. That means we have to track
        /// how far over to push subsequent columns manually. This class allows us to keep track of potentially
        /// multiple rowspan adjustments.
        /// </summary>
        private class RowAdjustment
        {
            public int Row { get; set; }
            public int StartAt { get; set; }
            public int AdjAmt { get; set; }
        }
    }

}

