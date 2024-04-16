using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;

namespace VirtoCommerce.CartModule.Data.MySql
{
    public class MySqlCartRawDatabaseCommand : ICartRawDatabaseCommand
    {
        public Task SoftRemove(CartDbContext dbContext, IList<string> ids)
        {
            return ExecuteStoreQueryAsync(dbContext, "UPDATE Cart SET IsDeleted = '1' WHERE Id IN ({0})", ids);
        }

        public Task<IList<ProductWishlistEntity>> FindWishlistsByProductsAsync(CartDbContext dbContext, string customerId, string organizationId, string storeId, IList<string> productIds)
        {
            return new Task<IList<ProductWishlistEntity>>(() => new List<ProductWishlistEntity>());
        }

        protected virtual async Task<int> ExecuteStoreQueryAsync(CartDbContext dbContext, string commandTemplate, IEnumerable<string> parameterValues)
        {
            var command = CreateCommand(commandTemplate, parameterValues);
            return await dbContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
        }

        protected virtual Command CreateCommand(string commandTemplate, IEnumerable<string> parameterValues)
        {
            var parameters = parameterValues.Select((v, i) => new MySqlParameter($"@p{i}", v)).ToArray();
            var parameterNames = string.Join(",", parameters.Select(p => p.ParameterName));

            return new Command
            {
                Text = string.Format(commandTemplate, parameterNames),
                Parameters = parameters.OfType<object>().ToList(),
            };
        }

        protected class Command
        {
            public string Text { get; set; } = string.Empty;
            public IList<object> Parameters { get; set; } = new List<object>();
        }
    }
}
