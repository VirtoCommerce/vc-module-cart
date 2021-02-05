using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CartModule.Data.Repositories
{
    public class RedisCartRepository : ICartRepository
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabaseAsync _database;
        private readonly IServer _server;

        public RedisCartRepository(IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("RedisConnectionString");
            _redis = ConnectionMultiplexer.Connect(connection);
            _database = _redis.GetDatabase();
            UnitOfWork = new RedisUnitOfWork();
            _server = _redis.GetServer(_redis.GetEndPoints().First());
        }

        public IQueryable<ShoppingCartEntity> ShoppingCarts
        {
            get
            {
                var keys = _server.Keys(pattern: $"*{typeof(ShoppingCartEntity).FullName}*").ToArray();
                var values = _database.StringGetAsync(keys).GetAwaiter().GetResult();
                var carts = values.Select(x => JsonConvert.DeserializeObject<ShoppingCartEntity>(x)).ToArray();
                return carts.AsQueryable();
            }
        }

        public IUnitOfWork UnitOfWork { get; private set; }

        public void Add<T>(T item) where T : class
        {
            if (item is IEntity entity)
            {
                GenerateId(entity);
                var key = CacheKey.With(typeof(T).FullName, entity.Id);
                _database.StringSetAsync(key, JsonConvert.SerializeObject(item))
                    .GetAwaiter()
                    .GetResult();
            }
            
        }

        public void Attach<T>(T item) where T : class
        {
            if (item is IEntity entity && entity.Id != null)
            {
                var key = CacheKey.With(typeof(T).FullName, entity.Id);
                _database.StringSetAsync(key, JsonConvert.SerializeObject(item))
                    .GetAwaiter()
                    .GetResult();
            }
            else
            {
                Add(item);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<ShoppingCartEntity[]> GetShoppingCartsByIdsAsync(string[] ids, string responseGroup = null)
        {
            var keys = ids.Select(x => new RedisKey(CacheKey.With(typeof(ShoppingCartEntity).FullName, x))).ToArray();
            var values = await _database.StringGetAsync(keys);
            var carts = values.Where(x => x.HasValue)
                .Select(x => JsonConvert.DeserializeObject<ShoppingCartEntity>(x))
                .ToArray();
            return carts;
        }

        public void Remove<T>(T item) where T : class
        {
            if (item is IEntity entity && entity.Id != null)
            {
                var key = CacheKey.With(typeof(T).FullName, entity.Id);
                _database.KeyDeleteAsync(key).GetAwaiter().GetResult();
            }
                
        }

        public Task RemoveCartsAsync(string[] ids)
        {
            var keys = ids.Select(x => new RedisKey(CacheKey.With(typeof(ShoppingCartEntity).FullName, x))).ToArray();
            return _database.KeyDeleteAsync(keys);
        }

        public Task SoftRemoveCartsAsync(string[] ids)
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T item) where T : class
        {
            Attach<T>(item);
        }

        private void GenerateId(IEntity entity)
        {
            entity.Id = Guid.NewGuid().ToString();
        }

        

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //DbContext.Dispose();
                //DbContext = null;
                _redis?.Close();
                _redis?.Dispose();
                UnitOfWork = null;
            }
        }
    }

    class RedisUnitOfWork : IUnitOfWork
    {
        public int Commit()
        {
            return 1;
        }

        public Task<int> CommitAsync()
        {
            return Task.FromResult(Commit());
        }
    }
}
