using System;

namespace Domain.Entities.Implementation
{
    public class CallDetail
    {
        public CallDetail(
            DateTime startDate, DateTime endDate,
            string sourceNumber, string destinationNumber)
        {
            StartDate = startDate;
            EndDate = endDate;
            SourceNumber = sourceNumber;
            DestinationNumber = destinationNumber;

            Duration = CalculateCallDuration();
        }

        private double CalculateCallDuration()
        {
            var callDuration = EndDate - StartDate;
            var callDurationInMinutes = Math.Ceiling(callDuration.TotalMinutes);

            return callDurationInMinutes;
        }

        public DateTime StartDate { get; internal set; }

        public DateTime EndDate { get; internal set; }

        public string SourceNumber { get; internal set; }

        public string DestinationNumber { get; internal set; }

        public double Duration { get; internal set; }

        public double Price { get; set; }
    }
}