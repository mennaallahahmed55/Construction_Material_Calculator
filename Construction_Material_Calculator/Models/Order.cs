using Construction_Material_Calculator.Models.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace Construction_Material_Calculator.Models
{

    public class Order
    {
        public int OrderNumber { get; set; }
        public string MaterialName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))] //to convert enum to string in json
        public ElementType ElementType { get; set; }
        public string Category { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; } // Quantity * UnitPrice
        public DateTime Date { get; set; }
        public string Status { get; set; } // Pending or Delivered
    }
}
