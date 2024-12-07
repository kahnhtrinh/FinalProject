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
    public class NewController : ControllerBase
    {
        private readonly DataContext _context;

        public NewController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<New>>> GetNews()
        {
            if (_context.News == null)
            {
                return NotFound();
            }
            return await _context.News.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<New>> GetNew(int id)
        {
            if (_context.News == null)
            {
                return NotFound();
            }
            var newNew = await _context.News.FindAsync(id);
            if (newNew == null)
            {
                return NotFound();
            }
            return newNew;
        }

        [HttpPost]
        public async Task<ActionResult<BusRoute>> PostNew(New newNew)
        {
            var news = new New()
            {
                Title = newNew.Title,
                Content = newNew.Content,
                PublishedDate = newNew.PublishedDate,
            };
            _context.News.Add(news);
            await _context.SaveChangesAsync();
            return Ok(news);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNew(int id, New news)
        {
            var newss = await _context.News.FindAsync(id);
            if (newss == null) return NotFound("Không tìm thấy tuyến tin tức");
            newss.PublishedDate = news.PublishedDate;
            newss.Title = news.Title;
            newss.Content = news.Content;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (newss == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(newss);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNew(int id)
        {
            if (_context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);

            if (news == null)
            {
                return NotFound();
            }

            _context.News.Remove(news);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
