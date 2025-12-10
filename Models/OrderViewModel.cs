using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderManagementWeb.Models
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }

        [Required]
        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "Agent")]
        public int AgentID { get; set; }

        public List<OrderDetailViewModel> OrderDetails { get; set; }

        public OrderViewModel()
        {
            OrderDate = DateTime.Now;
            OrderDetails = new List<OrderDetailViewModel>();
        }
    }

    public class OrderDetailViewModel
    {
        public int ID { get; set; }
        public int OrderID { get; set; }

        [Display(Name = "Product")]
        public int ItemID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Display(Name = "Unit Price")]
        public decimal UnitAmount { get; set; }
    }
}
