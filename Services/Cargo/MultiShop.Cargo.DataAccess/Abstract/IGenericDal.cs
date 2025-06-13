namespace MultiShop.Cargo.DataAccess.Abstract;

public interface IGenericDal<T>where T : class, new()
{
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
    T GetById(int id);
    List<T> GetAll();
    List<T> GetByFilter(Func<T, bool> filter);
}
