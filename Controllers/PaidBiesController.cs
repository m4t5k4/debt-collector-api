using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using debt_collector_api.Data;
using debt_collector_api.Models;

namespace debt_collector_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaidBiesController : ControllerBase
    {
        private readonly DebtCollectorContext _context;

        public PaidBiesController(DebtCollectorContext context)
        {
            _context = context;
        }

        // GET: api/PaidBies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaidBy>>> GetPaidBy()
        {
            return await _context.PaidBy.ToListAsync();
        }

        // GET: api/PaidBies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaidBy>> GetPaidBy(int id)
        {
            var paidBy = await _context.PaidBy.FindAsync(id);

            if (paidBy == null)
            {
                return NotFound();
            }

            return paidBy;
        }

        // PUT: api/PaidBies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaidBy(int id, PaidBy paidBy)
        {
            if (id != paidBy.Id)
            {
                return BadRequest();
            }

            _context.Entry(paidBy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaidByExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PaidBies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PaidBy>> PostPaidBy(PaidBy paidBy)
        {
            _context.PaidBy.Add(paidBy);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaidBy", new { id = paidBy.Id }, paidBy);
        }

        // DELETE: api/PaidBies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaidBy(int id)
        {
            var paidBy = await _context.PaidBy.FindAsync(id);
            if (paidBy == null)
            {
                return NotFound();
            }

            _context.PaidBy.Remove(paidBy);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaidByExists(int id)
        {
            return _context.PaidBy.Any(e => e.Id == id);
        }
    }
}
