using AMB.Domain.Entities;

public interface IMenuRepository
{
    Task<MenuItem> AddAsync(MenuItem item);
    IQueryable<MenuItem> GetQuery();
}