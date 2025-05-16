using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Models
{
    public class BestSellingProductViewModel
    {
        public Product Product { get; set; }
        public int TotalSold { get; set; }
    }
}