﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnLapTrinhWeb2023.Models;
namespace DoAnLapTrinhWeb2023.Controllers
{
    public class LaptopController : Controller
    {
        // GET: Laptop
            BanHangOnlineEntities objBanHangOnlineEntities = new BanHangOnlineEntities();
            public ActionResult Index(string tenTH)
            {
                //loc cac ten brand vat dau tu phan string 
                var objKB = objBanHangOnlineEntities.sanPhams.Where(n => n.loaiSP.loaiSP1.Equals("Laptop") && (string.IsNullOrEmpty(tenTH) || (n.Brand.tenTH.Equals(tenTH)))).ToList<sanPham>();
                ViewData["lt"] = objKB;
                //khi co ai do click vao thi no se lien tuc update
                updateBrand();
                return View(objKB);

            }
            public void updateBrand()
            {
                ViewData["up"] = objBanHangOnlineEntities.Brands.ToList();

            }
        }
    }