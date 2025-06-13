using MultiShop.Cargo.DataAccess.Abstract;
using MultiShop.Cargo.DataAccess.Concrete.EntityFramework.Contexts;

namespace MultiShop.Cargo.DataAccess.Concrete.EntityFramework.Repositories;

public class GenericRepository<T> : IGenericDal<T> where T : class, new()
{
    private readonly MultiShopCargoContext _context;

    public GenericRepository(MultiShopCargoContext context)
    {
        _context = context;
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var values = _context.Set<T>().Find(id);
        _context.Set<T>().Remove(values);
        _context.SaveChanges();
    }

    public List<T> GetAll()
    {
        var values = _context.Set<T>().ToList();
        return values;
    }

    public List<T> GetByFilter(Func<T, bool> filter)
    {
        var values = _context.Set<T>().Where(filter).ToList();
        return values;
    }

    public T GetById(int id)
    {
        var values = _context.Set<T>().Find(id);
        return values;
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
        _context.SaveChanges();
    }
}
