using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// 消费者写日志接口
    /// </summary>
    internal interface IWriteLog
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="host">主Host，如jbk，ypk，console</param>
        /// <param name="connStr">数据库连接字符串，若非数据库则是文件物理路径</param>
        void Init(string host, string connStr);

        /// <summary>
        /// 批量添加日志
        /// </summary>
        /// <param name="logs">日志</param>
        void AddLogs(List<Log> logs);
    }
}
