using Application.Services.Entity;

namespace Application.Services.Contract
{
    public interface ICallPriceService
    {
        CallLogSummary CalculateCallPrice();

        double CalculateCallPrice(string sourceNumber);
    }
}