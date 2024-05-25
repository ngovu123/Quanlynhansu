using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Quanlynhansu.Models;
using System.Data.Entity.Infrastructure;



namespace Quanlynhansu.Controllers
{
    public class LuongController : Controller
    {
        private QLNSEntities db = new QLNSEntities();

        // GET: Luong
        public ActionResult Index(FormCollection f)
        {
            var kt = db.LUONG1.ToList();
            HashSet<int> list_thang = new HashSet<int>();
            HashSet<int> list_nam = new HashSet<int>();
            foreach (var i in kt)
            {
                if (i.THANG.HasValue) // Kiểm tra i.THANG có giá trị không
                {
                    list_thang.Add((int)i.THANG);
                }

                if (i.NAM.HasValue) // Kiểm tra i.NAM có giá trị không
                {
                    list_nam.Add((int)i.NAM);
                }
            }
            ViewBag.thang = new SelectList(list_thang.Reverse());
            ViewBag.nam = new SelectList(list_nam.Reverse());
            if (f["thang"] == null && f["nam"] == null)
            {
                return View(db.LUONG1.Include(x => x.NHANVIEN).ToList());
            }
            else if (f["thang"] != null && f["nam"] != null)
            {
                int thang = int.Parse(f["thang"].ToString());
                int nam = int.Parse(f["nam"].ToString());

                foreach (var i in db.BANGCONGs)
                {
                    if (nam == i.NAM && thang == i.THANG)
                    {
                        //manv = (int)i.MANV;
                        return View(db.LUONG1.Include(x => x.NHANVIEN).Where(x => x.NAM == nam && x.THANG == thang).ToList());
                    }
                }
            }
            

            return View(db.LUONG1.ToList());

        }
        public ActionResult Export()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("luong");

                // Add headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "TÊN NHÂN VIÊN";
                worksheet.Cells[1, 3].Value = "LƯƠNG CƠ BẢN";
                worksheet.Cells[1, 4].Value = "PHỤ CẤP";
                worksheet.Cells[1, 5].Value = "TRỪ LƯƠNG";
                worksheet.Cells[1, 6].Value = "TIỀN THƯỞNG";
                worksheet.Cells[1, 7].Value = "TIỀN PHẠT";
                worksheet.Cells[1, 8].Value = "TỔNG LÃNH";


                var range0 = worksheet.Cells[1, 1, 1, 18];
                range0.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range0.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range0.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range0.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range0.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range0.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                var headerFont = range0.Style.Font;
                headerFont.Bold = true;
                // Add data
                var ex = db.LUONG1.ToList();
                int row = 2;
                foreach (var employee in ex)
                {
                    worksheet.Cells[row, 1].Value = employee.MAL;
                    worksheet.Cells[row, 2].Value = employee.NHANVIEN.HOTEN;
                    worksheet.Cells[row, 3].Value = employee.LUONGCB;
                    worksheet.Cells[row, 4].Value = employee.PHUCAP;
                    worksheet.Cells[row, 5].Value = employee.TULUONG;
                    worksheet.Cells[row, 6].Value = employee.TIENKB;
                    worksheet.Cells[row, 7].Value = employee.TTIENKL;
                    worksheet.Cells[row, 8].Value = employee.TONGLANH;


                    var range = worksheet.Cells[row, 1, row, 8];
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();
                    worksheet.Column(5).AutoFit();
                    worksheet.Column(6).AutoFit();
                    worksheet.Column(7).AutoFit();
                    worksheet.Column(8).AutoFit();


                    // Set horizontal alignment and vertical alignment to center
                    var range1 = worksheet.Cells[row, 1, row, 1];
                    range1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    row++;
                }

                // Set the content type and filename for the file
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "luong.xlsx";

                // Convert the package to a byte array
                var byteArray = package.GetAsByteArray();

