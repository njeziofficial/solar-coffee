using SolarCoffee.Web.ViewModels;
using System;

namespace SolarCoffee.Web.Controllers
{
    public class SalesOrderItemModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public ProductModel Product { get; set; }
    }
}