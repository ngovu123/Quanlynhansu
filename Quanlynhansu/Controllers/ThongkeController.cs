using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Quanlynhansu.Models;

namespace Quanlynhansu.Controllers
{
    public class ThongkeController : Controller
    {
        public ActionResult Index()
        {
            using (QLNSEntities db = new QLNSEntities())
            {
                // Get the list of employees and LUONG1 records from the database
                List<NHANVIEN> employees = db.NHANVIENs.ToList();
                List<LUONG1> luong1Records = db.LUONG1.ToList();
                List<PHONGBAN> phongBans = db.PHONGBANs.ToList();
                List<HOSOTD> hosoTDs = db.HOSOTDs.ToList(); // Add this line to get the list of hồ sơ ứng tuyển

                // Calculate the total earnings
                double totalEarnings = 0;
                foreach (var item in luong1Records)
                {
                    // Calculate total earnings here based on LUONG1 data
                    // Assuming you want to add item.TONGLANH to the total
                    totalEarnings += (double)item.TONGLANH;
                }

                // Create the view model and set the lists and total earnings
                ThongkeViewModel viewModel = new ThongkeViewModel
                {
                    Employees = employees,
                    Luong1Records = luong1Records,
                    TotalEarnings = totalEarnings,
                    PhongBans = phongBans,
                    HosoTDs = hosoTDs // Add this line to set the list of hồ sơ ứng tuyển in the view model
                };

                // Pass the view model to the Thongke view
                return View(viewModel);
            }
        }
    }
}
