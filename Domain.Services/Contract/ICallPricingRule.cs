using System;

namespace Domain.Services.Contract
{
    public interface ICallPricingRule
    {
        double GetRulePrice(double callDuration);
    }
}