using Microsoft.EntityFrameworkCore;
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
    public class CustomerService : ICustomerService
    {
        private readonly SolarDbContext db;

        public CustomerService(SolarDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Add Customer record
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>ServiceResponse<Customer></returns>
        public ServiceResponse<Customer> CreateCustomer(Customer customer)
        {
            try
            {
                db.Add(customer);
                var response = db.SaveChanges();
                if (response >= 1)
                {
                    return new ServiceResponse<Customer>
                    {
                        IsSuccess = true,
                        Message = "New Customer Added",
                        Time = DateTime.UtcNow,
                        Data = customer
                    };
                }
                return new ServiceResponse<Customer>
                {
                    IsSuccess = false,
                    Message = "Customer not Added",
                    Time = DateTime.UtcNow,
                    Data = customer
                };
            }
            catch (Exception ex)
            {

                return new ServiceResponse<Customer>
                {
                    IsSuccess = false,
                    Message = ex.StackTrace,
                    Time = DateTime.UtcNow,
                    Data = null
                };
            }
        }

        /// <summary>
        /// Deletes a customer record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResponse<bool> DeleteCustomer(int id)
        {
            try
            {
                var customer = db.Customers.Find(id);
                if (customer == null)
                {
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = false,
                        Message = "Invalid customer",
                        Time = DateTime.UtcNow,
                        Data = false
                    };
                }
                db.Customers.Remove(customer);
                var response = db.SaveChanges();
                if (response >= 1)
                {
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = true,
                        Message = "Customer Deleted",
                        Time = DateTime.UtcNow,
                        Data = true
                    };
                }
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Record not persisted correctly",
                    Time = DateTime.UtcNow,
                    Data = false
                };
            }
            catch (Exception ex)
            {

                return new ServiceResponse<bool>
                {
                    IsSuccess=false,
                    Message = ex.StackTrace,
                    Time=DateTime.UtcNow,
                    Data=false
                };
            }
        }

        public Tuple<Customer, string> GetById(int id)
        {
            var nullCustomer = new Customer();
            try
            {
                var customer = db.Customers.Find(id);
                if (customer == null)
                {
                    return Tuple.Create(nullCustomer, "Customer does not exist");
                }
                return Tuple.Create(customer, "Customer returned successfully");

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Returns a list of Customers from the Database
        /// </summary>
        /// <returns>List<Customer></Csutomer></returns>
        public Tuple<List<Customer>, int> GetAllCustomers(PagingParameterModel paging)
        {
            var pageSort = paging.pageSize * paging.pageNumber - paging.pageSize;
            var customers = db.Customers
                .Include(customer => customer.PrimaryAddress)
                .OrderBy(customer => customer.LastName)                
                .ToList();
            var count = customers.Count;
           var paginatedCustomers = customers.Skip(pageSort)
                .Take(paging.pageSize)
                .ToList();
            return Tuple.Create(paginatedCustomers, count);
        }
    }
}
