using API.Models;
using API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.DTO;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly DataContext _context;

        public BusController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bus>>> GetBuses()
        {
            if (_context.Buses == null)
            {
                return NotFound();
            }
            return await _context.Buses.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bus>> GetBus(int id)
        {
            if (_context.Buses == null)
            {
                return NotFound();
            }
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null)
            {
                return NotFound();
            }
            return bus;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusesByRouteId(int id)
        {
            var route = await _context.BusRoutes
                .Include(r => r.Buses)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                return NotFound("Không tìm thấy tuyến xe.");
            }

            // Trả về danh sách Bus dưới dạng DTO
            var buses = route.Buses.Select(b => new BusDTO
            {
                Id = b.Id,
                BusNumber = b.BusNumber,
                BeginDate = b.BeginDate,
                BusRouteId = b.BusRouteId,
                Status = b.Status,
            });

            return Ok(buses);
        }

        //Thêm Bus Không thông qua RouteId
        [HttpPost]
        public async Task<ActionResult<Bus>> PostBus(BusDTO busDTO)
        {
            var bus = new Bus()
            {
                Id = busDTO.Id,
                BusNumber = busDTO.BusNumber,
                BeginDate = busDTO.BeginDate,
                BusRouteId = busDTO.BusRouteId,
                Status = busDTO.Status
            };
            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();
            return Ok(bus);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBus(int id, BusDTO busDTO)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null) return NotFound("Không tìm thấy xe");
            bus.BusNumber = busDTO.BusNumber;
            bus.BeginDate = busDTO.BeginDate;
            bus.BusRouteId = busDTO.BusRouteId;
            bus.Status = busDTO.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (bus == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(bus);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            if (_context.Buses == null)
            {
                return NotFound();
            }

            var bus = await _context.Buses.FindAsync(id);

            if (bus == null)
            {
                return NotFound();
            }

            _context.Buses.Remove(bus);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
