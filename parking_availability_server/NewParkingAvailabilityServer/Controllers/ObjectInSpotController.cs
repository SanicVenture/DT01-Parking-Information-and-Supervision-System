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
    [Route("api/ObjectInSpotItems")]
    [ApiController]
    public class ObjectInSpotController : ControllerBase
    {
        private readonly ObjectInSpotContext _context;

        private SQLManager sQLManager = new SQLManager();

        public ObjectInSpotController(ObjectInSpotContext context)
        {
            _context = context;
        }

        // GET: api/objectinspotitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ObjectInSpotItem>>> GetObjectInSpotItems()
        {
            return await _context.ObjectInSpotItems.ToListAsync();
        }

        // GET: api/objectinspotitems/id
        [HttpGet("{id}")]
        public async Task<ActionResult<ObjectInSpotItem>> GetObjectInSpotItem(long id)
        {
            var todoItem = await _context.ObjectInSpotItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/objectinspotitems/id
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutObjectInSpotItem(long id, ObjectInSpotItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            if (!ObjectInSpotItemExists(id))
            {
                await sQLManager.createNewObjectInSpotEntry(todoItem);
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ObjectInSpotItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            sQLManager.CheckForOpenCVData(id);

            return NoContent();
        }

        // POST: api/objectinspotitems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ObjectInSpotItem>> PostObjectInSpotItem(ObjectInSpotItem todoItem)
        {
            _context.ObjectInSpotItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetObjectInSpotItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/objectinspotitems/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteObjectInSpotItem(long id)
        {
            var todoItem = await _context.ObjectInSpotItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.ObjectInSpotItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ObjectInSpotItemExists(long id)
        {
            return _context.ObjectInSpotItems.Any(e => e.Id == id);
        }
    }
}
