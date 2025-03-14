using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using libraryManegement.Context;
using libraryManegement.Models;
using libraryManegement.Dtos;

namespace libraryManegement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserDto userDto)
        {
            try
            {

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = userDto.Name,
                    Books = new List<Book>()
                };

                _context.Users.Add(user);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            try
            {

                var user = await _context.Users.Include(u => u.Books).FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            try
            {

                var users = await _context.Users.Include(u => u.Books).ToListAsync();


                Console.WriteLine(users.ToList());

                return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("borrowed-users")]
        public async Task<ActionResult<List<User>>> GetUsersWithBorrowedBooks()
        {
            try
            {

                var users = await _context.Users.Include(u => u.Books)
              .Where(u => u.Books.Any())
              .ToListAsync(); 

                return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("borrow")]
        public Task<ActionResult<User>> BorrowBook([FromBody] BorrowBookDto borrowBookDto)
        {
            try
            {

                var user = _context.Users.Include(u => u.Books).FirstOrDefault(u => u.Id == borrowBookDto.UserId);
                var book = _context.Books.FirstOrDefault(b => b.Id == borrowBookDto.BookId);



                if (user == null) return Task.FromResult<ActionResult<User>>(NotFound("User not found."));

                if (book == null) return Task.FromResult<ActionResult<User>>(NotFound("Book not found."));

                if (!book.IsAvailable) return Task.FromResult<ActionResult<User>>(BadRequest("Book not Avaiable"));



                if (user.Books.Count >= 3) return Task.FromResult<ActionResult<User>>(BadRequest("User cannot borrow more than 3 books."));

                book.IsAvailable = false;
                book.UserId = user.Id;
                user.Books.Add(book);
                _context.SaveChanges();



                return Task.FromResult<ActionResult<User>>(Ok(user));
            }
            catch (Exception e)
            {
                return Task.FromResult<ActionResult<User>>(BadRequest(e.Message));
            }
        }

        [HttpPost("return")]
        public Task<ActionResult<User>> ReturnBook([FromBody] BorrowBookDto borrowBookDto)
        {


            try
            {

                var user = _context.Users.Include(u => u.Books).FirstOrDefault(u => u.Id == borrowBookDto.UserId);
                var book = _context.Books.FirstOrDefault(b => b.Id == borrowBookDto.BookId);


                if (user == null) return Task.FromResult<ActionResult<User>>(NotFound("User not found."));

                if (book == null || !user.Books.Contains(book)) return Task.FromResult<ActionResult<User>>(NotFound("Book not found."));

                book.IsAvailable = true;
                book.UserId = null;
                user.Books.Remove(book);
                _context.SaveChanges();

                return Task.FromResult<ActionResult<User>>(Ok(user));
            }
            catch (Exception e)
            {
                return Task.FromResult<ActionResult<User>>(BadRequest(e.Message));
            }
        }
    }
}