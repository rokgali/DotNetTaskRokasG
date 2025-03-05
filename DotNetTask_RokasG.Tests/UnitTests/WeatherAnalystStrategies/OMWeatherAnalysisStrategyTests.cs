using DotNetTask_RokasG.Configurations;
using DotNetTask_RokasG.Utils.Transformers.DTOs;
using Moq;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Xunit;
using DotNetTask_RokasG.Functionality.WeatherForecastAnalyzer.Strategies;

namespace DotNetTask_RokasG.Tests.UnitTests.WeatherAnalystStrategies
{
    public class OMWeatherAnalysisStrategyTests
    {
        private readonly Mock<IOptionsSnapshot<DecisionMakingSettings>> _mockOptionsSnapshot;
        private readonly OMWAnalysisStrategy _strategy;

        public OMWeatherAnalysisStrategyTests()
        {
            _mockOptionsSnapshot = new Mock<IOptionsSnapshot<DecisionMakingSettings>>();
            _mockOptionsSnapshot
                .Setup(options => options.Value)
                .Returns(new DecisionMakingSettings { PrecipitationThresholdPercentage = 10, PrecipitationLeadTime = 2, RefreshFrequencyMinutes = 1 });

            _strategy = new OMWAnalysisStrategy(_mockOptionsSnapshot.Object);
        }

        [Fact]
        public void ShouldTakeUmbrella_ShouldReturnTrue_WhenPrecipitationLikelihoodAndTimeMatch()
        {
            var testResponses = new List<OpenMeteoTransformedHourlyResponse>
            {
                new OpenMeteoTransformedHourlyResponse
                {
                    Time = DateTime.Now.AddHours(-1),
                    Precipitation = 5,
                    Temperature_2m = 20,
                    Precipitation_Probability = 50,
                    Rain = 5,
                    Snowfall = 0,
                    GettingWetLikelyhood = true
                },
                new OpenMeteoTransformedHourlyResponse
                {
                    Time = DateTime.Now.AddHours(1),
                    Precipitation = 0,
                    Temperature_2m = 20,
                    Precipitation_Probability = 0,
                    Rain = 0,
                    Snowfall = 0,
                    GettingWetLikelyhood = false
                }
            };

            var transformedResponseList = new OpenMeteoTransformedReponseList
            {
                TransformedHourlyResponses = testResponses
            };

            var result = _strategy.ShouldTakeUmbrella(transformedResponseList);

            Assert.True(result);
        }

        [Fact]
        public void ShouldTakeUmbrella_ShouldReturnFalse_WhenPrecipitationLikelihoodIsFalse()
        {
            var testResponses = new List<OpenMeteoTransformedHourlyResponse>
            {
                new OpenMeteoTransformedHourlyResponse
                {
                    Time = DateTime.Now.AddHours(-1),
                    Precipitation = 5,
                    Temperature_2m = 20,
                    Precipitation_Probability = 50,
                    Rain = 5,
                    Snowfall = 0,
                    GettingWetLikelyhood = false
                }
            };

            var transformedResponseList = new OpenMeteoTransformedReponseList
            {
                TransformedHourlyResponses = testResponses
            };

            var result = _strategy.ShouldTakeUmbrella(transformedResponseList);

            Assert.False(result);
        }

        [Fact]
        public void WillGetWet_ShouldReturnTrue_WhenPrecipitationExceedsThreshold()
        {
            int precipitation = 15;

            var result = _strategy.WillGetWet(precipitation);

            Assert.True(result);
        }

        [Fact]
        public void WillGetWet_ShouldReturnFalse_WhenPrecipitationIsBelowThreshold()
        {
            int precipitation = 5;

            var result = _strategy.WillGetWet(precipitation);

            Assert.False(result);
        }

        [Fact]
        public void ShouldTakeUmbrella_ShouldReturnFalse_WhenTransformedResponseListIsEmpty()
        {
            var transformedResponseList = new OpenMeteoTransformedReponseList
            {
                TransformedHourlyResponses = new List<OpenMeteoTransformedHourlyResponse>()
            };

            var result = _strategy.ShouldTakeUmbrella(transformedResponseList);

            Assert.False(result);
        }
    }
}
