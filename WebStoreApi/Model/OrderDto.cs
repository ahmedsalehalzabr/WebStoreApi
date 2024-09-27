using System.ComponentModel.DataAnnotations;

namespace WebStoreApi.Model
{
    public class OrderDto
    {
        [Required]
        public string PoductIdentifiers { get; set; } = "";
        [Required, MinLength(30), MaxLength(100)]
        public string DeliveryAddress { get; set; } = "";
        [Required]
        public string PaymentMethods { get; set; } = "";
    }
}
