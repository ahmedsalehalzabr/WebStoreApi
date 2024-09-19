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

        [HttpGet]
        public async Task <IActionResult> GetContacts()
        {
            var contacts = await context.Contacts.ToListAsync();
            return Ok(contacts);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(int id)
        {
            var contact = await context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }
            return Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact(ContactDto contactDto)
        {
            Contact contact = new Contact()
            {
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                Email = contactDto.Email,
                Phone = contactDto.Phone,
                Subject = contactDto.Subject,
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
            var contact = await context.Contacts.FindAsync(id);
            if(contact == null)
            {
                return NotFound();
            }
            contact.FirstName = contactDto.FirstName;
            contact.LastName = contactDto.LastName;
            contact.Email = contactDto.Email;
            contact.Phone = contactDto.Phone ?? "";
            contact.Subject = contactDto.Subject;
            contact.Message = contactDto.Message;
            contact.CreatedAt = DateTime.Now;

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
