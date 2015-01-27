namespace Instructables.DataModel
{
    public class License
    {
        public string fullName { get; set; }
        public string url { get; set; }

        public string Abbr()
        {
            switch (fullName)
            {
                case "None ":
                    return "NONE";
                case "Public Domain":
                    return "PD";
                case "Attribution":
                    return "BY";
                case "Attribution-NonCommercial":
                    return "BY_NC";
                case "Attribution-NonCommercial-ShareAlike":
                    return "BY_NC_SA";
                case "Attribution-ShareAlike ":
                    return "BY_SA";
                case "Attribution-NonCommercial-NoDerivs":
                    return "BY_NC_ND";
                case "Attribution-NoDerivs ":
                    return "BY_ND";
                case "General Public License":
                    return "GPL";
                case "Lesser General Public License ":
                    return "LGPL";
                case "Apache License":
                    return "APACHE";
                default:
                    return "BY_NC_SA";
            }
        }
    }
}
