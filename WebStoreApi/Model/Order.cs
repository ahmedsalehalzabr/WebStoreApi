using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebStoreApi.Model
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreateAt { get; set; }
        [Precision(16,2)]
        public decimal ShippingFee { get; set; }
        [MaxLength(100)]
        public string DeliveryAddress { get; set; } = "";
        [MaxLength(30)]
        public string PaymentMethods { get; set; } = "";
        [MaxLength(30)]
        public string PaymentStatuses { get; set; } = "";
        [MaxLength(30)]
        public string OrderStatuses { get; set; } = "";

        public User User { get; set; } = null!;
        public List<OrderItem> Items { get; set; } = new();
    }
}
