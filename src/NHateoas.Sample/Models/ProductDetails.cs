﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHateoas.Sample.Models
{
    public class ProductDetails
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Details { get; set; }
    }
}