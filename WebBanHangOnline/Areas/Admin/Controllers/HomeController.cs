using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Home
        public ActionResult Index()
        {
            ViewBag.TotalCustomers = db.Users.Count(); // Tổng tài khoản
            ViewBag.TotalRevenue = db.Orders.Where(o => o.Status == 3).Sum(o => (decimal?)o.TotalAmount) ?? 0;
            ViewBag.TotalProductsSold = db.OrderDetails.Sum(d => (int?)d.Quantity) ?? 0;
            ViewBag.NewOrders = db.Orders.Count(o => o.Status == 3);
            return View();
        }
    }
}