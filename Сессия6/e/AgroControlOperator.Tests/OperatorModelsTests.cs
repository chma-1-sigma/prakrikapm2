using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using AgroControlOperator.Models;

namespace AgroControlOperator.Tests
{
    [TestClass]
    public class OperatorModelsTests
    {
        [TestMethod]
        public void User_Create_ValidData_ReturnsCorrectObject()
        {
            var user = new User
            {
                Id = 1,
                Username = "operator1",
                FullName = "Заводов Сергей Николаевич",
                Role = "operator",
                Shift = "Утренняя"
            };

            user.Id.Should().Be(1);
            user.Role.Should().Be("operator");
            user.Shift.Should().Be("Утренняя");
        }

        [TestMethod]
        public void ActiveBatch_Create_ValidData_ReturnsCorrectObject()
        {
            var batch = new ActiveBatch
            {
                Id = 1,
                BatchNumber = "B-2402-01",
                ProductName = "Инсектицид Б",
                Line = "Линия 1",
                CurrentStep = "Экструзия",
                Status = "running",
                Progress = 45,
                HasWarning = false,
                HasCritical = false
            };

            batch.BatchNumber.Should().Be("B-2402-01");
            batch.Progress.Should().Be(45);
            batch.HasWarning.Should().BeFalse();
        }

        [TestMethod]
        public void ActiveBatch_ZeroProgress_ReturnsZero()
        {
            var batch = new ActiveBatch { Progress = 0 };
            batch.Progress.Should().Be(0);
        }

        [TestMethod]
        public void ActiveBatch_FullProgress_Returns100()
        {
            var batch = new ActiveBatch { Progress = 100 };
            batch.Progress.Should().Be(100);
        }

        [TestMethod]
        public void ActiveBatch_ProgressOver100_ReturnsOver100()
        {
            var batch = new ActiveBatch { Progress = 150 };
            batch.Progress.Should().Be(150);
        }

        [TestMethod]
        public void ActiveBatch_HasWarningTrue_ReturnsTrue()
        {
            var batch = new ActiveBatch { HasWarning = true };
            batch.HasWarning.Should().BeTrue();
            batch.HasCritical.Should().BeFalse();
        }

        [TestMethod]
        public void ActiveBatch_HasCriticalTrue_ReturnsTrue()
        {
            var batch = new ActiveBatch { HasCritical = true };
            batch.HasCritical.Should().BeTrue();
        }

        [TestMethod]
        public void ActiveBatch_EmptyCurrentStep_ReturnsEmpty()
        {
            var batch = new ActiveBatch { CurrentStep = "" };
            batch.CurrentStep.Should().BeEmpty();
        }

        [TestMethod]
        public void User_RoleOperator_ReturnsCorrectRole()
        {
            var user = new User { Role = "operator" };
            user.Role.Should().Be("operator");
        }

        [TestMethod]
        public void User_RoleTechnologist_ReturnsCorrectRole()
        {
            var user = new User { Role = "technologist" };
            user.Role.Should().Be("technologist");
        }

        [TestMethod]
        public void User_RoleAdmin_ReturnsCorrectRole()
        {
            var user = new User { Role = "admin" };
            user.Role.Should().Be("admin");
        }

        [TestMethod]
        public void User_InvalidRole_ReturnsUnknown()
        {
            var user = new User { Role = "hacker" };
            user.Role.Should().Be("hacker");
        }

        [TestMethod]
        public void User_NullShift_ReturnsNull()
        {
            var user = new User { Shift = null };
            user.Shift.Should().BeNull();
        }

        [TestMethod]
        public void ActiveBatch_StatusPlanned_ReturnsPlanned()
        {
            var batch = new ActiveBatch { Status = "planned" };
            batch.Status.Should().Be("planned");
        }
    }
}