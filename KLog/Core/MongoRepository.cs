using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Core
{
    internal class MongoRepository<T>
    {
        private MongoServer _server;
        private MongoCollection<BsonDocument> _collection;

        public MongoRepository()
        {
            SetServer(ConfigurationManager.AppSettings["ConnStr"]);
        }

        /// <summary>
        /// 设置默认的数据库连接
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <param name="tableName">表</param>
        public MongoRepository(string dbName, string tableName)
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
        public MongoRepository(string dbName, string tableName, string connStr)
        {
            SetServer(connStr);
            SetTable(dbName, tableName);
        }

        /// <summary>
        /// 设置默认的数据库连接
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <param name="tableName">表</param>
        /// /// <param name="host">数据库IP</param>
        /// <param name="port">数据库端口</param>
        /// <param name="connectTimeout">连接超时时间</param>
        /// <param name="socketTimeout">socket超时时间</param>
        public MongoRepository(string dbName, string tableName, string host, int port, TimeSpan connectTimeout, TimeSpan socketTimeout)
        {
            SetServer(host, port, connectTimeout, socketTimeout);
            SetTable(dbName, tableName);
        }

        /// <summary>
        /// 设置当前连接的数据库和表
        /// </summary>
        /// <param name="dbName">数据库</param>
        /// <param name="tableName">表</param>
        public void SetTable(string dbName, string tableName)
        {
            _collection = _server.GetDatabase(dbName).GetCollection(tableName);
        }

        /// <summary>
        /// 设置连接
        /// </summary>
        /// <param name="connStr"></param>
        public void SetServer(string connStr)
        {
            _server = new MongoClient(connStr).GetServer();
        }

        /// <summary>
        /// 设置连接
        /// </summary>
        /// <param name="host">数据库IP</param>
        /// <param name="port">数据库端口</param>
        /// <param name="connectTimeout">连接超时时间</param>
        /// <param name="socketTimeout">socket超时时间</param>
        public void SetServer(string host, int port, TimeSpan connectTimeout, TimeSpan socketTimeout)
        {
            _server = new MongoClient(new MongoClientSettings
            {
                ConnectTimeout = connectTimeout,
                SocketTimeout = socketTimeout,
                Server = new MongoServerAddress(host, port)
            }).GetServer();
        }

        /// <summary>
        /// 设置数据库连接字符串
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        public void SetConnStr(string connStr)
        {
            _server = new MongoClient(connStr).GetServer();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public WriteConcernResult Remove(IMongoQuery query)
        {
            return _collection.Remove(query, RemoveFlags.None);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public List<T> Find(IMongoQuery query, params string[] fields)
        {
            var doc = _collection.FindAs<T>(query);
            if (fields != null)
                doc.SetFields(fields);
            return doc.ToList();
        }

        /// <summary>
        /// 获取单条数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public T FindOne(IMongoQuery query)
        {
            var doc = _collection.FindOneAs<T>(query);
            return doc;
        }

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindById(int id)
        {
            return FindOne(Query.EQ("_id", id));
        }

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindById(string id)
        {
            return FindOne(Query.EQ("_id", id));
        }


        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sortBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public List<T> Find(IMongoQuery query, IMongoSortBy sortBy, int pageIndex,
            int pageSize, params string[] fields)
        {
            var doc = _collection.FindAs<T>(query);
            if (fields != null)
            {
                doc.SetFields(fields);
            }
            if (sortBy != null)
            {
                doc.SetSortOrder(sortBy);
            }
            doc.Skip = pageIndex * pageSize - pageSize;
            doc.Limit = pageSize;
            return doc.ToList();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="update"></param>
        /// <param name="updateFlags"></param>
        /// <returns></returns>
        public WriteConcernResult Update(IMongoQuery query, IMongoUpdate update,
            UpdateFlags updateFlags = UpdateFlags.Upsert)
        {
            return _collection.Update(query, update, updateFlags, WriteConcern.Acknowledged);
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public WriteConcernResult Insert(BsonDocument doc)
        {
            //return _collection.Save(doc, WriteConcern.Acknowledged);
            return _collection.Insert(doc, WriteConcern.Acknowledged);
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public WriteConcernResult Insert(T doc)
        {
            return _collection.Insert(doc, WriteConcern.Acknowledged);
        }

        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public IEnumerable<WriteConcernResult> InsertBatch(IEnumerable<BsonDocument> doc)
        {
            return _collection.InsertBatch(doc, WriteConcern.Acknowledged);
        }

        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public IEnumerable<WriteConcernResult> InsertBatch(IEnumerable<T> doc)
        {
            return _collection.InsertBatch(doc, WriteConcern.Acknowledged);
        }
    }
}
