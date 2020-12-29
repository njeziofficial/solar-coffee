﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SolarCoffee.Data.Models;
using SolarCoffee.Services;
using SolarCoffee.Services.Abstract;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        
        public ProductController(ILogger<ProductController>logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        //[Authorize]
        [HttpPost("/api/products")]
        public IActionResult GetProduct(PagingParameterModel paging)
        {
            _logger.LogInformation("Getting all product");
           var products = _productService.GetAllProducts(paging);
          //var  products.Item1.Select(product => ProductMapper.SerializeProductModel(product));
            IQueryable<Product> source = products.Item1.AsQueryable();
            int count = products.Item2;
            int CurrentPage = paging.pageNumber;
            int PageSize = paging.pageSize;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Select(ProductMapper.SerializeProductModel).ToList();
            var previousPage = CurrentPage > 1 ? "Yes" : "No";
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

            var paginationMetadata = new
            {
                totalCount = count,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };

            // Setting Header  
            HttpContext.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(Tuple.Create(item1: items, item2: products.Item2));
        }
    }
}
