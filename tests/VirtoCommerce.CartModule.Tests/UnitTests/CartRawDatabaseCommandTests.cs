using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VirtoCommerce.CartModule.Data.MySql;
using VirtoCommerce.CartModule.Data.PostgreSql;
using VirtoCommerce.CartModule.Data.SqlServer;
using Xunit;

namespace VirtoCommerce.CartModule.Tests.UnitTests
{
    /// <summary>
    /// Covers the SQL generated for IWishlistService.FindWishlistsByProductsAsync.
    ///
    /// Three behaviours matter here and none of them are visible from the outside without
    /// a database, so these tests inspect the generated command text and parameters:
    ///
    ///  1. storeId must be applied. The parameter was accepted and silently ignored, so the
    ///     query returned wishlists from every store the customer had one in.
    ///  2. A call with organizationId but no customerId must filter by organization. The old
    ///     if/else-if chain had no branch for it and emitted a query with no owner predicate.
    ///  3. A call with neither customerId nor organizationId must not query at all. The old
    ///     code fell through and returned every wishlist in the database.
    ///  4. The owner predicate stays ONE statement. An earlier revision split it into two
    ///     UNION ALL branches to avoid the non-SARGable OR; measurement showed that doubles the
    ///     work (19 -> 38 logical reads) whenever an index on CartLineItem.ProductId lets the
    ///     optimizer drive from the line-item side, and it does not help without one, because
    ///     Cart.OrganizationId has no index either way.
    /// </summary>
    public class CartRawDatabaseCommandTests
    {
        private const string CustomerId = "customer-1";
        private const string OrganizationId = "org-1";
        private const string StoreId = "store-1";

        private static readonly IList<string> ProductIds = new List<string> { "product-1", "product-2" };

        private static string Normalize(string sql) => Regex.Replace(sql ?? string.Empty, @"\s+", " ").Trim();

        #region storeId must be applied

        [Fact]
        public void SqlServer_AppliesStoreIdFilter()
        {
            var (text, parameters) = new TestableSqlServer().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.Contains("StoreId", text);
            Assert.Contains("@storeId", parameters);
        }

        [Fact]
        public void MySql_AppliesStoreIdFilter()
        {
            var (text, parameters) = new TestableMySql().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.Contains("StoreId", text);
            Assert.Contains("@storeId", parameters);
        }

        [Fact]
        public void PostgreSql_AppliesStoreIdFilter()
        {
            var (text, parameters) = new TestablePostgreSql().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.Contains("StoreId", text);
            Assert.Contains("@storeId", parameters);
        }

        #endregion

        #region owner predicate stays a single statement

        // Measured on a database with an index on CartLineItem.ProductId: the optimizer drives
        // from the line-item side (4 matching rows), then PK-seeks Cart -- Cart scan count 0.
        // Rewriting the OR as two UNION ALL branches made CartLineItem scan count go 1 -> 2 and
        // doubled logical reads, 19 -> 38, for identical results. The OR is not the problem when
        // an index lets the optimizer avoid making Cart the driving table.

        [Fact]
        public void SqlServer_WithCustomerAndOrganization_EmitsOneStatementWithOrPredicate()
        {
            var (text, parameters) = new TestableSqlServer().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.DoesNotContain("UNION ALL", Normalize(text));
            Assert.Contains(" OR ", Normalize(text));
            Assert.Contains("@customerId", parameters);
            Assert.Contains("@organizationId", parameters);
        }

        [Fact]
        public void MySql_WithCustomerAndOrganization_EmitsOneStatementWithOrPredicate()
        {
            var (text, parameters) = new TestableMySql().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.DoesNotContain("UNION ALL", Normalize(text));
            Assert.Contains(" OR ", Normalize(text));
            Assert.Contains("@customerId", parameters);
            Assert.Contains("@organizationId", parameters);
        }

        [Fact]
        public void PostgreSql_WithCustomerAndOrganization_EmitsOneStatementWithOrPredicate()
        {
            var (text, parameters) = new TestablePostgreSql().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.DoesNotContain("UNION ALL", Normalize(text));
            Assert.Contains(" OR ", Normalize(text));
            Assert.Contains("@customerId", parameters);
            Assert.Contains("@organizationId", parameters);
        }

