using System.Collections.Generic;

namespace Instructables.DataModel
{
    public class Introduction
    {
        public string id { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public int stepIndex { get; set; }
        public List<File> files { get; set; }
        public string XamlBody { get; set; }
    }
}
