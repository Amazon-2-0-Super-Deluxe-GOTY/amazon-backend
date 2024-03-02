using amazon_backend.Data.Entity;

namespace amazon_backend.Data.Dao
{
    public interface IWishListDAO : IDataAccessObject<WishList, Guid>
    {
        WishList[] GetByName(string name);
        void Restore(Guid id);
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

        public WishList? GetById(Guid id)
        {
            return _context.WishLists.Find(id);
        }

        public WishList[] GetByName(string name)
        {
            return _context.WishLists.Where(c => c.Name.Contains(name.ToLower())).ToArray();
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

        public void Restore(Guid id)
        {
            var wishList = _context.WishLists.Find(id);
            if (wishList != null)
            {
                wishList.IsDeleted = false;
                _context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            var wishList = _context.WishLists.Find(id);
            if (wishList != null)
            {
                wishList.IsDeleted = true;
                _context.SaveChanges();
            }
        }
    }
}