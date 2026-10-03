using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Services;

namespace VirtoCommerce.CartModule.Data.BackgroundJobs;

/// <summary>
/// This background job hardly removes previously softly removed carts
/// </summary>
public class DeleteObsoleteCartsJob
{
    private readonly IDeleteObsoleteCartsHandler _deleteHandler;

    public DeleteObsoleteCartsJob(IDeleteObsoleteCartsHandler deleteHandler)
    {
        _deleteHandler = deleteHandler;
    }

    public async Task Process()
    {
        await _deleteHandler.DeleteObsoleteCarts();
    }
}
