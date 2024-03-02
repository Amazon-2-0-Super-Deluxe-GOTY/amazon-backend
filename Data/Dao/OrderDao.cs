using amazon_backend.Data.Entity;

namespace amazon_backend.Data.Dao
{
    public interface IOrderDAO : IDataAccessObject<Order, Guid>
    {
    }

    public class OrderDao : IOrderDAO
    {
        private readonly DataContext _context;
        public OrderDao(DataContext context)
        {
            _context = context;
        }
        public Order[] GetAll()
        {
            return _context.Orders.ToArray();
        }

        public Order GetById(Guid id)
        {
            return _context.Orders.Find(id);
        }

        public void Add(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void Update(Order order)
        {
            _context.Update(order);
            _context.SaveChanges();
        }

        public void Restore(Guid id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {

                _context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {

                _context.SaveChanges();
            }
        }
    }
}