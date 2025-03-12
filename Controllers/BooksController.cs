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
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateBook([FromBody] CreateBookDto bookDto)
        {
            try
            {
                var book = new Book
                {
                    Id = Guid.NewGuid(),
                    Title = bookDto.Title,
                    Author = bookDto.Author,
                    PublicationYear = bookDto.PublicationYear,
                    ISBN = bookDto.ISBN,
                    IsAvailable = true,
                    UserId = null
                };

                _context.Books.Add(book);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetBook(Guid id)
        {
            try
            {
                var book = _context.Books.Find(id);
                if (book == null) return NotFound();
                return Ok(book);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("search")]
        public IActionResult GetBooksByFilter([FromQuery] string? title, [FromQuery] string? author, [FromQuery] string? isbn)
        {
            try
            {

                var result = _context.Books.AsQueryable();

                if (!string.IsNullOrEmpty(title)) result = result.Where(b => b.Title == title);
                if (!string.IsNullOrEmpty(author)) result = result.Where(b => b.Author == author);
                if (!string.IsNullOrEmpty(isbn)) result = result.Where(b => b.ISBN == isbn);

                var books = result.ToList();
                if (!books.Any()) return NotFound("No books found matching the search criteria.");

                return Ok(books);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _context.Books.ToList();
                return Ok(books);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("available")]
        public IActionResult GetAvailableBooks()
        {
            try
            {
                var availableBooks = _context.Books.Where(b => b.IsAvailable).ToList();
                return Ok(availableBooks);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("unavailable")]
        public IActionResult GetUnavailableBooks()
        {
            try
            {
                var unavailableBooks = _context.Books.Where(b => !b.IsAvailable).ToList();
                return Ok(unavailableBooks);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}