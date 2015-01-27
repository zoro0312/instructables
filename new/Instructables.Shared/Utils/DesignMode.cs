using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instructables.Utils
{
    public static class DesignMode
    {
        public static bool IsInDesignMode()
        {
            return Windows.ApplicationModel.DesignMode.DesignModeEnabled;
        }
    }

}
