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
        private SQLManager sqlManager = new SQLManager();
        public ObjectInSpotController(ObjectInSpotContext context)
        {
            _context = context;
        }

        // GET: api/objectinspotitems
        // can probably be removed
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ObjectInSpotItem>>> GetObjectInSpotItems()
        {
            return await _context.ObjectInSpotItems.ToListAsync();
        }

        // GET: api/objectinspotitems/id
        // can probably be removed
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
        // just for testing
        [HttpPut("{id}")]
        public async Task<IActionResult> PutObjectInSpotItem(long id, ObjectInSpotItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            if (!ObjectInSpotItemExists(id))
            {
                await sqlManager.createNewObjectInSpotEntry(todoItem);
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

            sqlManager.CheckForOpenCVData(id); //will create a complete parking space if there is OpenCV data present

            return NoContent();
        }

        // POST: api/objectinspotitems
        // can probably be removed
        [HttpPost]
        public async Task<ActionResult<ObjectInSpotItem>> PostObjectInSpotItem(ObjectInSpotItem todoItem)
        {
            _context.ObjectInSpotItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetObjectInSpotItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/objectinspotitems/id
        // can probably be removed
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
