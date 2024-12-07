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
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            return await _context.Accounts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            var bus = await _context.Accounts.FindAsync(id);
            if (bus == null)
            {
                return NotFound();
            }
            return bus;
        }

        [HttpGet("{staffId}")]
        public async Task<IActionResult> GetAccountByStaffId(int staffId)
        {
            var staff = await _context.Staffs
                                      .Include(s => s.Account)
                                      .FirstOrDefaultAsync(s => s.Id == staffId);

            if (staff == null || staff.Account == null)
            {
                return NotFound("Account không tìm thấy cho nhân viên này.");
            }
            return Ok(staff.Account);
        }

        //Thêm Bus Không thông qua RouteId
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(AccountDTO accountDTO)
        {
            var account = new Account()
            {
                Id = accountDTO.Id,
                CreatedDate = DateTime.UtcNow,
                Username = accountDTO.Username,
                Password = accountDTO.Password,
                Role = accountDTO.Role
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return Ok(account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, AccountDTO accountDTO)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound("Không tìm thấy tìa khoản");
            account.Username = accountDTO.Username;
            account.Password = account.Password;
            account.CreatedDate = account.CreatedDate;
            account.Role = account.Role;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (account == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(account);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }

            var bus = await _context.Accounts.FindAsync(id);

            if (bus == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(bus);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
