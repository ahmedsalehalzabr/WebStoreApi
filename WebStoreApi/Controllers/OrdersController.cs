using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebStoreApi.Model;
using WebStoreApi.Services;

namespace WebStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext context;

        public OrdersController(AppDbContext context)
        {
            this.context = context;
        }

        //[Authorize]
        //[HttpPost]
        //public IActionResult CreateOrder(OrderDto orderDto)
        //{
        //    if (!OrderHelper.PaymentMethods.ContainsKey(orderDto.PaymentMethods))
        //    {
        //        ModelState.AddModelError("Payment Method", "Please select a valid payment method");
        //        return BadRequest(ModelState);
        //    }
        //    //  int userId = JwtReader.GetUserId(User);
        //    var user = context.Users.Find(userId);
        //} 
    }
}
