using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class StatisticalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate, string month, string year)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.OrderId
                        join p in db.Products on od.ProductId equals p.Id
                        where o.IsPaid == true
                        select new
                        {
                            o.CreatedDate,
                            od.Quantity,
                            od.Price,
                            p.OriginalPrice
                        };

            // Lọc theo ngày
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime start = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= start);
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime end = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate <= end);
            }

            // Lọc theo tháng/năm
            if (!string.IsNullOrEmpty(month))
            {
                var parts = month.Split('/');
                int m = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);
                query = query.Where(x => x.CreatedDate.Month == m && x.CreatedDate.Year == y);
            }

            // Lọc theo năm
            if (!string.IsNullOrEmpty(year))
            {
                int y = int.Parse(year);
                query = query.Where(x => x.CreatedDate.Year == y);
            }

            List<object> result;

            // Group theo tháng nếu chỉ có năm
            if (!string.IsNullOrEmpty(year) && string.IsNullOrEmpty(month) && string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(toDate))
            {
                result = query
                    .GroupBy(x => new { x.CreatedDate.Year, x.CreatedDate.Month })
                    .Select(g => new
                    {
                        Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                        DoanhThu = g.Sum(x => x.Price * x.Quantity),
                        LoiNhuan = g.Sum(x => (x.Price - x.OriginalPrice) * x.Quantity)
                    }).OrderBy(x => x.Date).ToList<object>();
            }
            else
            {
                // Group theo ngày
                result = query
                    .GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate))
                    .Select(g => new
                    {
                        Date = g.Key.Value,
                        DoanhThu = g.Sum(x => x.Price * x.Quantity),
                        LoiNhuan = g.Sum(x => (x.Price - x.OriginalPrice) * x.Quantity)
                    }).OrderBy(x => x.Date).ToList<object>();
            }

            var totalRevenue = result.Sum(x => (decimal)x.GetType().GetProperty("DoanhThu").GetValue(x));
            var totalProfit = result.Sum(x => (decimal)x.GetType().GetProperty("LoiNhuan").GetValue(x));

            return Json(new
            {
                Data = result,
                TotalRevenue = totalRevenue,
                TotalProfit = totalProfit
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetBestSelling()
        {
            var result = db.OrderDetails
                .Where(od => od.Order.IsPaid == true)
                .GroupBy(x => x.ProductId)
                .Select(g => new
                {
                    ProductName = g.FirstOrDefault().Product.Title,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(10)
                .ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSlowSelling()
        {
            var soldProductIds = db.OrderDetails
                .Where(od => od.Order.IsPaid == true)
                .Select(od => od.ProductId)
                .Distinct();

            var result = db.Products
                .Where(p => !soldProductIds.Contains(p.Id))
                .Select(p => new
                {
                    ProductName = p.Title,
                    TotalSold = 0
                }).Take(10).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
    }
}
