using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Quanlynhansu.Models;
using PagedList;
using PagedList.Mvc;
using System.Web.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Aspose.Words;

namespace Quanlynhansu.Controllers
{
    public class BangCongController : Controller
    {
        QLNSEntities db = new QLNSEntities();

        // GET: BangCong
        public ActionResult Index(FormCollection f)
        {
            var kt = db.BANGCONGs.ToList();
            HashSet<int> list_thang = new HashSet<int>();
            HashSet<int> list_nam = new HashSet<int>();
            DateTime currentDateTime = DateTime.Now;
            int Month = currentDateTime.Month;
            int year = currentDateTime.Year;
            foreach (var i in kt)
            {
                list_thang.Add((int)i.THANG);
                list_nam.Add((int)i.NAM);
            }

            ViewBag.thang = new SelectList(list_thang.Reverse());
            ViewBag.nam = new SelectList(list_nam.Reverse());

            int thang = list_thang.Max();
            int nam = list_nam.Max();
            ViewBag.t = thang.ToString();
            ViewBag.n = nam.ToString();
            var bangCongList = db.BANGCONGs.Include(x => x.NHANVIEN).Where(x => x.NAM == nam && x.THANG == thang).ToList();
            return View(bangCongList);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BANGCONG bANGCONG = db.BANGCONGs.Find(id);
            Session["MANVS"] = bANGCONG.MANV;
            if (bANGCONG == null)
            {
                return HttpNotFound();
            }
            ViewBag.MANV = new SelectList(db.NHANVIENs, "MANV", "HOTEN", bANGCONG.MANV);
            return View(bANGCONG);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MAC,MANV,NAM,THANG,CONGLE,NGHICP,NGHIKP,CONGTHUONG,CONGCN,TONGC")] BANGCONG bANGCONG)
        {
            if (ModelState.IsValid)
            {
                bANGCONG.MAC = int.Parse(Session["MANVS"].ToString());
                db.Entry(bANGCONG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MANV = new SelectList(db.NHANVIENs, "MANV", "HOTEN", bANGCONG.MANV);
            return View(bANGCONG);
        }

        public ActionResult Tinh(string thang, string nam)
        {
            int t = int.Parse(thang);
            int n = int.Parse(nam);
            double bhxh = 0;
            double bhyt = 0;
            double bhtn = 0;

            if (ModelState.IsValid)
            {
                var luong = new LUONG1();
                double tienthuong = 0;
                double kiluat = 0;
                var TruLuong = new TRULUONG();

                foreach (var i in db.BANGCONGs)
                {
                    if (t == i.THANG && n == i.NAM)
                    {
                        var manv = from s in db.LUONG1 where s.MANV == i.MANV select s.MANV;

                        luong.MANV = i.MANV;
                        TruLuong.MANV = i.MANV;

                        foreach (var item in db.HOPDONGs)
                        {
                            if (item.MANV == i.MANV)
                            {
                                luong.HSL = item.HESOLUONG;
                                luong.LUONGCB = item.LUONGCB;
                                luong.PHUCAP = item.PHUCAP;
                                break;
                            }
                        }

                        foreach (var item in db.KHENTHUONGs)
                        {
                            if (item.MANV == i.MANV)
                            {
                                tienthuong = (double)item.SOTIEN;
                                break;
                            }
                        }

                        foreach (var item in db.KYLUATs)
                        {
                            if (item.MANV == i.MANV)
                            {
                                kiluat = (double)item.SOTIEN;
                                break;
                            }
                        }

                        double luongngay = 0;

                        if (i.THANG == 1 || i.THANG == 3 || i.THANG == 5 || i.THANG == 7 || i.THANG == 8 || i.THANG == 10 || i.THANG == 12)
                        {
                            luongngay = (double)((luong.LUONGCB * luong.HSL) / 31);
                        }
                        else if (i.THANG == 4 || i.THANG == 6 || i.THANG == 9 || i.THANG == 11)
                        {
                            luongngay = (double)((luong.LUONGCB * luong.HSL) / 30);
                        }
                        else
                        {
                            if (i.NAM % 400 == 0 || (i.NAM % 4 == 0 && i.NAM % 100 == 0))
                            {
                                luongngay = (double)((luong.LUONGCB * luong.HSL) / 29);
                            }
                            else
                            {
                                luongngay = (double)((luong.LUONGCB * luong.HSL) / 28);
                            }
                        }

                        double congcn = (double)((i.CONGCN * luongngay) * 2);
                        double congle = (double)((i.CONGLE * luongngay) * 3);
                        double luongphep = (double)((0.3 * luongngay) * i.NGHICP);

                        luong.TULUONG = (luongngay * i.CONGTHUONG + luong.PHUCAP + congcn + congle + luongphep) * 0.105;
                        luong.TONGLANH = (luongngay * i.CONGTHUONG + luong.PHUCAP + congcn + congle + luongphep) - (i.NGHIKP * 500) - luong.TULUONG;

                        bhxh = (double)(luongngay * i.CONGTHUONG + luong.PHUCAP + congcn + congle + luongphep) * 0.08;
                        bhyt = (double)(luongngay * i.CONGTHUONG + luong.PHUCAP + congcn + congle + luongphep) * 0.015;
                        bhtn = (double)(luongngay * i.CONGTHUONG + luong.PHUCAP + congcn + congle + luongphep) * 0.01;

                        luong.TIENKB = tienthuong;
                        luong.TTIENKL = kiluat;
                        luong.THANG = i.THANG;
                        luong.NAM = i.NAM;

                        TruLuong.BHXH = bhxh;
                        TruLuong.BHYT = bhyt;
                        TruLuong.BHTN = bhtn;

                        db.LUONG1.Add(luong);
                        db.TRULUONGs.Add(TruLuong);

                        tienthuong = 0;
                        kiluat = 0;
                        luong = new LUONG1();
                        TruLuong = new TRULUONG();
                    }
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
