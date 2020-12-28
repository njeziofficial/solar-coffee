using SolarCoffee.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarCoffee.Services.Abstract
{
   public interface IProductService
    {
        List<Product> GetAllProducts();
        Product GetProductById(int id);
        ServiceResponse<Product> CreatedProduct(Product product);
        ServiceResponse<Product> ArchiveProduct(int id);
    }
}
