using Domain.Entities.Implementation;
using System;
using System.Linq;
using System.Text;

namespace Application.Services.Entity
{
    public class CallLogSummary
    {
        public CallLogSummary(CallPriceCalculationDetail calculationDetail)
        {
            TotalCalls = calculationDetail.CallsDetail.Count;
            TotalDistinctCallers = calculationDetail.CallsDetail.GroupBy(x => x.SourceNumber).Count();
            MostActiveCaller = calculationDetail.SourceNumberWithMaxOverallCallDuration;
            FullCallsAmount = Math.Round(calculationDetail.CallsDetail.Sum(x => x.Price), 2);
            TotalCallsAmountWithoutActiveCaller = calculationDetail.TotalOfRemainingSourceNumbers;
        }

        public int TotalCalls { get; internal set; }

        public int TotalDistinctCallers { get; internal set; }

        public string MostActiveCaller { get; internal set; }

        public double FullCallsAmount { get; internal set; }

        public double TotalCallsAmountWithoutActiveCaller { get; internal set; }

        public override string ToString()
        {
            var msg = new StringBuilder();
            msg
                .Append("**Call log report**")
                .Append("\n")
                .Append(string.Format("Number of calls: {0}", TotalCalls))
                .Append("\n")
                .Append(string.Format("Number of distinguish source callers: {0}", TotalDistinctCallers))
                .Append("\n")
                .Append(string.Format("Source caller with highest total call duration: {0}", MostActiveCaller))
                .Append("\n")
                .Append(string.Format("Total of the day: {0}", FullCallsAmount))
                .Append("\n")
                .Append(string.Format("Total of the day discounting caller '{0}': {1}", MostActiveCaller, TotalCallsAmountWithoutActiveCaller));

            return msg.ToString();
        }
    }
}