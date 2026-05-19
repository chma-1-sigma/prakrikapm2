using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControlAPI.Models;

namespace AgroControlAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TechCardStepsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TechCardStepsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/techcardsteps/techcard/{techCardId}
        [HttpGet("techcard/{techCardId}")]
        public async Task<IActionResult> GetByTechCardId(int techCardId)
        {
            var steps = await _context.TechCardSteps
                .Where(s => s.TechCardId == techCardId)
                .OrderBy(s => s.StepOrder)
                .Include(s => s.Parameters)
                .ToListAsync();
            return Ok(steps);
        }

        // POST: api/techcardsteps
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TechCardStep step)
        {
            // Определяем следующий порядок
            var maxOrder = await _context.TechCardSteps
                .Where(s => s.TechCardId == step.TechCardId)
                .MaxAsync(s => (int?)s.StepOrder) ?? 0;
            step.StepOrder = maxOrder + 1;

            _context.TechCardSteps.Add(step);
            await _context.SaveChangesAsync();
            return Ok(new { id = step.Id, message = "Шаг добавлен", step });
        }

        // PUT: api/techcardsteps/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TechCardStep step)
        {
            if (id != step.Id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.TechCardSteps.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Шаг не найден" });

            existing.StepName = step.StepName;
            existing.StepType = step.StepType;
            existing.PlannedDurationMin = step.PlannedDurationMin;
            existing.IsMandatory = step.IsMandatory;
            existing.Instruction = step.Instruction;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Шаг обновлен", step = existing });
        }

        // PUT: api/techcardsteps/{id}/moveup
        [HttpPut("{id}/moveup")]
        public async Task<IActionResult> MoveUp(int id)
        {
            var step = await _context.TechCardSteps.FindAsync(id);
            if (step == null)
                return NotFound(new { message = "Шаг не найден" });

            var previousStep = await _context.TechCardSteps
                .Where(s => s.TechCardId == step.TechCardId && s.StepOrder < step.StepOrder)
                .OrderByDescending(s => s.StepOrder)
                .FirstOrDefaultAsync();

            if (previousStep != null)
            {
                var tempOrder = step.StepOrder;
                step.StepOrder = previousStep.StepOrder;
                previousStep.StepOrder = tempOrder;
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Шаг перемещен вверх" });
        }

        // PUT: api/techcardsteps/{id}/movedown
        [HttpPut("{id}/movedown")]
        public async Task<IActionResult> MoveDown(int id)
        {
            var step = await _context.TechCardSteps.FindAsync(id);
            if (step == null)
                return NotFound(new { message = "Шаг не найден" });

            var nextStep = await _context.TechCardSteps
                .Where(s => s.TechCardId == step.TechCardId && s.StepOrder > step.StepOrder)
                .OrderBy(s => s.StepOrder)
                .FirstOrDefaultAsync();

            if (nextStep != null)
            {
                var tempOrder = step.StepOrder;
                step.StepOrder = nextStep.StepOrder;
                nextStep.StepOrder = tempOrder;
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Шаг перемещен вниз" });
        }

        // DELETE: api/techcardsteps/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var step = await _context.TechCardSteps.FindAsync(id);
            if (step == null)
                return NotFound(new { message = "Шаг не найден" });

            // Перенумеровываем остальные шаги
            var remainingSteps = await _context.TechCardSteps
                .Where(s => s.TechCardId == step.TechCardId && s.StepOrder > step.StepOrder)
                .ToListAsync();

            foreach (var s in remainingSteps)
                s.StepOrder--;

            _context.TechCardSteps.Remove(step);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Шаг удален" });
        }
    }
}