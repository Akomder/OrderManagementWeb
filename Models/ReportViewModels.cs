using System;
using System.ComponentModel.DataAnnotations;

namespace OrderManagementWeb.Models
{
    public class BestItemViewModel
    {
        [Display(Name = "Item ID")]
        public int ItemID { get; set; }

        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "Unit Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Total Quantity Sold")]
        public int TotalQuantitySold { get; set; }

        [Display(Name = "Total Revenue")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Number of Orders")]
        public int OrderCount { get; set; }
    }

    public class CustomerPurchaseViewModel
    {
        [Display(Name = "Agent ID")]
        public int AgentID { get; set; }

        [Display(Name = "Agent Name")]
        public string AgentName { get; set; }

        [Display(Name = "Order ID")]
        public int OrderID { get; set; }

        [Display(Name = "Order Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Item ID")]
        public int ItemID { get; set; }

        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Unit Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal UnitAmount { get; set; }

        [Display(Name = "Total Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalAmount { get; set; }
    }

    public class ItemPurchasedViewModel
    {
        [Display(Name = "Item ID")]
        public int ItemID { get; set; }

        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "Total Quantity")]
        public int TotalQuantity { get; set; }

        [Display(Name = "Total Spent")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalSpent { get; set; }

        [Display(Name = "Last Purchase Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime LastPurchaseDate { get; set; }

        [Display(Name = "Number of Purchases")]
        public int PurchaseCount { get; set; }
    }
}
