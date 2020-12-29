using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarCoffee.Services.Concrete
{
    public class OrderService : IOrderService
    {
        readonly DateTime now = DateTime.UtcNow;
        private readonly SolarDbContext db;
        private readonly ILogger<OrderService> logger;
        private readonly IProductService productService;
        private readonly IInventoryService inventoryService;

        public OrderService(SolarDbContext db, ILogger<OrderService> logger, IProductService productService, IInventoryService inventoryService)
        {
            this.db = db;
            this.logger = logger;
            this.productService = productService;
            this.inventoryService = inventoryService;
        }
        public ServiceResponse<bool> GenerateOpenOrder(SalesOrder order)
        {
            logger.LogInformation("Generating new order");
            Parallel.ForEach(order.SalesOrderItems.ToArray(), item =>
            {
                item.Product = productService.GetProductById(item.Product.Id);
                var inventoryId = inventoryService.GetByProductId(item.Product.Id).Id;
                inventoryService.UpdateUnitsAvailable(inventoryId, -item.Quantity);
            });
            try
            {
                db.SalesOrders.Add(order);
                var response = db.SaveChanges();
                if (response >= 1)
                {
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = true,
                        Message = "Open order created.",
                        Time = now,
                        Data = true
                    };
                }
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Error in saving open order record created.",
                    Time = now,
                    Data = false
                };
            }
            catch (Exception ex)
            {

                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    Message = ex.StackTrace,
                    Time = now,
                    Data = false
                };
            }
        }

        public List<SalesOrder> GetOrders()
        {
            return db.SalesOrders
                .Include(so => so.Customer)
                    .ThenInclude(customer => customer.PrimaryAddress)
                .Include(so => so.SalesOrderItems)
                    .ThenInclude(item => item.Product)
                .ToList();
        }

        /// <summary>
        /// Marks an OpenSalesOrder as paid.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResponse<bool> MarkFulfilled(int id)
        {
            var order = db.SalesOrders.Find(id);
            order.UpdatedOn = now;
            order.IsPaid = true;
            try
            {
                db.SalesOrders.Update(order);
                var response = db.SaveChanges();
                if (response >= 1)
                {
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = true,
                        Message = $"Order {order.Id} closed: Invoice paid in full..",
                        Time = now,
                        Data = true
                    };
                }
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Error in saving open order record created.",
                    Time = now,
                    Data = false
                };
            }
            catch (Exception e)
            {

                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    Message = e.StackTrace,
                    Time = now,
                    Data = false
                };
            }
        }
    }
}
