﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Commands
{
    public class UpdateProductCommand
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
