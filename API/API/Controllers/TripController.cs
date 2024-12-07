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
    public class TripController : ControllerBase
    {
        private readonly DataContext _context;

        public TripController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trip>>> GetTrips()
        {
            if (_context.Trips == null)
            {
                return NotFound();
            }
            return await _context.Trips.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Trip>> GetTripById(int id)
        {
            if (_context.Trips == null)
            {
                return NotFound();
            }
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }
            return trip;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripsByRouteId(int id)
        {
            var route = await _context.BusRoutes
                .Include(r => r.Trips)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                return NotFound("Không tìm thấy tuyến xe.");
            }

            var trips = route.Trips.Select(b => new Trip
            {
                Id = b.Id,
                BusId = b.BusId,
                DriverId = b.DriverId,
                BusRouteId = b.BusRouteId,
                CoDriverId = b.CoDriverId,
                DepartureDate = b.DepartureDate,
                Duration = b.Duration,
                EndTime = b.EndTime,
                Status = b.Status
            });

            return Ok(trips);
        }

        //Lấy danh sách chuyến xe dựa trên id của bus
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripsByBusId(int id)
        {
            var route = await _context.Buses
                .Include(r => r.Trips)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                return NotFound("Không tìm thấy xe.");
            }

            var trips = route.Trips.Select(b => new Trip
            {
                Id = b.Id,
                BusRouteId = b.BusRouteId,
                BusId = b.BusId,
                DriverId= b.DriverId,
                CoDriverId = b.CoDriverId,
                DepartureDate = b.DepartureDate,
                Duration = b.Duration,
                EndTime = b.EndTime,
                Status = b.Status
            });

            return Ok(trips);
        }

        //LẤY DANH SÁCH CHUYẾN DỰA TRÊN ID CỦA TÀI XẾ
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripsByDriverId(int id)
        {
            var route = await _context.Drivers
                .Include(r => r.Trips)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                return NotFound("Không tìm thấy tài xế!");
            }

            var trips = route.Trips.Select(b => new Trip
            {
                Id = b.Id,
                BusRouteId = b.BusRouteId,
                BusId = b.BusId,
                DriverId = b.DriverId,
                CoDriverId = b.CoDriverId,
                DepartureDate = b.DepartureDate,
                Duration = b.Duration,
                EndTime = b.EndTime,
                Status = b.Status
            });

            return Ok(trips);
        }

        //LẤY DANH SÁCH CHUYẾN DỰA TRÊN ID CỦA PHỤ LÁI
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripsByCoDriverId(int id)
        {
            var route = await _context.CoDrivers
                .Include(r => r.Trips)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                return NotFound("Không tìm thấy phụ lái");
            }

            var trips = route.Trips.Select(b => new Trip
            {
                Id = b.Id,
                BusRouteId = b.BusRouteId,
                BusId = b.BusId,
                DriverId = b.DriverId,
                CoDriverId = b.CoDriverId,
                DepartureDate = b.DepartureDate,
                Duration = b.Duration,
                EndTime = b.EndTime,
                Status = b.Status
            });

            return Ok(trips);
        }

        // Thêm chuyến xe không thông qua Route ID
        [HttpPost]
        public async Task<ActionResult<Trip>> PostTrip(TripDTO tripDTO)
        {
            var busRoute = await _context.BusRoutes.FirstOrDefaultAsync(br => br.Id == tripDTO.BusRouteId);
            if (busRoute == null)
            {
                return NotFound("Không tìm thấy tuyến xe");
            }

            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == tripDTO.BusId && b.BusRouteId == tripDTO.BusRouteId);
            if (bus == null)
            {
                return BadRequest("Xe buýt không thuộc tuyến đường này");
            }

            DateTime dateTimeDeparture = tripDTO.DepartureDate;
            TimeSpan duration = busRoute.Duration;
            DateTime endTime = dateTimeDeparture.Add(duration);

            var trip = new Trip
            {
                BusRouteId = tripDTO.BusRouteId,
                BusId = tripDTO.BusId,
                DriverId = tripDTO.DriverId,
                CoDriverId = tripDTO.CoDriverId,
                DepartureDate = tripDTO.DepartureDate,
                Duration = duration,
                EndTime = endTime,
                Status = tripDTO.Status
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Thêm chuyến đi mới
                    _context.Trips.Add(trip);
                    await _context.SaveChangesAsync();

                    // Tạo 24 ghế cho chuyến mới
                    var seats = new List<Seat>();
                    for (int i = 1; i <= 24; i++)
                    {
                        string prefix = i <= 12 ? "A" : "B";
                        string seatNumber = (i <= 12 ? i : i - 12).ToString("D2");
                        seats.Add(new Seat
                        {
                            TripId = trip.Id,
                            Status = "Available",
                            SeatName = $"{prefix}{seatNumber}" // Tên ghế: A01, A02, ..., B12
                        });
                    }

                    // Thêm danh sách ghế vào database
                    _context.Seats.AddRange(seats);
                    await _context.SaveChangesAsync();

                    // Commit giao dịch
                    await transaction.CommitAsync();
                    return Ok(trip);
                }
                catch (Exception ex)
                {
                    // Rollback nếu có lỗi
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }


        // Thêm chuyến xe thông qua Route ID
        [HttpPost("{busRouteId}")]
        public async Task<ActionResult<Trip>> PostTripByRouteId(int busRouteId, TripDTO tripDTO)
        {
            var busRoute = await _context.BusRoutes.FirstOrDefaultAsync(br => br.Id == busRouteId);
            if (busRoute == null)
            {
                return NotFound("Không tìm thấy tuyến xe");
            }

            DateTime dateTimeDeparture = tripDTO.DepartureDate;
            TimeSpan duration = busRoute.Duration;
            DateTime endTime = dateTimeDeparture.Add(duration);

            var trip = new Trip()
            {
                Id = tripDTO.Id,
                BusRouteId = busRouteId,
                BusId = tripDTO.BusId,
                DriverId = tripDTO.DriverId,
                CoDriverId = tripDTO.CoDriverId,
                DepartureDate = tripDTO.DepartureDate,
                Duration = duration,
                EndTime = endTime,
                Status = tripDTO.Status
            };
            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();
            return Ok(trip);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrip(int id, TripDTO tripDTO)
        {
            var busRoute = await _context.BusRoutes.FirstOrDefaultAsync(br => br.Id == tripDTO.BusRouteId);
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null) return NotFound("Không tìm thấy chuyến xe");

            DateTime dateTimeDeparture = tripDTO.DepartureDate;
            TimeSpan duration = busRoute.Duration;
            DateTime endTime = dateTimeDeparture.Add(duration);

            trip.BusRouteId = tripDTO.BusRouteId;
            trip.BusId = tripDTO.BusId;
            trip.DriverId = tripDTO.DriverId;
            trip.CoDriverId = tripDTO.CoDriverId;
            trip.DepartureDate = tripDTO.DepartureDate;
            trip.Status = tripDTO.Status;
            trip.Duration = duration;
            trip.EndTime = endTime;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (trip == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(trip);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            if (_context.Trips == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips.FindAsync(id);

            if (trip == null)
            {
                return NotFound();
            }

            _context.Trips.Remove(trip);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
