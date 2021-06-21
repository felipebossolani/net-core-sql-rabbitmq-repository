using Domain.Models;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IProductWriteRepository
    {
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}
