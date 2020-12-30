using System;
using System.Collections.Generic;

namespace SolarCoffee.Web.Controllers
{
    public class InvoiceModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int CustomerId { get; set; }
        public List<SalesOrderItemModel> LineItems { get; set; }
        public bool IsPaid { get; set; }

    }
}