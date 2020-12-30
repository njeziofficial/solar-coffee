using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Abstract;
using SolarCoffee.Web.ViewModels;
using System;
using System.Linq;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class CustomerController : ControllerBase
    {
        DateTime now = DateTime.UtcNow;
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

        [HttpGet("/api/customer")]
        public IActionResult GetCustomers()
        {
            logger.LogInformation("Getting customers");
            var customers = customerService.GetAllCustomers();
            var customerModel = customers.Select(c => new CustomerModel
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
                PrimaryAddress = CustomerMapper
                .MapCustomerAddress(c.PrimaryAddress),
                CreatedOn = c.CreatedOn,
                UpdatedOn = c.UpdatedOn
            })
                .OrderByDescending(c => c.CreatedOn)
                .ToList();
            return Ok(customerModel);
        }
    }
}
