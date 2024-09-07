using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnLapTrinhWeb2023.Models;
namespace DoAnLapTrinhWeb2023.Controllers
{
    public class ProductController : Controller
    {
       static BanHangOnlineEntities objBanHangOnlineEntities = new BanHangOnlineEntities();
        // GET: Product
        public ActionResult Details(String maSP)
        {
            var objSanPham = objBanHangOnlineEntities.sanPhams.Where(n => n.maSP == maSP).FirstOrDefault();
 
            return View(objSanPham);
        }
    }   
}