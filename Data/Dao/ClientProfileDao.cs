using amazon_backend.Data.Entity;

namespace amazon_backend.Data.Dao
{
    public interface IClientProfileDao : IDataAccessObject<ClientProfile, Guid>
    {
        ClientProfile? GetByUserId(Guid userId);
    }

    public class ClientProfileDao : IClientProfileDao
    {
        private readonly DataContext dataContext;

        public ClientProfileDao(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Add(ClientProfile item)
        {
            dataContext.ClientProfiles.Add(item);
            dataContext.SaveChanges();
        }

        public void Delete(Guid id)
        {
            ClientProfile? profile = dataContext.ClientProfiles.Find(id);

            if(profile is not null)
            {
                dataContext.ClientProfiles.Remove(profile);
                dataContext.SaveChanges();
            }
        }

        public ClientProfile[] GetAll()
        {
            return dataContext.ClientProfiles.ToArray();
        }

        public ClientProfile? GetById(Guid id)
        {
            return dataContext.ClientProfiles.Find(id);
        }

        public ClientProfile? GetByUserId(Guid userId)
        {
            return dataContext.ClientProfiles.FirstOrDefault(p => p.UserId == userId);
        }

        public void Update(ClientProfile item)
        {
            dataContext.ClientProfiles.Update(item);
            dataContext.SaveChanges();
        }
    }
}
