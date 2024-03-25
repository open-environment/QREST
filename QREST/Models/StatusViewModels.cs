using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QREST.Models
{
    public class vmStatusIndex
    {
        public DateTime currTime { get; set; }
        public DateTime sampTime { get; set; }
        public double staleness { get; set; }
    }
}