using Models;

namespace DataLayer.Interfaces
{
    public interface IEmployeeHandler
    {
        Task Add(Employee company);
        Task Delete(string id);
        IEnumerable<Employee> Get(string orderByPropertyName, bool? ascending);
        Task<Employee?> GetById(string id);
        Task Update(Employee company);
    }
}