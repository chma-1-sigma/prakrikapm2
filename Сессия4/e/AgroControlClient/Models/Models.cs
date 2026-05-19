using System;
using System.Collections.Generic;

namespace AgroControlClient.Models
{
    // ==================== АВТОРИЗАЦИЯ ====================
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    // ==================== ПРОДУКТЫ ====================
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string ReleaseForm { get; set; } = string.Empty;
        public string Unit { get; set; } = "кг";
        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; }
    }

    // ==================== РЕЦЕПТУРЫ ====================
    public class Recipe
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Version { get; set; }
        public string Status { get; set; } = "draft";
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<RecipeComponent> Components { get; set; } = new List<RecipeComponent>();
    }

    public class RecipeComponent
    {
        public int Id { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public decimal QuantityPercent { get; set; }
        public int LoadOrder { get; set; }
        public bool IsCritical { get; set; }
    }

    // ==================== ТЕХНОЛОГИЧЕСКИЕ КАРТЫ ====================
    public class TechCard
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Version { get; set; }
        public string Status { get; set; } = "draft";
        public bool IsActive { get; set; }
        public List<TechCardStep> Steps { get; set; } = new List<TechCardStep>();
    }

    public class TechCardStep
    {
        public int Id { get; set; }
        public int StepOrder { get; set; }
        public string StepName { get; set; } = string.Empty;
        public string StepType { get; set; } = string.Empty;
        public int PlannedDurationMin { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public List<StepParameter> Parameters { get; set; } = new List<StepParameter>();
    }

    public class StepParameter
    {
        public string ParameterName { get; set; } = string.Empty;
        public decimal PlannedValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal ToleranceMin { get; set; }
        public decimal ToleranceMax { get; set; }
        public bool IsCritical { get; set; }
    }

    // ==================== ЗАКАЗЫ И ПАРТИИ ====================
    public class ProductionOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int RecipeId { get; set; }
        public string RecipeName { get; set; } = string.Empty;
        public int PlannedQuantityKg { get; set; }
        public string Status { get; set; } = "planned";
        public int Priority { get; set; }
        public DateTime? PlannedStartDate { get; set; }
    }

    public class Batch
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = "planned";
        public int ActualQuantityKg { get; set; }
        public string LabStatus { get; set; } = "pending";
        public List<BatchStep> Steps { get; set; } = new List<BatchStep>();
    }

    public class BatchStep
    {
        public int Id { get; set; }
        public int StepOrder { get; set; }
        public string StepName { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool DeviationFlag { get; set; }
        public string OperatorComment { get; set; } = string.Empty;
        public List<ActualParameter> ActualParameters { get; set; } = new List<ActualParameter>();
    }

    public class ActualParameter
    {
        public string ParameterName { get; set; } = string.Empty;
        public decimal ActualValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsDeviated { get; set; }
        public string DeviationReason { get; set; } = string.Empty;
    }

    // ==================== ЛАБОРАТОРИЯ ====================
    public class QualityControl
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string SampleType { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
        public decimal MeasuredValue { get; set; }
        public string StandardValue { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string Decision { get; set; } = "pending";
        public DateTime AnalysisDate { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    // ==================== ОТКЛОНЕНИЯ ====================
    public class Deviation
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string StepName { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
        public string PlannedValue { get; set; } = string.Empty;
        public string ActualValue { get; set; } = string.Empty;
        public string Severity { get; set; } = "warning";
        public string Description { get; set; } = string.Empty;
        public DateTime ReportedAt { get; set; }
    }
}