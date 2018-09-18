using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stomatology.Class
{
    public class Time
    {
        public TimeSpan time;
        public string user;

        public string ProcInfo { get; private set; }

        public Time(TimeSpan time, string user, string info)
        {
            this.time = time;
            this.user = user;
            ProcInfo = info;
        }
    }
}
