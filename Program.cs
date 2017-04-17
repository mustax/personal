using Application.Services.Implementation;
using Data.Entities.Builders;
using Data.Repositories;
using Domain.Services.Contract;
using Domain.Services.Implementation;
using Domain.Services.Implementation.PriceRules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CallPriceCalculator
{
    internal class Program
    {
        private static IDictionary<string, string> flags = new Dictionary<string, string>();
        private const string FileFlag = "-f";
        private const string FileValueSeparator = "-s";
        private const string FileSourceNumber = "-sourceNumber";

        private static void Main(string[] args)
        {
            ParseFlagsFromArguments(args);
            var repository = InitializeRepository();
            var calculator = InitializeCalculatorService();

            var service = new CallPriceService(repository, calculator);

            if (flags.ContainsKey(FileSourceNumber))
            {
                CalculateToSourceNumber(service);
            }
            else
            {
                CalculateCallLogFile(service);
            }

            Console.ReadLine();
        }

        private static void CalculateCallLogFile(CallPriceService service)
        {
            try
            {
                Console.WriteLine(service.CalculateCallPrice());
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format("An exception has occured: {0}", exception.Message));
            }
        }

        private static void CalculateToSourceNumber(CallPriceService service)
        {
            try
            {
                var sourceNumber = flags[FileSourceNumber];
                Console.WriteLine(string.Format("Total for source caller {0}: {1}", sourceNumber, service.CalculateCallPrice(sourceNumber)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format("An exception has occured: {0}", exception.Message));
            }
        }

        private static void ParseFlagsFromArguments(string[] args)
        {
            var arguments = args.ToList();

            CaptureFlag(arguments, FileFlag);
            CaptureFlag(arguments, FileValueSeparator);
            CaptureFlag(arguments, FileSourceNumber);
        }

        private static void CaptureFlag(List<string> arguments, string flag)
        {
            if (!arguments.Contains(flag))
            {
                return;
            }

            var index = arguments.IndexOf(flag);

            if (arguments.Count > index + 1)
            {
                var value = arguments.ElementAt(index + 1);
                flags.Add(flag, value);
            }
        }

        private static ICallPriceCalculatorService InitializeCalculatorService()
        {
            var pricingRules = new List<ICallPricingRule>();
            pricingRules.Add(new FirstFiveMinutesPricingRule());
            pricingRules.Add(new RemainingMinutesPricingRule());
            ICallPriceCalculatorService calculator = new CallPriceCalculatorService(pricingRules);

            return calculator;
        }

        private static CallLogRepository InitializeRepository()
        {
            CallLogBuilder builder;
            string valueSeparator;
            if (flags.TryGetValue(FileValueSeparator, out valueSeparator))
            {
                builder = new CallLogBuilder(valueSeparator[0]);
            }
            else
            {
                builder = new CallLogBuilder();
            }
            var repository = new CallLogRepository(flags[FileFlag], builder);

            return repository;
        }
    }
}