using System.Text;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;

namespace VirtoCommerce.CartModule.Data.SqlServer
{
    public class SqlServerCartRawDatabaseCommand : ICartRawDatabaseCommand
    {
        public Task SoftRemove(CartDbContext dbContext, IList<string> ids)
        {
            return ExecuteStoreQueryAsync(dbContext, "UPDATE \"Cart\" SET \"IsDeleted\"='1' WHERE \"Id\" IN ({0})", ids);
        }

        public async Task<IList<ProductWishlistEntity>> FindWishlistsByProductsAsync(CartDbContext dbContext, string customerId, string organizationId, string storeId, IList<string> productIds)
        {
            var commandTemlate = new StringBuilder();

            commandTemlate.Append(@"
                  SELECT c.ID, li.ProductId
                  FROM Cart AS C
                  LEFT JOIN CartLineItem AS LI
                  ON C.id = LI.ShoppingCartId
                  WHERE C.IsDeleted = 0 AND C.Type = 'Wishlist'
                  AND LI.ProductId IN (@productIds)");

            if (!string.IsNullOrEmpty(organizationId) && !string.IsNullOrEmpty(customerId))
            {
                commandTemlate.Append(@"
                    AND (C.CustomerId = @customerId OR C.OrganizationId = @organizationId)
                ");
            }
            else if (!string.IsNullOrEmpty(customerId))
            {
                commandTemlate.Append(@"
                    AND C.CustomerId = @customerId
                ");
            }

            var commandText = commandTemlate.ToString();
            var command = new Command
            {
                Text = commandText,
                Parameters = new List<object>
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@customerId", customerId),
                    new Microsoft.Data.SqlClient.SqlParameter("@organizationId", organizationId),
                }
            };

            AddArrayParameters(command, "@productIds", productIds);

            var result = await dbContext.Set<ProductWishlistEntity>().FromSqlRaw(command.Text, command.Parameters.ToArray()).ToListAsync();
            return result;
        }

        protected virtual Task<int> ExecuteStoreQueryAsync(CartDbContext dbContext, string commandTemplate, IEnumerable<string> parameterValues)
        {
            var command = CreateCommand(commandTemplate, parameterValues);
            return dbContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
        }

        protected virtual Command CreateCommand(string commandTemplate, IEnumerable<string> parameterValues)
        {
            var parameters = parameterValues.Select((v, i) => new Microsoft.Data.SqlClient.SqlParameter($"@p{i}", v)).ToArray();
            var parameterNames = string.Join(",", parameters.Select(p => p.ParameterName));

            return new Command
            {
                Text = string.Format(commandTemplate, parameterNames),
                Parameters = parameters.OfType<object>().ToList(),
            };
        }

        protected static Microsoft.Data.SqlClient.SqlParameter[] AddArrayParameters<T>(Command cmd, string paramNameRoot, IEnumerable<T> values)
        {
            /* An array cannot be simply added as a parameter to a SqlCommand so we need to loop through things and add it manually.
             * Each item in the array will end up being it's own Microsoft.Data.SqlClient.SqlParameter so the return value for this must be used as part of the
             * IN statement in the CommandText.
             */
            var parameters = new List<Microsoft.Data.SqlClient.SqlParameter>();
            var parameterNames = new List<string>();
            var paramNbr = 1;
            foreach (var value in values)
            {
                var paramName = $"{paramNameRoot}{paramNbr++}";
                parameterNames.Add(paramName);
                var p = new Microsoft.Data.SqlClient.SqlParameter(paramName, value);
                cmd.Parameters.Add(p);
                parameters.Add(p);
            }
            cmd.Text = cmd.Text.Replace(paramNameRoot, string.Join(",", parameterNames));

            return parameters.ToArray();
        }

        protected class Command
        {
            public string Text { get; set; } = string.Empty;
            public IList<object> Parameters { get; set; } = new List<object>();
        }
    }
}
