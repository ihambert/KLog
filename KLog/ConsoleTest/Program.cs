using System;
using System.Threading;
using Core;

namespace ConsoleTest
{
    class Program
    {
        static void Main()
        {
            Logger.Register("console");
            Logger.Error("错误啦", new Exception("error"));
            Logger.Warn("警告，姓名不能为空！");
            Logger.Info("张三登录成功！");
            Logger.Debug("当前线程ID:" + Thread.CurrentThread.ManagedThreadId);
            Logger.EndProcess();
            Console.ReadKey();
        }
    }
}
