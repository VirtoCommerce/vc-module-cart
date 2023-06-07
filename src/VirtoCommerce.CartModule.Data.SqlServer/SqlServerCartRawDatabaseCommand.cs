using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CartModule.Data.Repositories;

namespace VirtoCommerce.CartModule.Data.SqlServer
{
    public class SqlServerCartRawDatabaseCommand : ICartRawDatabaseCommand
    {
        public Task SoftRemove(CartDbContext dbContext, IList<string> ids)
        {
            return ExecuteStoreQueryAsync(dbContext, "UPDATE \"Cart\" SET \"IsDeleted\"='1' WHERE \"Id\" IN ({0})", ids);
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
