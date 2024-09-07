using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnLapTrinhWeb2023.Models;
namespace DoAnLapTrinhWeb2023.Controllers
{
    public class NewsController : Controller
    {
        // GET: News
        static BanHangOnlineEntities objBanHangOnlineEntities = new BanHangOnlineEntities();
        // GET: Product
        public ActionResult Details(String maBV)
        {
            var objBaiView = objBanHangOnlineEntities.baiViets.Where(n => n.maBV == maBV).FirstOrDefault();

            return View(objBaiView);
        }
    }
}
