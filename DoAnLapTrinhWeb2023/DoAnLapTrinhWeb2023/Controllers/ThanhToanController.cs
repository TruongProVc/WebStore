using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnLapTrinhWeb2023.Models;
using DoAnLapTrinhWeb2023.Models.Payments;

namespace DoAnLapTrinhWeb2023.Controllers
{
    public class ThanhToanController : Controller
    {
        static BanHangOnlineEntities objBanHangOnlineEntities = new BanHangOnlineEntities();
        static donHang donhangtam = null;

        // GET: ThanhToan
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult addItem(string maSP)
        {
            //---Tìm  kiếm theo mã sản phẩm
            sanPham sp = objBanHangOnlineEntities.sanPhams.FirstOrDefault(n => n.maSP.Equals(maSP));
            List<Cart> gh = Session["GioHang"] as List<Cart> ?? new List<Cart>();
            Cart cc = gh.FirstOrDefault(n => n.maSP.Equals(maSP));
            if (cc == null)
            {
                Cart cc1 = new Cart
                {
                    maSP = maSP,
                    tenSP = sp.tenSP,
                    giaSP = (int)sp.giaBan,
                    giamGia = (int)sp.giamGia,
                    tongGia = (int)((sp.giaBan - sp.giamGia)),
                    soLuong = 1
                };
                gh.Add(cc1);
            }
            else
            {
                cc.soLuong += 1;
                cc.tongGia = (int)((sp.giaBan - sp.giamGia) * cc.soLuong);
            }
            Session["GioHang"] = gh;
            List<sanPham> sff = objBanHangOnlineEntities.sanPhams.ToList();
            ViewData["dssp"] = sff;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult RemoveItem(string maSP)
        {
            List<Cart> cart = (Session["GioHang"] as List<Cart>) ?? new List<Cart>();
            Cart item = cart.FirstOrDefault(m => m.maSP.Equals(maSP));
            cart.Remove(item);
            Session["GioHang"] = cart;
            return RedirectToAction("Index", "ThanhToan");
        }
        public ActionResult UppdateItem(string maSP, int sl)
        {
            List<Cart> cart = (Session["GioHang"] as List<Cart>) ?? new List<Cart>();
            Cart item = cart.FirstOrDefault(m => m.maSP.Equals(maSP));
            if (item != null)
            {
                item.soLuong = sl;
                item.tongGia = (int)((item.giaSP - item.giamGia) * item.soLuong);
            }
            return RedirectToAction("Index", "ThanhToan");
        }
        [HttpGet]
        public ActionResult Payment()
        {
            khachHang x = new khachHang();
            List<Cart> gh = Session["GioHang"] as List<Cart> ?? new List<Cart>();
            Session["GioHang"] = gh;
            return View(x);
        }
        [HttpPost]
        public ActionResult Payment(khachHang x, string phuongthucthanhtoan)
        {
            if (x != null)
            {
                x.maKH = string.Format("{0:ssddhhyy}", DateTime.Now);
                x.ngaySinh = DateTime.Now;
                objBanHangOnlineEntities.khachHangs.Add(x);
                objBanHangOnlineEntities.SaveChanges();
                donHang d = new donHang();
                d.soDH = string.Format("{0:ssMMddhh}", DateTime.Now);
                d.maKH = x.maKH;
                d.ngayDat = DateTime.Now;
                d.ngayGH = DateTime.Now.AddDays(2);
                if(Session["idUser"] != null){
                    d.taiKhoan=Session["idUser"].ToString();
                }
                else
                {
                    d.taiKhoan = "minh";
                }            
                d.diaChiGH = x.diaChi;
                d.ghiChu = x.ghiChu;
                d.daKichHoat = false;
                objBanHangOnlineEntities.donHangs.Add(d);
                objBanHangOnlineEntities.SaveChanges();
                donhangtam = d;
                foreach (var i in Session["GioHang"] as List<Cart>)
                {
                    ctDonHang ct = new ctDonHang();
                    ct.maSP = i.maSP;
                    ct.giaBan = i.giaSP;
                    ct.giamGia = i.giamGia;
                    ct.soDH = d.soDH;
                    ct.soLuong = i.soLuong;
                    objBanHangOnlineEntities.ctDonHangs.Add(ct);
                }
                objBanHangOnlineEntities.SaveChanges();
                if (phuongthucthanhtoan.Equals("thanhToanOnline"))
                {
                    objBanHangOnlineEntities.SaveChanges();
                    return Redirect(UrlPayment(phuongthucthanhtoan, d.soDH));
                }
                else
                {
                    objBanHangOnlineEntities.SaveChanges();
                    return RedirectToAction("Success");
                }

            }

            return RedirectToAction("Unsuccess");
        }
   
        public ActionResult Success()
        {
            if (VnpayReturn(donhangtam.soDH))
            {

            }
            return View();
        }
        public ActionResult UnSuccess()
        {
            return View();
        }
        public bool VnpayReturn(string phuongthucthanhtoan)
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (phuongthucthanhtoan.Equals("thanhToanOnline"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private string UrlPayment(string pttt, string soDH)
        {
            donHang dh = objBanHangOnlineEntities.donHangs.FirstOrDefault(m => m.soDH.Equals(soDH));
            var chiTietDonhang = objBanHangOnlineEntities.ctDonHangs.ToList();
            int total = 0;
            foreach(var item in chiTietDonhang)
            {
                if (item.soDH.Equals(soDH))
                {
                    total += (int)((item.giaBan - item.giamGia) * item.soLuong);
                }
            }
            total = total * 100;
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key
      
            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (total).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (pttt.Equals("thanhToanOnline"))
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + dh.soDH);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", dh.soDH.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            //Add Params of 2.1.0 Version
            //Billing
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

    }
}