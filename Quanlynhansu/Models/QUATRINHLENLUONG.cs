//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Quanlynhansu.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class QUATRINHLENLUONG
    {
        public int MAQD { get; set; }
        public Nullable<int> MAHD { get; set; }
        public Nullable<double> HESOLUONG_HIENTAI { get; set; }
        public Nullable<double> HESOLUONG_MOI { get; set; }
        public Nullable<System.DateTime> NGAYKY { get; set; }
        public Nullable<System.DateTime> NGAYLENLUONG { get; set; }
    
        public virtual HOPDONG HOPDONG { get; set; }
    }
}
