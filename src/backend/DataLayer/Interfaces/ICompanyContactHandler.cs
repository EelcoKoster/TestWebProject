using Models;
using Models.ViewModels;

namespace DataLayer.Interfaces
{
    public interface ICompanyContactHandler
    {
        Task Add(CompanyContact company);
        Task Delete(string id);
        Task<CompanyContactView[]> Get(string orderByPropertyName = "Name", bool ascending = true);
        Task<CompanyContact> GetById(string id);
        Task<CompanyContact[]> GetByCompanyId(string companyId);
        Task Update(CompanyContact company);
    }
}