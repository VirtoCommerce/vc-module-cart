using System.Threading.Tasks;

namespace VirtoCommerce.CartModule.Data.Repositories
{
    public interface ICartRawDatabaseCommand
    {
        Task SoftRemove(CartDbContext dbContext, string[] ids);
    }
}
