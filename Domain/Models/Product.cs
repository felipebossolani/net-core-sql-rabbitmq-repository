﻿using System;
using System.Text.Json.Serialization;

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

        [JsonInclude]
        public Guid Id { get; private init; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }

        public void Update(string description, decimal price) => (Description, Price) = (description, price);        
    }
}
