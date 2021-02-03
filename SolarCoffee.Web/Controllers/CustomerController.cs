using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SolarCoffee.Data.Models;
using SolarCoffee.Services;
using SolarCoffee.Services.Abstract;
using SolarCoffee.Web.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class CustomerController : ControllerBase
    {
        readonly DateTime now = DateTime.UtcNow;
        private readonly ILogger<CustomerController> logger;
        private readonly ICustomerService customerService;

        public CustomerController(ILogger<CustomerController>logger, ICustomerService customerService)
        {
            this.logger = logger;
            this.customerService = customerService;
        }

        [HttpPost("/api/customer")]
        public IActionResult CreateCustomer(CustomerModel customer)
        {
            logger.LogInformation("Creating a new Customer");
            customer.CreatedOn = now;
            customer.UpdatedOn = now;
            var customerData = CustomerMapper.SerializeCustomer(customer);
            var response = customerService.CreateCustomer(customerData);
            return Ok(response);
        }

        [HttpPost("/api/customers")]
        public async Task<IActionResult> GetCustomers(PagingParameterModel paging)
        {
            logger.LogInformation("Getting customers");
            var customers = customerService.GetAllCustomers(paging);
            IQueryable<Customer> source = customers.Item1.AsQueryable();
            int count = customers.Item2;
            int CurrentPage = paging.pageNumber;
            int PageSize = paging.pageSize;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Select(CustomerMapper.SerializeCustomer)
                .ToList()
                .OrderBy(p => p.Id);
            var previousPage = CurrentPage > 1 ? "Yes" : "No";
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

            var customerModel = source.Select(c => new CustomerModel
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PrimaryAddress = CustomerMapper
                .MapCustomerAddress(c.PrimaryAddress),
                CreatedOn = c.CreatedOn,
                UpdatedOn = c.UpdatedOn
            })
                .OrderByDescending(c => c.CreatedOn)
                .ToList();
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
            return Ok(Tuple.Create(item1: items, item2: count));
        }

        [HttpDelete("/api/customer/{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            logger.LogInformation("Deleting a customer");
            var response = customerService.DeleteCustomer(id);
            return Ok(response);
        }
    }
}
