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
    ///  2. The customer/organization predicate must be two seekable branches, not an OR
    ///     spanning two different columns. The OR form is non-SARGable and forces a scan.
    ///  3. A call with neither customerId nor organizationId must not query at all. The old
    ///     code fell through and returned every wishlist in the database.
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

        #region customer/organization predicate must be seekable branches

        [Fact]
        public void SqlServer_WithCustomerAndOrganization_UsesUnionAllInsteadOfOr()
        {
            var (text, _) = new TestableSqlServer().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.Contains("UNION ALL", Normalize(text));
            Assert.DoesNotContain(" OR ", Normalize(text));
        }

        [Fact]
        public void MySql_WithCustomerAndOrganization_UsesUnionAllInsteadOfOr()
        {
            var (text, _) = new TestableMySql().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.Contains("UNION ALL", Normalize(text));
            Assert.DoesNotContain(" OR ", Normalize(text));
        }

        [Fact]
        public void PostgreSql_WithCustomerAndOrganization_UsesUnionAllInsteadOfOr()
        {
            var (text, _) = new TestablePostgreSql().Build(CustomerId, OrganizationId, StoreId, ProductIds);

            Assert.Contains("UNION ALL", Normalize(text));
            Assert.DoesNotContain(" OR ", Normalize(text));
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
