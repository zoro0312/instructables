using System;
using System.Collections.Generic;
using System.Text;

using Instructables.DataModel;

namespace Instructables.DataModel
{
    public class UploadResult
    {
        public List<File> loaded { get; set; }
    }

    public class CreateResult 
    {
        public string message;
        public string id;
        public List<string> stepIds;
    }
}
