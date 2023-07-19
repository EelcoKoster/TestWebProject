using Models;
using Models.ViewModels;

namespace DataLayer.Interfaces
{
    public interface IClientHandler
    {
        Task Add(Client client);
        Task Delete(string id);
        Task<ClientView[]> Get(string orderByPropertyName, bool? ascending);
        Task<Client?> GetById(string id);
        Task Update(Client client);
    }
}