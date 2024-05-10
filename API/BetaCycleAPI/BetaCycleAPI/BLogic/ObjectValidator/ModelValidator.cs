using BetaCycleAPI.Models;
using System.ComponentModel.DataAnnotations;
using System;

namespace BetaCycleAPI.BLogic.ObjectValidator
{
    public static class ModelValidator
    {

        public static bool ValidateCustomer(Customer customer)
        {
            var context = new ValidationContext(customer, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(customer, context, results, true);
        }

        public static bool ValidateAddress(Address address)
        {
            var context = new ValidationContext(address, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(address, context, results, true);
        }


        public static bool ValidateProduct(Product product)
        {
            var context = new ValidationContext(product, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(product, context, results, true);
        }



    }
}
