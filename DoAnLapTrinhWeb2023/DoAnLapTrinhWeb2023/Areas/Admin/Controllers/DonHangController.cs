using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnLapTrinhWeb2023.Models;
using PagedList;

namespace DoAnLapTrinhWeb2023.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {
        BanHangOnlineEntities objBanHangOnlineEntities = new BanHangOnlineEntities();

        // GET: Admin/  
        public ActionResult Index()
        {
            ViewData["DonHang"] = objBanHangOnlineEntities.donHangs.Where(n => n.daKichHoat == false).ToList();
            return View();
        }

        public ActionResult Duyet(string soDH)
        {
            donHang dh = objBanHangOnlineEntities.donHangs.Where(n => n.soDH.Equals(soDH)).FirstOrDefault();
            if (dh != null)
            {
                dh.daKichHoat = true;
                objBanHangOnlineEntities.SaveChanges();
            }
            ViewData["DonHang"] = objBanHangOnlineEntities.donHangs.Where(n => n.daKichHoat == false).ToList();
            return View("Index");
        }   
       
        public ActionResult DonHangDaDuyet()
        {
            ViewData["DonHang"] = objBanHangOnlineEntities.donHangs.Where(n => n.daKichHoat == true).ToList();
            return View();
        }
        [HttpPost]
    
    }
}
