namespace WebStoreApi.Model
{
    public class CartDto
    {
        public List<CartItemDto> CartItem { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
