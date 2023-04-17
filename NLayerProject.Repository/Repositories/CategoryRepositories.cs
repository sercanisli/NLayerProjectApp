using Microsoft.EntityFrameworkCore;
using NLayerProject.Core.Models;
using NLayerProject.Core.Repositories;
using NLayerProject.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayerProject.Repository.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoriyRepository
    {
        public CategoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<Category> GetSingleCategoryByIdWithProductAsync(int categoryId)
        {
            return await _context.Categories.Include(x=>x.Products).Where(x=>x.Id==categoryId).SingleOrDefaultAsync();
        }
    }
}
