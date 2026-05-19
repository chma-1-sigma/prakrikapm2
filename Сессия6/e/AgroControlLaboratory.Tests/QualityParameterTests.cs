using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using AgroControlLaboratory.Models;
using System;

namespace AgroControlLaboratory.Tests
{
    [TestClass]
    public class QualityParameterTests
    {
        [TestMethod]
        public void QualityParameter_Create_ValidData_ReturnsCorrectObject()
        {
            var param = new QualityParameter
            {
                Id = 1,
                ParameterName = "concentration",
                StandardValue = "≥97%",
                Unit = "%",
                MeasuredValue = 97.5m,
                ToleranceMin = 97.0m,
                ToleranceMax = 100.0m,
                IsMandatory = true
            };

            param.ParameterName.Should().Be("concentration");
            param.MeasuredValue.Should().Be(97.5m);
            param.IsMandatory.Should().BeTrue();
        }

        [TestMethod]
        public void QualityParameter_DefaultResult_ReturnsPending()
        {
            var param = new QualityParameter();
            param.Result.Should().Be("⏳ Не проверено");
        }

        [TestMethod]
        public void CheckResult_Concentration_97_5_ReturnsPass()
        {
            var result = CheckResult(97.5m, "≥97%");
            result.Should().Be("pass");
        }

        [TestMethod]
        public void CheckResult_Concentration_96_ReturnsFail()
        {
            var result = CheckResult(96.0m, "≥97%");
            result.Should().Be("fail");
        }

        [TestMethod]
        public void CheckResult_pH_6_8_ReturnsPass()
        {
            var result = CheckResult(6.8m, "6.5-7.0");
            result.Should().Be("pass");
        }

        [TestMethod]
        public void CheckResult_pH_7_0_ReturnsPass()
        {
            var result = CheckResult(7.0m, "6.5-7.0");
            result.Should().Be("pass");
        }

        [TestMethod]
        public void CheckResult_pH_6_4_ReturnsFail()
        {
            var result = CheckResult(6.4m, "6.5-7.0");
            result.Should().Be("fail");
        }

        [TestMethod]
        public void CheckResult_pH_7_1_ReturnsFail()
        {
            var result = CheckResult(7.1m, "6.5-7.0");
            result.Should().Be("fail");
        }

        [TestMethod]
        public void CheckResult_Moisture_2_3_ReturnsPass()
        {
            var result = CheckResult(2.3m, "≤2.5%");
            result.Should().Be("pass");
        }

        [TestMethod]
        public void CheckResult_Moisture_3_0_ReturnsFail()
        {
            var result = CheckResult(3.0m, "≤2.5%");
            result.Should().Be("fail");
        }

        [TestMethod]
        public void BatchForQuality_ArrivalDateFormatting_ReturnsCorrectString()
        {
            var batch = new BatchForQuality
            {
                ArrivalDate = new DateTime(2026, 5, 16)
            };

            batch.ArrivalDateStr.Should().Be("16.05.2026");
        }

        [TestMethod]
        public void BatchForQuality_NullArrivalDate_ReturnsEmptyString()
        {
            var batch = new BatchForQuality { ArrivalDate = null };
            batch.ArrivalDateStr.Should().BeEmpty();
        }

        [TestMethod]
        public void BatchForQuality_QuantityFormatting_ReturnsCorrectString()
        {
            var batch = new BatchForQuality
            {
                ActualQuantityKg = 500
            };

            batch.QuantityStr.Should().Be("500 кг");
        }

        [TestMethod]
        public void BatchForQuality_StartTimeFormatting_ReturnsCorrectString()
        {
            var batch = new BatchForQuality
            {
                StartTime = new DateTime(2026, 5, 16, 10, 30, 0)
            };

            batch.StartTimeStr.Should().Be("16.05.2026 10:30");
        }

        [TestMethod]
        public void QualityTestHistory_TestDateFormatting_ReturnsCorrectString()
        {
            var history = new QualityTestHistory
            {
                TestDate = new DateTime(2026, 5, 16, 14, 0, 0)
            };

            history.TestDateStr.Should().Be("16.05.2026 14:00");
        }

        // Вспомогательный метод — копия логики из QualityController
        private string CheckResult(decimal measuredValue, string standardValue)
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;

            if (standardValue.Contains("≥"))
            {
                var valueStr = standardValue.Replace("≥", "").Replace("%", "").Trim();
                var minValue = decimal.Parse(valueStr, culture);
                return measuredValue >= minValue ? "pass" : "fail";
            }
            if (standardValue.Contains("≤"))
            {
                var valueStr = standardValue.Replace("≤", "").Replace("%", "").Trim();
                var maxValue = decimal.Parse(valueStr, culture);
                return measuredValue <= maxValue ? "pass" : "fail";
            }
            if (standardValue.Contains("-"))
            {
                var parts = standardValue.Split('-');
                var minValue = decimal.Parse(parts[0].Trim(), culture);
                var maxValue = decimal.Parse(parts[1].Trim(), culture);
                return measuredValue >= minValue && measuredValue <= maxValue ? "pass" : "fail";
            }
            return "pass";
        }
    }
}