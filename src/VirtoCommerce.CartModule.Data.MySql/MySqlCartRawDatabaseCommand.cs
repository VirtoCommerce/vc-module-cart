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
            var command = BuildFindWishlistsCommand(customerId, organizationId, storeId, productIds);
            if (command == null)
            {
                return Array.Empty<ProductWishlistEntity>();
            }

            return await dbContext.Set<ProductWishlistEntity>().FromSqlRaw(command.Text, command.Parameters.ToArray()).ToListAsync();
        }

        /// <summary>
        /// Builds the wishlist lookup as one seekable branch per owner, combined with UNION ALL.
        /// Returns null when there is no owner to filter by, meaning the caller must not query.
        /// </summary>
        protected virtual Command? BuildFindWishlistsCommand(string customerId, string organizationId, string storeId, IList<string> productIds)
        {
            var hasCustomer = !string.IsNullOrEmpty(customerId);
            var hasOrganization = !string.IsNullOrEmpty(organizationId);

            // Without an owner there is nothing to scope the query to. Returning a command here
            // would match every wishlist in the database.
            if (!hasCustomer && !hasOrganization)
            {
                return null;
            }

            var command = new Command();

            var parameterNames = productIds.Select((x, i) => new { ProductId = x, ParameterName = $"@pId{i}" }).ToList();
            var parameterNamesTemplate = string.Join(",", parameterNames.Select(x => x.ParameterName));

            var storeFilter = string.Empty;

            if (!string.IsNullOrEmpty(storeId))
            {
                storeFilter = " AND c.StoreId = @storeId";
                command.Parameters.Add(new MySqlParameter("@storeId", storeId));
            }

            var ownerFilters = new List<string>();

            if (hasCustomer)
            {
                ownerFilters.Add("c.CustomerId = @customerId AND c.OrganizationId IS NULL");
                command.Parameters.Add(new MySqlParameter("@customerId", customerId));
            }

            if (hasOrganization)
            {
                ownerFilters.Add("c.OrganizationId = @organizationId");
                command.Parameters.Add(new MySqlParameter("@organizationId", organizationId));
            }

            // One statement -- see the note in SqlServerCartRawDatabaseCommand: splitting the OR
            // into UNION ALL branches doubles the work when an index on CartLineItem.ProductId
            // exists, and does not help when it does not.
            var ownerFilter = ownerFilters.Count > 1
                ? $"({string.Join(" OR ", ownerFilters)})"
                : ownerFilters[0];

            // INNER JOIN, not LEFT JOIN: the predicates on li.* already discard every
            // null-extended row, so an outer join only constrains the optimizer's join order.
            command.Text = $@"
                  SELECT c.Id, li.ProductId
                  FROM Cart c
                  INNER JOIN CartLineItem li
                  ON c.Id = li.ShoppingCartId
                  WHERE c.IsDeleted = '0' AND c.Type = 'Wishlist'
                  AND li.IsGift = '0'
                  AND li.ProductId IN ({parameterNamesTemplate}){storeFilter}
                  AND {ownerFilter}";

            foreach (var parameterName in parameterNames)
            {
                command.Parameters.Add(new MySqlParameter(parameterName.ParameterName, parameterName.ProductId));
            }

            return command;
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
