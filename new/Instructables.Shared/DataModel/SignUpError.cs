using System;
using System.Collections.Generic;

namespace Instructables.DataModel
{
    public class SignUpError : IRequestError
    {
        public DetailError validationErrors { get; set; }

        public string toString()
        {
            return validationErrors.ToString();
        }
    }

    public class DetailError
    {
        public string screenName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string resetCode { get; set; }
    }  
}
