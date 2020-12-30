using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Abstract;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        readonly DateTime now = DateTime.UtcNow;
        private readonly ILogger<OrderController> logger;
        private readonly IOrderService orderService;
        private readonly ICustomerService customerService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService, ICustomerService customerService)
        {
            this.logger = logger;
            this.orderService = orderService;
            this.customerService = customerService;
        }

        [HttpPost("/api/invoice")]
        public IActionResult GenerateNewOrder([FromBody] InvoiceModel invoice)
        {
            logger.LogInformation("Generating invoice");
            var order = OrderMapper.SerializeInvoiceToOrder(invoice);
            var response = customerService.GetById(invoice.CustomerId);
            order.Customer = response.Item1;
            orderService.GenerateOpenOrder(order);
            return Ok();
        }
    }

    public class OrderMapper
    {
        public static SalesOrder SerializeInvoiceToOrder(InvoiceModel invoice)
        {
            DateTime now = DateTime.UtcNow;
            var salesOrderItems = invoice.LineItems
                .Select(item => new SalesOrderItem
                {
                    Id = item.Id,
                    Quantity = item.Quantity,
                    Product = ProductMapper.SerializeProductModel(item.Product),
                    UpdatedOn = now
                }).ToList();
            return new SalesOrder
            {
                SalesOrderItems = salesOrderItems,
                CreatedOn = now,
                UpdatedOn = now
            };
        }
        public static List<OrderModel> SerializeOrdersToViewModel(IEnumerable<SalesOrder> orders)
        {
            return orders.Select(order => new OrderModel
            {
                Id = order.Id,
                CreatedOn = order.CreatedOn,
                UpdatedOn = order.UpdatedOn,
                SalesOrderItems = SerializeSalesOrderItems(order.SalesOrderItems),
                Customer = CustomerMapper.SerializeCustomer(order.Customer),
                IsPaid = order.IsPaid
            }).ToList();
        }

        private static List<SalesOrderItemModel> SerializeSalesOrderItems(IEnumerable<SalesOrderItem> orderItems)
        {
            return orderItems.Select(item => new SalesOrderItemModel
            {
                Id = item.Id,
                Quantity = item.Quantity,
                Product = ProductMapper.SerializeProductModel(item.Product)
            }).ToList();
        }        
    }
}
