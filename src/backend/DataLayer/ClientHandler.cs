using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using System.Linq.Dynamic.Core;

namespace DataLayer
{
    public sealed class ClientHandler : BaseHandler<Client>, IClientHandler
    {
        public ClientHandler(DataContext dataContext) : base(dataContext) { }

        public async new Task<ClientView[]> Get(string orderByPropertyName, bool? ascending)
        {
            string orderByQuery = GetOrderStatement(orderByPropertyName, ascending);

            var query = from c in db.Clients
                        join company in db.Companies
                        on c.CompanyNumber equals company.Id into grouping
                        from company in grouping.DefaultIfEmpty()
                        select new ClientView
                        {
                            Id = c.Id,
                            Firstname = c.Firstname, 
                            Lastname = c.Lastname,
                            Status = c.Status,
                            TrajectType = c.TrajectType,
                            CompanyNumber = c.CompanyNumber,
                            DurationMonths = c.DurationMonths,
                            IssuedBy = c.IssuedBy,
                            StartDate= c.StartDate,
                            EndDate = c.EndDate,
                            CompanyName = company.Name
                        };

            return await query.OrderBy(orderByQuery).ToArrayAsync();
        }
    }
}
