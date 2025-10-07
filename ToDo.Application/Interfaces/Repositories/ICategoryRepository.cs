using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Domain.Entities;

namespace ToDo.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category, Guid>
    {
    }
}
