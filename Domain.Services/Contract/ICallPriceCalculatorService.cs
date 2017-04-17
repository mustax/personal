using Domain.Entities.Implementation;
using System.Collections.Generic;

namespace Domain.Services.Contract
{
    public interface ICallPriceCalculatorService
    {
        CallPriceCalculationDetail Calculate(IList<CallDetail> callLogs);
    }
}