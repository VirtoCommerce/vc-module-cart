namespace VirtoCommerce.CartModule.Data.BackgroundJobs;

/// <summary>
/// Payload of the recurring job that sends abandoned-cart reminders. Carries no data: each occurrence scans every
/// store that has reminders enabled. Extend it via <c>AbstractTypeFactory</c> to pass parameters to an overridden handler.
/// </summary>
public class AbandonedCartReminderJobPayload
{
}
