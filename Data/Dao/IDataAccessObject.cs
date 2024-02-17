namespace amazon_backend.Data.Dao
{
    public interface IDataAccessObject<T, Tid>
    {
        T[] GetAll();
        T? GetById(Tid id);
        void Add(T item);
        void Update(T item);
        void Delete(Tid id);
    }
}
