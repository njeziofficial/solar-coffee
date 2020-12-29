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
        Tuple<List<Product>, int> GetAllProducts(PagingParameterModel paging);
        Product GetProductById(int id);
        ServiceResponse<Product> CreatedProduct(Product product);
        ServiceResponse<Product> ArchiveProduct(int id);
    }
}
