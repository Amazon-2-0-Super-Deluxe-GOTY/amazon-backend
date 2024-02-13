namespace amazon_backend.Data.Dao
{
    public interface IDataAccessObject<T, Tid>
    {
        T[] GetAll();
        T GetById(Tid id);
        void Add(T item);
        void Update(T item);
        void Restore(Tid id);
        void Delete(Tid id);
    }
}
