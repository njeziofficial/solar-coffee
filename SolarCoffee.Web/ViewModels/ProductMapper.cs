using SolarCoffee.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.ViewModels
{
    public class ProductMapper
    {
        public static ProductViewModel SerializeProductModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                CreatedOn = product.CreatedOn,
                UpdatedOn = product.UpdatedOn,
                Description = product.Description,
                IsArchived = product.IsArchived,
                IsTaxable = product.IsTaxable,
                Name = product.Name,
                Price = product.Price
            };
        }

        public static Product SerializeProductModel(ProductViewModel product)
        {
            return new Product
            {
                Id = product.Id,
                CreatedOn = product.CreatedOn,
                UpdatedOn = product.UpdatedOn,
                Description = product.Description,
                IsArchived = product.IsArchived,
                IsTaxable = product.IsTaxable,
                Name = product.Name,
                Price = product.Price
            };
        }
    }
}
