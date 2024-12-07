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
    public class BusRouteController : ControllerBase
    {
        private readonly DataContext _context;

        public BusRouteController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusRoute>>> GetBusRoutes()
        {
            if (_context.BusRoutes == null)
            {
                return NotFound();
            }
            return await _context.BusRoutes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusRoute>> GetBusRoute(int id)
        {
            if (_context.BusRoutes == null)
            {
                return NotFound();
            }
            var busRoute = await _context.BusRoutes.FindAsync(id);
            if (busRoute == null)
            {
                return NotFound();
            }
            return busRoute;
        }

        [HttpPost]
        public async Task<ActionResult<BusRoute>> PostBusRoute(BusRouteDTO busRouteDTO)
        {
            var busRoute = new BusRoute()
            {
                Departure = busRouteDTO.Departure,
                Destionation = busRouteDTO.Destionation,
                Distance = busRouteDTO.Distance,
                Duration = busRouteDTO.Duration,
                Price = busRouteDTO.Price,
                Available = busRouteDTO.Available
            };
            _context.BusRoutes.Add(busRoute);
            await _context.SaveChangesAsync();  
            return Ok(busRoute); 
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusRoute(int id, BusRouteDTO busRoute)
        {
            var route = await _context.BusRoutes.FindAsync(id);
            if (route == null) return NotFound("Không tìm thấy tuyến xe");
            route.Departure = busRoute.Departure;
            route.Available = busRoute.Available;
            route.Duration = busRoute.Duration;
            route.Price = busRoute.Price;
            route.Destionation = busRoute.Destionation;
            route.Distance = busRoute.Distance;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (route == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(route);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusRoute(int id)
        {
            if (_context.BusRoutes == null)
            {
                return NotFound();
            }

            var busRoute = await _context.BusRoutes.FindAsync(id);

            if (busRoute == null)
            {
                return NotFound();
            }

            _context.BusRoutes.Remove(busRoute);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
