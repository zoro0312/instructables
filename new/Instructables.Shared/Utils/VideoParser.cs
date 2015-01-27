using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Instructables.DataModel;

namespace Instructables.Utils
{
    public static class VideoParser
    {
        public static readonly Regex YoutubeVideoRegex = new Regex(@"youtu(?:\.be|be\.com)/(?:(.*)v(/|=)|(.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase);

        public static List<Video> ParseVideos(string htmlContent)
        {
            try
            {
                List<Video> result = new List<Video>();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);
                List<HtmlNode> iframeList = new List<HtmlNode>();
                foreach (var node in doc.DocumentNode.ChildNodes)
                {
                    if (node.Name == "iframe" || node.Name == "embed")
                        iframeList.Add(node);
                    if (node.HasChildNodes)
                    {
                        var childFrames = ParseChildNode(node);
                        if (childFrames != null)
                            iframeList.AddRange(childFrames);
                    }
                }
                foreach (var iframe in iframeList)
                {
                    Video video = new Video();
                    if (iframe.Name == "iframe")
                        video.UriString = iframe.Attributes.FirstOrDefault(x => x.Name == "src").Value;
                    else if (iframe.Name == "embed")
                    {
                        var srcString = iframe.Attributes.FirstOrDefault(x => x.Name == "src").Value;
                        Match youtubeMatch = YoutubeVideoRegex.Match(srcString);

                        string id = string.Empty;

                        if (youtubeMatch.Success)
                        {
                            id = youtubeMatch.Groups[4].Value;
                            video.UriString = String.Format(@"http://www.youtube.com/embed/{0}", id);
                        }
                        else
                        {
                            video.UriString = srcString;
                            Debug.WriteLine("Unable to match video type");
                        }
                    }
                    var heightAttribute = iframe.Attributes.FirstOrDefault(x => x.Name == "height");
                    string heightString = String.Empty;
                    if (heightAttribute != null)
                        heightString = heightAttribute.Value;
                    if (!String.IsNullOrEmpty(heightString))
                    {
                        int height;
                        if (int.TryParse(heightString, out height))
                        {
                            video.Height = height;
                        }
                        else
                        {
                            Debug.WriteLine("Setting video height to default value");
                            video.Height = 240;
                        }
                    }
                    string widthString = String.Empty;
                    var widthAttribute = iframe.Attributes.FirstOrDefault(x => x.Name == "width");
                    if (widthAttribute != null)
                        widthString = widthAttribute.Value;
                    if (!String.IsNullOrEmpty(widthString))
                    {
                        int width;
                        if (int.TryParse(widthString, out width))
                        {
                            video.Width = width;
                        }
                        else
                        {
                            Debug.WriteLine("Setting video width to default value");
                            video.Width = 320;
                        }
                    }
                    result.Add(video);
                }
                if (result.Count > 0)
                    return result;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static List<HtmlNode> ParseChildNode(HtmlNode node)
        {
            List<HtmlNode> results = null;
            if (node.Name == "iframe" || node.Name == "embed")
            {
                results = new List<HtmlNode>();
                results.Add(node);
            }
            if (node.HasChildNodes)
            {
                foreach (var childNode in node.ChildNodes)
                {
                    var childResults = ParseChildNode(childNode);
                    if (childResults != null)
                    {
                        if (results == null)
                            results = new List<HtmlNode>();
                        results.AddRange(childResults);
                    }
                }
            }
            return results;
        }
    }
}
