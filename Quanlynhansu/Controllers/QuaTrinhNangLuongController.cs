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

namespace Quanlynhansu.Controllers
{
    public class QuaTrinhNangLuongController : Controller
    {
        private QLNSEntities db = new QLNSEntities();

        // GET: QUATRINHLENLUONGs
        public ActionResult Index(int? page, string searchString)
        {

            TempData["list"] = Request["list"];
            ViewBag.Keyword = searchString;
            ViewBag.list = new List<lookfor>() {

                new lookfor(){ID="0",Name="-- Chọn điền kiện --"},

                new lookfor(){ID="1",Name="Mã quyết định"},

                new lookfor(){ID="2",Name="Tên nhân viên"},


            };

            if (TempData["list"] == null)
            {
                var hopdong = db.QUATRINHLENLUONGs.Include(h => h.HOPDONG);

                int PageNum = (page ?? 1);
                int PageSize = 5;
                return View(hopdong.ToList().OrderBy(n => n.MAQD).ToPagedList(PageNum, PageSize));

            }
            if (TempData["list"].ToString() == "1")
            {
                var hopdong = db.QUATRINHLENLUONGs.Include(h => h.HOPDONG);
                if (!String.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    hopdong = hopdong.Where(b => b.MAQD.ToString().ToLower().Contains(searchString));
                }
                int PageNum = (page ?? 1);
                int PageSize = 5;
                return View(hopdong.ToList().OrderBy(n => n.MAQD).ToPagedList(PageNum, PageSize));

            }

            else if (TempData["list"].ToString() == "2")
            {
                var hopdong = db.QUATRINHLENLUONGs.Include(h => h.HOPDONG);
                if (!String.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    hopdong = hopdong.Where(b => b.HOPDONG.NHANVIEN.HOTEN.ToString().ToLower().Contains(searchString));
                }
                int PageNum = (page ?? 1);
                int PageSize = 5;
                return View(hopdong.ToList().OrderBy(n => n.MAHD).ToPagedList(PageNum, PageSize));

            }
            else
            {
                var hopdong = db.QUATRINHLENLUONGs.Include(h => h.HOPDONG);/*if (!String.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    nhanvien = nhanvien.Where(b => b.DIACHI.ToLower().Contains(searchString));
                }*/
                int PageNum = (page ?? 1);
                int PageSize = 5;
                return View(hopdong.ToList().OrderBy(n => n.MAQD).ToPagedList(PageNum, PageSize));

            }


        }

        // GET: QUATRINHLENLUONGs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUATRINHLENLUONG qUATRINHLENLUONG = db.QUATRINHLENLUONGs.Find(id);
            List<QUATRINHLENLUONG_CHITIET> QT = new List<QUATRINHLENLUONG_CHITIET>();
            foreach (var item in db.QUATRINHLENLUONG_CHITIET)
            {

                if (item.MAQD == id)
                {
                    QUATRINHLENLUONG_CHITIET qt = new QUATRINHLENLUONG_CHITIET();
                    qt.MAQD = item.MAQD;
                    qt.HESOLUONG = item.HESOLUONG;
                    qt.THANG = item.THANG;
                    qt.NAM = item.NAM;
                    QT.Add(qt);
                }

            }
            ViewBag.ten = qUATRINHLENLUONG.HOPDONG.NHANVIEN.HOTEN;
            ViewBag.qt = qUATRINHLENLUONG;
            ViewBag.QT = QT.ToList();
            if (qUATRINHLENLUONG == null)
            {
                return HttpNotFound();
            }
            return View(qUATRINHLENLUONG);
        }

        // GET: QUATRINHLENLUONGs/Create
        public ActionResult Create()
        {
            ViewBag.MANV = new SelectList(db.NHANVIENs, "MANV", "HOTEN");
            return View();
        }

        // POST: QUATRINHLENLUONGs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MAQD,MAHD,HESOLUONG_HIENTAI,HESOLUONG_MOI,NGAYKY,NGAYLENLUONG")] QUATRINHLENLUONG qUATRINHLENLUONG, FormCollection f)
        {
            if (ModelState.IsValid)
            {
                int d = int.Parse(f["MANV"].ToString());
                HOPDONG s = db.HOPDONGs.Where(x => x.MANV == d).FirstOrDefault();
                qUATRINHLENLUONG.MAHD = s.MAHD;
                qUATRINHLENLUONG.HESOLUONG_HIENTAI = s.HESOLUONG;
                qUATRINHLENLUONG.HESOLUONG_MOI = s.HESOLUONG;
                db.QUATRINHLENLUONGs.Add(qUATRINHLENLUONG);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MANV = new SelectList(db.NHANVIENs, "MANV", "HOTEN", qUATRINHLENLUONG.HOPDONG.NHANVIEN.MANV);
            return View(qUATRINHLENLUONG);
        }

        // GET: QUATRINHLENLUONGs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUATRINHLENLUONG qUATRINHLENLUONG = db.QUATRINHLENLUONGs.Find(id);
            Session["heso"] = qUATRINHLENLUONG.HESOLUONG_MOI;

            if (qUATRINHLENLUONG == null)
            {
                return HttpNotFound();
            }
            ViewBag.MAHD = new SelectList(db.HOPDONGs, "MAHD", "LOAIHD", qUATRINHLENLUONG.MAHD);
            return View(qUATRINHLENLUONG);
        }

        // POST: QUATRINHLENLUONGs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MAQD,MAHD,HESOLUONG_MOI,NGAYKY,NGAYLENLUONG")] QUATRINHLENLUONG qUATRINHLENLUONG)
        {
            if (ModelState.IsValid)
            {
                QUATRINHLENLUONG_CHITIET QT = new QUATRINHLENLUONG_CHITIET();
                QT.MAQD = int.Parse(qUATRINHLENLUONG.MAQD.ToString());
                QT.HESOLUONG = qUATRINHLENLUONG.HESOLUONG_MOI;
                QT.THANG = DateTime.Now.Month;
                QT.NAM = DateTime.Now.Year;
                db.QUATRINHLENLUONG_CHITIET.Add(QT);
                double s = Convert.ToDouble(Session["heso"]);
                qUATRINHLENLUONG.HESOLUONG_HIENTAI = s;
                qUATRINHLENLUONG.NGAYKY = DateTime.Now;
                DateTime date = DateTime.Now;
                qUATRINHLENLUONG.NGAYLENLUONG = date.AddMonths(1);


                db.Entry(qUATRINHLENLUONG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MAHD = new SelectList(db.HOPDONGs, "MAHD", "LOAIHD", qUATRINHLENLUONG.MAHD);
            return View(qUATRINHLENLUONG);
        }

        // GET: QUATRINHLENLUONGs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUATRINHLENLUONG qUATRINHLENLUONG = db.QUATRINHLENLUONGs.Find(id);
            if (qUATRINHLENLUONG == null)
            {
                return HttpNotFound();
            }
            return View(qUATRINHLENLUONG);
        }

        // POST: QUATRINHLENLUONGs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QUATRINHLENLUONG qUATRINHLENLUONG = db.QUATRINHLENLUONGs.Find(id);
            db.QUATRINHLENLUONGs.Remove(qUATRINHLENLUONG);
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
