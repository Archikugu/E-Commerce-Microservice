namespace MultiShop.Cargo.Business.Abstract;

public interface IGenericService<T> where T : class
{
    void TAdd(T entity);
    void TUpdate(T entity);
    void TDelete(int id);
    T TGetById(int id);
    List<T> TGetAll();
    List<T> TGetByFilter(Func<T, bool> filter);
}