        [Fact]
        public void SqlServer_WithCustomerOnly_HasSingleBranch()
        {
            var (text, parameters) = new TestableSqlServer().Build(CustomerId, organizationId: null, StoreId, ProductIds);

            Assert.DoesNotContain("UNION ALL", Normalize(text));
            Assert.Contains("@customerId", parameters);
            Assert.DoesNotContain("@organizationId", parameters);
        }

        [Fact]
        public void SqlServer_WithOrganizationOnly_FiltersByOrganization()
        {
            var (text, parameters) = new TestableSqlServer().Build(customerId: null, OrganizationId, StoreId, ProductIds);

            Assert.DoesNotContain("UNION ALL", Normalize(text));
            Assert.Contains("OrganizationId", text);
            Assert.Contains("@organizationId", parameters);
            Assert.DoesNotContain("@customerId", parameters);
        }

        #endregion

        #region a call with no identity must not query

        [Fact]
        public void SqlServer_WithoutCustomerAndOrganization_BuildsNoCommand()
        {
            var (text, _) = new TestableSqlServer().Build(customerId: null, organizationId: null, StoreId, ProductIds);

            Assert.Null(text);
        }

        [Fact]
        public void MySql_WithoutCustomerAndOrganization_BuildsNoCommand()
        {
            var (text, _) = new TestableMySql().Build(customerId: null, organizationId: null, StoreId, ProductIds);

            Assert.Null(text);
        }

        [Fact]
        public void PostgreSql_WithoutCustomerAndOrganization_BuildsNoCommand()
        {
            var (text, _) = new TestablePostgreSql().Build(customerId: null, organizationId: null, StoreId, ProductIds);

            Assert.Null(text);
        }

        #endregion

        #region join must not be an outer join

        [Fact]
        public void SqlServer_UsesInnerJoin()
        {
            var (text, _) = new TestableSqlServer().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.Contains("INNER JOIN", Normalize(text));
            Assert.DoesNotContain("LEFT JOIN", Normalize(text));
        }

        [Fact]
        public void MySql_UsesInnerJoin()
        {
            var (text, _) = new TestableMySql().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.Contains("INNER JOIN", Normalize(text));
            Assert.DoesNotContain("LEFT JOIN", Normalize(text));
        }

        [Fact]
        public void PostgreSql_UsesInnerJoin()
        {
            var (text, _) = new TestablePostgreSql().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.Contains("INNER JOIN", Normalize(text));
            Assert.DoesNotContain("LEFT JOIN", Normalize(text));
        }

        #endregion

        #region product ids are still expanded into one parameter each, in every branch

        [Fact]
        public void SqlServer_ExpandsEachProductIdIntoItsOwnParameter()
        {
            var (text, parameters) = new TestableSqlServer().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.Contains("@productIds1", parameters);
            Assert.Contains("@productIds2", parameters);
            Assert.DoesNotContain("@productIds)", text);
        }

        #endregion

        #region testable subclasses -- reach the protected command builder

        private sealed class TestableSqlServer : SqlServerCartRawDatabaseCommand
        {
            public (string Text, string[] Parameters) Build(string customerId, string organizationId, string storeId, IList<string> productIds)
            {
                var command = BuildFindWishlistsCommand(customerId, organizationId, storeId, productIds);

                return command == null
                    ? (null, [])
                    : (command.Text, command.Parameters.Cast<Microsoft.Data.SqlClient.SqlParameter>().Select(x => x.ParameterName).ToArray());
            }
        }

        private sealed class TestableMySql : MySqlCartRawDatabaseCommand
        {
            public (string Text, string[] Parameters) Build(string customerId, string organizationId, string storeId, IList<string> productIds)
            {
                var command = BuildFindWishlistsCommand(customerId, organizationId, storeId, productIds);

                return command == null
                    ? (null, [])
                    : (command.Text, command.Parameters.Cast<MySqlConnector.MySqlParameter>().Select(x => x.ParameterName).ToArray());
            }
        }

        private sealed class TestablePostgreSql : PostgreSqlCartRawDatabaseCommand
        {
            public (string Text, string[] Parameters) Build(string customerId, string organizationId, string storeId, IList<string> productIds)
            {
                var command = BuildFindWishlistsCommand(customerId, organizationId, storeId, productIds);

                return command == null
                    ? (null, [])
                    : (command.Text, command.Parameters.Cast<Npgsql.NpgsqlParameter>().Select(x => x.ParameterName).ToArray());
            }
        }

        #endregion
    }
}
