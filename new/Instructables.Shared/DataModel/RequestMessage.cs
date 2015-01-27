using System;
using System.Collections.Generic;

namespace Instructables.DataModel
{
    public class RequestMessage : IRequestError
    {
        public string message { get; set; }
        public string screenName { get; set; }
        public string locale { get; set; }
        public string toString()
        {
            return message + " " + screenName + " " + locale;
        }
    }
}
