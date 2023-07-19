using DataLayer.Interfaces;
using Models;

namespace DataLayer
{
    public sealed class CompanyHandler : BaseHandler<Company>, ICompanyHandler
    {
        public CompanyHandler(DataContext dataContext) : base(dataContext) { }
    }
}
