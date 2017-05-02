using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Core
{
    internal class BaseDal<T>
    {
        private MongoClient _client;
        private IMongoDatabase _db;
        private IMongoCollection<T> _collection;

        public BaseDal()
        {
            SetServer(ConfigurationManager.AppSettings["ConnStr"]);
        }

        /// <summary>
        /// 设置默认的数据库连接
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <param name="tableName">表</param>
        public BaseDal(string dbName, string tableName)
        {
            SetServer(ConfigurationManager.AppSettings["ConnStr"]);
            SetTable(dbName, tableName);
        }

        /// <summary>
        /// 设置默认的数据库连接
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <param name="tableName">表</param>
        /// <param name="connStr">数据库连接字符串</param>
        public BaseDal(string dbName, string tableName, string connStr)
        {
            SetServer(connStr);
            SetTable(dbName, tableName);
        }

        /// <summary>
        /// 设置当前连接的数据库和表
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <param name="tableName">表</param>
        public void SetTable(string dbName, string tableName)
        {
            _db = _client.GetDatabase(dbName);
            _collection = _db.GetCollection<T>(tableName);
        }

        /// <summary>
        /// 设置连接
        /// </summary>
        /// <param name="connStr"></param>
        public void SetServer(string connStr)
        {
            _client = new MongoClient(connStr);
            var url = new MongoUrl(connStr);
            _db = _client.GetDatabase(url.DatabaseName);
        }

        public void Insert(T model)
        {
            _collection.InsertOneAsync(model);
        }

        public Task<DeleteResult> Delete(object id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return _collection.DeleteOneAsync(filter);
        }

        public Task<T> Find(object id, ProjectionDefinition<T, T> project)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            var ts = _collection.FindAsync(filter, new FindOptions<T> {Projection = project});
            return ts.Result.FirstOrDefaultAsync();
        }

        public Task<IAsyncCursor<T>> FindList(Expression<Func<T, bool>> filter, params string[] fields)
        {
            var x = _collection.Find<T>(filter);
            //x.
            //var xx  = x.Result.Current;
            return _collection.FindAsync<T>(filter);
        }

        public void Update()
        {

        }
    }
}
