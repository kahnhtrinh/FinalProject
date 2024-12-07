using API.Models;
using API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.DTO;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TicketController : ControllerBase
    {
        private readonly DataContext _context;

        public TicketController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            var tickets = await _context.Tickets.ToListAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicketById(int id)
        {
            if (_context.Tickets == null)
            {
                return NotFound();
            }
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return ticket;
        }


        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] TicketDTO ticketDTO)
        {
            // Lấy thông tin chuyến xe từ TripId
            var trip = await _context.Trips.Include(t => t.Seats)
                                            .FirstOrDefaultAsync(t => t.Id == ticketDTO.TripId);

            if (trip == null)
            {
                return NotFound("Chuyến xe không tồn tại.");
            }

            // Tạo vé mới
            var ticket = new Ticket
            {
                TripId = ticketDTO.TripId,
                CreateDate = ticketDTO.CreateDate,
                CustomerName = ticketDTO.CustomerName,
                CustomerPhone = ticketDTO.CustomerPhone,
                CustomerEmail = ticketDTO.CustomerEmail,
                Status = ticketDTO.Status
            };

            // Tìm các ghế từ tên ghế
            var seatNames = ticketDTO.SeatNames;
            var selectedSeats = await _context.Seats
                                               .Where(s => seatNames.Contains(s.SeatName) && s.TripId == ticketDTO.TripId)
                                               .ToListAsync();

            // Kiểm tra xem các ghế đã được đặt chưa
            foreach (var seat in selectedSeats)
            {
                if (seat.Status == "Unavailable")
                {
                    return BadRequest($"Ghế {seat.SeatName} đã được đặt, không thể tạo vé.");
                }

                // Đánh dấu ghế là "Unavailable" và liên kết với vé
                seat.Status = "Unavailable";
                seat.TicketId = ticket.Id;
            }

            // Thêm vé vào cơ sở dữ liệu
            _context.Tickets.Add(ticket);

            // Thêm các ghế vào vé
            ticket.Seats = selectedSeats;

            // Lưu các thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return Ok("Vé được tạo thành công.");
        }


        [HttpGet("{tripId}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByTripId(int tripId)
        {
            var tickets = await _context.Tickets.Where(t => t.TripId == tripId).ToListAsync();
            if (tickets == null || tickets.Count == 0)
            {
                return NotFound("Không tìm thấy vé cho chuyến đi này.");
            }
            return Ok(tickets);
        }

        [HttpPut("{ticketId}")]
        public async Task<IActionResult> UpdateTicket(int ticketId, [FromBody] TicketDTO ticketDTO)
        {
            // Lấy vé cần cập nhật
            var ticket = await _context.Tickets
                                       .Include(t => t.Seats) // Bao gồm ghế liên kết
                                       .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return NotFound("Vé không tồn tại.");
            }

            // Cập nhật thông tin khách hàng
            ticket.CustomerName = ticketDTO.CustomerName;
            ticket.CustomerPhone = ticketDTO.CustomerPhone;
            ticket.CustomerEmail = ticketDTO.CustomerEmail;
            ticket.Status = ticketDTO.Status;
            ticket.CreateDate = ticketDTO.CreateDate;

            // Lấy danh sách tên ghế mới từ DTO
            var newSeatNames = ticketDTO.SeatNames;

            // Lấy danh sách ghế cũ liên kết với vé
            var oldSeats = ticket.Seats.ToList();

            // Trả lại trạng thái "Available" cho ghế cũ không còn được chọn
            foreach (var seat in oldSeats)
            {
                if (!newSeatNames.Contains(seat.SeatName))
                {
                    seat.Status = "Available";
                    seat.TicketId = null; // Xóa liên kết với vé
                }
            }

            // Lấy danh sách ghế mới theo tên ghế
            var newSeats = await _context.Seats
                                         .Where(s => newSeatNames.Contains(s.SeatName) && s.TripId == ticketDTO.TripId)
                                         .ToListAsync();

            // Kiểm tra tính hợp lệ của ghế mới
            foreach (var seat in newSeats)
            {
                if (seat.Status == "Unavailable" && seat.TicketId != ticket.Id)
                {
                    return BadRequest($"Ghế {seat.SeatName} đã được đặt, không thể cập nhật vé.");
                }

                // Đánh dấu ghế là "Unavailable" và liên kết với vé
                seat.Status = "Unavailable";
                seat.TicketId = ticket.Id;
            }

            // Cập nhật danh sách ghế mới cho vé
            ticket.Seats = newSeats;

            // Lưu các thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return Ok("Cập nhật vé thành công.");
        }


        [HttpDelete("{ticketId}")]
        public async Task<IActionResult> DeleteTicket(int ticketId)
        {
            var ticket = await _context.Tickets
                                        .Include(t => t.Seats) // Bao gồm thông tin ghế đã gán cho vé
                                        .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return NotFound("Vé không tồn tại.");
            }

            // Cập nhật trạng thái ghế về "Available"
            foreach (var seat in ticket.Seats)
            {
                seat.Status = "Available"; // Đặt lại trạng thái ghế thành Available
            }

            // Xóa vé
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return Ok("Vé đã được xóa và ghế đã được trả lại.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSeatsByTicketId(int id)
        {
            try
            {
                // Lấy danh sách các ghế theo TicketId
                var seats = await _context.Seats
                                          .Where(s => s.TicketId == id)
                                          .ToListAsync();

                if (seats == null || !seats.Any())
                {
                    return NotFound($"Không có ghế nào được gắn với vé ID {id}.");
                }

                return Ok(seats);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpGet("{tripId}")]
        public async Task<IActionResult> GetSeatsByTripId(int tripId)
        {
            var seats = await _context.Seats
                                      .Where(s => s.TripId == tripId && s.Status == "Available")
                                      .Select(s => new { s.Id, s.SeatName })
                                      .ToListAsync();

            return Ok(seats);
        }


    }
}