                // Return the file
                return File(byteArray, contentType, fileName);
            }
        }
        // GET: Luong/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LUONG1 lUONG1 = db.LUONG1.Find(id);
            if (lUONG1 == null)
            {
                return HttpNotFound();
            }
            ViewBag.ML = lUONG1.MANV;
            return View(lUONG1);
        }

        // GET: Luong/Create
        public ActionResult Create()
        {
            ViewBag.MAC = new SelectList(db.BANGCONGs, "MAC", "MAC");
            ViewBag.MANV = new SelectList(db.NHANVIENs, "MANV", "HOTEN");
            return View();
        }

        // POST: Luong/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MAL,MANV,HSL,LUONGCB,PHUCAP,TULUONG,TIENKB,TTIENKL,TONGLANH,MAC")] LUONG1 lUONG1)
        {
            if (ModelState.IsValid)
            {
                double tienthuong = 0;
                double kiluat = 0;
                int manv = 0;
                foreach (var item in db.HOPDONGs)
                {
                    if (item.MANV == lUONG1.MANV)
                    {
                        manv = item.MANV;
                        lUONG1.HSL = item.HESOLUONG;
                        lUONG1.LUONGCB = item.LUONGCB;
                        lUONG1.PHUCAP = item.PHUCAP;
                        break;
                    }
                }
                foreach (var item in db.KHENTHUONGs)
                {
                    if (item.MANV == manv)
                    {
                        tienthuong = (double)item.SOTIEN;
                        break;
                    }
                }
                foreach (var item in db.KYLUATs)
                {
                    if (item.MANV == manv)
                    {
                        kiluat = (double)item.SOTIEN;
                        break;
                    }
                }
                foreach (var item in db.BANGCONGs)
                {
                    if (item.MANV == manv)
                    {
                        double luongngay = 0;

                        if (item.THANG == 1 || item.THANG == 3 || item.THANG == 5 || item.THANG == 7 || item.THANG == 8 || item.THANG == 10 || item.THANG == 12)
                        {
                            luongngay = (double)((lUONG1.LUONGCB * lUONG1.HSL) / 31);
                        }
                        else if (item.THANG == 4 || item.THANG == 6 || item.THANG == 9 || item.THANG == 11)
                        {
                            luongngay = (double)((lUONG1.LUONGCB * lUONG1.HSL) / 30);
                        }
                        else
                        {
                            if (item.NAM % 400 == 0 || (item.NAM % 4 == 0 && item.NAM % 100 == 0))
                            {
                                luongngay = (double)((lUONG1.LUONGCB * lUONG1.HSL) / 29);
                            }
                            else
                            {
                                luongngay = (double)((lUONG1.LUONGCB * lUONG1.HSL) / 28);
                            }
                        }

                        double congcn = (double)((item.CONGCN * luongngay) * 2);
                        double congle = (double)((item.CONGLE * luongngay) * 3);
                        double luongphep = (double)((0.3 * luongngay) * item.NGHICP);
                        lUONG1.TULUONG = (luongngay * item.CONGTHUONG + lUONG1.PHUCAP + congcn + congle + luongphep) * (10.5 / 100);
                        lUONG1.TONGLANH = (luongngay * item.CONGTHUONG + lUONG1.PHUCAP + congcn + congle + luongphep) - (item.NGHIKP * 500) - lUONG1.TULUONG;
                        break;
                    }
                }
                lUONG1.TIENKB = tienthuong;
                lUONG1.TTIENKL = kiluat;
                db.LUONG1.Add(lUONG1);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MAC = new SelectList(db.BANGCONGs, "MAC", "MAC", lUONG1.MAC);
            ViewBag.MANV = new SelectList(db.NHANVIENs, "MANV", "HOTEN", lUONG1.MANV);

            return View(lUONG1);
        }

        // GET: Luong/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LUONG1 lUONG1 = db.LUONG1.Find(id);
            if (lUONG1 == null)
            {
                return HttpNotFound();
            }
            ViewBag.MANV = new SelectList(db.NHANVIENs, "MANV", "HOTEN", lUONG1.MANV);
            return View(lUONG1);
        }

        // POST: Luong/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MAL,MANV,HSL,LUONGCB,PHUCAP,TULUONG,TIENKB,TTIENKL,TONGLANH")] LUONG1 lUONG1)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lUONG1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MANV = new SelectList(db.NHANVIENs, "MANV", "HOTEN", lUONG1.MANV);
            return View(lUONG1);
        }


        // GET: Luong/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LUONG1 lUONG1 = db.LUONG1.Find(id);
            if (lUONG1 == null)
            {
                return HttpNotFound();
            }
            return View(lUONG1);
        }

        // POST: Luong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LUONG1 lUONG1 = db.LUONG1.Find(id);
            db.LUONG1.Remove(lUONG1);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Xoaluong()
        {
            db.LUONG1.RemoveRange(db.LUONG1.ToList());
            db.TRULUONGs.RemoveRange(db.TRULUONGs.ToList());
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}