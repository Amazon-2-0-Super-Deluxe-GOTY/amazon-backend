using amazon_backend.Data.Entity;

namespace amazon_backend.Data.Dao
{
    public interface IWishListDAO : IDataAccessObject<WishList, string>
    {

    }

    public class WishListDao : IWishListDAO
    {
        private readonly DataContext _context;
        public WishListDao(DataContext dataContext)
        {
            _context = dataContext;
        }

        public WishList[] GetAll()
        {
            return _context.WishLists.ToArray();
        }

        public WishList? GetById(string id)
        {
            return _context.WishLists.Find(id);
        }

        public void Add(WishList wishList)
        {
            _context.WishLists.Add(wishList);
            _context.SaveChanges();
        }

        public void Update(WishList wishList)
        {
            _context.Update(wishList);
            _context.SaveChanges();
        }

        public void Restore(string id)
        {
            var wishList = _context.WishLists.Find(id);
            if (wishList != null)
            {
                wishList.DeletedAt = null;
                _context.SaveChanges();
            }
        }

        public void Delete(string id)
        {
            var wishList = _context.WishLists.Find(id);
            if (wishList != null)
            {
                wishList.DeletedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }
    }
}
