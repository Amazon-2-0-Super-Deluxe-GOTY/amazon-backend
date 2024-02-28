using amazon_backend.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace amazon_backend.Data.Dao
{
    public interface IUserDao : IDataAccessObject<User, Guid>
    {
        Task<User[]> GetByRoleAsync(string role);
        Task<User[]> GetAllAsync();
        void Restore(Guid id);
    }

    public class UserDao : IUserDao
    {
        private readonly DataContext dataContext;

        public UserDao(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Add(User item)
        {
            dataContext.Users.Add(item);
            dataContext.SaveChanges();
        }

        public void Delete(Guid id)
        {
            User? user = dataContext.Users.Find(id);

            if(user is not null)
            {
                user.DeletedAt = DateTime.Now;
                dataContext.SaveChanges();
            }
        }

        public User[] GetAll()
        {
            return dataContext.Users.Include(u => u.ClientProfile).ToArray();
        }

        public Task<User[]> GetAllAsync()
        {
            return dataContext.Users.ToArrayAsync();
        }

        public User? GetById(Guid id)
        {
            return dataContext.Users.Include(u => u.ClientProfile).SingleOrDefault(u => u.Id == id);
        }

        public Task<User[]> GetByRoleAsync(string role)
        {
            return dataContext.Users.Where(u => u.Role == role.ToLower()).ToArrayAsync();
        }

        public void Restore(Guid id)
        {
            User? user = dataContext.Users.Find(id);

            if (user is not null)
            {
                user.DeletedAt = null;
                dataContext.SaveChanges();
            }
        }

        public void Update(User item)
        {
            dataContext.Users.Update(item);
            dataContext.SaveChanges();
        }
    }
}
