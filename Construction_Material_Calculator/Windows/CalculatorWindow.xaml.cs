using Construction_Material_Calculator.Models;
using Construction_Material_Calculator.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Construction_Material_Calculator.Helpers;


namespace Construction_Material_Calculator.Windows
{
   
    public partial class CalculatorWindow : Window
    {
        public List<ElementType> ElementType { get; set; }
        public List<Material> MaterialName { get; set; }

        public List <BarDia> BarDia { get; set; }
        public AppData _appData { get; set; }
        public CalculatorWindow(AppData data) 
        {
            InitializeComponent();
            _appData = data;
            ElementType = Enum.GetValues(typeof(ElementType)).Cast<ElementType>().ToList(); //convert enum to list 
            BarDia = Enum.GetValues(typeof(BarDia)).Cast<BarDia>().ToList(); //convert enum to list
            MaterialName = _appData.Materials.Where(m => m.Category == "Concrete" ).ToList();  //filter materials by category
            DataContext = this;

        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
           double length = Helper.GetNumValue(LengthBox); 
           double Width = Helper.GetNumValue(WidthBox); 
           double Depth = Helper.GetNumValue(DepthBox);
           double Quantity = Helper.GetNumValue(QuantityBox);
           VolumeBox.Text = (length * Width * Depth* Quantity).ToString(); //display volume in volumebox

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var selectedMaterial = MaterialNameCombo.SelectedItem as Material; 
            var newOrder = new Order //create order & fill it with data from the form 
            {
                OrderNumber = _appData.Orders.Count + 1,
                MaterialName = selectedMaterial.Name,
                Category = selectedMaterial.Category,
                Unit = selectedMaterial.Unit,
                UnitPrice = selectedMaterial.UnitPrice,
                Quantity = Helper.GetNumValue(QuantityBox),
                Status = "Pending",
                Date = DateTime.Now,
                ElementType = (ElementType)ElementTypeCombo.SelectedItem,
               
            };
            _appData.Orders.Add(newOrder);
            Helper.SaveToJson(_appData);
                
        }

        private double CalculateSteel()
        {
            double diameter = double.Parse(((ComboBoxItem)DiameterCombo.SelectedItem).Content.ToString()); 
            double length = Helper.GetNumValue(BarLengthBox);
            double bars = Helper.GetNumValue(BarsCountBox);

            double weight = (Math.Pow(diameter, 2) / 162) * length * bars;

            return weight * 1.05; // waste
        }
        private void SteelCalculate_Click(object sender, RoutedEventArgs e)
        {
            double weight = CalculateSteel();
            double tons = weight / 1000;

            SteelResultText.Text = $"Total Steel Weight: {weight:F2} kg ({tons:F3} Tons)";
        }
        private void SteelSave_Click(object sender, RoutedEventArgs e)
        {
            double weight = CalculateSteel();

            var order = new Order
            { 
                OrderNumber = _appData.Orders.Count + 1,
                MaterialName = "Steel",
                Category = "Steel",
                Unit = "kg",
                Quantity = weight,
                Status = "Pending",
                Date = DateTime.Now
            };

            _appData.Orders.Add(order);
            Helper.SaveToJson(_appData);
        }

        private double CalculatePaint()
        {
            double area;
            //calculate area (areabox or leingth*height) based on selection
            if (AreaMode.IsChecked == true) 
                area = Helper.GetNumValue(AreaBox);
            else
                area = Helper.GetNumValue(LengthPaintBox) * Helper.GetNumValue(HeightPaintBox);

            int coats = int.Parse(((ComboBoxItem)CoatsCombo.SelectedItem).Content.ToString());
            double coverage = Helper.GetNumValue(CoverageBox);

            return (area * coats) / coverage;
        }
        private void PaintCalculate_Click(object sender, RoutedEventArgs e)
        {
            double liters = CalculatePaint();

            PaintResultText.Text = $"Paint Required: {liters:F2} Liters";
        }
        private void PaintSave_Click(object sender, RoutedEventArgs e)
        {
            double liters = CalculatePaint();

            var order = new Order
            {
                OrderNumber = _appData.Orders.Count + 1,
                MaterialName = "Paint",
                Category = "Paint",
                Unit = "Liter",
                Quantity = liters,
                Status = "Pending",
                Date = DateTime.Now
            };

            _appData.Orders.Add(order);
            Helper.SaveToJson(_appData);
        }

        private int CalculateTiles()
        { 
            double roomArea = Helper.GetNumValue(RoomLengthBox) * Helper.GetNumValue(RoomWidthBox); 

            string tileSize = ((ComboBoxItem)TileSizeCombo.SelectedItem).Content.ToString(); 
            string[] parts = tileSize.Split('×');

            double tileLength = double.Parse(parts[0]) / 100;
            double tileWidth = double.Parse(parts[1]) / 100;

            double tileArea = tileLength * tileWidth;

            double waste = Helper.GetNumValue(WasteBox); 

            double tiles = (roomArea / tileArea) * (1 + waste / 100);

            return (int)Math.Ceiling(tiles); //round up to nearest 
        }
        private void TilesCalculate_Click(object sender, RoutedEventArgs e)
        {
            int tiles = CalculateTiles();

            TilesResultText.Text = $"Tiles Needed: {tiles}";
        }
        private void TilesSave_Click(object sender, RoutedEventArgs e)
        {
            int tiles = CalculateTiles();

            var order = new Order
            {
                OrderNumber = _appData.Orders.Count + 1,
                MaterialName = "Tiles",
                Category = "Tiles",
                Unit = "Piece",
                Quantity = tiles,
                Status = "Pending",
                Date = DateTime.Now
            };

            _appData.Orders.Add(order);
            Helper.SaveToJson(_appData);
        }

    }
}
