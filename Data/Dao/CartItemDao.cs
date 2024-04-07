using amazon_backend.Data.Entity;

namespace amazon_backend.Data.Dao
{
    public class CartItemDao
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }

}
