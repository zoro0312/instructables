using System;
using System.Collections.Generic;

namespace Instructables.DataModel
{
    public class Instructable
    {
        public string id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string urlString { get; set; }
        public Author author { get; set; }
        public string ByAuthor
        {
            get
            {
                return author.byScreenName;
            }
        }

        public string viewsLabel
        {
            get
            {
                if (views > 1)
                    return "views";
                return "view";
            }
        }

        public string favoritesLabel
        {
            get
            {
                if (favorites > 1)
                    return "favorites";
                return "favorite";
            }
        }

        public string CommentLabel
        {
            get
            {
                if (comments > 1)
                    return "comments";
                return "comment";
            }
        }

        public string square2Url { get; set; }
        public string square2UrlDefault
        {
            get
            {
                if (square2Url == null || square2Url == string.Empty)
                    return "http://www.instructables.com/static/defaultIMG/default.SQUARE2.png";
                else
                    return square2Url;
            }
        }
        public string rectangle1Url { get; set; }
        public string type { get; set; }
        public string publishDate { get; set; }
        public CoveredImage coverImage { get; set; }
        public DateTime PublishDate
        {
            get
            {
                if (publishDate == null)
                {
                    return DateTime.Now;
                }
                else
                {
                    return DateTime.Parse(publishDate);
                }
            }
        }
        public string video { get; set; }
        public int editVersion { get; set; }
        public string category { get; set; }
        public string displayCategory { get; set; }
        public string channel { get; set; }
        public string displayChannel { get; set; }
        public int views { get; set; }
        public int favorites { get; set; }
        public int comments { get; set; }
        public License license { get; set; }
        public bool featureFlag { get; set; }
        public bool popularFlag { get; set; }
        public bool pgFlag { get; set; }
        public List<string> keywords { get; set; }
        public List<Step> steps { get; set; }
        public List<File> files { get; set; }
        public List<File> downloadFiles { get; set; }
        public Introduction introduction { get; set; }
        public List<InstructableGuideItem> instructables { get; set; }
        public bool favorite { get; set; }
        public bool following { get; set; }
        public bool HasVideos { get; set; }
        public List<VotableContest> votableContests { get; set; }
        public List<string> contestID { get; set; }
        public int GroupOrdinal { get; set; }
        public DataGroup Group { get; set; }
        public bool sponsoredFlag { get; set; }
        public bool isCollection
        {
            get
            {
                return type.StartsWith("guide");
            }
        }

        public Instructable()
        {
            // init the default value
            id = string.Empty;
            body = string.Empty;
            category = string.Empty;
            displayCategory = string.Empty;
            channel = string.Empty;
            displayChannel = string.Empty;
            license = new License();
            steps = new List<Step>();
            files = new List<File>();
            keywords = new List<string>();
        }
    }
}
