using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quanlynhansu.Models;

namespace Quanlynhansu.Models
{
    public class ThongkeViewModel
    {
        public List<NHANVIEN> Employees { get; set; }
        public List<LUONG1> Luong1Records { get; set; }
        public double TotalEarnings { get; set; }
        public List<PHONGBAN> PhongBans { get; set; }
        public List<HOSOTD> HosoTDs { get; set; } // Add this property to hold the list of hồ sơ ứng tuyển

    }
}
