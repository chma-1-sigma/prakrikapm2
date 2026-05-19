using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControlAPI.Models;

namespace AgroControlAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StepParametersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StepParametersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/stepparameters/step/{stepId}
        [HttpGet("step/{stepId}")]
        public async Task<IActionResult> GetByStepId(int stepId)
        {
            var parameters = await _context.StepParameters
                .Where(p => p.StepId == stepId)
                .ToListAsync();
            return Ok(parameters);
        }

        // POST: api/stepparameters
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StepParameter parameter)
        {
            _context.StepParameters.Add(parameter);
            await _context.SaveChangesAsync();
            return Ok(new { id = parameter.Id, message = "Параметр добавлен", parameter });
        }

        // PUT: api/stepparameters/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StepParameter parameter)
        {
            if (id != parameter.Id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.StepParameters.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Параметр не найден" });

            existing.ParameterName = parameter.ParameterName;
            existing.PlannedValue = parameter.PlannedValue;
            existing.Unit = parameter.Unit;
            existing.ToleranceMin = parameter.ToleranceMin;
            existing.ToleranceMax = parameter.ToleranceMax;
            existing.IsCritical = parameter.IsCritical;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Параметр обновлен", parameter = existing });
        }

        // DELETE: api/stepparameters/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var parameter = await _context.StepParameters.FindAsync(id);
            if (parameter == null)
                return NotFound(new { message = "Параметр не найден" });

            _context.StepParameters.Remove(parameter);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Параметр удален" });
        }
    }
}