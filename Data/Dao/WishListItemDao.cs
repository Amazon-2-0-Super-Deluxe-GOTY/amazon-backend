using amazon_backend.Data.Entity;

namespace amazon_backend.Data.Dao
{
    public interface IWishListItemDAO : IDataAccessObject<WishListItem, string>
    {

    }

    public class WishListItemDao : IWishListItemDAO
    {
        private readonly DataContext _context;
        public WishListItemDao(DataContext context)
        {
            _context = context;
        }

        public WishListItem[] GetAll()
        {
            return _context.WishListItems.ToArray();
        }

        public WishListItem? GetById(string id)
        {
            return _context.WishListItems.Find(id);
        }

        public void Add(WishListItem wishListItem)
        {
            _context.WishListItems.Add(wishListItem);
            _context.SaveChanges();
        }

        public void Update(WishListItem wishListItem)
        {
            _context.Update(wishListItem);
            _context.SaveChanges();
        }

        public void Restore(string id)
        {
            var wishListItem = _context.WishListItems.Find(id);
            if (wishListItem != null)
            {
                // wishListItem.IsDeleted = false;

                _context.SaveChanges();
            }
        }

        public void Delete(string id)
        {
            var wishListItem = _context.WishListItems.Find(id);
            if (wishListItem != null)
            {
                // wishListItem.IsDeleted = true;

                _context.WishListItems.Remove(wishListItem);
                _context.SaveChanges();
            }
        }
    }
}
