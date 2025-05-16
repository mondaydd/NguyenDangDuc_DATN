using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OrderHistory
        public ActionResult History()
        {
            var userId = User.Identity.GetUserId();  // Lấy ID người dùng đang đăng nhập
            var orders = db.Orders.Include("OrderDetails.Product")
        .Where(x => x.UserId == userId)
        .OrderByDescending(x => x.CreatedDate)
        .ToList();

            return View(orders);  // Trả về View với danh sách đơn hàng
        }

        // GET: OrderDetails/5
        public ActionResult Details(int id)
        {
            var order = db.Orders
    .Include("OrderDetails.Product") // dùng chuỗi để include navigation property
    .FirstOrDefault(o => o.Id == id);



            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);  // Trả về View với chi tiết đơn hàng
        }
        [HttpPost]
        public ActionResult Cancel(int id)
        {
            var userId = User.Identity.GetUserId();
            var order = db.Orders.Include("OrderDetails").FirstOrDefault(x => x.Id == id && x.UserId == userId);

            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            if (order.Status != 1)
            {
                return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang chờ xác nhận." });
            }

            foreach (var item in order.OrderDetails)
            {
                var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                {
                    product.Quantity += item.Quantity;
                }
            }

            order.Status = 4; // Đã hủy
            db.SaveChanges();

            return Json(new { success = true });
        }

    }
}