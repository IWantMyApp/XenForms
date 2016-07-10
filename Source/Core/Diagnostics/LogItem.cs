using System;

namespace XenForms.Core.Diagnostics
{
    public class LogItem
    {
        public DateTime EventTime { get; set; }
        public string Severity { get; set; }
        public string Component { get; set; }
        public string Description { get; set; }


        public override string ToString()
        {
            return $"{EventTime:G}\t{Severity}\t{Description}";
        }
    }
}