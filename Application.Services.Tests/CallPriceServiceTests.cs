using Application.Services.Contract;
using Application.Services.Implementation;
using Data.Entities;
using Data.Repositories.Contract;
using Domain.Services.Contract;
using Domain.Services.Implementation;
using Domain.Services.Implementation.PriceRules;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Tests
{
    [TestFixture]
    public class CallPriceServiceTests
    {
        private CallPriceService service;
        private Mock<ICallLogRepository> repository;

        [SetUp]
        public void Setup()
        {
            repository = new Mock<ICallLogRepository>();

            var pricingRules = new List<ICallPricingRule>();
            pricingRules.Add(new FirstFiveMinutesPricingRule());
            pricingRules.Add(new RemainingMinutesPricingRule());
            var calculator = new CallPriceCalculatorService(pricingRules);

            service = new CallPriceService(repository.Object, calculator);
        }

        [Test]
        public void Constructor_InvalidArguments_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CallPriceService(null, new Mock<ICallPriceCalculatorService>().Object));
            Assert.Throws<ArgumentNullException>(() => new CallPriceService(repository.Object, null));
        }

        [Test]
        public void CalculateCallPrice_NullCallLog_ThrowInvalidOperationException()
        {
            //arrange
            var callLogEntities = new List<CallLog>();

            repository.Setup(x => x.GetCallLogs()).Returns((IEnumerable<CallLog>)null);

            //act
            Assert.Throws<InvalidOperationException>(() => service.CalculateCallPrice());
        }

        [Test]
        public void CalculateCallPrice_EmptyListCallLog_ThrowInvalidOperationException()
        {
            //arrange
            var callLogEntities = new List<CallLog>();

            repository.Setup(x => x.GetCallLogs()).Returns(new List<CallLog>());

            //act
            Assert.Throws<InvalidOperationException>(() => service.CalculateCallPrice());
        }

        [Test]
        public void CalculateCallPrice_SingleCallLog_ReturnSummaryOfSingleCallLog()
        {
            //arrange
            var callLogEntities = new List<CallLog>();
            var currentTime = DateTime.Now;
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(3),
                SourceNumber = "a",
                DestinationNumber = "b"
            });
            repository.Setup(x => x.GetCallLogs()).Returns(callLogEntities);

            //act
            var result = service.CalculateCallPrice();

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0.15, result.FullCallsAmount, "FullCallsAmount is different");
            Assert.AreEqual("a", result.MostActiveCaller, "MostActiveCaller is different");
            Assert.AreEqual(1, result.TotalCalls, "TotalCalls is different");
            Assert.AreEqual(1, result.TotalDistinctCallers, "TotalDistinctCallers is different");
            Assert.AreEqual(0, result.TotalCallsAmountWithoutActiveCaller, "TotalCallsAmountWithoutActiveCaller is different");
        }

        [Test]
        public void CalculateCallPrice_FourCallLog_ReturnSummaryOfAllCallLog()
        {
            //arrange
            var callLogEntities = new List<CallLog>();
            var currentTime = DateTime.Now;
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(3),
                SourceNumber = "a",
                DestinationNumber = "b"
            });
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(6),
                SourceNumber = "a",
                DestinationNumber = "b"
            });
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(2),
                SourceNumber = "c",
                DestinationNumber = "a"
            });
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(8),
                SourceNumber = "c",
                DestinationNumber = "b"
            });
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(2),
                SourceNumber = "b",
                DestinationNumber = "a"
            });
            repository.Setup(x => x.GetCallLogs()).Returns(callLogEntities);

            //act
            var result = service.CalculateCallPrice();

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0.93, result.FullCallsAmount, "FullCallsAmount is different");
            Assert.AreEqual("c", result.MostActiveCaller, "MostActiveCaller is different");
            Assert.AreEqual(5, result.TotalCalls, "TotalCalls is different");
            Assert.AreEqual(3, result.TotalDistinctCallers, "TotalDistinctCallers is different");
            Assert.AreEqual(0.52, result.TotalCallsAmountWithoutActiveCaller, "TotalCallsAmountWithoutActiveCaller is different");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("  ")]
        public void CalculateCallPriceSourceCallA_InvalidSource_ThrowArgumentNullException(string sourceCaller)
        {
            //act, assert
            Assert.Throws<ArgumentNullException>(() => service.CalculateCallPrice(sourceCaller));
        }

        [Test]
        public void CalculateCallPriceSourceCallA_RepositoryReturnNull_ThrownInvalidOperationException()
        {
            //arrange
            repository.Setup(x => x.GetCallLogs()).Returns((IEnumerable<CallLog>)null);

            //act
            Assert.Throws<InvalidOperationException>(() => service.CalculateCallPrice("a"));
        }

        [Test]
        public void CalculateCallPriceSourceCallA_RepositoryReturnCallFromSourceCallB_ThrownInvalidOperationException()
        {
            //arrange
            var callLogEntities = new List<CallLog>();
            var currentTime = DateTime.Now;
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(3),
                SourceNumber = "b",
                DestinationNumber = "c"
            });
            repository.Setup(x => x.GetCallLogs()).Returns(callLogEntities);

            //act, assert
            Assert.Throws<InvalidOperationException>(() => service.CalculateCallPrice("a"));
        }

        [Test]
        public void CalculateCallPriceSourceCallA_RepositoryReturnCallFromSourceCallAWithThreeMinutes_ReturnFifteenCents()
        {
            //arrange
            var callLogEntities = new List<CallLog>();
            var currentTime = DateTime.Now;
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(3),
                SourceNumber = "a",
                DestinationNumber = "c"
            });
            repository.Setup(x => x.GetCallLogs()).Returns(callLogEntities);

            //act
            var result = service.CalculateCallPrice("a");

            //assert
            Assert.AreEqual(0.15, result);
        }

        [Test]
        public void CalculateCallPriceSourceCallA_RepositoryReturnCallFromSourceCallAndB_ReturnFifteenCents()
        {
            //arrange
            var callLogEntities = new List<CallLog>();
            var currentTime = DateTime.Now;
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(3),
                SourceNumber = "a",
                DestinationNumber = "c"
            });
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(14),
                SourceNumber = "b",
                DestinationNumber = "a"
            });
            repository.Setup(x => x.GetCallLogs()).Returns(callLogEntities);

            //act
            var result = service.CalculateCallPrice("a");

            //assert
            Assert.AreEqual(0.15d, result);
        }

        [Test]
        public void CalculateCallPriceSourceCallA_RepositoryReturnCallFromSourceCallAndB_ReturnFityCents()
        {
            //arrange
            var callLogEntities = new List<CallLog>();
            var currentTime = DateTime.Now;
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(3),
                SourceNumber = "a",
                DestinationNumber = "c"
            });
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(14),
                SourceNumber = "b",
                DestinationNumber = "a"
            });
            callLogEntities.Add(new CallLog
            {
                CallStartDate = currentTime,
                CallEndDate = currentTime.AddMinutes(10),
                SourceNumber = "a",
                DestinationNumber = "c"
            });
            repository.Setup(x => x.GetCallLogs()).Returns(callLogEntities);

            //act
            var result = service.CalculateCallPrice("a");

            //assert
            Assert.AreEqual(0.50, result);
        }
    }
}