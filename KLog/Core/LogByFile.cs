using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace Core
{
    /// <summary>
    /// 用文本记录日记
    /// </summary>
    internal class LogByFile : IWriteLog
    {
        /// <summary>
        /// 主Host，如jbk，ypk，console
        /// </summary>
        private string _host;
        /// <summary>
        /// 日志文件物理路径
        /// </summary>
        private string _filePath;
        /// <summary>
        /// 冒号
        /// </summary>
        private const string Colon = "：";
        /// <summary>
        /// 错误信息
        /// </summary>
        private const string ErrMsg = "。ErrMsg：";
        /// <summary>
        /// 错误堆栈
        /// </summary>
        private const string ErrStacktrace = "。ErrStacktrace：";
        /// <summary>
        /// 错误信息
        /// </summary>
        private const string ErrInnerMsg = "。ErrInnerMsg：";
        /// <summary>
        /// 错误堆栈
        /// </summary>
        private const string ErrInnerStacktrace = "。ErrInnerStacktrace：";
        /// <summary>
        /// 换行
        /// </summary>
        private const string Newline = "\r\n";
        /// <summary>
        /// 时间格式化
        /// </summary>
        private const string TimeFormat = "yy-M-d H:m:s ";
        /// <summary>
        /// 存放要写入的日志字符串
        /// </summary>
        private StringBuilder _sb = new StringBuilder();

        public void AddLogs(List<Log> logs)
        {
            _sb.Clear();

            for (int i = 0; i < logs.Count; i++)
            {
                var log = logs[i];
                _sb.Append(log.Time.ToString(TimeFormat)).Append(_host).Append(log.Type).Append(Colon).Append(log.Message);
                if (!string.IsNullOrEmpty(log.ErrMessage))
                {
                    _sb.Append(ErrMsg)
                        .Append(log.ErrMessage)
                        .Append(ErrStacktrace)
                        .Append(log.ErrStackTrace);

                    if (!string.IsNullOrEmpty(log.ErrInnerMessage))
                    {
                        _sb.Append(ErrInnerMsg)
                            .Append(log.ErrInnerMessage)
                            .Append(ErrInnerStacktrace)
                            .Append(log.ErrInnerStackTrace);
                    }
                }
                _sb.Append(Newline);
            }
            File.AppendAllText(_filePath, _sb.ToString());
        }

        public void Init(string host, string connStr)
        {
            _sb = new StringBuilder();
            _host = host;
            _filePath = HostingEnvironment.IsHosted ? HostingEnvironment.MapPath("~/" + connStr) : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, connStr);
            var dir = Path.GetDirectoryName(_filePath);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
