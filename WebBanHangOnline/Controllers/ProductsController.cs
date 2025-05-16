using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Search(string keyword)
        {
            var items = new List<Product>();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                items = db.Products
                          .Where(x => x.Title.ToLower().Contains(keyword) && x.IsActive)
                          .ToList();
            }

            ViewBag.Keyword = keyword;
            return View("Search", items);
        }
        // GET: Products
        public ActionResult Index(int? id)
        {
            var items = db.Products.ToList();
            if (id != null)
            {
                items = items.Where(x => x.ProductCategoryId == id).ToList();
            }
            return View(items);
        }

        public ActionResult Detail(string alias, int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                db.Products.Attach(item);
                item.ViewCount = item.ViewCount + 1;
                db.Entry(item).Property(x => x.ViewCount).IsModified = true;
                db.SaveChanges();
            }
            var countReview = db.Reviews.Where(x => x.ProductId == id).Count();
            ViewBag.CountReview = countReview;
            ViewBag.Stock = item.Quantity;
            return View(item);
        }
        public ActionResult ProductCategory(string alias, int id)
        {
            var items = db.Products.ToList();
            if (id > 0)
            {
                items = items.Where(x => x.ProductCategoryId == id).ToList();
            }
            var cate = db.ProductCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }
            ViewBag.CateId = id;
            return View(items);
        }

        public ActionResult Partial_ItemsByCateId()
        {
            DateTime tenDaysAgo = DateTime.Now.AddDays(-10);
            var items = db.Products
                          .Where(x => x.IsHome && x.IsActive && x.CreatedDate >= tenDaysAgo)
                          .OrderByDescending(x => x.CreatedDate)  // ưu tiên sản phẩm mới nhất lên đầu
                          .Take(15)
                          .ToList();
            return PartialView(items);
        }

        //public ActionResult Partial_ProductSales()
        //{
        //    var items = db.OrderDetails
        //            .Where(od => od.Order.Status == 3) // Chỉ lấy đơn đã giao
        //            .GroupBy(od => od.ProductId)
        //        .Select(g => new
        //        {
        //            Product = g.FirstOrDefault().Product,
        //            TotalSold = g.Sum(x => x.Quantity)
        //        })
        //        .OrderByDescending(x => x.TotalSold)
        //        .Take(12)
        //        .Select(x => new BestSellingProductViewModel
        //        {
        //            Product = x.Product,
        //            TotalSold = x.TotalSold
        //        })
        //        .ToList();

        //    return PartialView(items);
        //}
        public ActionResult Partial_ProductSales()
        {
            var items = db.OrderDetails
                .Where(od => od.Order.Status == 3 && od.Product != null) // Tránh null
                .GroupBy(od => od.Product)
                .Select(g => new BestSellingProductViewModel
                {
                    Product = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(12)
                .ToList();

            return PartialView(items);
        }

    }
}