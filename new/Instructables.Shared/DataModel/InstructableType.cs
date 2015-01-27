using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instructables.DataModel
{
    public class InstructableType
    {
        public string Display { get; set; }
        public string Network { get; set; }
        public string ImageName { get; set; }
        public string MonoImageName { get; set; }
        public string ImageFullName
        {
            get { return String.Format("/assets/menuimages/{0}", ImageName); }
        }
        public string MonoImageFullName
        {
            get { return String.Format("/assets/menuimages/{0}", MonoImageName); }
        }
    }

    public class InstructableTypeOptions : List<InstructableType>
    {
        public InstructableTypeOptions()
        {
            this.Add(new InstructableType() { Display = "All Types", Network = "id", ImageName = "alltypes.png", MonoImageName = "alltypes-white.png" });
            this.Add(new InstructableType() { Display = "Step by Step", Network = "stepbystep", ImageName = "stepbystep.png", MonoImageName = "stepbystep-white.png" });
            this.Add(new InstructableType() { Display = "Photos", Network = "photos", ImageName = "photos.png", MonoImageName = "photos-white.png" });
            this.Add(new InstructableType() { Display = "Video", Network = "video", ImageName = "video.png", MonoImageName = "video-white.png" });
            // come back and fix this later, but for now e-Books are guides for lack of anything better
            this.Add(new InstructableType() { Display = "e-Book", Network = "guide&ebookFlag=true", ImageName = "ebook.png", MonoImageName = "ebook-white.png" });
            this.Add(new InstructableType() { Display = "Guides", Network = "guide", ImageName = "guide.png", MonoImageName = "guide-white.png" });
        }

        public InstructableType Find(string typeOption)
        {
            return this.Find(x => x.Display == typeOption);
        }
    }
}
