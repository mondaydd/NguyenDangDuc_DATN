using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Order
        public ActionResult Index(int? page)
        {
            var items = db.Orders.OrderByDescending(x => x.CreatedDate).ToList();

            if (page == null)
            {
                page = 1;
            }
            var pageNumber = page ?? 1;
            var pageSize = 10;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult View(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }

        public ActionResult Partial_SanPham(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return PartialView(items);
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng
        /// </summary>
        /// <param name="id">ID đơn hàng</param>
        /// <param name="trangthai">Giá trị int tương ứng với trạng thái mới</param>
        [HttpPost]
        public ActionResult UpdateStatus(int id, int trangthai)
        {
            // Tìm đơn hàng bằng id
            var item = db.Orders.FirstOrDefault(o => o.Id == id);

            if (item != null)
            {
                // Cập nhật trạng thái đơn hàng
                item.Status = trangthai;

                // Nếu là đơn COD và đã giao thì set IsPaid = true
                if (item.TypePayment == 1 && trangthai == 3) // 1 = COD, 3 = Đã giao
                {
                    item.IsPaid = true;
                }

                // Nếu là đơn VNPay, luôn set IsPaid = true
                if (item.TypePayment == 2) // 2 = VNPay
                {
                    item.IsPaid = true;
                }

                // Đánh dấu đối tượng đã thay đổi để lưu vào cơ sở dữ liệu
                db.Entry(item).Property(x => x.Status).IsModified = true;
                db.Entry(item).Property(x => x.IsPaid).IsModified = true;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                return Json(new { message = "Cập nhật trạng thái thành công", Success = true });
            }

            return Json(new { message = "Không tìm thấy đơn hàng", Success = false });
        }

    }
}
