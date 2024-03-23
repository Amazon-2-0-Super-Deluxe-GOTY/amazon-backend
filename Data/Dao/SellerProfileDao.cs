using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using amazon_backend.Data.Entity;
using static amazon_backend.Data.Dao.SellerProfileDao;

namespace amazon_backend.Data.Dao
{
        public interface ISellerProfileDao : IDataAccessObject<SellerProfile, Guid>
        {
             SellerProfile? GetByUserId(Guid userId);
        }

        public class SellerProfileDao : ISellerProfileDao
        {
            private readonly DataContext dataContext;

            public SellerProfileDao(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public void Add(SellerProfile item)
            {
                dataContext.SellerProfiles.Add(item);
                dataContext.SaveChanges();
            }

            public void Delete(Guid id)
            {
                SellerProfile? profile = dataContext.SellerProfiles.Find(id);

                if (profile is not null)
                {
                    dataContext.SellerProfiles.Remove(profile);
                    dataContext.SaveChanges();
                }
            }

            public SellerProfile[] GetAll()
            {
                return dataContext.SellerProfiles.ToArray();
            }

            public SellerProfile? GetById(Guid id)
            {
                return dataContext.SellerProfiles.Find(id);
            }

            public SellerProfile? GetByUserId(Guid userId)
            {
                return dataContext.SellerProfiles.FirstOrDefault(p => p.UserId == userId);
            }

             public void Update(SellerProfile item)
             {
                 dataContext.SellerProfiles.Update(item);
                 dataContext.SaveChanges();
             }
        }

 }

