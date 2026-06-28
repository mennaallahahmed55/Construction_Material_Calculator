using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction_Material_Calculator.Models
{
    public class Material
    {
        public string Name { get; set; }
        public string Category { get; set; } // Concrete, Steel, Paint, Tiles, General
        public string Unit { get; set; } // Ton, kg, m³, m², Liter, Piece
        public decimal UnitPrice { get; set; } // Price in EGP
    }
}
