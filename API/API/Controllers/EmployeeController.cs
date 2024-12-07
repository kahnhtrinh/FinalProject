using API.Data;
using API.DTO;
using API.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Net.Mail;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly DataContext _context;

        public EmployeeController(DataContext context)
        {
            _context = context;
        }

        //Lấy tất cả TÀI XẾ
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDrivers()
        {
            if (_context.Drivers == null)
            {
                return NotFound();
            }
            return await _context.Drivers.ToListAsync();
        }

        //Lấy tất cả PHỤ LÁI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoDriver>>> GetCoDrivers()
        {
            if (_context.CoDrivers == null)
            {
                return NotFound();
            }
            return await _context.CoDrivers.ToListAsync();
        }

        //Lấy tất cả NHÂN VIÊN
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaffs()
        {
            if (_context.Staffs == null)
            {
                return NotFound();
            }
            return await _context.Staffs.ToListAsync();
        }


        //LẤY EMPLOYEE DỰA VÀO MÃ
        [HttpGet("{id}")]
        public async Task<ActionResult<Staff>> GetStaffById(int id)
        {
            if (_context.Staffs == null)
            {
                return NotFound();
            }
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            return staff;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriverById(int id)
        {
            if (_context.Drivers == null)
            {
                return NotFound();
            }
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            return driver;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoDriver>> GetCoDriverById(int id)
        {
            if (_context.CoDrivers == null)
            {
                return NotFound();
            }
            var codriver = await _context.CoDrivers.FindAsync(id);
            if (codriver == null)
            {
                return NotFound();
            }
            return codriver;
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetStaffByAccountId(int accountId)
        {
            var staff = await _context.Staffs
                                      .Include(s => s.Account)
                                      .FirstOrDefaultAsync(s => s.AccountId == accountId);

            if (staff == null)
            {
                return NotFound("Nhân viên không tìm thấy với AccountId này.");
            }
            return Ok(staff);
        }

        //LẤY EMPLOYEE QUA ROUTE ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDriversByRouteId(int id)
        {
            var route = await _context.BusRoutes
                .Include(r => r.Drivers)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                return NotFound("Không tìm thấy tuyến xe.");
            }

            var drivers = route.Drivers.Select(b => new DriverDTO
            {
                Id = b.Id,
                Name = b.Name,
                BirthDate = b.BirthDate,
                BusRouteId = b.BusRouteId,
                PhoneNumber = b.PhoneNumber,
                Image = b.Image,
                Role = b.Role,
                LicenseNumber = b.LicenseNumber,
                ExperienceYear = b.ExperienceYear,
                Status = b.Status
                
            });

            return Ok(drivers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCoDriversByRouteId(int id)
        {
            var route = await _context.BusRoutes
                .Include(r => r.CoDrivers)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                return NotFound("Không tìm thấy tuyến xe.");
            }

            var codrivers = route.CoDrivers.Select(b => new CoDriverDTO
            {
                Id = b.Id,
                Name = b.Name,
                BirthDate = b.BirthDate,
                BusRouteId = b.BusRouteId,
                PhoneNumber = b.PhoneNumber,
                Image = b.Image,
                Role = b.Role,
                LicenseNumber = b.LicenseNumber,
                ExperienceYear = b.ExperienceYear,
                Status = b.Status
            });

            return Ok(codrivers);
        }

        //Thêm Employee Không thông qua RouteId
        [HttpPost]
        public async Task<IActionResult> PostDriver(DriverDTO driveDTO)
        {

            var driver = new Driver
            {
                Id = driveDTO.Id,
                Name = driveDTO.Name,
                PhoneNumber = driveDTO.PhoneNumber,
                Image = driveDTO.Image,
                BirthDate = driveDTO.BirthDate,
                Role = driveDTO.Role,
                LicenseNumber = driveDTO.LicenseNumber,
                ExperienceYear = driveDTO.ExperienceYear,
                BusRouteId = driveDTO.BusRouteId,
                Status = driveDTO.Status,
            };
            await _context.Drivers.AddAsync(driver);
            await _context.SaveChangesAsync();
            return Ok($"Tài xế {driver.Name} tạo thành công!");
        }

        [HttpPost]
        public async Task<IActionResult> PostCoDriver(CoDriverDTO coDriveDTO)
        {

            var coDriver = new CoDriver
            {
                Id = coDriveDTO.Id,
                Name = coDriveDTO.Name,
                PhoneNumber = coDriveDTO.PhoneNumber,
                Image = coDriveDTO.Image,
                BirthDate = coDriveDTO.BirthDate,
                Role = coDriveDTO.Role,
                LicenseNumber = coDriveDTO.LicenseNumber,
                ExperienceYear = coDriveDTO.ExperienceYear,
                BusRouteId = coDriveDTO.BusRouteId,
                Status = coDriveDTO.Status,
            };
            await _context.CoDrivers.AddAsync(coDriver);
            await _context.SaveChangesAsync();
            return Ok($"Tài xế {coDriver.Name} tạo thành công!");
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] StaffDTO staffDTO)
        {
            // Kiểm tra nếu AccountId không tồn tại trong bảng Account
            var account = await _context.Accounts.FindAsync(staffDTO.AccountId);
            if (account == null)
            {
                return NotFound("Tài khoản không tồn tại.");
            }

            var staff = new Staff
            {
                Id =staffDTO.Id,
                Name = staffDTO.Name,
                PhoneNumber = staffDTO.PhoneNumber,
                BirthDate = staffDTO.BirthDate,
                Image = staffDTO.Image,
                Role = staffDTO.Role,
                Status = staffDTO.Status,
                EmailAddress = staffDTO.EmailAddress,
                AccountId = staffDTO.AccountId
            };
           
            _context.Staffs.Add(staff);
            
            await _context.SaveChangesAsync();
            account.StaffId = staff.Id;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Nhân viên đã được tạo thành công." });
        }

        //UPDATE EMPLOYEE
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDriver(int id, DriverDTO driverDTO)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null) return NotFound("Không tìm thấy tài xế");
            driver.Name = driverDTO.Name;
            driver.PhoneNumber = driverDTO.PhoneNumber;
            driver.Image = driverDTO.Image;
            driver.BirthDate = driverDTO.BirthDate;
            driver.Role = driverDTO.Role;
            driver.LicenseNumber = driverDTO.LicenseNumber;
            driver.ExperienceYear = driverDTO.ExperienceYear;
            driver.BusRouteId = driverDTO.BusRouteId;
            driver.Status = driverDTO.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (driver == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(driver);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaff(int id, StaffDTO staffDTO)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null) return NotFound("Không tìm thấy nhân viên");
            staff.Name = staffDTO.Name;
            staff.PhoneNumber = staffDTO.PhoneNumber;
            staff.Image = staffDTO.Image;
            staff.BirthDate = staffDTO.BirthDate;
            staff.Role = staffDTO.Role;
            staff.AccountId = staffDTO.AccountId;
            staff.EmailAddress = staffDTO.EmailAddress;
            staff.Status = staffDTO.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (staff == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(staff);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCoDriver(int id, CoDriverDTO codriverDTO)
        {
            var codriver = await _context.CoDrivers.FindAsync(id);
            if (codriver == null) return NotFound("Không tìm thấy tài xế");
            codriver.Name = codriverDTO.Name;
            codriver.PhoneNumber = codriverDTO.PhoneNumber;
            codriver.Image = codriverDTO.Image;
            codriver.BirthDate = codriverDTO.BirthDate;
            codriver.Role = codriverDTO.Role;
            codriver.LicenseNumber = codriverDTO.LicenseNumber;
            codriver.ExperienceYear = codriverDTO.ExperienceYear;
            codriver.BusRouteId = codriverDTO.BusRouteId;
            codriver.Status = codriverDTO.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (codriver == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(codriver);
        }

        //XÓA EMPLOYEE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            if (_context.Drivers == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
            {
                return NotFound();
            }

            _context.Drivers.Remove(driver);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            if (_context.Staffs == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs.FindAsync(id);
            var account = await _context.Accounts.FindAsync(staff.AccountId);

            if (staff == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            _context.Staffs.Remove(staff);

            await _context.SaveChangesAsync();
            

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoDriver(int id)
        {
            if (_context.CoDrivers == null)
            {
                return NotFound();
            }

            var codriver = await _context.CoDrivers.FindAsync(id);

            if (codriver == null)
            {
                return NotFound();
            }

            _context.CoDrivers.Remove(codriver);

            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
