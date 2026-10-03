namespace VirtoCommerce.CartModule.Data.BackgroundJobs;

/// <summary>
/// Payload of the recurring job that hard-deletes soft-deleted carts. Carries no data: each occurrence processes
/// everything that is due. Extend it via <c>AbstractTypeFactory</c> to pass parameters to an overridden handler.
/// </summary>
public class DeleteObsoleteCartsJobPayload
{
}
