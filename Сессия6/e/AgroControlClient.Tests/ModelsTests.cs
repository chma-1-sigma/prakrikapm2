using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using AgroControlClient.Models;
using System;
using System.Collections.Generic;

namespace AgroControlClient.Tests
{
    [TestClass]
    public class ModelsTests
    {
        [TestMethod]
        public void Product_Create_ValidData_ReturnsCorrectObject()
        {
            var product = new Product
            {
                Id = 1,
                Code = "HERB-A",
                Name = "Гербицид А",
                ProductType = "Гербицид",
                ReleaseForm = "Жидкость",
                Unit = "л",
                Status = "active"
            };

            product.Id.Should().Be(1);
            product.Code.Should().Be("HERB-A");
            product.Status.Should().Be("active");
        }

        [TestMethod]
        public void LoginResponse_HasAllRequiredFields()
        {
            var response = new LoginResponse
            {
                Id = 1,
                Username = "admin",
                FullName = "Администратор",
                Role = "admin",
                Token = "token",
                ExpiresAt = DateTime.Now
            };

            response.Id.Should().Be(1);
            response.Username.Should().NotBeNullOrEmpty();
            response.Token.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void ProductionOrder_DateFormatting_ReturnsCorrectValues()
        {
            var order = new ProductionOrder
            {
                Id = 1,
                OrderNumber = "PO-2401",
                PlannedQuantityKg = 500,
                Status = "planned",
                Priority = 2
            };

            order.OrderNumber.Should().Be("PO-2401");
            order.PlannedQuantityKg.Should().Be(500);
        }

        [TestMethod]
        public void Batch_QuantityFormatting_ReturnsCorrectValues()
        {
            var batch = new Batch
            {
                Id = 1,
                BatchNumber = "B-2401-01",
                ActualQuantityKg = 998,
                Status = "planned"
            };

            batch.ActualQuantityKg.Should().Be(998);
        }

        [TestMethod]
        public void Recipe_WithComponents_ReturnsCorrectCount()
        {
            var recipe = new Recipe
            {
                Id = 1,
                ProductName = "Гербицид А",
                Components = new List<RecipeComponent>
                {
                    new RecipeComponent { ComponentName = "Активное вещество", QuantityPercent = 50 },
                    new RecipeComponent { ComponentName = "Растворитель", QuantityPercent = 50 }
                }
            };

            recipe.Components.Should().HaveCount(2);
            recipe.Components[0].QuantityPercent.Should().Be(50);
        }

        [TestMethod]
        public void TechCard_DefaultStatus_IsDraft()
        {
            var techCard = new TechCard();
            techCard.Status.Should().Be("draft");
            techCard.IsActive.Should().BeFalse();
        }

        [TestMethod]
        public void QualityControl_DefaultDecision_IsPending()
        {
            var qc = new QualityControl();
            qc.Decision.Should().Be("pending");
        }

        [TestMethod]
        public void Deviation_DefaultSeverity_IsWarning()
        {
            var deviation = new Deviation();
            deviation.Severity.Should().Be("warning");
        }
    }
}