using Domain.Entities.Implementation;
using Domain.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services.Implementation
{
    public class CallPriceCalculatorService : ICallPriceCalculatorService
    {
        private IEnumerable<ICallPricingRule> pricingRules;

        public CallPriceCalculatorService(IEnumerable<ICallPricingRule> pricingRules)
        {
            if (pricingRules == null || !pricingRules.Any())
            {
                throw new ArgumentNullException("pricingRules", "pricingRules can not be null or empty");
            }

            this.pricingRules = pricingRules;
        }

        public CallPriceCalculationDetail Calculate(IList<CallDetail> callsDetail)
        {
            CalculateCallPrice(callsDetail);

            var sourceNumberWithMaxOverallCallDuration = GetSourceNumberWithMaxOverallCallDuration(callsDetail);

            var totalOfRemainingSourceNumbers = callsDetail
                .Where(x => !sourceNumberWithMaxOverallCallDuration.Equals(x.SourceNumber))
                .Sum(x => x.Price);

            var callPriceCalculationDetail = new CallPriceCalculationDetail(callsDetail, sourceNumberWithMaxOverallCallDuration, totalOfRemainingSourceNumbers);

            return callPriceCalculationDetail;
        }

        private string GetSourceNumberWithMaxOverallCallDuration(IEnumerable<CallDetail> callsDetail)
        {
            var callsBySource = callsDetail.GroupBy
                (
                    x => x.SourceNumber,
                    (key, g) => new { SourceNumber = key, OverallCallDuration = g.Sum(t => t.Duration) }
                );

            var sourceNumber = callsBySource.Max(x => x.SourceNumber);

            return sourceNumber;
        }

        private void CalculateCallPrice(IList<CallDetail> callsDetail)
        {
            if (callsDetail == null || !callsDetail.Any())
            {
                throw new InvalidOperationException("there is no logs to calculate");
            }

            foreach (var callDetail in callsDetail)
            {
                callDetail.Price = pricingRules.Sum(x => x.GetRulePrice(callDetail.Duration));
            }
        }
    }
}