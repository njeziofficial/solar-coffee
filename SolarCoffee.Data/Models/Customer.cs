using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarCoffee.Data.Models
{
   public class Customer
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
       [MaxLength(32)] public string FirstName { get; set; }
        [MaxLength(32)] public string LastName { get; set; }
        public CustomerAddress PrimaryAddress { get; set; }

    }
}
