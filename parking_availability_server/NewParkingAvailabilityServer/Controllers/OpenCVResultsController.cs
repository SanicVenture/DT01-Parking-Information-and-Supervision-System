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
    [Route("api/OpenCVResultsItems")]
    [ApiController]
    public class OpenCVResultsController : ControllerBase
    {
        private readonly OpenCVResultsContext _context;

        private SQLManager sQLManager = new SQLManager();

        public OpenCVResultsController(OpenCVResultsContext context)
        {
            _context = context;
        }

        // GET: api/OpenCVResultsItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OpenCVResultsItem>>> GetOpenCVResultsItems()
        {
            return await _context.OpenCVResultsItems.ToListAsync();
        }

        // GET: api/OpenCVResultsItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OpenCVResultsItem>> GetOpenCVResultsItem(long id)
        {
            var todoItem = await _context.OpenCVResultsItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/OpenCVResultsItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOpenCVResultsItem(long id, OpenCVResultsItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            if (!OpenCVResultsItemExists(id))
            {
                await sQLManager.createnewOpenCVResultsEntry(todoItem);
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OpenCVResultsItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            sQLManager.CheckForMicrocontrollerData(id);

            return NoContent();
        }

        // POST: api/OpenCVResultsItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OpenCVResultsItem>> PostOpenCVResultsItem(OpenCVResultsItem todoItem)
        {
            _context.OpenCVResultsItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOpenCVResultsItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/OpenCVResultsItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpenCVResultsItem(long id)
        {
            var todoItem = await _context.OpenCVResultsItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.OpenCVResultsItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OpenCVResultsItemExists(long id)
        {
            return _context.OpenCVResultsItems.Any(e => e.Id == id);
        }
    }
}
