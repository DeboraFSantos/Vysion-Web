using System;
using System.Collections.Generic;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public interface IClientsRepository
    {
        Client GetClient(Guid id);
        IEnumerable<Client> GetClients();

        void CreateClient(Client client);
        void UpdateClient (Client client);
        void DeleteClient (Guid id);
    }
}