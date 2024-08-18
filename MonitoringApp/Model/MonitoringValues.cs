using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringApp.Model
{
    public class MonitoringValues
    {
        public string TimeStamp { get; set; } = string.Empty;
        public double Temp { get; set; }
        public double Light { get; set; }
        public double Air { get; set; }
        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }
        public double MaxLight { get; set; }
        public double MinLight { get; set; }
        public double MaxAir { get; set; }
        public double MinAir { get; set; }


    }
}
