using System;
using System.Threading;
using System.Web.Mvc;
using Core;

namespace KLog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Logger.Error("错误啦", new Exception("error"));
            Logger.Warn("警告，姓名不能为空！");
            Logger.Info("张三登录成功！");
            Logger.Debug("当前线程ID:" + Thread.CurrentThread.ManagedThreadId);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}