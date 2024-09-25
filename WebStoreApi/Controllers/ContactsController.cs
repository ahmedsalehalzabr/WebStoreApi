using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStoreApi.Model;
using WebStoreApi.Services;

namespace WebStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly AppDbContext context;

        public ContactsController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects()
        {
            var listSubjects = await context.Subjects.ToListAsync();
            return Ok(listSubjects);
        }

        [HttpGet]
        public async Task <IActionResult> GetContacts(int? page)
        {
            if(page == null || page < 1)
            {
                page = 1;
            }
            int pageSize = 5;
            int totalPages = 0;

            decimal count = await context.Contacts.CountAsync();
            totalPages = (int) Math.Ceiling(count / pageSize);

            var contacts = await context.Contacts
                .Include(c => c.Subject)
                .OrderByDescending(c => c.Id)
                .Skip((int)(page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new
            {
                Contacts = contacts,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = page

            };
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(int id)
        {
            var contact = await context.Contacts.Include(c => c.Subject).FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
            {
                return NotFound();
            }
            return Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact(ContactDto contactDto)
        {
            var subject = await context.Subjects.FindAsync(contactDto.SubjectId);
            if (subject == null)
            {
                ModelState.AddModelError("Subject", "Please select a valid subject");
                return BadRequest(ModelState);
            }

            Contact contact = new Contact()
            {
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                Email = contactDto.Email,
                Phone = contactDto.Phone,
                Subject = subject,
                Message = contactDto.Message,
                CreatedAt = DateTime.Now
            };
            await context.Contacts.AddAsync(contact);
            context.SaveChanges();
            return Ok(contact);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, ContactDto contactDto)
        {
            var subject = await context.Subjects.FindAsync(contactDto.SubjectId);
            if (subject == null)
            {
                ModelState.AddModelError("Subject", "Please select a valid subject");
                return BadRequest(ModelState);
            }

            var contact = await context.Contacts.FindAsync(id);
            if(contact == null)
            {
                return NotFound();
            }
            contact.FirstName = contactDto.FirstName;
            contact.LastName = contactDto.LastName;
            contact.Email = contactDto.Email;
            contact.Phone = contactDto.Phone ?? "";
            contact.Subject = subject;
            contact.Message = contactDto.Message;
           

            context.SaveChanges();
            return Ok(contact);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id) 
        {
            var contact = await context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            context.Contacts.Remove(contact);

            context.SaveChanges();
            return Ok(contact);
        }
    }
}
