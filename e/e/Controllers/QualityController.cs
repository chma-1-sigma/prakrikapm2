using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControlAPI.Models;

namespace AgroControlAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QualityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QualityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/quality/batches/pending
        [HttpGet("batches/pending")]
        public async Task<IActionResult> GetPendingBatches()
        {
            var batches = await _context.Batches
                .Where(b => b.Status == "completed" && b.LabStatus == "pending")
                .ToListAsync();
            return Ok(batches);
        }

        // GET: api/quality/batches/{batchId}/controls
        [HttpGet("batches/{batchId}/controls")]
        public async Task<IActionResult> GetBatchControls(int batchId)
        {
            var controls = await _context.QualityControls
                .Where(q => q.BatchId == batchId)
                .OrderByDescending(q => q.AnalysisDate)
                .ToListAsync();
            return Ok(controls);
        }

        // POST: api/quality/control
        [HttpPost("control")]
        public async Task<IActionResult> CreateQualityControl([FromBody] CreateQualityControlRequest request)
        {
            var batch = await _context.Batches.FindAsync(request.BatchId);
            if (batch == null)
                return BadRequest(new { message = "Партия не найдена" });

            batch.LabStatus = "in_progress";
            await _context.SaveChangesAsync();

            foreach (var param in request.Parameters)
            {
                var qualityControl = new QualityControl
                {
                    BatchId = request.BatchId,
                    SampleType = request.SampleType,
                    ParameterName = param.ParameterName,
                    MeasuredValue = param.MeasuredValue,
                    Unit = param.Unit,
                    StandardValue = GetStandardValue(param.ParameterName),
                    Result = CheckResult(param.MeasuredValue, GetStandardValue(param.ParameterName)),
                    Decision = "pending",
                    AnalystId = request.AnalystId,
                    AnalysisDate = DateTime.UtcNow,
                    Comment = param.Comment
                };

                _context.QualityControls.Add(qualityControl);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Результаты контроля сохранены" });
        }

        // PUT: api/quality/approve
        [HttpPut("approve")]
        public async Task<IActionResult> ApproveBatch([FromBody] ApproveQualityRequest request)
        {
            var batch = await _context.Batches.FindAsync(request.BatchId);
            if (batch == null)
                return BadRequest(new { message = "Партия не найдена" });

            var controls = await _context.QualityControls
                .Where(q => q.BatchId == request.BatchId && q.Decision == "pending")
                .ToListAsync();

            var hasFailed = controls.Any(c => c.Result == "fail");

            if (request.Decision == "approve" && hasFailed)
                return BadRequest(new { message = "Невозможно одобрить партию с не пройденными тестами" });

            foreach (var control in controls)
            {
                control.Decision = request.Decision == "approve" ? "approved" : "blocked";
                control.Comment = request.Comment;
            }

            batch.LabStatus = request.Decision == "approve" ? "approved" : "blocked";
            if (request.Decision == "block")
                batch.Status = "blocked";

            batch.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Партия {(request.Decision == "approve" ? "одобрена" : "заблокирована")}" });
        }

        private string GetStandardValue(string parameterName)
        {
            return parameterName switch
            {
                "concentration" => "≥97%",
                "ph" => "6.5-7.0",
                "moisture" => "≤2.5%",
                _ => "По ТУ"
            };
        }

        private string CheckResult(decimal measuredValue, string standardValue)
        {
            if (standardValue.Contains("≥"))
            {
                var minValue = decimal.Parse(standardValue.Replace("≥", "").Replace("%", ""));
                return measuredValue >= minValue ? "pass" : "fail";
            }
            if (standardValue.Contains("≤"))
            {
                var maxValue = decimal.Parse(standardValue.Replace("≤", "").Replace("%", ""));
                return measuredValue <= maxValue ? "pass" : "fail";
            }
            if (standardValue.Contains("-"))
            {
                var parts = standardValue.Split('-');
                var minValue = decimal.Parse(parts[0]);
                var maxValue = decimal.Parse(parts[1]);
                return measuredValue >= minValue && measuredValue <= maxValue ? "pass" : "fail";
            }
            return "pass";
        }
    }

    public class CreateQualityControlRequest
    {
        public int BatchId { get; set; }
        public string SampleType { get; set; } = string.Empty;
        public int AnalystId { get; set; }
        public List<QualityParameterDto> Parameters { get; set; } = new();
    }

    public class QualityParameterDto
    {
        public string ParameterName { get; set; } = string.Empty;
        public decimal MeasuredValue { get; set; }
        public string? Unit { get; set; }
        public string? Comment { get; set; }
    }

    public class ApproveQualityRequest
    {
        public int BatchId { get; set; }
        public string Decision { get; set; } = string.Empty;
        public string? Comment { get; set; }
    }
}