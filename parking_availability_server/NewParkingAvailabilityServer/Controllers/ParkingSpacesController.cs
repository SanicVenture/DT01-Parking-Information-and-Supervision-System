using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewParkingAvailabilityServer.Models;

namespace NewParkingAvailabilityServer.Controllers
{
    [Route("api/parkingspaceitems")]
    [ApiController]
    public class ParkingSpacesController : ControllerBase
    {
        private readonly ParkingSpaceContext _context;

        public ParkingSpacesController(ParkingSpaceContext context)
        {
            _context = context;
        }

        // GET: api/parkingspaceitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParkingSpaceItem>>> GetParkingSpaceItems()
        {
            return await _context.ParkingSpaceItems.ToListAsync();
        }

        // GET: api/parkingspaceitems/id
        [HttpGet("{id}")]
        public async Task<ActionResult<ParkingSpaceItem>> GetParkingSpaceItem(long id)
        {
            var todoItem = await _context.ParkingSpaceItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/parkingspaceitems/id
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParkingSpaceItem(long id, ParkingSpaceItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkingSpaceItemExists(id))
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

        // POST: api/parkingspaceitems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ParkingSpaceItem>> PostParkingSpaceItem(ParkingSpaceItem todoItem)
        {
            _context.ParkingSpaceItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetParkingSpaceItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/parkingspaceitems/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingSpaceItem(long id)
        {
            var todoItem = await _context.ParkingSpaceItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.ParkingSpaceItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ParkingSpaceItemExists(long id)
        {
            return _context.ParkingSpaceItems.Any(e => e.Id == id);
        }
    }
}
