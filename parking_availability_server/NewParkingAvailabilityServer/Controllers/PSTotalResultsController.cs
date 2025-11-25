using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewParkingAvailabilityServer.Models;
using NewParkingAvailabilityServer;

namespace NewParkingAvailabilityServer.Controllers
{
    [Route("api/pstotalresultsitems")]
    [ApiController]
    public class PSTotalResultsController : ControllerBase
    {
        private readonly PSTotalResultsContext _context;

        private SQLManager sQLManager = new SQLManager();


        public PSTotalResultsController(PSTotalResultsContext context)
        {
            _context = context;
        }

        // GET: api/PSTotalResultsItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PSTotalResultsItem>>> GetPSTotalResultsItems()
        {
            return await _context.PSTotalResultsItems.ToListAsync();
        }

        // GET: api/PSTotalResultsItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PSTotalResultsItem>> GetPSTotalResultsItem(long id)
        {
            var todoItem = await _context.PSTotalResultsItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/PSTotalResultsItem/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPSTotalResultsItem(long id, PSTotalResultsItem todoItem)
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
                if (!PSTotalResultsItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            sQLManager.ChangeSpotFromFinalState(id);

            return NoContent();
        }

        // POST: api/PSTotalResultsItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ParkingSpaceItem>> PostPSTotalResultsItem(PSTotalResultsItem todoItem)
        {
            _context.PSTotalResultsItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPSTotalResultsItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/PSTotalResultsItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingSpaceItem(long id)
        {
            var todoItem = await _context.PSTotalResultsItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.PSTotalResultsItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PSTotalResultsItemExists(long id)
        {
            return _context.PSTotalResultsItems.Any(e => e.Id == id);
        }

    }
}
