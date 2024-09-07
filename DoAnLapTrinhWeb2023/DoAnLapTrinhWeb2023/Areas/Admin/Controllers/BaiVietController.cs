using DoAnLapTrinhWeb2023.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnLapTrinhWeb2023.Areas.Admin.Controllers
{
    public class BaiVietController : Controller
    {
        // GET: Admin/BaiViet
        BanHangOnlineEntities objBanHangOnlineEntities = new BanHangOnlineEntities();
        // GET: Admin/ProductAdmin
        public ActionResult Index(string SearchString, int? page, string curentFilter)
        {
            var listBaiViet = new List<baiViet>();
            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = curentFilter;
            }
            if (!string.IsNullOrEmpty(SearchString))
            {
                //lấy ds sản phẩm theo từ khóa tìm kiếm
                listBaiViet = objBanHangOnlineEntities.baiViets.Where(n => n.tenBV.Contains(SearchString)).ToList();
            }
            else
            {
                listBaiViet = objBanHangOnlineEntities.baiViets.ToList();
            }
            ViewBag.curentFilter = SearchString;
            // sô lượng item của 1 trang = 13
            int pageSize = 13;
            int pageNumber = (page ?? 1);
            // sắp xếp theo id sản phẩm mới đưa lên đầu
            return View(listBaiViet.ToPagedList(pageNumber, pageSize));




           
        }
        [HttpGet]
        public ActionResult Create()
        {
            List<loaiSP> sp = objBanHangOnlineEntities.loaiSPs.ToList();
            ViewData["loais"] = sp;
            List<Brand> br = objBanHangOnlineEntities.Brands.ToList();
            ViewData["brand"] = br;

            return View();
        }
        [HttpPost]
        public ActionResult Create(baiViet objbaiViet)
        {
            try
            {
                //kiem tra doi tuong image co load dc hay khong
                if (objbaiViet.ImageUpload != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(objbaiViet.ImageUpload.FileName);
                    string extension = Path.GetExtension(objbaiViet.ImageUpload.FileName);
                    filename = filename + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                    objbaiViet.hinhDD = filename;
                    objbaiViet.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/image/"), filename));
                }
                objBanHangOnlineEntities.baiViets.Add(objbaiViet);
                objBanHangOnlineEntities.SaveChanges();
                //neu luu thanh cong return ve trang danh sach san pham
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public ActionResult Details(string id)
        {
            var objbaiViet = objBanHangOnlineEntities.sanPhams.Where(n => n.maSP == id).FirstOrDefault();
            return View(objbaiViet);
        }


        [HttpGet]
        public ActionResult Delete(string id)
        {
            var objbaiViet = objBanHangOnlineEntities.baiViets.Where(n => n.maBV == id).FirstOrDefault();
            return View(objbaiViet);
        }
        [HttpPost]
        public ActionResult Delete(string bv, string ip)
        {
            var objsbaiViets = objBanHangOnlineEntities.baiViets.Where(n => n.maBV == bv).FirstOrDefault();
            objBanHangOnlineEntities.baiViets.Remove(objsbaiViets);
            objBanHangOnlineEntities.SaveChanges();
            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult Edit(string id)
        {
            var objsbaiViets = objBanHangOnlineEntities.baiViets.Where(n => n.maBV == id).FirstOrDefault();
            return View(objsbaiViets);
        }
        [HttpPost]
        public ActionResult Edit(sanPham objbaiViet)
        //phương thức đẩy ảnh 
        {
            if (objbaiViet.ImageUpload != null)
            {
                string filename = Path.GetFileNameWithoutExtension(objbaiViet.ImageUpload.FileName);
                string extension = Path.GetExtension(objbaiViet.ImageUpload.FileName);
                filename = filename + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                objbaiViet.hinhDD = filename;
                objbaiViet.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/image/"), filename));
            }
            objBanHangOnlineEntities.Entry(objbaiViet).State = EntityState.Modified;
            objBanHangOnlineEntities.SaveChanges();
            //back ve trang danh sach san pham
            return RedirectToAction("Index");
        }

    }
}
