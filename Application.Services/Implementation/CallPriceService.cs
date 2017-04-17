using Application.Services.Contract;
using Application.Services.Entity;
using Data.Entities;
using Data.Repositories.Contract;
using Domain.Entities.Implementation;
using Domain.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Implementation
{
    public class CallPriceService : ICallPriceService
    {
        private readonly ICallLogRepository repository;
        private readonly ICallPriceCalculatorService calculator;

        public CallPriceService(ICallLogRepository repository, ICallPriceCalculatorService calculator)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository", "repository can not be null");
            }

            if (calculator == null)
            {
                throw new ArgumentNullException("calculator", "calculator can not be null");
            }

            this.repository = repository;
            this.calculator = calculator;
        }

        public CallLogSummary CalculateCallPrice()
        {
            var callLogs = GetCallLogs();

            var calculationDetail = calculator.Calculate(callLogs.Select(ToCallDetail).ToList());

            var result = new CallLogSummary(calculationDetail);
            return result;
        }

        private IEnumerable<CallLog> GetCallLogs()
        {
            var callLogs = repository.GetCallLogs();

            if (callLogs == null || !callLogs.Any())
            {
                throw new InvalidOperationException("there is no logs to calculate");
            }

            return callLogs;
        }

        public double CalculateCallPrice(string sourceNumber)
        {
            if (string.IsNullOrWhiteSpace(sourceNumber))
            {
                throw new ArgumentNullException("sourceNumber", "sourceNumber can not be null or empty");
            }

            var callLogs = GetCallLogs();
            var sourceNumberCalls = callLogs.Where(x => x.SourceNumber.Equals(sourceNumber));

            if (!sourceNumberCalls.Any())
            {
                throw new InvalidOperationException(string.Format("There is no logs for source caller {0}", sourceNumber));
            }

            var result = calculator.Calculate(sourceNumberCalls.Select(ToCallDetail).ToList());

            return Math.Round(result.CallsDetail.Sum(x => x.Price), 2);
        }

        private CallDetail ToCallDetail(CallLog callLog)
        {
            var callDetail = new CallDetail(callLog.CallStartDate, callLog.CallEndDate, callLog.SourceNumber, callLog.DestinationNumber);

            return callDetail;
        }
    }
}