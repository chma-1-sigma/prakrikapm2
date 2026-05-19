using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControlAPI.Models;

namespace AgroControlAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BatchStepsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BatchStepsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/batchsteps/batch/{batchId}
        [HttpGet("batch/{batchId}")]
        public async Task<IActionResult> GetByBatchId(int batchId)
        {
            var steps = await _context.BatchStepExecutions
                .Where(s => s.BatchId == batchId)
                .Include(s => s.ActualParameters)
                .OrderBy(s => s.Id)
                .ToListAsync();
            return Ok(steps);
        }

        // POST: api/batchsteps/start
        [HttpPost("start")]
        public async Task<IActionResult> StartStep([FromBody] StartStepRequest request)
        {
            var batch = await _context.Batches.FindAsync(request.BatchId);
            if (batch == null)
                return NotFound(new { message = "Партия не найдена" });

            if (batch.Status != "running")
                return BadRequest(new { message = "Партия не запущена" });

            var existing = await _context.BatchStepExecutions
                .FirstOrDefaultAsync(s => s.BatchId == request.BatchId && s.StepId == request.StepId && s.Status == "in_progress");

            if (existing != null)
                return BadRequest(new { message = "Шаг уже выполняется" });

            var execution = new BatchStepExecution
            {
                BatchId = request.BatchId,
                StepId = request.StepId,
                OperatorId = request.OperatorId,
                StartTime = DateTime.UtcNow,
                Status = "in_progress"
            };

            _context.BatchStepExecutions.Add(execution);
            await _context.SaveChangesAsync();

            return Ok(new { id = execution.Id, message = "Шаг начат" });
        }

        // POST: api/batchsteps/complete
        [HttpPost("complete")]
        public async Task<IActionResult> CompleteStep([FromBody] CompleteStepRequest request)
        {
            // ИСПРАВЛЕНО: используем FindAsync без Include
            var execution = await _context.BatchStepExecutions
                .FindAsync(request.StepExecutionId);

            if (execution == null)
                return NotFound(new { message = "Выполнение шага не найдено" });

            if (execution.Status != "in_progress")
                return BadRequest(new { message = "Шаг не в процессе выполнения" });

            // Сохраняем фактические параметры
            foreach (var param in request.ActualParameters)
            {
                var actualParam = new StepActualParameter
                {
                    StepExecutionId = execution.Id,
                    ParameterName = param.ParameterName,
                    ActualValue = param.ActualValue,
                    Unit = param.Unit
                };

                // Проверяем отклонение
                var plannedParam = await _context.StepParameters
                    .FirstOrDefaultAsync(p => p.StepId == execution.StepId && p.ParameterName == param.ParameterName);

                if (plannedParam != null)
                {
                    if (plannedParam.ToleranceMin.HasValue && param.ActualValue < plannedParam.ToleranceMin)
                    {
                        actualParam.IsDeviated = true;
                        actualParam.DeviationReason = $"Значение {param.ActualValue} ниже допустимого минимума {plannedParam.ToleranceMin}";

                        var deviation = new Deviation
                        {
                            BatchId = execution.BatchId,
                            StepExecutionId = execution.Id,
                            ParameterName = param.ParameterName,
                            PlannedValue = plannedParam.PlannedValue?.ToString(),
                            ActualValue = param.ActualValue.ToString(),
                            Severity = plannedParam.IsCritical ? "critical" : "warning",
                            Description = actualParam.DeviationReason,
                            ReportedBy = request.OperatorId,
                            ReportedAt = DateTime.UtcNow
                        };
                        _context.Deviations.Add(deviation);
                    }
                    else if (plannedParam.ToleranceMax.HasValue && param.ActualValue > plannedParam.ToleranceMax)
                    {
                        actualParam.IsDeviated = true;
                        actualParam.DeviationReason = $"Значение {param.ActualValue} выше допустимого максимума {plannedParam.ToleranceMax}";

                        var deviation = new Deviation
                        {
                            BatchId = execution.BatchId,
                            StepExecutionId = execution.Id,
                            ParameterName = param.ParameterName,
                            PlannedValue = plannedParam.PlannedValue?.ToString(),
                            ActualValue = param.ActualValue.ToString(),
                            Severity = plannedParam.IsCritical ? "critical" : "warning",
                            Description = actualParam.DeviationReason,
                            ReportedBy = request.OperatorId,
                            ReportedAt = DateTime.UtcNow
                        };
                        _context.Deviations.Add(deviation);
                    }
                }

                _context.StepActualParameters.Add(actualParam);
            }

            execution.EndTime = DateTime.UtcNow;
            execution.Status = "completed";
            execution.OperatorComment = request.Comment;

            // Проверяем критические отклонения
            var hasCritical = await _context.Deviations
                .AnyAsync(d => d.StepExecutionId == execution.Id && d.Severity == "critical");

            if (hasCritical)
            {
                execution.DeviationFlag = true;
                // ИСПРАВЛЕНО: загружаем batch отдельно
                var batch = await _context.Batches.FindAsync(execution.BatchId);
                if (batch != null)
                {
                    batch.Status = "blocked";
                    batch.LabStatus = "blocked";
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Шаг завершен", hasCritical });
        }
    }

    public class StartStepRequest
    {
        public int BatchId { get; set; }
        public int StepId { get; set; }
        public int OperatorId { get; set; }
    }

    public class CompleteStepRequest
    {
        public int StepExecutionId { get; set; }
        public int OperatorId { get; set; }
        public List<ActualParameterDto> ActualParameters { get; set; } = new();
        public string? Comment { get; set; }
    }

    public class ActualParameterDto
    {
        public string ParameterName { get; set; } = string.Empty;
        public decimal ActualValue { get; set; }
        public string? Unit { get; set; }
    }
}