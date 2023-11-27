using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public interface IClientsRepository
    {
        Task<Client> GetClient(Guid id);
        IEnumerable<Client> GetClients();

        void CreateClient(Client client);
        void UpdateClient (Client client);
        void DeleteClient (Guid id);
    }
}