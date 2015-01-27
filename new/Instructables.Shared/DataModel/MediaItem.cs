using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instructables.DataModel
{
    public enum MediaTypeOption
    {
        Video,
        Photo
    };

    public class MediaItem
    {
        public File FileMedia { get; set; }
        public Video VideoMedia { get; set; }
        public MediaTypeOption MediaType { get; set; }
    }
}
