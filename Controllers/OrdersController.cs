using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OrderManagementWeb.Filters;
using OrderManagementWeb.Models;

namespace OrderManagementWeb.Controllers
{
    [AuthenticationFilter]
    public class OrdersController : Controller
    {
        private OrderManagementDbEntities db = new OrderManagementDbEntities();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Agent).ToList();
            return View(orders);
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Include(o => o.Agent)
                                   .Include(o => o.OrderDetails.Select(od => od.Item))
                                   .FirstOrDefault(o => o.OrderID == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.AgentID = new SelectList(db.Agents, "AgentID", "AgentName");
            ViewBag.Items = db.Items.ToList();
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var order = new Order
                {
                    OrderDate = model.OrderDate,
                    AgentID = model.AgentID
                };

                db.Orders.Add(order);

                // Add order details
                if (model.OrderDetails != null)
                {
                    foreach (var detail in model.OrderDetails.Where(d => d.ItemID > 0 && d.Quantity > 0))
                    {
                        var item = db.Items.Find(detail.ItemID);
                        if (item != null)
                        {
                            var orderDetail = new OrderDetail
                            {
                                OrderID = order.OrderID,
                                ItemID = detail.ItemID,
                                Quantity = detail.Quantity,
                                UnitAmount = item.UnitPrice
                            };
                            db.OrderDetails.Add(orderDetail);
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AgentID = new SelectList(db.Agents, "AgentID", "AgentName", model.AgentID);
            ViewBag.Items = db.Items.ToList();
            return View(model);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Include(o => o.OrderDetails)
                                   .FirstOrDefault(o => o.OrderID == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            var model = new OrderViewModel
            {
                OrderID = order.OrderID,
                OrderDate = order.OrderDate,
                AgentID = order.AgentID,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailViewModel
                {
                    ID = od.ID,
                    ItemID = od.ItemID,
                    Quantity = od.Quantity,
                    UnitAmount = od.UnitAmount
                }).ToList()
            };

            ViewBag.AgentID = new SelectList(db.Agents, "AgentID", "AgentName", order.AgentID);
            ViewBag.Items = db.Items.ToList();
            return View(model);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var order = db.Orders.Include(o => o.OrderDetails)
                                     .FirstOrDefault(o => o.OrderID == model.OrderID);

                if (order != null)
                {
                    order.OrderDate = model.OrderDate;
                    order.AgentID = model.AgentID;

                    // Remove existing order details
                    var detailsToRemove = order.OrderDetails.ToList();
                    foreach (var detail in detailsToRemove)
                    {
                        db.OrderDetails.Remove(detail);
                    }

                    // Add updated order details
                    if (model.OrderDetails != null)
                    {
                        foreach (var detail in model.OrderDetails.Where(d => d.ItemID > 0 && d.Quantity > 0))
                        {
                            var item = db.Items.Find(detail.ItemID);
                            if (item != null)
                            {
                                var orderDetail = new OrderDetail
                                {
                                    OrderID = order.OrderID,
                                    ItemID = detail.ItemID,
                                    Quantity = detail.Quantity,
                                    UnitAmount = item.UnitPrice
                                };
                                db.OrderDetails.Add(orderDetail);
                            }
                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.AgentID = new SelectList(db.Agents, "AgentID", "AgentName", model.AgentID);
            ViewBag.Items = db.Items.ToList();
            return View(model);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Include(o => o.Agent)
                                   .Include(o => o.OrderDetails)
                                   .FirstOrDefault(o => o.OrderID == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Include(o => o.OrderDetails)
                                   .FirstOrDefault(o => o.OrderID == id);
            
            // Remove order details first
            var detailsToRemove = order.OrderDetails.ToList();
            foreach (var detail in detailsToRemove)
            {
                db.OrderDetails.Remove(detail);
            }
            
            // Remove order
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Orders/Print/5
        public ActionResult Print(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Include(o => o.Agent)
                                   .Include(o => o.OrderDetails.Select(od => od.Item))
                                   .FirstOrDefault(o => o.OrderID == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
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
