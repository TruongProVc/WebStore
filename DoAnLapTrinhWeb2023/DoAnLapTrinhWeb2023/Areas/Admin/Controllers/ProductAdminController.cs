using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnLapTrinhWeb2023.Models;
using PagedList;

namespace DoAnLapTrinhWeb2023.Areas.Admin.Controllers
{
    public class ProductAdminController : Controller
    {
  
        BanHangOnlineEntities objBanHangOnlineEntities = new BanHangOnlineEntities();
        // GET: Admin/ProductAdmin
        public ActionResult Index(string SearchString ,int? page , string curentFilter)
        {
            var listsanPham = new List<sanPham>();
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
                listsanPham = objBanHangOnlineEntities.sanPhams.Where(n => n.tenSP.Contains(SearchString)).ToList();
            }
            else
            {
                listsanPham = objBanHangOnlineEntities.sanPhams.ToList();
            }
            ViewBag.curentFilter = SearchString;
            // sô lượng item của 1 trang = 13
            int pageSize = 13;
            int pageNumber = (page ?? 1);
            // sắp xếp theo id sản phẩm mới đưa lên đầu
            return View(listsanPham.ToPagedList(pageNumber, pageSize));




            //if (SearchString !=null)
            //{
            //    var listSanPhams = objBanHangOnlineEntities.sanPhams.Where(n => n.tenSP.ToUpper().Contains(SearchString)).ToList();
            //    return View(listSanPhams.ToList());

            //}

            var listSanPham = objBanHangOnlineEntities.sanPhams.ToList();
            return View(listSanPham);
        }
        [HttpGet]
        public ActionResult Create()
        {
            List<loaiSP> sp = objBanHangOnlineEntities.loaiSPs.ToList();
            ViewData["loaisp"] = sp;
            List<Brand> br = objBanHangOnlineEntities.Brands.ToList();
            ViewData["brand"] = br;
         
            return View();
        }
        [HttpPost]
        public ActionResult Create(sanPham objsanPham, sanPham dh)
        {
            dh.ngayDang = DateTime.Now;
            if (ModelState.IsValid)
            {
                sanPham sp = new sanPham
                { 
                //taiKhoan = "minh",
                daDuyet = false,
                ngayDang = dh.ngayDang,
                nhaSanXuat = "VietNam",
                dvt = "cái",
                };              
        }          
            try
            {         
                //kiem tra doi tuong image co load dc hay khong
                if (objsanPham.ImageUpload != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(objsanPham.ImageUpload.FileName);
                    string extension = Path.GetExtension(objsanPham.ImageUpload.FileName);
                    filename = filename + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                    objsanPham.hinhDD = filename;
                    objsanPham.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/image/"), filename));
                    
                }
                objBanHangOnlineEntities.sanPhams.Add(objsanPham);
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
            var objsanPhams = objBanHangOnlineEntities.sanPhams.Where(n => n.maSP == id).FirstOrDefault();
            return View(objsanPhams);
        }


        [HttpGet]
        public ActionResult Delete(string id)
        {
            var objsanPhams = objBanHangOnlineEntities.sanPhams.Where(n => n.maSP == id).FirstOrDefault();
            return View(objsanPhams);
        }
        [HttpPost]
        public ActionResult Delete(string sp, string ip)
        {
            var objsanPhams = objBanHangOnlineEntities.sanPhams.Where(n => n.maSP == sp).FirstOrDefault();
            objBanHangOnlineEntities.sanPhams.Remove(objsanPhams);
            objBanHangOnlineEntities.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit (string id)
        {
            var objsanPhams = objBanHangOnlineEntities.sanPhams.Where(n => n.maSP == id).FirstOrDefault();
            return View(objsanPhams);
        }
        [HttpPost]  
        public ActionResult Edit(sanPham objsanPham)
        //phương thức đẩy ảnh 
        {
            if (objsanPham.ImageUpload != null)
            {
                string filename = Path.GetFileNameWithoutExtension(objsanPham.ImageUpload.FileName);
                string extension = Path.GetExtension(objsanPham.ImageUpload.FileName);
                filename = filename + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                objsanPham.hinhDD = filename;
                objsanPham.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/image/"), filename));
            }
            objBanHangOnlineEntities.Entry(objsanPham).State = EntityState.Modified;
            objBanHangOnlineEntities.SaveChanges();
            //back ve trang danh sach san pham
            return RedirectToAction("Index");
        }
    }
 }
