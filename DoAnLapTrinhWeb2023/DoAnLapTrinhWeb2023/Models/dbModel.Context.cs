﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoAnLapTrinhWeb2023.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BanHangOnlineEntities : DbContext
    {
        public BanHangOnlineEntities()
            : base("name=BanHangOnlineEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<baiViet> baiViets { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<ctDonHang> ctDonHangs { get; set; }
        public virtual DbSet<donHang> donHangs { get; set; }
        public virtual DbSet<khachHang> khachHangs { get; set; }
        public virtual DbSet<loaiSP> loaiSPs { get; set; }
        public virtual DbSet<nhomTk> nhomTks { get; set; }
        public virtual DbSet<quanHuyen> quanHuyens { get; set; }
        public virtual DbSet<sanPham> sanPhams { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<taiKhoanTV> taiKhoanTVs { get; set; }
    }
}
