using Domain.Entities.Implementation;
using Domain.Services.Contract;
using Domain.Services.Implementation;
using Domain.Services.Implementation.PriceRules;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services.Tests
{
    [TestFixture]
    public class CallPriceCalculatorServiceTests
    {
        private CallPriceCalculatorService service;

        [SetUp]
        public void Setup()
        {
            var pricingRules = new List<ICallPricingRule>();
            pricingRules.Add(new FirstFiveMinutesPricingRule());
            pricingRules.Add(new RemainingMinutesPricingRule());

            service = new CallPriceCalculatorService(pricingRules);
        }

        [Test]
        public void Constructor_NullPricingRuleList_ThrowArgumentNullException()
        {
            //act, assert
            Assert.Throws<ArgumentNullException>(() => new CallPriceCalculatorService(null));
        }

        [Test]
        public void Constructor_EmptyPricingRuleList_ThrowArgumentNullException()
        {
            //act, assert
            Assert.Throws<ArgumentNullException>(() => new CallPriceCalculatorService(new List<ICallPricingRule>()));
        }

        [Test]
        public void Calculate_NullCallDetailList_ThrowInvalidOperationException()
        {
            //act
            Assert.Throws<InvalidOperationException>(() => service.Calculate(null));
        }

        [Test]
        public void Calculate_EmptyCallDetailList_ThrowInvalidOperationException()
        {
            //act
            Assert.Throws<InvalidOperationException>(() => service.Calculate(new List<CallDetail>()));
        }

        [Test]
        public void Calculate_SingleCallDetailWithOneMinutes_ReturnCallWithPriceEqualToFiveCents()
        {
            //arrange
            var callsDetail = new List<CallDetail>();
            var currentTime = DateTime.Now;
            callsDetail.Add(new CallDetail(currentTime, currentTime.AddMinutes(1), "a", "b"));

            //act
            var result = service.Calculate(callsDetail);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0.05, result.CallsDetail.First().Price);
        }

        [Test]
        public void Calculate_SingleCallDetailWithFiveMinutes_ReturnCallWithPriceEqualToTweentyFiveCents()
        {
            //arrange
            var callsDetail = new List<CallDetail>();
            var currentTime = DateTime.Now;
            callsDetail.Add(new CallDetail(currentTime, currentTime.AddMinutes(5), "a", "b"));

            //act
            var result = service.Calculate(callsDetail);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0.25, result.CallsDetail.First().Price);
        }

        [Test]
        public void Calculate_SingleCallDetailWithSixMinutes_ReturnCallWithPriceEqualToTweentySevenCents()
        {
            //arrange
            var callsDetail = new List<CallDetail>();
            var currentTime = DateTime.Now;
            callsDetail.Add(new CallDetail(currentTime, currentTime.AddMinutes(6), "a", "b"));

            //act
            var result = service.Calculate(callsDetail);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0.27, result.CallsDetail.First().Price);
        }

        //[Test]
        //public void CalculateForSourceNumberA_NullCallDetailList_ThrowInvalidOperationException()
        //{
        //    //act
        //    Assert.Throws<InvalidOperationException>(() => service.Calculate(null, "a"));
        //}

        //[Test]
        //public void CalculateForSourceNumberA_EmptyCallDetailList_ThrowInvalidOperationException()
        //{
        //    //act
        //    Assert.Throws<InvalidOperationException>(() => service.Calculate(new List<CallDetail>(), "a"));
        //}

        //[Test]
        //public void CalculateForSourceNumberA_CallDetailWithOneMinutes_ReturnCallWithPriceEqualToFiveCents()
        //{
        //    //arrange
        //    var callsDetail = new List<CallDetail>();
        //    var currentTime = DateTime.Now;
        //    callsDetail.Add(new CallDetail(currentTime, currentTime.AddMinutes(1), "a", "b"));
        //    callsDetail.Add(new CallDetail(currentTime, currentTime.AddMinutes(34), "b", "a"));

        //    //act
        //    var result = service.Calculate(callsDetail, "a");

        //    //assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(0.05, result);
        //}

        //[Test]
        //public void CalculateForSourceNumberA_CallDetailWithFiveMinutes_ReturnCallWithPriceEqualToTweentyFiveCents()
        //{
        //    //arrange
        //    var callsDetail = new List<CallDetail>();
        //    var currentTime = DateTime.Now;
        //    callsDetail.Add(new CallDetail(currentTime, currentTime.AddMinutes(34), "b", "a"));
        //    callsDetail.Add(new CallDetail(currentTime, currentTime.AddMinutes(5), "a", "b"));

        //    //act
        //    var result = service.Calculate(callsDetail, "a");

        //    //assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(0.25, result);
        //}

        //[Test]
        //public void CalculateForSourceNumberA_CallDetailWithSixMinutes_ReturnCallWithPriceEqualToTweentySevenCents()
        //{
        //    //arrange
        //    var callsDetail = new List<CallDetail>();
        //    var currentTime = DateTime.Now;
        //    callsDetail.Add(new CallDetail(currentTime, currentTime.AddMinutes(6), "a", "b"));

        //    //act
        //    var result = service.Calculate(callsDetail, "a");

        //    //assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(0.27, result);
        //}
    }
}