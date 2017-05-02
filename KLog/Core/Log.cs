using System;

namespace Core
{
    internal struct Log
    {
        /// <summary>
        /// 日志类型，0错误 1警告 2普通日志 3调试信息
        /// </summary>
        public int Type;
        /// <summary>
        /// 日志创建时间
        /// </summary>
        public DateTime Time;
        /// <summary>
        /// 日志描述信息
        /// </summary>
        public string Message;
        /// <summary>
        /// 对应的网页链接
        /// </summary>
        public string Url;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMessage;
        /// <summary>
        /// 错误堆栈
        /// </summary>
        public string ErrStackTrace;
        /// <summary>
        /// 错误内部信息
        /// </summary>
        public string ErrInnerMessage;
        /// <summary>
        /// 错误内部堆栈
        /// </summary>
        public string ErrInnerStackTrace;
    }
}
