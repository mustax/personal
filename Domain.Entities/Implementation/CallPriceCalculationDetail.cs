using System.Collections.Generic;

namespace Domain.Entities.Implementation
{
    public class CallPriceCalculationDetail
    {
        public CallPriceCalculationDetail(
            IList<CallDetail> callsDetail,
            string sourceNumberWithMaxOverallCallDuration,
            double totalOfRemainingSourceNumbers)
        {
            CallsDetail = callsDetail;
            SourceNumberWithMaxOverallCallDuration = sourceNumberWithMaxOverallCallDuration;
            TotalOfRemainingSourceNumbers = totalOfRemainingSourceNumbers;
        }

        public IList<CallDetail> CallsDetail { get; set; }

        public string SourceNumberWithMaxOverallCallDuration { get; set; }

        public double TotalOfRemainingSourceNumbers { get; set; }
    }
}