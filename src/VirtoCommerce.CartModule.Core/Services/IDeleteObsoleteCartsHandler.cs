using System.Threading.Tasks;

namespace VirtoCommerce.CartModule.Core.Services;

public interface IDeleteObsoleteCartsHandler
{
    Task DeleteObsoleteCarts();
}
