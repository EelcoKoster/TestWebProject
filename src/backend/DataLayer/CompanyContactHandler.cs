using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using System.Linq.Dynamic.Core;

namespace DataLayer
{
    public sealed class CompanyContactHandler : BaseHandler<CompanyContact>, ICompanyContactHandler
    {
        public CompanyContactHandler(DataContext dataContext) : base(dataContext) { }

        public async Task<CompanyContact[]> GetByCompanyId(string companyId)
        {
            return await entities.Where(_ => _.CompanyId == companyId).ToArrayAsync();
        }

        public async Task<CompanyContactView[]> Get(string orderByPropertyName, bool ascending = true)
        {
            string orderByQuery = GetOrderStatement(orderByPropertyName, ascending);

            var query = from cc in db.CompanyContacts
                        join company in db.Companies
                        on cc.CompanyId equals company.Id into grouping
                        from company in grouping.DefaultIfEmpty()
                        select new CompanyContactView { 
                            Id = cc.Id, 
                            Name = cc.Name,
                            Function = cc.Function, 
                            Phone = cc.Phone, 
                            EmailAddress = cc.EmailAddress, 
                            CompanyId = cc.CompanyId, 
                            CompanyName = company.Name 
                        };

            return await query.OrderBy(orderByQuery).ToArrayAsync();
        }
    }
}
