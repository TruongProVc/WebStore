using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

using DoAnLapTrinhWeb2023.Models;

namespace DoAnLapTrinhWeb2023.Controllers
{
    public class HomeController : Controller
    {
        //add lisst san pham
        static BanHangOnlineEntities db = new BanHangOnlineEntities();
        BanHangOnlineEntities objBanHangOnlineEntities = new BanHangOnlineEntities();
        public ActionResult Index()
        {

            List<sanPham> sp = db.sanPhams.ToList();
            ViewData["dssp"] = sp;       
            List<baiViet> bv = db.baiViets.ToList();
            ViewData["dsbv"] = bv;
            return View();
        }

        //dang ki dang nhap
        [HttpGet]
        public ActionResult Dangki()
        {
            return View();
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Dangki(taiKhoanTV _taikhoan)
        {
            if (ModelState.IsValid)
            {
                var check = objBanHangOnlineEntities.taiKhoanTVs.FirstOrDefault(s => s.email == _taikhoan.email);
                if (check == null)
                {
                    _taikhoan.matKhau = GetMD5(_taikhoan.matKhau);
                    objBanHangOnlineEntities.Configuration.ValidateOnSaveEnabled = false;
                    objBanHangOnlineEntities.taiKhoanTVs.Add(_taikhoan);
                    objBanHangOnlineEntities.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Email đã tồn tại";
                    return View();
                }
            }
            return View();
        }
        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("DangNhap");
        }
    




[HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
 [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(string email, string matkhau)
        {
            if (ModelState.IsValid)
            {


                var f_password = GetMD5(matkhau);
                //no se check xem cai email do co ton tai trong he thong ko 
                var data = objBanHangOnlineEntities.taiKhoanTVs.Where(s => s.email.Equals(email.ToLower().Trim()) && s.matKhau.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["Name"] = data.FirstOrDefault().tenTV;
                    Session["Email"] = data.FirstOrDefault().email;
                    Session["idUser"] = data.FirstOrDefault().taiKhoan;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Đăng nhập thất bại";
                    return RedirectToAction("DangNhap");
                }
            }
            return View();
        }


        //Logout


        //tao cai phuong thuc string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
    }
}

