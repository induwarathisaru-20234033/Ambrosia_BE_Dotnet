using AMB.Domain.Entities;

public interface IMenuRepository
{
    Task<MenuItem> AddAsync(MenuItem item);
    IQueryable<MenuItem> GetQuery();
    Task<MenuItem?> GetByIdAsync(int id);
    Task<MenuItem> UpdateAsync(MenuItem item);
}