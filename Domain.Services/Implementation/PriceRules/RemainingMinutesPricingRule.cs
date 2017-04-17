using Domain.Services.Contract;

namespace Domain.Services.Implementation.PriceRules
{
    public class RemainingMinutesPricingRule : ICallPricingRule
    {
        private readonly double price = 0.02;
        private readonly int minutesToExclude = 5;

        public double GetRulePrice(double callDuration)
        {
            if (callDuration > minutesToExclude)
            {
                return (callDuration - minutesToExclude) * price;
            }

            return 0;
        }
    }
}