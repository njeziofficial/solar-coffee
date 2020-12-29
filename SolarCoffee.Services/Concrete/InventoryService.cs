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
    public class InventoryService : IInventoryService
    {
        readonly DateTime now = DateTime.UtcNow;
        private readonly SolarDbContext db;
        private readonly ILogger<InventoryService> logger;

        public InventoryService(SolarDbContext db, ILogger<InventoryService> logger)
        {
            this.db = db;
            this.logger = logger;
        }        

        public ProductInventory GetByProductId(int productID)
        {
           return db.ProductInventories
                .Include(p => p.Product)
                .Where(p => p.Product.Id == productID)
                .FirstOrDefault();
        }

        public List<ProductInventory> GetCurrentInventory()
        {
           return db.ProductInventories
                .Include(p => p.Product)
                .Where(p => !p.Product.IsArchived)
                .ToList();
        }

        public List<ProductInventorySnapshot> GetSnapshotHistory()
        {
            var earliest = DateTime.UtcNow - TimeSpan.FromHours(6);
            return db.ProductInventorySnapshots
                .Include(p => p.Product)
                .Where(p => p.SnapshotTime == earliest
                && !p.Product.IsArchived)
                .ToList();
                
        }

        /// <summary>
        /// Updates number of units available of the provided product id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="adjustment"></param>
        /// <returns></returns>
        public ServiceResponse<ProductInventory> UpdateUnitsAvailable(int id, int adjustment)
        {
            try
            {
                var inventory = db.ProductInventories
                .Include(p => p.Product)
                .First(inv => inv.Product.Id == id);
                inventory.QuantityOnHand += adjustment;

                try
                {
                    CreateSnapshot(inventory);
                }
                catch (Exception ex)
                {

                    logger.LogError("Error creating inventory snapshots");
                    logger.LogError(ex.StackTrace);
                }

                var response = db.SaveChanges();

                if (response >= 1)
                {
                    return new ServiceResponse<ProductInventory>
                    {
                        IsSuccess = true,
                        Message = $"Product {id} Updated",
                        Time = now,
                        Data = inventory
                    };
                }
                return new ServiceResponse<ProductInventory>
                {
                    IsSuccess = false,
                    Message = $"There is an issue in updating product with id = {id}",
                    Time = now,
                    Data = null
                };
            }
            catch (Exception ex)
            {

                return new ServiceResponse<ProductInventory>
                {
                    IsSuccess = false,
                    Message = ex.StackTrace,
                    Time = now,
                    Data = null
                };
            }
        }
        private void CreateSnapshot(ProductInventory inventory)
        {
            var snapshot = new ProductInventorySnapshot
            {
                SnapshotTime = now,
                Product = inventory.Product,
                QuantityOnHand = inventory.QuantityOnHand
            };
            db.Add(snapshot);
        }
    }
}
