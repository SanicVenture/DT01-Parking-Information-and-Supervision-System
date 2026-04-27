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
    [Route("api/opencvpolygonsitems")]
    [ApiController]
    public class OpenCVPolygonsController : ControllerBase
    {
        private readonly OpenCVPolygonsContext _context;

        public OpenCVPolygonsController(OpenCVPolygonsContext context)
        {
            _context = context;
        }

        // DELETE: api/objectinspotitems/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpenCVPolygons(long id)
        {
            var todoItem = await _context.OpenCVPolygonsItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.OpenCVPolygonsItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
