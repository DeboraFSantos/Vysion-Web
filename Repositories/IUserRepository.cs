using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public interface IUsersRepository
    {
        User GetUser(Guid id);
        IEnumerable<User> GetUsers();

        void CreateUser(User user);
        void UpdateUser (User user);
        void DeleteUser (Guid id);
    }
}