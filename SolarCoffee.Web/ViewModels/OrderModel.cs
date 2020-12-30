using SolarCoffee.Data.Models;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;

namespace SolarCoffee.Web.Controllers
{
    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public CustomerModel Customer { get; set; }
        public List<SalesOrderItemModel> SalesOrderItems { get; set; }
        public bool IsPaid { get; set; }
    }
}