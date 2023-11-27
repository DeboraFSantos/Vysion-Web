using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public interface ICategoriesRepository
    {
        Task<Category> GetCategory(Guid id);
        IEnumerable<Category> GetCategories();

        void CreateCategory(Category category);
        void UpdateCategory (Category category);
        void DeleteCategory (Guid id);
    }
}