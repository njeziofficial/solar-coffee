using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Abstract;
using SolarCoffee.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarCoffee.Services.Concrete
{
    public class ProductService : IProductService
    {
        private readonly SolarDbContext _db;

        public ProductService(SolarDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Archives a product by setting a flag on product IsArchived to true.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResponse<Product> ArchiveProduct(int id)
        {
            var productToArchive = _db.Products.Find(id);
            try
            {
                if (productToArchive == null)
                {
                    return new ServiceResponse<Product>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = "Product not found on database",
                        Time = DateTime.UtcNow
                    };
                }
                productToArchive.IsArchived = true;
                _db.SaveChanges();

                return new ServiceResponse<Product>
                {
                    Data = productToArchive,
                    IsSuccess = true,
                    Message = "Product Archived successfully",
                    Time = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {

                return new ServiceResponse<Product>
                {
                    Data = productToArchive,
                    IsSuccess = true,
                    Message = ex.StackTrace.ToString(),
                    Time = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Adds a new product to the Database.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public ServiceResponse<Product> CreatedProduct(Product product)
        {
            try
            {
                _db.Products.Add(product);

                var newInventory = new ProductInventory
                {
                    Product = product,
                    QuantityOnHand = 0,
                    IdealQuantity = 10
                };
                _db.ProductInventories.Add(newInventory);
                _db.SaveChanges();

                return new ServiceResponse<Product>
                {
                    Data = product,
                    Time = DateTime.UtcNow,
                    Message = "Saved new Product",
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {

                return new ServiceResponse<Product>
                {
                    Data = product,
                    IsSuccess = false,
                    Time = DateTime.UtcNow,
                    Message = ex.StackTrace.ToString()
                };
            }
        }

        /// <summary>
        /// Retreives all Products from the Database.
        /// </summary>
        /// <returns></returns>
        public Tuple<List<Product>,int> GetAllProducts(PagingParameterModel paging)
        {
            var pageSort = paging.pageSize * paging.pageNumber - paging.pageSize;
            var products = _db.Products
                .ToList();
            var paginatedProducts = products
                .OrderByDescending(p=> p.CreatedOn)
                .Skip(pageSort)
                .Take(paging.pageSize)
                .ToList();

            var count = products.Count();

            return Tuple.Create(paginatedProducts, count);

        }

        /// <summary>
        /// Gets product by primary key from a Database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product GetProductById(int id)
        {
            return _db.Products.Find(id);
        }
    }
}
