using Domain.Services.Contract;

namespace Domain.Services.Implementation.PriceRules
{
    public class FirstFiveMinutesPricingRule : ICallPricingRule
    {
        private readonly double price = 0.05;

        public double GetRulePrice(double callDuration)
        {
            if (callDuration > 5)
            {
                return 5 * price;
            }

            return callDuration * price;
        }
    }
}