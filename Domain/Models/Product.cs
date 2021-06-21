using System;

namespace Domain.Models
{
    public class Product
    {  
        public Product(string description, decimal price)
        {
            Id = Guid.NewGuid();
            Description = description;
            Price = price;
        }

        public Guid Id { get; init; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }

        public void Update(string description, decimal price) => (Description, Price) = (description, price);        
    }
}
