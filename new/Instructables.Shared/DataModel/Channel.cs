using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instructables.Common;

namespace Instructables.DataModel
{
    public class Channel : BindableBase
    {
        public string title { get; set; }
        public string CategoryName { get; set; }
        private string _display;
        public string display 
        { 
            get
            {
                if (!String.IsNullOrEmpty(_display))
                    return _display;
                else
                {
                    return title;
                }
            }
            set { _display = value; }
        }
    }

    public class Channels
    {
        public List<Channel> technology { get; set; }
        public List<Channel> workshop { get; set; }
        public List<Channel> living { get; set; }
        public List<Channel> food { get; set; }
        public List<Channel> outside { get; set; }
        public List<Channel> play { get; set; }
    }
}
