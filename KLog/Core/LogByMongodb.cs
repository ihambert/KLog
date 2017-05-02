using System.Collections.Generic;
using MongoDB.Bson;

namespace Core
{
    /// <summary>
    /// 用Mongodb记录日志
    /// </summary>
    internal class LogByMongodb : IWriteLog
    {
        /// <summary>
        /// 主Host，如jbk，ypk，console
        /// </summary>
        private string _host;
        /// <summary>
        /// 
        /// </summary>
        private MongoRepository<Log> _db;

        public void AddLogs(List<Log> logs)
        {
            var docs = new BsonDocument[logs.Count];
            for (var i = 0; i < logs.Count; i++)
            {
                var log = logs[i];
                docs[i] = new BsonDocument
                {
                    {"Host", _host},
                    {"Type", log.Type},
                    {"Time", log.Time},
                    {"Message", log.Message},
                    {"Url", log.Url},
                    {"ErrMessage", log.ErrMessage},
                    {"ErrStackTrace", log.ErrStackTrace},
                    {"ErrInnerMessage", log.ErrInnerMessage},
                    {"ErrInnerStackTrace", log.ErrInnerStackTrace}
                };
            }
            var rtn = _db.InsertBatch(logs);
        }

        public void Init(string host, string connStr)
        {
            _host = host;
            _db = new MongoRepository<Log>("log", "KLog", connStr);
        }
    }
}
