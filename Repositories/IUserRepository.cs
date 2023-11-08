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

        void CreateUser(User User);
        void UpdateUser (User User);
        void DeleteUser (Guid id);
    }
}