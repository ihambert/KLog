using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// 超高性能日志类
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// 日志-生产者
        /// </summary>
        private static List<Log> _logProducers;
        /// <summary>
        /// 日志-消费者
        /// </summary>
        private static List<Log> _logConsumers;
        /// <summary>
        /// 日志-临时，用来交换生产者日志和消费者日志
        /// </summary>
        private static List<Log> _logTemps;
        /// <summary>
        /// 写日志对象
        /// </summary>
        private static IWriteLog _log;
        /// <summary>
        /// 需要记日志的等级
        /// </summary>
        private static int _level;
        /// <summary>
        /// 是否在等待唤醒消费者消费
        /// </summary>
        private static bool _wait;
        /// <summary>
        /// 等待唤醒消费者的时间间隔
        /// </summary>
        private static int _waitTime;

        /// <summary>
        /// 注册启动KLog
        /// </summary>
        /// <param name="host">主Host，如jbk、ypk、console</param>
        public static void Register(string host)
        {
            if (_logConsumers != null)
            {
                return;
            }

            _logConsumers = new List<Log>();
            _logProducers = new List<Log>();
            var connStr = ConfigurationManager.AppSettings["KLogConnStr"];
            //默认间隔1分钟写一次日志
            _waitTime = 60000;

            try
            {
                _level = int.Parse(ConfigurationManager.AppSettings["KLogLevel"]);
            }
            catch
            {
                //默认除了debug的日志外都记录
                _level = 2;
            }

            try
            {
                //记录日志的方式（File文本，Mongodb、Sqlserver、Mysql、Oracle数据库，Excel表格。需要的自己去仿照LogByMongodb实现）
                var logMode = "Core.LogBy" + ConfigurationManager.AppSettings["KLogBy"];
                //抽象工厂模式初始化日志类
                var type = Type.GetType(logMode, true);
                _log = (IWriteLog)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                //默认用文本记录日志
                _log = new LogByFile();
            }


            _log.Init(host, connStr);
            Warn("进程启动！");
        }

        /// <summary>
        /// 把日志加入生产者队列，判断需要消费者写日志时延迟启动消费者写日志
        /// </summary>
        /// <param name="log">日志</param>
        private static void WaitToStartLog(Log log)
        {
            lock (_logProducers)
            {
                _logProducers.Add(log);

                if (_wait)
                {
                    return;
                }

                //_wait的判断和赋值必须在锁里进行，避免多个线程同时进入的问题
                _wait = true;
            }

            Task.Factory.StartNew(() =>
            {
                //一分钟消费一次日志
                Thread.Sleep(_waitTime);

                //若消费者还未消费完，则延长等待
                while (_logConsumers.Count != 0)
                {
                    Thread.Sleep(_waitTime);
                }

                //交换生产者队列和消费者队列
                lock (_logProducers)
                {
                    _logTemps = _logConsumers;
                    _logConsumers = _logProducers;
                    _logProducers = _logTemps;
                }
                _wait = false;
                //消费者不存在多线程同时进入可能，所以无需加锁
                try
                {
                    _log.AddLogs(_logConsumers);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                //执行完毕清空日志
                _logConsumers.Clear();
            });
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="msg">异常描述信息</param>
        /// <param name="ex">异常</param>
        /// <param name="url">对应网页链接</param>
        public static void Error(string msg, Exception ex, string url = null)
        {
            if (ex == null)
            {
                Warn(msg);
                return;
            }

            var log = new Log
            {
                Time = DateTime.Now,
                Message = msg,
                Url = url,
                Type = 0,
                ErrMessage = ex.Message,
                ErrStackTrace = ex.StackTrace
            };

            if (ex.InnerException != null)
            {
                log.ErrInnerMessage = ex.InnerException.Message;
                log.ErrInnerStackTrace = ex.InnerException.StackTrace;
            }

            WaitToStartLog(log);
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="msg">警告信息</param>
        /// <param name="url">对应网页链接</param>
        public static void Warn(string msg, string url = null)
        {
            if (_level == 0)
            {
                return;
            }

            var log = new Log
            {
                Time = DateTime.Now,
                Message = msg,
                Url = url,
                Type = 1
            };

            WaitToStartLog(log);
        }

        /// <summary>
        /// 记录普通日志
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="url">对应网页链接</param>
        public static void Info(string msg, string url = null)
        {
            if (_level < 2)
            {
                return;
            }

            var log = new Log
            {
                Time = DateTime.Now,
                Message = msg,
                Url = url,
                Type = 2
            };

            WaitToStartLog(log);
        }

        /// <summary>
        /// 记录调试日志
        /// </summary>
        /// <param name="msg">调试信息</param>
        /// <param name="url">对应网页链接</param>
        public static void Debug(string msg, string url = null)
        {
            if (_level < 3)
            {
                return;
            }

            var log = new Log
            {
                Time = DateTime.Now,
                Message = msg,
                Url = url,
                Type = 3
            };

            WaitToStartLog(log);
        }

        /// <summary>
        /// 关闭进程时调用的方法
        /// </summary>
        public static void EndProcess()
        {
            _waitTime = 0;
            Warn("进程关闭！");
        }
    }
}
