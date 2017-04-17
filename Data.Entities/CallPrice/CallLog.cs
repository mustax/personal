using System;

namespace Data.Entities
{
    public class CallLog
    {
        public DateTime CallStartDate { get; set; }

        public DateTime CallEndDate { get; set; }

        public string SourceNumber { get; set; }

        public string DestinationNumber { get; set; }
    }
}