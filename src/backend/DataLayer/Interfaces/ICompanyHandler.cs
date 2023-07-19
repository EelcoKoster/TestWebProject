using Models;

namespace DataLayer.Interfaces
{
    public interface ICompanyHandler
    {
        Task Add(Company company);
        Task Delete(string id);
        IEnumerable<Company> Get(string orderByPropertyName, bool? ascending);
        Task<Company?> GetById(string id);
        Task Update(Company company);
    }
}