using System.Text;
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

        public async Task<IList<ProductWishlistEntity>> FindWishlistsByProductsAsync(CartDbContext dbContext, string customerId, string organizationId, string storeId, IList<string> productIds)
        {
            var command = new Command();
            var commandTemlate = new StringBuilder();

            var parameterNames = productIds.Select((x, i) => new { ProductId = x, ParameterName = $"@pId{i}" });
            var parameterNamesTemplate = string.Join(",", parameterNames.Select(x => x.ParameterName));

            commandTemlate.Append($@"
                  SELECT c.Id, li.ProductId
                  FROM Cart c
                  LEFT JOIN CartLineItem li
                  ON c.Id = li.ShoppingCartId
                  WHERE c.IsDeleted = '0' AND c.Type = 'Wishlist'
                  AND li.ProductId IN ({parameterNamesTemplate})");

            if (!string.IsNullOrEmpty(organizationId) && !string.IsNullOrEmpty(customerId))
            {
                commandTemlate.Append(@"
                    AND (c.CustomerId = @customerId OR c.OrganizationId = @organizationId)
                ");

                command.Parameters.Add(new MySqlParameter("@customerId", customerId));
                command.Parameters.Add(new MySqlParameter("@organizationId", organizationId));
            }
            else if (!string.IsNullOrEmpty(customerId))
            {
                commandTemlate.Append(@"
                    AND c.CustomerId = @customerId
                ");

                command.Parameters.Add(new MySqlParameter("@customerId", customerId));
            }

            command.Text = commandTemlate.ToString();

            foreach (var parameterName in parameterNames)
            {
                command.Parameters.Add(new MySqlParameter(parameterName.ParameterName, parameterName.ProductId));
            }

            var result = await dbContext.Set<ProductWishlistEntity>().FromSqlRaw(command.Text, command.Parameters.ToArray()).ToListAsync();
            return result;
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
