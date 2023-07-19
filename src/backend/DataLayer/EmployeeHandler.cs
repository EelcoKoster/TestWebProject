using DataLayer.Interfaces;
using Models;

namespace DataLayer
{
    public sealed class EmployeeHandler : BaseHandler<Employee>, IEmployeeHandler
    {
        public EmployeeHandler(DataContext dataContext) : base(dataContext) { }
    }
}
