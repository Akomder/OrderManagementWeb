using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderManagementWeb.Filters;
using OrderManagementWeb.Models;

namespace OrderManagementWeb.Controllers
{
    [AuthenticationFilter]
    public class ReportsController : Controller
    {
        private OrderManagementDbEntities db = new OrderManagementDbEntities();

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        // GET: Reports/BestItems
        public ActionResult BestItems(DateTime? startDate, DateTime? endDate)
        {
            var query = db.OrderDetails
                .Include(od => od.Item)
                .Include(od => od.Order)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(od => od.Order.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(od => od.Order.OrderDate <= endDate.Value);
            }

            var bestItems = query
                .GroupBy(od => new { od.ItemID, od.Item.ItemName, od.Item.Size, od.Item.UnitPrice })
                .Select(g => new BestItemViewModel
                {
                    ItemID = g.Key.ItemID,
                    ItemName = g.Key.ItemName,
                    Size = g.Key.Size,
                    UnitPrice = g.Key.UnitPrice,
                    TotalQuantitySold = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Quantity * od.UnitAmount),
                    OrderCount = g.Select(od => od.OrderID).Distinct().Count()
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .ToList();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(bestItems);
        }

        // GET: Reports/CustomerPurchases
        public ActionResult CustomerPurchases(int? agentId, DateTime? startDate, DateTime? endDate)
        {
            var query = db.Orders
                .Include(o => o.Agent)
                .Include(o => o.OrderDetails.Select(od => od.Item))
                .AsQueryable();

            if (agentId.HasValue && agentId.Value > 0)
            {
                query = query.Where(o => o.AgentID == agentId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= endDate.Value);
            }

            var customerPurchases = query
                .ToList()
                .SelectMany(o => o.OrderDetails.Select(od => new CustomerPurchaseViewModel
                {
                    AgentID = o.AgentID,
                    AgentName = o.Agent.AgentName,
                    OrderID = o.OrderID,
                    OrderDate = o.OrderDate,
                    ItemID = od.ItemID,
                    ItemName = od.Item.ItemName,
                    Quantity = od.Quantity,
                    UnitAmount = od.UnitAmount,
                    TotalAmount = od.Quantity * od.UnitAmount
                }))
                .OrderByDescending(x => x.OrderDate)
                .ToList();

            ViewBag.AgentID = new SelectList(db.Agents, "AgentID", "AgentName", agentId);
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(customerPurchases);
        }

        // GET: Reports/ItemsPurchasedByCustomer
        public ActionResult ItemsPurchasedByCustomer(int? agentId)
        {
            if (!agentId.HasValue || agentId.Value == 0)
            {
                ViewBag.AgentID = new SelectList(db.Agents, "AgentID", "AgentName");
                return View(new List<ItemPurchasedViewModel>());
            }

            var agent = db.Agents.Find(agentId.Value);
            ViewBag.AgentName = agent?.AgentName ?? "Unknown";

            var itemsPurchased = db.OrderDetails
                .Include(od => od.Item)
                .Include(od => od.Order)
                .Where(od => od.Order.AgentID == agentId.Value)
                .GroupBy(od => new { od.ItemID, od.Item.ItemName, od.Item.Size })
                .Select(g => new ItemPurchasedViewModel
                {
                    ItemID = g.Key.ItemID,
                    ItemName = g.Key.ItemName,
                    Size = g.Key.Size,
                    TotalQuantity = g.Sum(od => od.Quantity),
                    TotalSpent = g.Sum(od => od.Quantity * od.UnitAmount),
                    LastPurchaseDate = g.Max(od => od.Order.OrderDate),
                    PurchaseCount = g.Select(od => od.OrderID).Distinct().Count()
                })
                .OrderByDescending(x => x.TotalQuantity)
                .ToList();

            ViewBag.AgentID = new SelectList(db.Agents, "AgentID", "AgentName", agentId);

            return View(itemsPurchased);
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
