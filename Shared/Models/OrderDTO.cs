namespace Shared.Models
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public OrderStatus Status { get; set; }
    }
    public enum OrderStatus
    {
        Initial = 0,
        Delivered = 1
    }
}
