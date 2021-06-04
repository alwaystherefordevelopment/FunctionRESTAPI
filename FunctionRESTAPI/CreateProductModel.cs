using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionRESTAPI
{
    public class CreateProductModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }

        public int CategoryId { get; set; }
    }
}
